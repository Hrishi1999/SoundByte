//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views.Me;

namespace SoundByte.UWP.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public bool IsComboboxBlockingEnabled { get; set; }

        /// <summary>
        /// Disconnect the users soundcloud account
        /// </summary>
        public void DisconnectSoundCloudAccount()
        {
            // Disconnect from the service
            SoundByteService.Current.DisconnectService();
            // Navigate to the explore page
            App.NavigateTo(typeof(Views.HomeView));
        }

        /// <summary>
        /// Disconnect the users fanburst
        /// </summary>
        public void DisconnectFanburstAccount()
        {
            // Disconnect from the service
            SoundByteService.Current.DisconnectService();
            // Navigate to the explore page
            App.NavigateTo(typeof(Views.HomeView));
        }

        /// <summary>
        /// Connect the users fanburst account
        /// </summary>
        public void ConnectFanburstAccount()
        {
            // Navigate to the explore page
            App.NavigateTo(typeof(LoginView), "fanburst");
        }


        /// <summary>
        /// Connect the users soundcloud account
        /// </summary>
        public void ConnectSoundCloudAccount()
        {
            // Navigate to the explore page
            App.NavigateTo(typeof(LoginView), "soundcloud");
        }

        public async void ClearAppCache()
        {
            // Create a message dialog
            var dialog = new ContentDialog
            {
                Title = "Clear Application Cache?",
                Content = new TextBlock { Text = "Warning: Clearing Application cache will delete the following things:\n• Cached Images.\n• Jumplist Items.\n• Pinned Live Tiles.\n• Notifications.\n\n To Continue press 'Clear Cache', this may take a while.", TextWrapping = TextWrapping.Wrap },
                PrimaryButtonText = "Clear Cache",
                SecondaryButtonText = "Cancel",
                IsPrimaryButtonEnabled = true,
                IsSecondaryButtonEnabled = true
            };

            var response = await dialog.ShowAsync();

            if (response != ContentDialogResult.Primary)
            {
                return;
            }

            // Clear all jumplist items
            await JumplistHelper.RemoveAllAsync();
            // Clear all the live tiles
            await TileService.Current.RemoveAllAsync();
            // Remove all cached images from the app
            var rootCacheFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("cache", CreationCollisionOption.OpenIfExists);
            await rootCacheFolder.DeleteAsync();
            // Remove all toast notifications
            ToastNotificationManager.History.Clear();
        }

        /// <summary>
        /// Changes the language string and pompts the user to restart the app.
        /// </summary>
        public async void ChangeLangauge(object sender, SelectionChangedEventArgs e)
        {
            if (IsComboboxBlockingEnabled)
                return;

            // Get the langauge string
            var comboBoxItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            if (comboBoxItem != null)
            {
                var languageString = (comboBoxItem.Tag) as string;

                // If the langauge is the same, do nothing
                if (SettingsService.Current.CurrentAppLanguage == languageString || IsComboboxBlockingEnabled || string.IsNullOrEmpty(SettingsService.Current.CurrentAppLanguage))
                    return;

                // Set the current langauge
                SettingsService.Current.CurrentAppLanguage = languageString;
            }
            // Get the resource loader
            var resources = ResourceLoader.GetForCurrentView();
            // Create the app restart dialog
            var restartAppDialog = new ContentDialog
            {
                Title = resources.GetString("LanguageRestart_Title"),
                Content = new TextBlock { TextWrapping = TextWrapping.Wrap, Text = resources.GetString("LanguageRestart_Content") },
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = resources.GetString("LanguageRestart_Button")
            };
            // Show the dialog and get the respose
            var response = await restartAppDialog.ShowAsync();
            // Restart the app if the user canceled or clicked the button
            if (response == ContentDialogResult.Primary || response == ContentDialogResult.None || response == ContentDialogResult.Secondary)
            {
                // Exit the app
                Application.Current.Exit();
            }
        }
    }
}
