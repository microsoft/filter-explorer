/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Commands;
using FilterExplorer.Models;
using FilterExplorer.Views;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilterExplorer.ViewModels
{
    public class AboutPageViewModel : ViewModelBase
    {
        public IDelegateCommand GoBackCommand { get; private set; }

        public Version Version { get; set; }

        public AboutPageViewModel()
        {
            GoBackCommand = CommandFactory.CreateGoBackCommand();

            var packageVersion = Windows.ApplicationModel.Package.Current.Id.Version;

            Version = new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }

        public override Task<bool> InitializeAsync()
        {
            IsInitialized = true;

            return Task.FromResult(IsInitialized);
        }
    }
}
