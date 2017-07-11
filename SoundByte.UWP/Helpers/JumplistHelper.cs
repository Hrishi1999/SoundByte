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
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.StartScreen;

namespace SoundByte.UWP.Helpers
{
    /// <summary>
    /// This class contains helper functions for creating
    /// and managing jumplists
    /// </summary>
    public class JumplistHelper
    {
        // The Systems jumplist
        private static JumpList _systemJumpList;

        /// <summary>
        /// Gets a list of all the jumplists
        /// if supported by the platform
        /// </summary>
        public static async Task<IList<JumpListItem>> GetItemsAsync()
        {
            // Check if jumplists are supported
            if (!JumpList.IsSupported()) return new List<JumpListItem>();
            // Load the jumplist items
            _systemJumpList = await JumpList.LoadCurrentAsync();
            // Return the items
            return _systemJumpList.Items;
        }

        /// <summary>
        /// Gets a list of all the jumplist IDs
        /// if supported by the platform
        /// <param name="groupName">The group to get</param>
        /// </summary>
        public static async Task<List<string>> GetJumpListIDsAsync(string groupName)
        {
            // Check if jumplists are supported
            if (!JumpList.IsSupported()) return new List<string>();
            // Get a list of jumplist items
            var itemList = await GetItemsAsync();
            // Return the list
            return itemList.Where(x => x.GroupName == groupName).Select(x => x.Arguments).Select(item => item.Split('=')[1]).ToList();
        }

        /// <summary>
        /// Adds a recent item to the jumplist
        /// </summary>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="grp"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static async Task AddRecentAsync(string args, string name, string description, string grp, Uri image)
        {
            try
            {
                // Check if jumplists are supported
                if (!JumpList.IsSupported()) return;

                // Load the jumplist
                _systemJumpList = await JumpList.LoadCurrentAsync();

                // Change the kind to recent items
                _systemJumpList.SystemGroupKind = JumpListSystemGroupKind.Recent;

                // Check that the item is not already added
                if (_systemJumpList.Items.FirstOrDefault(x => x.Arguments == args) != null) return;

                // Loop through all the items and remove any items that will cause the jumplist
                // to go over 5 items (we only want 5 recent items max).
                while (_systemJumpList.Items.Count(x => x.GroupName == grp) >= 5)
                {
                    // Get a item
                    var recentItem = _systemJumpList.Items.FirstOrDefault(x => x.GroupName == grp);
                    // Check that the item is not null
                    if (recentItem != null)
                    {
                        // Remove the item from the list
                        _systemJumpList.Items.Remove(recentItem);
                    }
                }

                // Create a new jumplist item
                var item = JumpListItem.CreateWithArguments(args, name);
                item.Description = description;
                item.GroupName = grp;
                item.Logo = image;

                // Add the item to the jumplist
                _systemJumpList.Items.Add(item);

                // Save the jumplist
                await _systemJumpList.SaveAsync();
            }
            catch
            {
                await new MessageDialog("An Error Occured while pinning this item to the jumplist.", "Jumplist Error").ShowAsync();
            }
        }

        /// <summary>
        /// Removes all jumplist items
        /// </summary>
        public static async Task RemoveAllAsync()
        {
            // Check if jumplists are supported
            if (!JumpList.IsSupported()) return;
            // Load the jumplist Items
            _systemJumpList = await JumpList.LoadCurrentAsync();
            // Clear all jumplist items
            _systemJumpList.Items.Clear();
            // Save the Jumplist Items
            await _systemJumpList.SaveAsync();
        }
    }
}