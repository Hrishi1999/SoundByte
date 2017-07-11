//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using SoundByte.Core.API.Exceptions;
using SoundByte.UWP.Models;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.ViewModels
{
    /// <summary>
    /// View model for the notifications page
    /// </summary>
    public class NotificationsViewModel : BaseViewModel
    {
        private NotificationModel _notificationItems;

        public NotificationsViewModel()
        {
            NotificationItems = new NotificationModel();
        }

        public override void Dispose()
        {
            _notificationItems.Clear();
            _notificationItems = null;

            GC.Collect();
        }

        public NotificationModel NotificationItems
        {
            get => _notificationItems;
            private set
            {
                if (value == _notificationItems) return;

                _notificationItems = value;
                UpdateProperty();
            }
        }

        #region Binding Methods
        /// <summary>
        /// Reloads any notifications on the page
        /// </summary>
        public void RefreshPage()
        {
            // Show the loading ring as this task can take a while
            App.IsLoading = true;

            // Reload the items
            NotificationItems.RefreshItems();

            // Hide the loading ring as we are done
            App.IsLoading = false;
        }

        /// <summary>
        /// Navigates to a notification
        /// </summary>
        public async void NotificationNavigate(object sender, ItemClickEventArgs e)
        {
            // Show the loading ring as this process can take a while
            App.IsLoading = true;
            // Get the notification collection
            var notification = (Notification)e.ClickedItem;
            // Switch between the notification types
            try
            {
                switch (notification.Type)
                {
                    case "track-like":
                        // Play this item
                        var startPlayback = await PlaybackService.Current.StartMediaPlayback(new List<Track> { notification.Track }, $"Notification-{notification.Track.Id}");
                        if (!startPlayback.success)
                            await new MessageDialog(startPlayback.message, "Error opening Notification.").ShowAsync();
                        break;
                    case "comment":
                        // Play this item
                        var startPlaybackComment = await PlaybackService.Current.StartMediaPlayback(new List<Track> { notification.Comment.Track }, $"Notification-{notification.Comment.Track.Id}");
                        if (!startPlaybackComment.success)
                            await new MessageDialog(startPlaybackComment.message, "Error opening Notification.").ShowAsync();
                        break;
                    case "affiliation":
                        // Navigate to the user page
                        App.NavigateTo(typeof(Views.UserView), notification.User);
                        break;
                }
            }
            catch (SoundByteException ex)
            {
                // Get the resource loader
                var resources = ResourceLoader.GetForCurrentView();
                // Create and display the error dialog
                await new ContentDialog
                {
                    Title = ex.ErrorTitle,
                    Content = new TextBlock { TextWrapping = TextWrapping.Wrap, Text = ex.ErrorDescription },
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonText = resources.GetString("Close_Button"),
                }.ShowAsync();
            }
            // Hide the loading ring now that data has been loaded and displayed
            App.IsLoading = false;
        }
        #endregion
    }
}
