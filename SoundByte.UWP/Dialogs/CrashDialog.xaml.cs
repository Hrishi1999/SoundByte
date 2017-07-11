//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.Web.Http;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Dialogs
{
    /// <summary>
    /// A dialog to notify the user that the app has crashed.
    /// </summary>
    public sealed partial class CrashDialog
    {
        public CrashDialog(Exception ex)
        {
            InitializeComponent();

            RestartButton.Focus(FocusState.Programmatic);
            MoreInfo.Text = ex.Message;

            ProgressRing.Visibility = Visibility.Collapsed;
        }

        private async Task Send()
        {
            if (!App.HasInternet)
                return;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    string description;

                    Description.Document.GetText(TextGetOptions.None, out description);

                    if (string.IsNullOrEmpty(Contact.Text))
                        Contact.Text = "default@gridentertainment.net";

                    if (string.IsNullOrEmpty(description))
                        description = "n/a";

                    var param = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Title", MoreInfo.Text),
                        new KeyValuePair<string, string>("Description", description),
                        new KeyValuePair<string, string>("Category", "AutoGenerate"),
                        new KeyValuePair<string, string>("ContactEmail", Contact.Text),
                    };

                    var request = await httpClient.PostAsync(new Uri("http://gridentertainment.net/Tickets/Create"), new HttpFormUrlEncodedContent(param));
                    request.EnsureSuccessStatusCode();
                }
            }
            catch
            {
                // ignored
            }
        }

        private async void SendAndCloseApp(object sender, RoutedEventArgs e)
        {
            ProgressRing.Visibility = Visibility.Visible;
            TelemetryService.Current.TrackEvent("Crash Dialog - Send and Close App");
            await Send();
            Application.Current.Exit();
        }

        private async void SendAndContinue(object sender, RoutedEventArgs e)
        {
            ProgressRing.Visibility = Visibility.Visible;
            TelemetryService.Current.TrackEvent("Crash Dialog - Send and Continue");
            Hide();
            await Send();     
        }
    }
}
