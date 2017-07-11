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

namespace SoundByte.UWP.Views.CoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomeView
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("Welcome Page");

            App.IsLoading = true;

            try
            {
                // Get the changelog string from the azure api
                using (var httpClient = new HttpClient())
                {
                    var content =
                        await httpClient.GetStringAsync(
                            new Uri("http://gridentertainment.azurewebsites.net/api/soundbyte/welcome"));

                    PageContent.Text = content;
                }
            }
            catch (Exception ex)
            {
                TelemetryService.Current.TrackException(ex);
                PageContent.Text = "Something went wrong, we are trying our best to fix it!";
            }

            App.IsLoading = false;
        }
    }
}
