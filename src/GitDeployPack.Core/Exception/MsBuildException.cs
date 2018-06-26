using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack.Exceptionx
{
    public class MsBuildException : Exception
    {
        public MsBuildException(string msg) : base(msg)
        {

        }

        public MsBuildException(string message, Exception inner)
            : base(message, inner)
        { }

        public override string Message
        {
            get
            {
                return $"{ base.Data["File"].ToString()} -{base.Data["Message"].ToString()}";
            }
        }
    }
}
