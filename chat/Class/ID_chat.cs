using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat.Class
{
    class ID_chat
    {
        public static string Xor(string string1,string string2)
        {

            byte[] bytes1 = Encoding.UTF8.GetBytes(string1);
            byte[] bytes2 = Encoding.UTF8.GetBytes(string2);

            byte[] xorResult = new byte[Math.Max(bytes1.Length, bytes2.Length)];
            for (int i = 0; i < xorResult.Length; i++)
            {
                byte byte1 = i < bytes1.Length ? bytes1[i] : (byte)0;
                byte byte2 = i < bytes2.Length ? bytes2[i] : (byte)0;
                xorResult[i] = (byte)(byte1 ^ byte2);
            }

            // Chuyển đổi kết quả XOR thành chuỗi base16
            string base16Result = BitConverter.ToString(xorResult).Replace("-", string.Empty).ToLower();

            // Lấy 10 kí tự từ chuỗi base16
            string first10Characters = base16Result.Substring(0, Math.Min(10, base16Result.Length));

            return first10Characters;
        }
    }
}
