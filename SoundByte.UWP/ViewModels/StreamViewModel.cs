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
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.ViewModels
{
    /// <summary>
    /// The view model for the StreamView page
    /// </summary>
    public class StreamViewModel : BaseViewModel
    {
        // Model for stream items
        public StreamModel StreamItems { get; } = new StreamModel();

        /// <summary>
        /// Refreshes the models depending on what 
        /// page is being viewed
        /// </summary>
        public void RefreshItems()
        {
            // As this process can take a while
            // we need to enable the loading ring
            App.IsLoading = true;

            StreamItems.RefreshItems();

            // Now that we are complete, we need to hide
            // the loading ring.
            App.IsLoading = false;
        }

        public async void PlayAllTracks()
        {
            // We are loading
            App.IsLoading = true;

            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == "track" || t.Type == "track-repost" && t.Type != null).Select(t => t.Track).ToList();

            var startPlayback = await PlaybackService.Current.StartMediaPlayback(trackList.ToList(), StreamItems.Token);

            if (!startPlayback.success)
                await new MessageDialog(startPlayback.message, "Error playing stream.").ShowAsync();

            // We are not loading
            App.IsLoading = false;
        }

        public async void PlayShuffleTracks()
        {
            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == "track" || t.Type == "track-repost" && t.Type != null).Select(t => t.Track).ToList();

            // Shuffle and play the items
            await ShuffleTracksAsync(trackList, StreamItems.Token);
        }

        public async void NavigateStream(object sender, ItemClickEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            // Get a list of items
            var trackList = StreamItems.Where(t => t.Type == "track" || t.Type == "track-repost" && t.Type != null).Select(t => t.Track).ToList();

            // Get the clicked item
            var streamItem = (StreamItem)e.ClickedItem;

            switch (streamItem.Type)
            {
                case "track":
                case "track-repost":
                    if (streamItem.Track != null)
                    {
                        var startPlayback = await PlaybackService.Current.StartMediaPlayback(trackList.ToList(), StreamItems.Token, false, streamItem.Track);

                        if (!startPlayback.success)
                            await new MessageDialog(startPlayback.message, "Error playing stream.").ShowAsync();
                    }
                    break;
                case "playlist":
                case "playlist-repost":
                    App.NavigateTo(typeof(Views.Playlist), streamItem.Playlist);
                    break;
            }

            // We are not loading
            App.IsLoading = false;
        }

        public override void Dispose()
        {
            // Only clean if we are in the background
            if (!App.IsBackground)
                return;

            GC.Collect();
        }
    }
}
