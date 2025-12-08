using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace EnigmaCore
{
    /// <summary>
    /// Provides simple symmetric encryption (AES) helper methods for local data obfuscation.
    /// <para>
    /// <b>SECURITY NOTICE:</b> This implementation derives its key from the Project's metadata. 
    /// It is intended to deter casual users from tampering with save files (game integrity). 
    /// It is <b>NOT</b> suitable for securing sensitive user data (PII, passwords, financial info), 
    /// as the key derivation logic is exposed in the source code.
    /// </para>
    /// <para>
    /// <b>WARNING:</b> Changing <see cref="Application.productName"/> or <see cref="Application.companyName"/> 
    /// will change the encryption key, rendering previously saved data unreadable.
    /// </para>
    /// </summary>
    public static class EncryptionUtils
    {
        private static byte[] _dynamicKey;
        private static byte[] _dynamicIV;

        static EncryptionUtils()
        {
            GenerateKeyAndIV();
        }

        private static void GenerateKeyAndIV()
        {
            // Generates a unique signature based on the specific Unity Project settings.
            string signature = $"{Application.companyName}.{Application.productName}";

            using (var sha256 = SHA256.Create())
            {
                _dynamicKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(signature));
                
                // Uses the first 16 bytes of the hash as the Initialization Vector.
                _dynamicIV = new byte[16];
                Array.Copy(_dynamicKey, 0, _dynamicIV, 0, 16);
            }
        }

        /// <summary>
        /// Encrypts a plain text string using AES.
        /// </summary>
        /// <param name="plainText">The string to encrypt.</param>
        /// <returns>A Base64 string representation of the encrypted data, or null if an error occurs.</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _dynamicKey;
                    aesAlg.IV = _dynamicIV;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EnigmaCore.EncryptionUtils] Encryption Failed: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Decrypts a Base64 encrypted string back to plain text.
        /// </summary>
        /// <param name="cipherText">The encrypted Base64 string.</param>
        /// <returns>The decrypted plain text string. Returns the original input if it is not valid Base64 (legacy support), or null if decryption fails (tampering/corruption).</returns>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            
            // Fallback: If not Base64, assume it's legacy unencrypted JSON.
            if (!IsBase64String(cipherText)) return cipherText;

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _dynamicKey;
                    aesAlg.IV = _dynamicIV;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch
            {
                // Returns null to indicate that the file is corrupted or tampered with.
                return null; 
            }
        }

        /// <summary>
        /// Checks if a string is a valid Base64 string.
        /// </summary>
        private static bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0
               || base64.Contains(" ") || base64.Contains("\t") || base64.Contains("\r") || base64.Contains("\n"))
                return false;

            try
            {
                Convert.FromBase64String(base64);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}