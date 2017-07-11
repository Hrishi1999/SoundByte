//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.UI.Xaml.Controls;
using SoundByte.UWP.Services;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    /// Let the user view their playlists
    /// </summary>
    public sealed partial class PlaylistsView
    {
        /// <summary>
        /// The playlist model that contains the users playlists / liked playlists
        /// </summary>
        private Models.UserPlaylistModel PlaylistModel { get; } = new Models.UserPlaylistModel();

        public PlaylistsView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("User Playlist Page");
        }

        public void NavigatePlaylist(object sender, ItemClickEventArgs e)
        {
            App.NavigateTo(typeof(Playlist), e.ClickedItem as Core.API.Endpoints.Playlist);   
        }
    }
}
