using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Extensions
{
    public static class StringExtensions
    {
        private static readonly int STRINGNUM = 199;
        public static string FillEmptyString(this string str)
        {
            if (str.Length < STRINGNUM)
            {
                int length = STRINGNUM - str.Length;
                StringBuilder sb = new StringBuilder(str);
                for(int i=0;i<length;i++)
                {
                    sb.Append(" ");
                }
                return sb.ToString();
            }
            return str;
        }

        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
    }

}
