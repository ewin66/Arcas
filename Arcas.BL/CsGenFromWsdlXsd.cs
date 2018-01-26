using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Cav;
using WebDescription = System.Web.Services.Description;

namespace Arcas.BL
{
    class prepareData
    {
        public string MainTempFile { get; set; }
        public IEnumerable<String> Imperts { get; set; }
        public string TargetNamespace { get; internal set; }
    }

    public class CsGenFromWsdlXsd
    {
        public string GenFromWsdl(
            string uri,
            bool createAsync,
            string targetNamespace,
            string outputFile,
            Boolean generateClient)
        {
            var pd = new prepareData();

            var msg = checkAndPrepareData(uri, outputFile, targetNamespace, pd);

            if (!msg.IsNullOrWhiteSpace())
                return msg;


            MetadataSet mdSet = new MetadataSet();
            mdSet.MetadataSections.Add(MetadataSection.CreateFromServiceDescription(WebDescription.ServiceDescription.Read(pd.MainTempFile)));

            foreach (var item in pd.Imperts)
                using (var xr = XmlReader.Create(item))
                    mdSet.MetadataSections.Add(MetadataSection.CreateFromSchema(XmlSchema.Read(xr, null)));

            WsdlImporter importer = new WsdlImporter(mdSet);

            var xsdDCImporter = new XsdDataContractImporter();
            xsdDCImporter.Options = new ImportOptions();
            xsdDCImporter.Options.Namespaces.Add("*", pd.TargetNamespace);

            importer.State.Add(typeof(XsdDataContractImporter), xsdDCImporter);

            var xmlOptions = new XmlSerializerImportOptions();
            xmlOptions.ClrNamespace = pd.TargetNamespace;
            importer.State.Add(typeof(XmlSerializerImportOptions), xmlOptions);

            var generator = new ServiceContractGenerator();
            generator.NamespaceMappings.Add("*", pd.TargetNamespace);

            //var options = ServiceContractGenerationOptions.TypedMessages;
            var options = ServiceContractGenerationOptions.None;

            if (generateClient)
                options |= ServiceContractGenerationOptions.ClientClass;

            if (createAsync)
                options |= ServiceContractGenerationOptions.TaskBasedAsynchronousMethod;

            generator.Options = options;

            foreach (var contract in importer.ImportAllContracts())
                generator.GenerateServiceContractType(contract);

            if (generator.Errors.Count != 0)
                return generator.Errors.Select(x => x.Message).JoinValuesToString(separator: Environment.NewLine);

            StringBuilder sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
                CodeDomProvider.CreateProvider("C#")
                    .GenerateCodeFromCompileUnit(
                        generator.TargetCompileUnit,
                        new IndentedTextWriter(sw),
                        new CodeGeneratorOptions() { BracingStyle = "C" });

            File.WriteAllText(outputFile, sb.ToString());

            return null;
        }

        public string GenFromXsd(
            string uri,
            string targetNamespace,
            string outputFile)
        {
            var pd = new prepareData();

            var msg = checkAndPrepareData(uri, outputFile, targetNamespace, pd);

            if (!msg.IsNullOrWhiteSpace())
                return msg;


            return "gg";
        }

        private string checkAndPrepareData(
            string uri,
            string outputFile,
            string targetNamespace,
            prepareData pd)
        {
            if (uri.IsNullOrWhiteSpace())
                return "Не указан uri";

            var tempPath = Path.Combine(DomainContext.TempPath, Guid.NewGuid().ToString());

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            var mainTempFile = Path.Combine(tempPath, uri.ComputeMD5ChecksumString().ToString());

            if (File.Exists(mainTempFile))
                File.Delete(mainTempFile);

            var sourseUri = new Uri(uri);


            if (!sourseUri.IsFile)
            {
                if (outputFile.IsNullOrWhiteSpace())
                    return "Не указан выходной файл";

                HttpDownloadFile(uri, mainTempFile);
            }
            else
                File.Copy(uri, mainTempFile);


            String sourseDir = null;

            if (outputFile.IsNullOrWhiteSpace())
            {
                sourseDir = Path.GetDirectoryName(uri);
                outputFile = Path.Combine(sourseDir, Path.GetFileNameWithoutExtension(uri) + ".cs");
                try
                {
                    // проверяем доступность места записи

                    if (File.Exists(outputFile))
                        File.Delete(outputFile);

                    File.WriteAllText(outputFile, null);
                }
                catch (Exception ex)
                {
                    return "Нет доступа к месту назначения результурующего файла. Ошибка: " + ex.Message;
                }

                if (targetNamespace.IsNullOrWhiteSpace())
                    targetNamespace = Path.GetFileNameWithoutExtension(uri);
            }

            #region проверяем доступность места записи

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            File.WriteAllText(outputFile, null);
            File.Delete(outputFile);

            #endregion

            #region проверка пространства имен

            if (targetNamespace.IsNullOrWhiteSpace())
                targetNamespace = "CodeFromWsdl_" + DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");

            targetNamespace = targetNamespace.Trim('.');

            if (Char.IsDigit(targetNamespace[0]))
                targetNamespace = "_" + targetNamespace;

            foreach (var c in targetNamespace)
                if (!char.IsDigit(c) & !char.IsLetter(c) & c != '_' & c != '.')
                    return "В прострвнстве имен могут быть только буквы, цифры, знак подчеркивания и точка";

            #endregion

            pd.Imperts = downloadImport(mainTempFile, tempPath, sourseUri);
            pd.TargetNamespace = targetNamespace;
            pd.MainTempFile = mainTempFile;

            return null;
        }

        private IEnumerable<String> downloadImport(string filePath, string tempDir, Uri sourseUri)
        {
            var res = new HashSet<string>();
            XNamespace xsdNS = "http://www.w3.org/2001/XMLSchema";
            XNamespace wsdlNS = "http://schemas.xmlsoap.org/wsdl/";
            XDocument xdocfile = XDocument.Load(filePath);

            string importName = null;
            string fileinTemp = null;

            Action<XAttribute, string> lartImp = (locationAttrib, fileintemp) =>
            {
                Boolean importIsFile = true;

                try
                {
                    importIsFile = new Uri(locationAttrib.Value).IsFile;
                }
                catch { }

                // если в локации какоето имя и читали файл, то смотрим в тойже папке файл с именем локации
                if (importIsFile & sourseUri.IsFile)
                {
                    var sourceFile = Path.Combine(Path.GetDirectoryName(sourseUri.AbsolutePath), locationAttrib.Value);
                    File.Copy(sourceFile, fileintemp);
                }

                // если какоето имя в локации, но читали по сети, то суем имя локации в Query
                if (importIsFile & !sourseUri.IsFile)
                {
                    UriBuilder ub = new UriBuilder(sourseUri);
                    ub.Query = locationAttrib.Value;

                    HttpDownloadFile(ub.Uri.PathAndQuery, fileintemp);
                }

                // если в локации ссылка, то читаем по ней.
                if (!importIsFile)
                    HttpDownloadFile(locationAttrib.Value, fileintemp);

                foreach (var item in downloadImport(fileintemp, tempDir, sourseUri))
                    res.Add(item);
            };

            foreach (var item in xdocfile.Descendants(xsdNS + "import"))
            {
                var locationAttrib = item.Attribute("schemaLocation");

                if (locationAttrib == null)
                    throw new ArgumentException($"Отсутствует атрибут schemaLocation в элементе {item.Name}");

                if (locationAttrib.Value.IsNullOrWhiteSpace())
                    throw new ArgumentException($"Не заполнен атрибут schemaLocation в элементе {item.Name}");

                importName = locationAttrib.Value.ComputeMD5ChecksumString().ToString();
                fileinTemp = Path.Combine(tempDir, importName);

                lartImp(locationAttrib, fileinTemp);

                locationAttrib.Value = fileinTemp;

                res.Add(fileinTemp);
            }

            foreach (var item in xdocfile.Descendants(wsdlNS + "import").ToArray())
            {
                var locationAttrib = item.Attribute("location");

                if (locationAttrib == null)
                    throw new ArgumentException($"Отсутствует атрибут location в элементе {item.Name}");

                if (locationAttrib.Value.IsNullOrWhiteSpace())
                    throw new ArgumentException($"Не заполнен атрибут location в элементе {item.Name}");

                importName = locationAttrib.Value.ComputeMD5ChecksumString().ToString();
                fileinTemp = Path.Combine(tempDir, importName);

                lartImp(locationAttrib, fileinTemp);

                var wsdlI = XDocument.Load(fileinTemp);
                foreach (var el in wsdlI.Root.Elements())
                    xdocfile.Root.Add(el);

                item.Remove();
            }

            xdocfile.Save(filePath);

            return res;
        }

        private void HttpDownloadFile(string uri, string file)
        {
            using (var wc = new WebClient())
                wc.DownloadFile(uri, file);
        }
    }
}

