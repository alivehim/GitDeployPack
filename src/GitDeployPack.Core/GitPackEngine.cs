﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitDeployPack.Model;
using System.Reactive.Linq;
using System.IO;
using GitDeployPack.Logger;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace GitDeployPack.Core
{
    public class GitPackEngine : IGitPackEngine
    {
        private  Options options;
        private readonly IGitCommandHelper _gitCommandHelper;
        private readonly ILogger logger;
        private readonly IPathService pathService;
        private readonly IChangedFilePreparer changedFilePreparer;
        private readonly IVisualStudioProjectCompiler projectCompiler;
        private readonly IFilePackService filePackService;
        private readonly IProjectFinder projectFinder;
        private readonly IPackageCompressService compressService;
        public IPathService PathService => pathService;
        public Options Options => options;
        public IGitCommandHelper GitCommandHelper => _gitCommandHelper;
        public IChangedFilePreparer ChangedFilePreparer => changedFilePreparer;
        public IVisualStudioProjectCompiler ProjectCompiler => projectCompiler;

        public IFilePackService FilePackService => filePackService;
        public IProjectFinder ProjectFinder => projectFinder;
        public IPackageCompressService CompressService => compressService;

        public ILogger Logger => logger;

        public GitPackEngine(Options options,
            IGitCommandHelper gitCommandHelper,
            IChangedFilePreparer projectPreparer,
            IVisualStudioProjectCompiler projectCompiler,
            IPathService pathServie,
            IFilePackService filePackService,
            IProjectFinder projectFinder,
            IPackageCompressService compressService,
            ILogger logger)
        {
            this.options = options;
            this._gitCommandHelper = gitCommandHelper;
            this.changedFilePreparer = projectPreparer;
            this.logger = logger;
            this.pathService = pathServie;
            this.projectCompiler = projectCompiler;
            this.projectFinder = projectFinder;
            this.filePackService = filePackService;
            this.compressService = compressService;
        }
        private ChangedFileList PrepareChangedFile(Options options)
        {
            ChangedFileList fileList = new ChangedFileList();
            var changeFilelist = GitCommandHelper.CompareBranch(options.OriginRepository, options.NewRepository,options.GitWorkPath);
            fileList.AddRange(changeFilelist);
            return fileList;
        }

        public IDisposable Run()
        {
            Console.WriteLine("start....");
            if(GitCommandHelper.GetCurrentBranch(options.GitWorkPath)!=options.NewRepository)
            {
                logger.Information($"branch error {options.NewRepository}  ");
                return new CompositeDisposable();
            }


            var filelist = PrepareChangedFile(options);
            try
            {
                CompositeDisposable composite = new CompositeDisposable();
                var projects = ChangedFilePreparer.Analysis(filelist);
                // deploy project is exists
                if (ProjectFinder.FindDeployProject(projects))
                {
                    var compileSubscription = projects.ToObservable();
                    PathService.ClearTemperary();

                    var moveFileSubscription = compileSubscription.Publish();
                    IDisposable subscription = compileSubscription
                        .ObserveOn(ThreadPoolScheduler.Instance)
                        .Subscribe(
                           m =>
                           {
                               if (!ProjectCompiler.Compile(m))
                               {
                                   composite.Dispose();
                               }

                               FilePackService.Pack(m);
                           },
                           ex => Logger.Error("fatal error", ex),
                           () =>
                           {
                               Logger.Information("Compile Completed......");
                               if(CompressService.Compress())
                                    Logger.Information("over .....");
                           }
                       );

                    composite.Add(subscription);
                    //Logger.Information("aaaaaaaaaa...");
                    //var packSubscription = moveFileSubscription.RefCount()
                    //    .TakeUntil(compileSubscription)
                    //    .ObserveOn(ThreadPoolScheduler.Instance).
                    //   Subscribe(m =>
                    //   {
                    //       FilePackService.Pack(m);
                    //       Logger.Information(m.ToString());
                    //   },
                    //      ex => Logger.Error("fatal error", ex),
                    //      () => Logger.Information("Pack Completed....."));

                    //composite.Add(packSubscription);

                    moveFileSubscription.Connect();



                    return composite;
                }
                else
                    throw new Exception("find project error....");
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
    }
}
