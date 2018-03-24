using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdvancedFileViewer
{
    public partial class MainWindow
    {
        #region Метод TripleDES

        public static byte[] TripleDesEncrypt(byte[] plain, String key)
        {
            byte[] keyArray = SoapHexBinary.Parse(key).Value;

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.None;
            tdes.IV = new byte[8];

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(plain, 0, plain.Length);
            tdes.Clear();

            return resultArray;
        }
        public static byte[] TripleDesDecrypt(byte[] cipher, String key)
        {
            byte[] keyArray = SoapHexBinary.Parse(key).Value;

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.None;
            tdes.IV = new byte[8];

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(cipher, 0, cipher.Length);
            tdes.Clear();

            return resultArray;
        }

        #endregion
    }
}
