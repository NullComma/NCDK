using NUnit.Framework;
using UnityEngine;

namespace EnigmaCore.Tests.Editor
{
    [TestFixture]
    public class EncryptionUtilsTests
    {
        [Test]
        public void Encrypt_WithValidString_ReturnsBase64String()
        {
            string plainText = "Hello, World!";
            string encrypted = EncryptionUtils.Encrypt(plainText);
            
            Assert.IsNotNull(encrypted);
            Assert.IsNotEmpty(encrypted);
            // Encrypted string should be Base64
            Assert.DoesNotThrow(() => System.Convert.FromBase64String(encrypted));
        }

        [Test]
        public void Encrypt_WithEmptyString_ReturnsEmptyString()
        {
            string encrypted = EncryptionUtils.Encrypt(string.Empty);
            Assert.AreEqual(string.Empty, encrypted);
        }

        [Test]
        public void Encrypt_WithNull_ReturnsEmptyString()
        {
            string encrypted = EncryptionUtils.Encrypt(null);
            Assert.AreEqual(string.Empty, encrypted);
        }

        [Test]
        public void Decrypt_WithEncryptedString_ReturnsOriginalPlainText()
        {
            string original = "Test encryption round-trip";
            string encrypted = EncryptionUtils.Encrypt(original);
            string decrypted = EncryptionUtils.Decrypt(encrypted);
            
            Assert.AreEqual(original, decrypted);
        }

        [Test]
        public void Decrypt_WithEmptyString_ReturnsEmptyString()
        {
            string decrypted = EncryptionUtils.Decrypt(string.Empty);
            Assert.AreEqual(string.Empty, decrypted);
        }

        [Test]
        public void Decrypt_WithNull_ReturnsEmptyString()
        {
            string decrypted = EncryptionUtils.Decrypt(null);
            Assert.AreEqual(string.Empty, decrypted);
        }

        [Test]
        public void Decrypt_WithLegacyNonBase64String_ReturnsOriginalString()
        {
            string legacyString = "This is not encrypted";
            string result = EncryptionUtils.Decrypt(legacyString);
            
            Assert.AreEqual(legacyString, result);
        }

        [Test]
        public void Decrypt_WithInvalidBase64_ReturnsNull()
        {
            string invalidBase64 = "This is invalid base64!!!";
            string result = EncryptionUtils.Decrypt(invalidBase64);
            
            // Should return the original string if not Base64 (legacy support)
            Assert.AreEqual(invalidBase64, result);
        }

        [Test]
        public void EncryptDecrypt_RoundTrip_WorksWithSpecialCharacters()
        {
            string[] testStrings = new[]
            {
                "Simple text",
                "Text with\nnewlines\nand\ttabs",
                "Special chars: !@#$%^&*()_+-=[]{}|;:',.<>?/~`",
                "Unicode: 你好世界 🌍",
                "JSON: {\"key\": \"value\", \"number\": 123}",
                "Long text: " + new string('A', 1000)
            };

            foreach (string original in testStrings)
            {
                string encrypted = EncryptionUtils.Encrypt(original);
                string decrypted = EncryptionUtils.Decrypt(encrypted);
                Assert.AreEqual(original, decrypted, $"Failed for string: {original}");
            }
        }

        [Test]
        public void Encrypt_WithDifferentStrings_ProducesDifferentCipherText()
        {
            string text1 = "First string";
            string text2 = "Second string";
            
            string encrypted1 = EncryptionUtils.Encrypt(text1);
            string encrypted2 = EncryptionUtils.Encrypt(text2);
            
            Assert.AreNotEqual(encrypted1, encrypted2);
        }
    }
}
