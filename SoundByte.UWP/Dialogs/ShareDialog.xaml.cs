//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class ShareDialog
    {
        public Track Track { get; }

        public ShareDialog(Track trackItem)
        {
            // Do this before the xaml is loaded, to make sure
            // the object can be binded to.
            Track = trackItem;

            // Load the XAML page
            InitializeComponent();
        }

        private void ShareWindows(object sender, RoutedEventArgs e)
        {
            // Load Resources
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            // Create a share event
            void ShareEvent(DataTransferManager s, DataRequestedEventArgs a)
            {
                var dataPackage = a.Request.Data;
                dataPackage.Properties.Title = "SoundByte";
                dataPackage.Properties.Description = "Share this track with Windows 10.";
                dataPackage.SetText("Listen to " + Track.Title + " by " + Track.User.Username + " on #SoundByte #Windows10: " + Track.PermalinkUri);
            }

            // Remove any old share events
            DataTransferManager.GetForCurrentView().DataRequested -= ShareEvent;
            // Add this new share event
            DataTransferManager.GetForCurrentView().DataRequested += ShareEvent;
            // Show the share dialog
            DataTransferManager.ShowShareUI();
            // Hide the popup
            Hide();
            // Track Event
            TelemetryService.Current.TrackEvent("Share Menu - Windows Share");
        }

        private void ShareLink(object sender, RoutedEventArgs e)
        {
            // Create a data package
            var data = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
            // Set the link to the track on soundcloud
            data.SetText(Track.PermalinkUri);
            // Set the clipboard content
            Clipboard.SetContent(data);
            // Hide the popup
            Hide();
            // Track Event
            TelemetryService.Current.TrackEvent("Share Menu - Copy General Link");
        }

        private void ShareSoundByte(object sender, RoutedEventArgs e)
        {
            // Create a data package
            var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
            // Set the link to the track on soundcloud
            dataPackage.SetText("soundbyte://core/track?id=" + Track.Id);
            // Set the clipboard content
            Clipboard.SetContent(dataPackage);
            // Hide the popup
            Hide();
            // Track Event
            TelemetryService.Current.TrackEvent("Share Menu - Copy SoundByte Link");
        }
    }
}
