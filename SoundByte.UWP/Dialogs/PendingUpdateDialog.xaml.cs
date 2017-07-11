//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class PendingUpdateDialog
    {
        public PendingUpdateDialog()
        {
            InitializeComponent();
        }

        public void DeferUpdate()
        {
            TelemetryService.Current.TrackEvent("Defer Update");
            Hide();
        }

        private async void UpdateNow(object sender, RoutedEventArgs e)
        {
            // Setup the UI
            UpdateBar.Visibility = Visibility.Visible;
            UpdateButton.IsEnabled = false;
            CloseButton.IsEnabled = false;

            // Get a list of updates
            var updates = await StoreContext.GetDefault().GetAppAndOptionalStorePackageUpdatesAsync();

            // Download and install the updates.
            var downloadOperation = StoreContext.GetDefault()
                .RequestDownloadAndInstallStorePackageUpdatesAsync(updates);

            // The Progress async method is called one time for each step in the download
            // and installation process for each package in this request.
            downloadOperation.Progress = async (asyncInfo, progress) =>
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    UpdateBar.Value = progress.PackageDownloadProgress;
                });
            };

            var result = await downloadOperation.AsTask();

            Hide();
        }
    }
}
