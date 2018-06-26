using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Model
{
    public enum PackPeriod
    {
        GetChangedFile,
        Analysis,
        Compilie,
        Pack,
        Compress
    }
}
