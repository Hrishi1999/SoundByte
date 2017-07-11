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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core.API.Exceptions;
using SoundByte.Core.API.Holders;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Models
{
    /// <summary>
    /// Gets comments for a supplied track
    /// </summary>
    public class CommentModel : ObservableCollection<Core.API.Endpoints.Comment>, ISupportIncrementalLoading
    {
        // The track we want to get comments for
        private Core.API.Endpoints.Track _track;

        /// <summary>
        /// Get comments for a track
        /// </summary>
        /// <param name="track"></param>
        public CommentModel(Core.API.Endpoints.Track track)
        {
            _track = track;
        }

        /// <summary>
        /// The position of the comments, will be 'eol'
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
                _track = PlaybackService.Current.CurrentTrack;

                if (_track == null)
                    return new LoadMoreItemsResult { Count = 0 };

                // We are loading
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => { App.IsLoading = true; });

                try
                {
                    // Get the comments
                    var comments = await SoundByteService.Current.GetAsync<CommentListHolder>(string.Format("/tracks/{0}/comments", _track.Id), new Dictionary<string, string>
                    {
                        { "limit", "50" },
                        { "cursor", Token },
                        { "linked_partitioning", "1" }
                    });

                    // Parse uri for offset
                    var param = new QueryParameterCollection(comments.NextList);
                    var offset = param.FirstOrDefault(x => x.Key == "offset").Value;

                    // Get the comment offset
                    Token = string.IsNullOrEmpty(offset) ? "eol" : offset;

                    // Make sure that there are comments in the list
                    if (comments.Items.Count > 0)
                    {
                        // Set the count variable
                        count = (uint)comments.Items.Count;

                        // Loop though all the comments on the UI thread
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            comments.Items.ForEach(Add);
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
                            await new MessageDialog("Be the first to post a comment.", "No Comments").ShowAsync();
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
