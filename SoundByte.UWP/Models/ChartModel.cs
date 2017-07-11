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
    /// Model for the soundcloud charts
    /// </summary>
    public class ChartModel : ObservableCollection<Core.API.Endpoints.Track>, ISupportIncrementalLoading
    {
        // The genre to search for
        private string _genre = "all-music";
        // The kind to search for
        private string _kind = "top";

        /// <summary>
        /// The position of the track, will be 'eol'
        /// if there are no new trackss
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// The genre to search for.
        /// Note: By changing this variable will update
        /// the model.
        /// </summary>
        public string Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                RefreshItems();
            }
        }

        /// <summary>
        /// The kind of item to search for
        /// Note: By changing this variable it will
        /// update the model.
        /// </summary>
        public string Kind
        {
            get => _kind;
            set
            {
                _kind = value;
                RefreshItems();
            }
        }

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
        /// Loads chart items from the souncloud api
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

                try
                {
                    // Get the trending tracks
                    var exploreTracks = await SoundByteService.Current.GetAsync<ExploreTrackHolder>("/charts", new Dictionary<string, string>
                    {
                        { "genre", "soundcloud%3Agenres%3A" + _genre },
                        { "kind", _kind },
                        { "limit", "50" },
                        { "offset", Token },
                        { "linked_partitioning", "1" }
                    }, true);

                    // Parse uri for offset
                    var param = new QueryParameterCollection(exploreTracks.NextList);
                    var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                    // Get the stream offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    // Make sure that there are tracks in the list
                    if (exploreTracks.Items.Count > 0)
                    {
                        // Set the count variable
                        count = (uint)exploreTracks.Items.Count;

                        // Loop though all the tracks on the UI thread
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            exploreTracks.Items.ForEach(t => Add(t.Track));
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
                            await new MessageDialog(resources.GetString("ExploreTracks_Content"), resources.GetString("ExploreTracks_Header")).ShowAsync();
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
