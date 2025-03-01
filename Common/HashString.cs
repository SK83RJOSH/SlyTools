using System.Text;

namespace SlyTools.Common.Hashing
{
    public static class HashString
    {
        public static uint Get(string str)
        {
            uint result = 0;
            
            foreach (byte b in Encoding.ASCII.GetBytes(str))
            {
                byte c = b;

                if (c >= 97 && c <= 122)
                {
                    c -= 32;
                }

                result = (result * 31) + c;
            }

            return result;
        }
    }
}
