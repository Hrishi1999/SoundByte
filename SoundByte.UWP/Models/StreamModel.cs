//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

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
using SoundByte.Core.API.Endpoints;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Models
{
    /// <summary>
    ///  Model for the users stream
    /// </summary>
    public class StreamModel : ObservableCollection<StreamItem>, ISupportIncrementalLoading
    {
        /// <summary>
        /// The position of the track, will be 'eol'
        /// if there are no new tracks
        /// </summary>
        public string Token { get; private set; }

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
                        // Get items from the users stream
                        var streamTracks = await SoundByteService.Current.GetAsync<StreamTrackHolder>("/e1/me/stream", new Dictionary<string, string>
                        {
                            { "limit", "50" },
                            { "cursor", Token }
                        });

                        // Parse uri for offset
                        var param = new QueryParameterCollection(streamTracks.NextList);
                        var cursor = param.FirstOrDefault(x => x.Key == "cursor").Value;

                        // Get the stream cursor
                        Token = string.IsNullOrEmpty(cursor) ? "eol" : cursor;

                        // Make sure that there are tracks in the list
                        if (streamTracks.Items.Count > 0)
                        {
                            // Set the count variable
                            count = (uint)streamTracks.Items.Count;

                            // Loop though all the tracks on the UI thread
                            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                            {
                                streamTracks.Items.ForEach(Add);
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
                                await new MessageDialog(resources.GetString("StreamTracks_Content"), resources.GetString("StreamTracks_Header")).ShowAsync();
                            });
                        }
                    }
                    catch (Core.API.Exceptions.SoundByteException ex)
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
