using GitDeployPack.Extensions;
using GitDeployPack.Infrastructure;
using GitDeployPack.Model;
using GitDeployPack.Setting;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitDeployPack.Core.FilePack
{
    public class PackageCompressService : IPackageCompressService
    {
        private string fileNamePattern = @"\((?<key>[\d]*)\)";
        private IPathService _pathService;
        private readonly Options _options;
        private readonly PackSetting _packSetting;
        public PackageCompressService(
            IPathService pathService,
            Options option,
            PackSetting packSetting
            )
        {
            this._pathService = pathService;
            this._options = option;
            this._packSetting = packSetting;
        }

        public bool Compress()
        {
            return Zip($"{_pathService.PackLocation}\\{GetZipFileName()}.zip");
        }

        private bool Zip(string zipFilePath)
        {
            var locations = new string[] {
                _pathService.StaticLocation.FullName,
                _pathService.AssemblyLocation.FullName,
                _pathService.ScriptLocation.FullName
            };
            FileInfo file = new FileInfo(zipFilePath);
            if (!file.Exists)
            {
                return ZipHelper.ZipDirectory(locations, zipFilePath);
            }
            else
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(zipFilePath);
                var sameFiles=file.Directory.GetFiles().Where(p => p.Name.Contains(fileNameWithoutExtension));
                var latestFileName = sameFiles.OrderByDescending(p => p.CreationTime).First().Name;
                var mc=Regex.Match(latestFileName, fileNamePattern);
                if(mc.Success)
                {
                    if(int.TryParse(mc.Groups["key"].Value, out int index))
                    {
                        index += 1;
                        var newFileName = file.Name.Replace(fileNameWithoutExtension, $"{fileNameWithoutExtension}({index})");
                        zipFilePath = $"{file.Directory}\\{newFileName}";
                        return ZipHelper.ZipDirectory(locations, zipFilePath);
                    }
                    else
                    {
                        throw new Exception("file name error");
                    }
                }
                else
                {
                    var newFileName = file.Name.Replace(fileNameWithoutExtension, $"{fileNameWithoutExtension}(1)");
                    zipFilePath = $"{file.Directory}\\{newFileName}";
                    return ZipHelper.ZipDirectory(locations, zipFilePath);
                }
            }           
        }

        private string GetZipFileName()
        {
            var filename = _options.PackNamePattern?? _packSetting.PackNamePattern;
            if (filename.IsNotEmpty())
            {
                return Regex.Replace(filename, @"\{(?<key>.*?)\}", new MatchEvaluator((m) => DateTime.Now.ToString(m.Groups["key"].Value)));
            }
            return Guid.NewGuid().ToString();
        }
    }
}
