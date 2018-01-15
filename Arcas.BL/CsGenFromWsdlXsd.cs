using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Description;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Cav;

namespace Arcas.BL
{
    public class CsGenFromWsdlXsd
    {
        public string GenFromWsdl(
            string uriWsdl,
            bool createAsync,
            string targetNamespace,
            string reflectAssembly,
            string outputFile,
            Boolean generateClient)
        {
            if (uriWsdl.IsNullOrWhiteSpace())
                return "Не указан uri wsdl";

            var tempPath = Path.Combine(DomainContext.TempPath, Guid.NewGuid().ToString());

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            var wsdlTempFile = Path.Combine(tempPath, Guid.NewGuid().ToString());

            if (File.Exists(wsdlTempFile))
                File.Delete(wsdlTempFile);

            var sourseUri = new Uri(uriWsdl);

            if (!sourseUri.IsFile)
            {
                if (outputFile.IsNullOrWhiteSpace())
                    return "Не указан выходной файл";

                HttpDownloadFile(uriWsdl, wsdlTempFile);
            }
            else
                File.Copy(uriWsdl, wsdlTempFile);

            String sourseDir = null;

            if (outputFile.IsNullOrWhiteSpace())
            {
                sourseDir = Path.GetDirectoryName(uriWsdl);
                outputFile = Path.Combine(sourseDir, Path.GetFileNameWithoutExtension(uriWsdl) + ".cs");
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
            }

            // проверяем доступность места записи
            if (File.Exists(outputFile))
                File.Delete(outputFile);

            File.WriteAllText(outputFile, null);
            File.Delete(outputFile);

            var imperts = loadImport(wsdlTempFile, tempPath, sourseUri).ToArray();

            ServiceDescription wsdlDescription = ServiceDescription.Read(wsdlTempFile);
            foreach (var item in imperts)
                wsdlDescription.Imports.Add(item.Value);

            ServiceDescriptionImporter wsdlImporter = new ServiceDescriptionImporter();

            foreach (var item in imperts)
                using (var xr = XmlReader.Create(item.Key))
                    wsdlImporter.Schemas.Add(XmlSchema.Read(xr, null));

            //wsdlImporter.ProtocolName = "Soap12";
            wsdlImporter.AddServiceDescription(wsdlDescription, null, null);
            wsdlImporter.Style = ServiceDescriptionImportStyle.Server;

            //wsdlImporter.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties;
            var genOptions = CodeGenerationOptions.None;
            if (createAsync)
                genOptions = genOptions | CodeGenerationOptions.GenerateNewAsync;

            wsdlImporter.CodeGenerationOptions = genOptions;

            CodeNamespace codeNamespace = new CodeNamespace();
            CodeCompileUnit codeUnit = new CodeCompileUnit();
            codeUnit.Namespaces.Add(codeNamespace);

            ServiceDescriptionImportWarnings importWarning = wsdlImporter.Import(codeNamespace, codeUnit);

            if (importWarning == 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                StringWriter stringWriter = new StringWriter(stringBuilder);

                CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");
                codeProvider.GenerateCodeFromCompileUnit(codeUnit, stringWriter, new CodeGeneratorOptions());

                stringWriter.Close();

                File.WriteAllText(outputFile, stringBuilder.ToString(), Encoding.UTF8);
            }
            else
            {
                Console.WriteLine(importWarning);
            }

            return null;
        }

        private Dictionary<String, Import> loadImport(string filePath, string tempDir, Uri sourseUri)
        {
            var res = new Dictionary<String, Import>();
            XNamespace xsdNS = "http://www.w3.org/2001/XMLSchema";
            XNamespace wsdlNS = "http://schemas.xmlsoap.org/wsdl/";
            XDocument xdocfile = XDocument.Load(filePath);

            string importName = null;
            string fileinTemp = null;

            Action<XAttribute> lartImp = (locationAttrib) =>
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
                    File.Copy(sourceFile, fileinTemp);
                }

                // если какоето имя в локации, но читали по сети, то суем имя локации в Query
                if (importIsFile & !sourseUri.IsFile)
                {
                    UriBuilder ub = new UriBuilder(sourseUri);
                    ub.Query = locationAttrib.Value;

                    HttpDownloadFile(ub.Uri.PathAndQuery, fileinTemp);
                }

                // если в локации ссылка, то читаем по ней.
                if (!importIsFile)
                    HttpDownloadFile(locationAttrib.Value, fileinTemp);

                foreach (var item in loadImport(fileinTemp, tempDir, sourseUri))
                    res[item.Key] = item.Value;
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

                lartImp(locationAttrib);

                locationAttrib.Value = fileinTemp;
                res[fileinTemp] = new Import() { Location = fileinTemp, Namespace = item.Attribute("namespace").Value };
            }

            foreach (var item in xdocfile.Descendants(wsdlNS + "import"))
            {
                var locationAttrib = item.Attribute("location");

                if (locationAttrib == null)
                    throw new ArgumentException($"Отсутствует атрибут location в элементе {item.Name}");

                if (locationAttrib.Value.IsNullOrWhiteSpace())
                    throw new ArgumentException($"Не заполнен атрибут location в элементе {item.Name}");

                importName = locationAttrib.Value.ComputeMD5ChecksumString().ToString();
                fileinTemp = Path.Combine(tempDir, importName);

                lartImp(locationAttrib);

                locationAttrib.Value = fileinTemp;
                //res.Add(fileinTemp);
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
