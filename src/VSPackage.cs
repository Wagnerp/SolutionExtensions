﻿using System;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SolutionExtensions
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.guidVSPackageString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class VSPackage : Package
    {
        public static DTE2 DTE { get; set; }

        protected override void Initialize()
        {
            // Initialize shared components
            DTE = GetService(typeof(DTE)) as DTE2;

            Logger.Initialize(this, Vsix.Name);

            Settings.Initialize(this);
            SolutionHandler.Initialize(DTE);
            SuggestionHandler.Initialize();
            DocumentOpenListener.Initialize(DTE.Events.DocumentEvents);

            // Initialize other components
            var repository = (IVsExtensionRepository)GetService(typeof(SVsExtensionRepository));
            var manager = (IVsExtensionManager)GetService(typeof(SVsExtensionManager));
            ExtensionInstalledChecker.Initialize(this, repository, manager);
            ShowMissingCommand.Initialize(this);
            ModifyMissingCommand.Initialize(this);
            ShowSuggestionsCommand.Initialize(this, repository, manager);
            InfoBarService.Initialize(this, repository, manager);

            base.Initialize();
        }

        public static string GetSolution()
        {
            if (DTE.Solution == null)
                return null;

            return DTE.Solution.FullName;
        }
    }
}
