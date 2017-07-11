//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views.General
{
    /// <summary>
    /// Open a webview with the current changelog
    /// </summary>
    public sealed partial class WhatsNewView
    {
        public WhatsNewView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("What's New Page");

            App.IsLoading = true;

            try
            {
                // Get the changelog string from the azure api
                using (var httpClient = new HttpClient())
                {
                    var changelog =
                        await httpClient.GetStringAsync(
                            new Uri("http://gridentertainment.azurewebsites.net/api/soundbyte/changelog"));

                    ChangelogView.Text = changelog;
                }
            }
            catch (Exception)
            {
                ChangelogView.Text = "*Error:* An error occured while getting the changelog.";
            }

            App.IsLoading = false;
        }
    }
}
