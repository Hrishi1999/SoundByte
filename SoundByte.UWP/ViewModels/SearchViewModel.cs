//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using SoundByte.UWP.Models;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using Playlist = SoundByte.UWP.Views.Playlist;

namespace SoundByte.UWP.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        #region Private Variables
        // Args for filtering
        private string _filterArgs;
        // The query string
        private string _searchQuery;
        #endregion

        #region Models
        // Model for the track searches
        public SearchTrackModel SearchTracks { get; } = new SearchTrackModel();

        public FanburstSearchModel FanburstTracks { get; } = new FanburstSearchModel();

        // Model for the playlist searches
        public SearchPlaylistModel SearchPlaylists { get; } = new SearchPlaylistModel();
        // Model for the user searches
        public SearchUserModel SearchUsers { get; } = new SearchUserModel();
        #endregion

        #region Getters and Setters
     


        /// <summary>
        /// The current pivot item that the user is viewing
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (value != _searchQuery)
                {
                    _searchQuery = value;
                    UpdateProperty();  
                }

                // Update the models
                SearchTracks.Query = value;
                SearchTracks.RefreshItems();

                SearchPlaylists.Query = value;
                SearchPlaylists.RefreshItems();

                SearchUsers.Query = value;
                SearchUsers.RefreshItems();

                FanburstTracks.Query = value;
                FanburstTracks.RefreshItems();
            }
        }

        /// <summary>
        /// Args for filtering 
        /// </summary>
        public string FilterArgs
        {
            get => _filterArgs;
            set
            {
                if (value != _filterArgs)
                {
                    _filterArgs = value;
                    UpdateProperty();
                }

                // Update the models
                SearchTracks.Filter = value;
                SearchTracks.RefreshItems();
            }
        }
        #endregion

        #region Method Bindings

        public void RefreshAll()
        {
            // Update the models
            SearchTracks.RefreshItems();
            SearchPlaylists.RefreshItems();
            SearchUsers.RefreshItems();
            FanburstTracks.RefreshItems();
        }

        public void Search(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(Search), (e as UserControls.SearchBox.SearchEventArgs)?.Keyword);
        }

        public async void ShowFilterMenu()
        {
            var filterDialog = new FilterDialog();
            filterDialog.FilterApplied += (sender, args) =>
            {
                FilterArgs = (args as FilterDialog.FilterAppliedArgs)?.FilterArgs;
            };

            await filterDialog.ShowAsync();
        }

        public async void NavigateItem(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
                return;

            // Show the loading ring
            App.IsLoading = true;

            if (e.ClickedItem.GetType().Name == "Track")
            {
                var searchItem = e.ClickedItem as Core.API.Endpoints.Track;

                // Get the resource loader
                var resources = ResourceLoader.GetForCurrentView();
                // Create the error dialog
                var uiUpdateError = new ContentDialog
                {
                    Title = resources.GetString("PlaylistNavigateError_Title"),
                    Content = resources.GetString("PlaylistNavigateError_Content"),
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonText = resources.GetString("PlaylistNavigateError_Button")
                };

                switch (searchItem?.Kind)
                {
                    case "track":
                    case "track-repost":
                        // Play this item

                        if (searchItem.ServiceType == ServiceType.Fanburst)
                        {
                            var startPlayback = await PlaybackService.Current.StartMediaPlayback(FanburstTracks.ToList(), FanburstTracks.Token, false, searchItem);
                            if (!startPlayback.success)
                                await new MessageDialog(startPlayback.message, "Error playing searched track.").ShowAsync();
                        }
                        else
                        {
                            var startPlayback = await PlaybackService.Current.StartMediaPlayback(SearchTracks.ToList(), SearchTracks.Token, false, searchItem);
                            if (!startPlayback.success)
                                await new MessageDialog(startPlayback.message, "Error playing searched track.").ShowAsync();
                        }

                        break;
                    case "playlist":
                        try
                        {
                            var playlist = await SoundByteService.Current.GetAsync<Core.API.Endpoints.Playlist>("/playlist/" + searchItem.Id);
                            App.NavigateTo(typeof(Playlist), playlist);
                        }
                        catch (Exception)
                        {
                            await uiUpdateError.ShowAsync();
                        }
                        break;
                    case "playlist-repost":
                        try
                        {
                            var playlistR = await SoundByteService.Current.GetAsync<Core.API.Endpoints.Playlist>("/playlist/" + searchItem.Id);
                            App.NavigateTo(typeof(Playlist), playlistR);
                        }
                        catch (Exception)
                        {
                            await uiUpdateError.ShowAsync();
                        }

                        break;
                }
            }
            else if (e.ClickedItem.GetType().Name == "User")
            {
                App.NavigateTo(typeof(UserView), e.ClickedItem as Core.API.Endpoints.User);
            }
            else if (e.ClickedItem.GetType().Name == "Playlist")
            {
                App.NavigateTo(typeof(Playlist), e.ClickedItem as Core.API.Endpoints.Playlist);
            }

            App.IsLoading = false;
        }

        #endregion
    }
}
