using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace GitDeployPack.Infrastructure
{
    public class GitToolkit 
    {
        public static string PercentDecode(string url)
        {
            string ss = @"\345\217\221\345\270\203/2018.04.16\345\220\216\345\217\260\344\273\252\350\241\250\347\233\230\346\233\264\346\226\260\345\255\230\345\202\250\350\277\207\347\250\213";

            var mc=Regex.Split(ss, "\\\\");
            StringBuilder sb = new StringBuilder();
            foreach(var item in mc)
            {
                if (item != "")
                {
                    sb.Append( String.Format("%{0:X}", Convert.ToInt32(item, 8)));
                }
                   
            }

            Console.WriteLine(HttpUtility.UrlDecode(sb.ToString(), Encoding.UTF8));

            return null;
            
        }
    }
}
