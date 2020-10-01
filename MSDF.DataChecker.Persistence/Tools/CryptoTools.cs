// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MSDF.DataChecker.Persistence.Tools
{
    public static class CryptoTools
    {
        static string key;

        public static IConfiguration GetConfig()
        {
            var builder = new ConfigurationBuilder().SetBasePath(System.AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 
            return builder.Build();
        }

        static CryptoTools()
        {
            var config = GetConfig();
            string existKey = config["EncryptionKey"];
            if (string.IsNullOrEmpty(existKey) || existKey == "typeKeyHere")
            {
                TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider();
                TDES.GenerateIV();
                TDES.GenerateKey();
                key = Convert.ToBase64String(TDES.Key);

                string[] pathSplit = AppContext.BaseDirectory.Split("\\");
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < pathSplit.Length - 1; i++)
                {
                    sb.Append(pathSplit[i] + "\\");
                    if (pathSplit[i] == "MSDF.DataChecker.WebApp")
                        break;
                }
                sb.Append("appsettings.json");
                string appsettingsPath = sb.ToString();

                string fileContent = File.ReadAllText(appsettingsPath);
                fileContent = fileContent.Replace("typeKeyHere", key);
                File.WriteAllText(appsettingsPath, fileContent);
            }
            else
            {
                key = existKey;
            }
        }

        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;

            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
