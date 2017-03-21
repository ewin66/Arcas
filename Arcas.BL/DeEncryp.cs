using System.IO;
using System.Security.Cryptography;
using System.Text;
using Arcas.Settings;
using Cav;

namespace Arcas.BL
{
    public class DeEncryp
    {
        public byte[] Encript(UpdateDbSetting settings, string serverFilePath)
        {
            if (settings.AssemplyWithImplementDbConnection != null)
                settings.AssemplyWithImplementDbConnection = settings.AssemplyWithImplementDbConnection.GZipCompress();

            byte[] key = Encoding.UTF8.GetBytes(serverFilePath).ComputeMD5Checksum().ToByteArray();
            byte[] data = Encoding.UTF8.GetBytes(settings.XMLSerialize().ToString());

            var aes = new AesCryptoServiceProvider();
            aes.Key = key;
            aes.IV = key;

            using (ICryptoTransform crtr = aes.CreateEncryptor())
            using (var memres = new MemoryStream())
            using (var crstr = new CryptoStream(memres, crtr, CryptoStreamMode.Write))
            {
                crstr.Write(data, 0, data.Length);
                data = memres.ToArray();
            }

            return data;
        }

        public UpdateDbSetting Decript(byte[] data, string serverFilePath)
        {
            byte[] key = Encoding.UTF8.GetBytes(serverFilePath).ComputeMD5Checksum().ToByteArray();

            var aes = new AesCryptoServiceProvider();
            aes.Key = key;
            aes.IV = key;

            using (ICryptoTransform crtr = aes.CreateDecryptor())
            using (var memres = new MemoryStream())
            using (var crstr = new CryptoStream(memres, crtr, CryptoStreamMode.Read))
            {
                memres.Write(data, 0, data.Length);
                crstr.re
            }

            UpdateDbSetting res = Encoding.UTF8.GetString(data).XMLDeserialize<UpdateDbSetting>()

            if (res.ServerPathScripts.IsNullOrWhiteSpace())
                return null;

            if (res.AssemplyWithImplementDbConnection != null)
                res.AssemplyWithImplementDbConnection = res.AssemplyWithImplementDbConnection.GZipDecompress();

            return res;
        }
    }
}
