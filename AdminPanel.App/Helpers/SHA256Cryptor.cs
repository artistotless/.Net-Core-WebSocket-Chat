using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.App.Helpers
{
    public static class SHA256Cryptor
    {
        private const string _SALT = "THISISSALT";
        public async static Task<string> CreateHashAsync(string text)
        {
            var sha256 = SHA256.Create();
            Stream byteStream = new MemoryStream(Encoding.UTF8.GetBytes(text + _SALT));
            byte[] byteHash = await sha256.ComputeHashAsync(byteStream);
            string resultHash = Convert.ToBase64String(byteHash);
            return resultHash;
        }
    }
}
