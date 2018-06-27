using Autofac;
using GitDeployPack.Core;
using GitDeployPack.Core.FilePack;
using GitDeployPack.Core.MSBuild;
using GitDeployPack.Core.ProjectCompile;
using GitDeployPack.Core.ProjectParser;
using GitDeployPack.Infrastructure;
using GitDeployPack.Logger;
using GitDeployPack.Setting;
using GitDeployPacke.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDeployPack
{
    public class DependencyRegistrar
    {
        public static IContainer Container { get; private set; }
        public static  void Register(Action<ContainerBuilder> action)
        {
            var builder = new Autofac.ContainerBuilder();

            builder.Register(x => SettingFactory.Create<PackSetting>()).As<PackSetting>().InstancePerLifetimeScope();

            builder.RegisterType<GitCommandHelper>().As<IGitCommandHelper>().InstancePerLifetimeScope();
            builder.RegisterType<PathService>().As<IPathService>().SingleInstance();
            builder.RegisterType<GitPackEngine>().As<IGitPackEngine>().InstancePerLifetimeScope();
            builder.RegisterType<FileAnalysisFactory>().As<IFileAnalysisFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectFilePreparer>().As<IChangedFilePreparer>().InstancePerLifetimeScope();

            builder.RegisterType<ScreenLogger>().As<ILogger>().SingleInstance();
            builder.RegisterType<VisualStudioProjectCompiler>().As<IVisualStudioProjectCompiler>().InstancePerLifetimeScope();
            //builder.RegisterType<FakeCompilier>().As<IVisualStudioProjectCompiler>().InstancePerLifetimeScope();
            builder.RegisterType<FilePackService>().As<IFilePackService>().InstancePerLifetimeScope();

            builder.RegisterType<BuildServiceFactory>().As<IBuildServiceFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectParserServiceFactory>().As<IProjectParserServiceFactory>().InstancePerLifetimeScope();
            builder.RegisterType<VisualStudioProjectFinder>().As<IProjectFinder>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectFilter>().As<IProjectFilter>().SingleInstance();
            builder.RegisterType<PackageCompressService>().As<IPackageCompressService>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectDiffer>().As<IProjectDiffer>().InstancePerLifetimeScope();
            builder.RegisterType<VisualStudioSolutionFinder>().As<ISolutionFinder>().InstancePerLifetimeScope();
            builder.RegisterType<VisualStudioSolutionParser>().As<ISolutionParser>().InstancePerLifetimeScope();
            builder.RegisterType<NugetPackageManager>().As<INugetPackageManager>().InstancePerLifetimeScope();

            action(builder);

            IContainer iocContainer = builder.Build();
            ContainerManager.SetContainer(iocContainer);
        }
    }
}
