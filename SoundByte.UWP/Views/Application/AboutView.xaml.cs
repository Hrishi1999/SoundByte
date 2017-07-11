//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views.CoreApp;
using SoundByte.UWP.Views.General;

namespace SoundByte.UWP.Views.Application
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the app version
            AppVersion.Text = $"Version: {Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision}";
            AppBuildBranch.Text = "Build Branch: ...";
            AppBuildVersion.Text = "Build Number: ...";
            AppBuildTime.Text = "Build Time: ...";

            var dataFile = await Package.Current.InstalledLocation.GetFileAsync(@"Assets\build_info.json");
            var buildData = await Task.Run(() => JsonConvert.DeserializeObject<BuildInfo>(File.ReadAllText(dataFile.Path)));

            AppBuildBranch.Text = "Build Branch: " + buildData.BuildBranch;
            AppBuildVersion.Text = "Build Number: " + buildData.BuildNumber;
            AppBuildTime.Text = "Build Time: " + buildData.BuildTime;

            TelemetryService.Current.TrackPage("About Page");
        }

        public void NavigateWelcome()
        {
            App.NavigateTo(typeof(WelcomeView));
        }

        public void NavigateNew()
        {
            App.NavigateTo(typeof(WhatsNewView));
        }

        /// <summary>
        /// Called when the user taps on the rate_review button
        /// </summary>
        public async void RateAndReview()
        {
            TelemetryService.Current.TrackPage("Rate and Review App");

            await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
        } 
    }
}