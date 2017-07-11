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
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Models
{
    /// <summary>
    /// Gets the logged in users playlists and playlist likes
    /// </summary>
    public class UserPlaylistModel : ObservableCollection<Core.API.Endpoints.Playlist>, ISupportIncrementalLoading
    {
        /// <summary>
        /// The position of the track, will be 'eol'
        /// if there are no new tracks
        /// </summary>
        public string Token { get; protected set; }

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
        /// Loads stream items from the souncloud api
        /// </summary>
        /// <param name="count">The amount of items to load</param>
        // ReSharper disable once RedundantAssignment
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Return a task that will get the items
            return Task.Run(async () =>
            {
                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { App.IsLoading = true; });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                // Check if the user is not logged in
                if (SoundByteService.Current.IsSoundCloudAccountConnected)
                {
                    try
                    {
                        // Get the users playlists using the V2 API
                        var userPlaylists = await SoundByteService.Current.GetAsync<PlaylistHolder>($"/users/{SoundByteService.Current.CurrentUser.Id}/playlists/liked_and_owned", new Dictionary<string, string>
                        {
                            { "limit", "50" },
                            { "offset", Token },
                            { "linked_partitioning", "1" }
                        }, true);

                        // Parse uri for offset
                        var param = new QueryParameterCollection(userPlaylists.NextList);
                        var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                        // Get the playlist cursor
                        Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                        // Make sure that there are tracks in the list
                        if (userPlaylists.Playlists.Count > 0)
                        {
                            // Set the count variable
                            count = (uint)userPlaylists.Playlists.Count;

                            // Loop though all the playlists on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                userPlaylists.Playlists.ForEach(p => Add(p.Playlist));
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
                                await new MessageDialog(resources.GetString("UserPlaylists_Content"), resources.GetString("UserPlaylists_Header")).ShowAsync();
                            });
                        }
                    }
                    catch (SoundByteException ex)
                    {
                        // Exception, most likely did not add any new items
                        count = 0;

                        // Exception, display error to the user
                        await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                        {
                            await new MessageDialog(ex.ErrorDescription, ex.ErrorTitle).ShowAsync();
                        });
                    }
                }
                else
                {
                    // Not logged in, so no new items
                    count = 0;

                    // Reset the token
                    Token = "eol";

                    await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                    {
                        await new MessageDialog(resources.GetString("ErrorControl_LoginFalse_Content"), resources.GetString("ErrorControl_LoginFalse_Header")).ShowAsync();
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
