using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QP_Helpers
{
    public class QP_Helpers
    {
        /// <summary>
        /// Clear whitespace, newline, vertical tab
        /// </summary>
        /// <param name="text">Text that needs clearing</param>
        /// <returns></returns>
        public static string ClearBlankCharacters(string text) => text.Replace(" ", string.Empty).Replace("\n", string.Empty)
            .Replace(Environment.NewLine, string.Empty).Replace("\v", string.Empty).Replace("\r", string.Empty);

        #region LICENSE

        public static string GetRegister(string fullRegistryPath, string registryValueName)
        {
            return (string)Registry.GetValue(fullRegistryPath, registryValueName, "none");
        }

        public static bool _isLicensed { get; set; }

        public static bool IsLicensed(string key, string resourceName)
        {
            string[] licenseKeys;
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                licenseKeys = reader.ReadToEnd().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
            using (MD5 md5Hash = MD5.Create())
            {
                if (licenseKeys.Contains(GetMd5Hash(md5Hash, key)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new StringBuilder to collect the bytes and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static void SetRegister(string fullRegistryPath, string registryValueName, string value)
        {
            Registry.SetValue(fullRegistryPath, registryValueName, value);
        }

        #endregion
    }
}
