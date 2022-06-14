
using System;
using System.Text;

namespace AdminPanel.App.Helpers
{
    public static class Extensions
    {
        public static ArraySegment<byte> ToArraySegment(this string source)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(source);
            var segment = new ArraySegment<byte>(buffer, 0, buffer.Length);
            return segment;
        }
    }
}
