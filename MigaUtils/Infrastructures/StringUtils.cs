using System.Text;

namespace Acorisoft.Miga.Utils.Infrastructures
{
    public static class StringUtils
    {
        public static bool EqualsWithIgnoreCase(this string src, string dst)
        {
            return !string.IsNullOrEmpty(dst) && string.Equals(src, dst, StringComparison.OrdinalIgnoreCase);
        }

        public static bool StartWithEx(this string src, char character)
        {
            if (string.IsNullOrEmpty(src)) return false;
            var length = src.Length;
            
            for (var i = 0; i < length; i++)
            {
                switch (src[i])
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case '\x20':
                        continue;
                }

                return src[i] == character;
            }

            return false;
        }
        
        public static int GetRepeatCount(this string src, char character, out int startIndex)
        {
            startIndex = -1;
            
            if (string.IsNullOrEmpty(src))
            {
                return 0;
            }
            var length = src.Length;
            var count = 0;
            
            for (var i = 0; i < length; i++)
            {
                switch (src[i])
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case '\x20':
                        continue;
                }

                if (src[i] == character)
                {
                    if (startIndex == -1)
                    {
                        startIndex = i;
                    }
                    
                    count ++;
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        public static string Repeat(this string src, int count)
        {
            count = count == 0 ? 1 : count;
            var sb = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                sb.Append(src);
            }

            return sb.ToString();
        }
    }
}