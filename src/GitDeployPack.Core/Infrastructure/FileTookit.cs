using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Infrastructure
{
    public class FileTookit
    {
        public static void DeleteFile(string dirPath)
        {
            //去除文件夹和子文件的只读属性
            //去除文件夹的只读属性
            System.IO.DirectoryInfo fileInfo = new DirectoryInfo(dirPath);
            fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            //去除文件的只读属性
            System.IO.File.SetAttributes(dirPath, System.IO.FileAttributes.Normal);
            //判断文件夹是否还存在
            if (Directory.Exists(dirPath))
            {
                foreach (string f in Directory.GetFileSystemEntries(dirPath))
                {
                    if (File.Exists(f))
                    {
                        //如果有子文件删除文件
                        File.Delete(f);
                    }
                    else
                    {
                        //循环递归删除子文件夹 
                        DeleteFile(f);
                    }
                }
                //删除空文件夹 
                Directory.Delete(dirPath);
            }
        }
    }
}
