//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using SoundByte.Core.API.Exceptions;
using SoundByte.Core.API.Holders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;
using SoundByte.UWP.Services;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;

namespace SoundByte.UWP.Models
{
    public class SearchPlaylistModel : ObservableCollection<Core.API.Endpoints.Playlist>, ISupportIncrementalLoading
    {
        /// <summary>
        /// The position of the track, will be 'eol'
        /// if there are no new trackss
        /// </summary>
        public string Token { get; protected set; }

        /// <summary>
        /// What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Are there more items to load
        /// </summary>
        public bool HasMoreItems => Token != "eol";

        /// <summary>
        /// Refresh the list by removing any
        /// existing items and reseting the token.
        /// </summary>
        public void RefreshItems()
        {
            Token = null;
            Clear();
        }

        /// <summary>
        /// Loads search track items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Return a task that will get the items
            return Task.Run(async () =>
            {
                // If the query is empty, tell the user that they can search something
                if (string.IsNullOrEmpty(Query))
                    return new LoadMoreItemsResult { Count = 0 };

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { App.IsLoading = true; });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                try
                {
                    // Get the searched playlists
                    var searchPlaylists = await SoundByteService.Current.GetAsync<SearchPlaylistHolder>("/playlists", new Dictionary<string, string>
                    {
                        { "limit", SettingsService.TrackLimitor.ToString() },
                        { "linked_partitioning", "1" },
                        { "offset", Token },
                        { "q",  WebUtility.UrlEncode(Query) }
                    });

                    // Parse uri for offset
                    var param = new QueryParameterCollection(searchPlaylists.NextList);
                    var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                    // Get the search playlists offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    // Make sure that there are playlists in the list
                    if (searchPlaylists.Playlists.Count > 0)
                    {
                        // Set the count variable
                        count = (uint)searchPlaylists.Playlists.Count;

                        // Loop though all the search playlists on the UI thread
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            searchPlaylists.Playlists.ForEach(Add);
                        });
                    }
                    else
                    {
                        // There are no items, so we added no items
                        count = 0;

                        // Reset the token
                        Token = "eol";

                        // No items tell the user
                        await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                        {
                            await new MessageDialog(resources.GetString("SearchPlaylist_Content"), resources.GetString("SearchPlaylist_Header")).ShowAsync();
                        });
                    }
                }
                catch (SoundByteException ex)
                {
                    // Exception, most likely did not add any new items
                    count = 0;

                    // Reset the token
                    Token = "eol";

                    // Exception, display error to the user
                    await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                    {
                        await new MessageDialog(ex.ErrorDescription, ex.ErrorTitle).ShowAsync();
                    });
                }


                // We are not loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { App.IsLoading = false; });

                // Return the result
                return new LoadMoreItemsResult { Count = count };
            }).AsAsyncOperation();
        }
    }
}