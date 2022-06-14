using AdminPanel.App.Helpers;
using NUnit.Framework;
using System;
using System.Text;

namespace AdminPanel.Tests
{
    class PasswordCryptorTest
    {
        [Test]
        public void Every_Time_The_Same_Hash()
        {
            string password = "test";
            string expectedHash = "Z3NdhO3TeIBSGdNfQAZsJvWymeWfCuZzBSeBVIXpvHg=";
            var actualHash = SHA256Cryptor.CreateHashAsync(password).Result;
            Assert.AreEqual(expectedHash, actualHash);
        }

        [Test]
        public void Hash_Length_Less_64()
        {
            StringBuilder text = new StringBuilder();
            int textLength = new Random().Next(10, 40);
            for (int i = 0; i < textLength; i++)
            {
                text.Append((char)i);
            }
            string password = text.ToString();
            var hash = SHA256Cryptor.CreateHashAsync(password).Result;
            Assert.IsTrue(hash.Length < 64);

        }
    }
}
