//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// Lets the user navigate through soundcloud charts
    /// </summary>
    public sealed partial class ExploreView
    {
        /// <summary>
        /// The likes model that contains or the users liked tracks
        /// </summary>
        private Models.ChartModel ChartsModel { get; } = new Models.ChartModel();

        public ExploreView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("Explore Page");
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShuffleTracksAsync(ChartsModel.ToList(), ChartsModel.Token);
        }

        public async void PlayAllItems()
        {
            if (ChartsModel.FirstOrDefault() == null)
                return;

            var startPlayback = await PlaybackService.Current.StartMediaPlayback(ChartsModel.ToList(), ChartsModel.Token);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            var startPlayback = await PlaybackService.Current.StartMediaPlayback(ChartsModel.ToList(), ChartsModel.Token, false, (Core.API.Endpoints.Track)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();
        }

        /// <summary>
        /// Combobox for trending selection and 
        /// type of song.
        /// </summary>
        public void OnComboBoxChanged(object sender, SelectionChangedEventArgs e)
        {
            // Dislay the loading ring
            App.IsLoading = true;

            // Get the combo box
            var comboBox = sender as ComboBox;

            // See which combo box we got
            switch (comboBox?.Name)
            {
                case "ExploreTypeCombo":
                    ChartsModel.Kind = (comboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                    break;
                case "ExploreGenreCombo":
                    ChartsModel.Genre = (comboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
                    break;
            }

            ChartsModel.RefreshItems();

            // Hide loading ring
            App.IsLoading = false;
        }
    }
}
