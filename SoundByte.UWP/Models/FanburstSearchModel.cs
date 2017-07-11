//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

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
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.API.Endpoints;
using SoundByte.Core.API.Exceptions;
using SoundByte.Core.API.Holders;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Models
{
    public class FanburstSearchModel : ObservableCollection<Core.API.Endpoints.Track>, ISupportIncrementalLoading
    {
        /// <summary>
        /// The position of the track, will be 'eol'
        /// if there are no new trackss
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// What we are searching the soundcloud API for
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Filter the search
        /// </summary>
        public string Filter { get; set; }

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
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            // Return a task that will get the items
            return Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(Query))
                    return new LoadMoreItemsResult { Count = 0 };

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { App.IsLoading = true; });

                // Get the resource loader
                var resources = ResourceLoader.GetForViewIndependentUse();

                try
                {
                    // Search for matching tracks
                    var searchTracks = await SoundByteService.Current.GetAsync<List<dynamic>>(SoundByteService.ServiceType.Fanburst, "tracks/search", new Dictionary<string, string>
                    {
                        { "query", WebUtility.UrlEncode(Query) }
                    });

                    // Parse uri for offset
                 //   var param = new QueryParameterCollection(searchTracks.NextList);
                    var offset = "eol";//param.FirstOrDefault(x => x.Key == "offset").Value;

                    // Get the search offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    // Make sure that there are tracks in the list
                    if (searchTracks.Count > 0)
                    {
                        // Set the count variable
                        count = (uint)searchTracks.Count;

                        // Loop though all the tracks on the UI thread
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            foreach (var item in searchTracks)
                            {
                                Add(new Track
                                {
                                    ServiceType = ServiceType.Fanburst,
                                    Id = item.id,
                                    Title = item.title,
                                    PermalinkUri = item.permalink,
                                    Duration = item.duration,
                                    CreationDate = (DateTime)item.published_at.Value,
                                    Kind = "track",
                                    User = new User
                                    {
                                        Id = item.user.id,
                                        Username = item.user.name,
                                        Country = item.user.location,
                                        ArtworkLink = item.user.avatar_url
                                    },
                                    ArtworkLink = item.images.square_500
                                });
                            }
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
                            await new MessageDialog(resources.GetString("SearchTrack_Content"), resources.GetString("SearchTrack_Header")).ShowAsync();
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
