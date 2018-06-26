using System;

namespace GitDeployPack.Model
{
    [Flags]
    public enum VsProjectType
    {
        Undefined = 0,
        Web = 1,
        Console = 2,
        Service = 4,
        ClassLibrary = 8,
        Deployment = 16,
        Database = 32,
        Test = 64,
        WindowsApplication = 128,
        NetCore = 256
    }
}