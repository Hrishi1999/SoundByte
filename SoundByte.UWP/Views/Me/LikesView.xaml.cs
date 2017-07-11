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

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    /// Lets the user view their likes
    /// </summary>
    public sealed partial class LikesView
    {
        /// <summary>
        /// The likes model that contains or the users liked tracks
        /// </summary>
        private Models.LikeModel LikesModel { get; } = new Models.LikeModel(SoundByteService.Current.CurrentUser);

        public LikesView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("User Likes");
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShuffleTracksAsync(LikesModel.ToList(), LikesModel.Token);
        }

        public async void PlayAllItems()
        {
            // We are loading
            App.IsLoading = true;

            var startPlayback = await PlaybackService.Current.StartMediaPlayback(LikesModel.ToList(), LikesModel.Token);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing likes.").ShowAsync();
            
            // We are not loading
            App.IsLoading = false;
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            var startPlayback = await PlaybackService.Current.StartMediaPlayback(LikesModel.ToList(), LikesModel.Token, false, (Core.API.Endpoints.Track)e.ClickedItem);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing likes.").ShowAsync();

            // We are not loading
            App.IsLoading = false;
        }
    }
}
