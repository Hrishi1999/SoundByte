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
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Models;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;

namespace SoundByte.UWP.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        #region Models
        // Items that the user has liked
        public LikeModel LikeItems { get; } = new LikeModel(null);
        
        // Playlists that the user has liked / uploaded
        public PlaylistModel PlaylistItems { get; } = new PlaylistModel(null);

        // List of user followers
        public UserFollowersModel FollowersList { get; } = new UserFollowersModel(null, "followers");
        
        // List of user followings
        public UserFollowersModel FollowingsList { get; } = new UserFollowersModel(null, "followings");

        // List of users tracks
        public TrackModel TracksList { get; } = new TrackModel(null);

        #endregion

        // Icon for the pin button
        private string _pinButtonIcon = "\uE718";
        private string _followUserIcon = "\uE8FA";

        // Text for the pin button
        private string _pinButtonText;
        private string _followUserText;
        private User _user;
        private bool _showFollowButton = true;
        // The current pivot item
        private PivotItem _selectedPivotItem;

        public bool ShowFollowButton
        {
            get { return _showFollowButton; }
            set
            {
                if (value != _showFollowButton)
                {
                    _showFollowButton = value;
                    UpdateProperty();
                }
            }
        }

        /// <summary>
        /// The current pivot item that the user is viewing
        /// </summary>
        public PivotItem SelectedPivotItem
        {
            get { return _selectedPivotItem; }
            set
            {
                if (value != _selectedPivotItem)
                {
                    _selectedPivotItem = value;
                    UpdateProperty();
                }
            }
        }

        public string PinButtonIcon
        {
            get { return _pinButtonIcon; }
            set
            {
                if (value != _pinButtonIcon)
                {
                    _pinButtonIcon = value;
                    UpdateProperty();
                }
            }
        }

        public string FollowUserIcon
        {
            get { return _followUserIcon; }
            set
            {
                if (value != _followUserIcon)
                {
                    _followUserIcon = value;
                    UpdateProperty();
                }
            }
        }

        public string FollowUserText
        {
            get { return _followUserText; }
            set
            {
                if (value != _followUserText)
                {
                    _followUserText = value;
                    UpdateProperty();
                }
            }
        }

        public string PinButtonText
        {
            get { return _pinButtonText; }
            set
            {
                if (value != _pinButtonText)
                {
                    _pinButtonText = value;
                    UpdateProperty();
                }
            }
        }

        public User User
        {
            get => _user;
            private set
            {
                if (value == _user) return;

                _user = value;
                UpdateProperty();
            }
        }

        /// <summary>
        /// Setup this viewmodel for a specific user
        /// </summary>
        /// <param name="user"></param>
        public async Task UpdateModel(User user)
        {
            // Set the new user
            User = user;

            // Set the models
            TracksList.User = user;
            TracksList.RefreshItems();

            LikeItems.User = user;
            LikeItems.RefreshItems();

            PlaylistItems.User = user;
            PlaylistItems.RefreshItems();

            FollowersList.User = user;
            FollowersList.RefreshItems();

            FollowingsList.User = user;
            FollowingsList.RefreshItems();

            // Get the resource loader
            var resources = ResourceLoader.GetForCurrentView();

            // Check if the tile has been pinned
            if (TileService.Current.DoesTileExist("User_" + User.Id))
            {
                PinButtonIcon = "\uE77A";
                PinButtonText = resources.GetString("AppBarUI_Unpin_Raw");
            }
            else
            {
                PinButtonIcon = "\uE718";
                PinButtonText = resources.GetString("AppBarUI_Pin_Raw");
            }

            if (SoundByteService.Current.IsSoundCloudAccountConnected &&
                User.Id == SoundByteService.Current.CurrentUser.Id)
            {
                FollowUserIcon = "\uE8FA";
                FollowUserText = "Follow User";
                ShowFollowButton = false;
            }
            else
            {
                ShowFollowButton = true;

                // Check if we are following the user
                if (await SoundByteService.Current.ExistsAsync("/me/followings/" + User.Id))
                {
                    FollowUserIcon = "\uE1E0";
                    FollowUserText = "Unfollow User";
                }
                else
                {
                    FollowUserIcon = "\uE8FA";
                    FollowUserText = "Follow User";
                }
            }
        }

        /// <summary>
        /// Follows the requested user
        /// </summary>
        public async void FollowUser()
        {
            // Show the loading ring
            App.IsLoading = true;

            // Check if we are following the user
            if (await SoundByteService.Current.ExistsAsync("/me/followings/" + User.Id))
            {
                // Unfollow the user
                if (await SoundByteService.Current.DeleteAsync("/me/followings/" + User.Id))
                {
                    TelemetryService.Current.TrackEvent("Unfollow User");
                    FollowUserIcon = "\uE8FA";
                    FollowUserText = "Follow User";
                }
                else
                {
                    FollowUserIcon = "\uE1E0";
                    FollowUserText = "Unfollow User";
                }
            }
            else
            {
                // Follow the user
                if (await SoundByteService.Current.PutAsync("/me/followings/" + User.Id))
                {
                    TelemetryService.Current.TrackEvent("Follow User");
                    FollowUserIcon = "\uE1E0";
                    FollowUserText = "Unfollow User";
                }
                else
                {
                    FollowUserIcon = "\uE8FA";
                    FollowUserText = "Follow User";
                }
            }
            // Hide the loading ring
            App.IsLoading = false;
        }

        public async void PinUser()
        {
            // Show the loading ring
            App.IsLoading = true;
            // Get the resource loader
            var resources = ResourceLoader.GetForCurrentView();

            // Check if the tile exists
            if (TileService.Current.DoesTileExist("User_" + User.Id))
            {
                // Try remove the tile
                if (await TileService.Current.RemoveAsync("User_" + User.Id))
                {
                    TelemetryService.Current.TrackEvent("Unpin User");
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
                if (await TileService.Current.CreateTileAsync("User_" + User.Id, User.Username, "soundbyte://core/user?id=" + User.Id, new Uri(ArtworkConverter.ConvertObjectToImage(User)), ForegroundText.Light))
                {
                    TelemetryService.Current.TrackEvent("Pin User");
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

        public async void NavigateToUserTrack(object sender, ItemClickEventArgs e)
        {
            App.IsLoading = true;

            var startPlayback = await PlaybackService.Current.StartMediaPlayback(TracksList.ToList(), TracksList.Token, false, (Core.API.Endpoints.Track)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing user track.").ShowAsync();

            App.IsLoading = false;
        }

        public async void NavigateToLikedTrack(object sender, ItemClickEventArgs e)
        {
            App.IsLoading = true;

            var startPlayback = await PlaybackService.Current.StartMediaPlayback(LikeItems.ToList(), LikeItems.Token, false, (Core.API.Endpoints.Track)e.ClickedItem);
            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing liked user track.").ShowAsync();

            App.IsLoading = false;
        }

        public void NavigateToPlaylist(object sender, ItemClickEventArgs e)
        {
            App.NavigateTo(typeof(Views.Playlist), e.ClickedItem as Core.API.Endpoints.Playlist);
        }

        public void NavigateToUser(object sender, ItemClickEventArgs e)
        {
            App.NavigateTo(typeof(UserView), e.ClickedItem as User);
        }
    }
}
