//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.ViewModels
{
    public class PlaylistViewModel : BaseViewModel
    {
        #region Private Variables
        // The playlist object
        private Playlist _playlist;
        // List of tracks on the UI
        private ObservableCollection<Track> _tracks;
        // Icon for the pin button
        private string _pinButtonIcon = "\uE718";
        // Text for the pin button
        private string _pinButtonText;
        #endregion

        public PlaylistViewModel()
        {
            Tracks = new ObservableCollection<Track>();
        }

        public async Task SetupView(Playlist newPlaylist)
        {

            // Check if the models saved playlist is null
            if (newPlaylist != null && (Playlist == null || Playlist.Id != newPlaylist.Id))
            {
                // Set the playlist
                Playlist = newPlaylist;
                // Clear any existing tracks
                Tracks.Clear();

                // Get the resource loader
                var resources = ResourceLoader.GetForCurrentView();

                // Check if the tile is pinned
                if (TileService.Current.DoesTileExist("Playlist_" + Playlist.Id))
                {
                    PinButtonIcon = "\uE77A";
                    PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
                }
                else
                {
                    PinButtonIcon = "\uE718";
                    PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
                }

                try
                {
                    // Show the loading ring
                    App.IsLoading = true;
                    // Get the playlist tracks
                    var playlistTracks = (await SoundByteService.Current.GetAsync<Playlist>("/playlists/" + Playlist.Id)).Tracks;
                    playlistTracks.ForEach(x => Tracks.Add(x));
                    // Hide the loading ring
                    App.IsLoading = false;
                }
                catch (Exception)
                {
                    // Create the error dialog
                    var noItemsDialog = new ContentDialog
                    {
                        Title = "Could not load tracks",
                        Content = new TextBlock { TextWrapping = TextWrapping.Wrap, Text = "Something went wrong when trying to load the tracks for this playlist, please make sure you are connected to the internet and then go back, and click on this playlist again." },
                        IsPrimaryButtonEnabled = true,
                        PrimaryButtonText = "Close"
                    };
                    // Hide the loading ring
                    App.IsLoading = false;
                    // Show the dialog
                    await noItemsDialog.ShowAsync();
                }         
            }
        }

        #region Model
        /// <summary>
        /// Gets or sets a list of tracks in the playlist
        /// </summary>
        public ObservableCollection<Track> Tracks
        {
            get => _tracks;
            set
            {
                if (value == _tracks) return;

                _tracks = value;
                UpdateProperty();
            }
        }

        /// <summary>
        /// Gets or sets the current playlist object
        /// </summary>
        public Playlist Playlist
        {
            get => _playlist;
            set
            {
                if (value == _playlist) return;

                _playlist = value;
                UpdateProperty();
            }
        }

        public string PinButtonIcon
        {
            get => _pinButtonIcon;
            set
            {
                if (value != _pinButtonIcon)
                {
                    _pinButtonIcon = value;
                    UpdateProperty();
                }
            }
        }

        public string PinButtonText
        {
            get => _pinButtonText;
            set
            {
                if (value != _pinButtonText)
                {
                    _pinButtonText = value;
                    UpdateProperty();
                }
            }
        }

        /// <summary>
        /// Pins or unpins a playlist from the start
        /// menu / screen.
        /// </summary>
        public async void PinPlaylist()
        {
            // Show the loading ring
            App.IsLoading = true;
            // Get the resource loader
            var resources = ResourceLoader.GetForCurrentView();
            // Check if the tile exists
            if (TileService.Current.DoesTileExist("Playlist_" + Playlist.Id))
            {
                // Try remove the tile
                if (await TileService.Current.RemoveAsync("Playlist_" + Playlist.Id))
                {
                    PinButtonIcon = "\uE718";
                    PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
                }
                else
                {
                    PinButtonIcon = "\uE77A";
                    PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
                }
            }
            else
            {
                // Create the tile
                if (await TileService.Current.CreateTileAsync("Playlist_" + Playlist.Id, Playlist.Title, "soundbyte://core/playlist?id=" + Playlist.Id, new Uri(ArtworkConverter.ConvertObjectToImage(Playlist)), ForegroundText.Light))
                {
                    PinButtonIcon = "\uE77A";
                    PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
                }
                else
                {
                    PinButtonIcon = "\uE718";
                    PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
                }
            }
            // Hide the loading ring
            App.IsLoading = false;
        }

        /// <summary>
        /// Shuffles the tracks in the playlist
        /// </summary>
        public async void ShuffleItemsAsync()
        {         
            await ShuffleTracksAsync(Tracks.ToList(), $"playlist-{Playlist.Id}");
        }

        /// <summary>
        /// Called when the user taps on a sound in the
        /// Sounds tab
        /// </summary>
        public async void TrackClicked(object sender, ItemClickEventArgs e)
        {
            // Get the Click item
            var item = (Track)e.ClickedItem;

            var startPlayback = await PlaybackService.Current.StartMediaPlayback(Tracks.ToList(), $"playlist-{Playlist.Id}", false, item);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing playlist.").ShowAsync();
        }

        /// <summary>
        /// Starts playing the playlist
        /// </summary>
        public async void NavigatePlay()
        {
            var startPlayback = await PlaybackService.Current.StartMediaPlayback(Tracks.ToList(), $"playlist-{Playlist.Id}");

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing playlist.").ShowAsync();
        }
        #endregion
    }
}