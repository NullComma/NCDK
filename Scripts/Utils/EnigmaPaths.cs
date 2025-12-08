using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EnigmaCore
{
    /// <summary>
    /// Handles dynamic file extension generation based on project metadata.
    /// </summary>
    public static class EnigmaPaths
    {
        private static string _cachedExtension;

        /// <summary>
        /// Returns a short, unique hash-based file extension for this project.
        /// Format: ".ec" + first 6 hex chars of the project signature hash.
        /// Example: ".ec8f2a10"
        /// </summary>
        public static string SaveExtension
        {
            get
            {
                if (!string.IsNullOrEmpty(_cachedExtension)) return _cachedExtension;

                // 1. Create a unique signature string
                var signature = $"{Application.companyName}.{Application.productName}";

                // 2. Hash it to get a consistent, unique code regardless of length
                using (var algo = SHA256.Create())
                {
                    byte[] bytes = algo.ComputeHash(Encoding.UTF8.GetBytes(signature));
                    
                    // 3. Take the first 3 bytes (which equals 6 Hex characters)
                    // We use BitConverter to get "XX-YY-ZZ", remove dashes, and lower case.
                    string hexHash = BitConverter.ToString(bytes, 0, 3).Replace("-", "").ToLowerInvariant();

                    // 4. Construct the final extension
                    // "ec" stands for EnigmaCore (branding), followed by the unique hash.
                    _cachedExtension = $".ec{hexHash}";
                }

                return _cachedExtension;
            }
        }
    }
}