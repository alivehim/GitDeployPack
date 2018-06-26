using Autofac;
using CommandLine;
using GitDeployPack.Core;
using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GitDeployPack
{
    class Program
    {
        static void Main(string[] args)
        {
          
            var option = CommandLine.Parser.Default.ParseArguments<Options>(args)
               .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
               .WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("parmeter error");
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {

            DependencyRegistrar.Register((builder) => {
                builder.Register(c => opts).As<Options>().SingleInstance();
                builder.Register(c => new GitFilePackContext()).As<GitFilePackContext>().SingleInstance();
            });
            
            var packEngine= ContainerManager.Resolve<IGitPackEngine>();

            try
            {
                var subscription = packEngine.Run();
                var logger = ContainerManager.Resolve<ILogger>();
                logger.Information("Press ENTER to Unpack...");
                Console.ReadLine();
                subscription.Dispose();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
