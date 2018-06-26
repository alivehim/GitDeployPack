using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Model
{
    public enum AnalysisFileType
    {
        [Description("unkown")]
        UNKOWN,
        [Description("cs")]
        CS,
        [Description("html")]
        HTML,
        [Description("cshtml")]
        CSHTML,
        [Description("css")]
        CSS,
        [Description("js")]
        JS,
        [Description("json")]
        JSON,
        [Description("csproj")]
        CSPROJ,
        [Description("xml")]
        XML,
        [Description("config")]
        CONFIG
       
    }
}
