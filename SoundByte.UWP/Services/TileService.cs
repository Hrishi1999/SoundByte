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
using Windows.UI.StartScreen;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.Services
{
    /// <summary>
    /// Handles Live Tiles
    /// </summary>
    public class TileService
    {
        #region Variables
        // Class instance
        private static TileService _mPInstance;
        // Stores all the tiles that are currently pinned to the users screen
        private readonly Dictionary<string, SecondaryTile> _mPTileList = new Dictionary<string, SecondaryTile>();
        #endregion

        /// <summary>
        /// Gets the current instance
        /// </summary>
        public static TileService Current => _mPInstance ?? (_mPInstance = new TileService());

        private TileService()
        {
            // Gets all the tiles that are for the app
            var allTiles = AsyncHelper.RunSync(async () => await SecondaryTile.FindAllAsync());
            // Clear the list
            _mPTileList.Clear();
            // Loop  through all the tiles and add them to the list
            foreach (var tile in allTiles)
                _mPTileList.Add(tile.TileId, tile);
        }

        #region Public Methods
        /// <summary>
        /// Gets all the pinned tiles, make sure you load the tiles first
        /// </summary>
        private Dictionary<string, SecondaryTile> GetTiles()
        {
            // Check that the tile list is not null
            return _mPTileList ?? new Dictionary<string, SecondaryTile>();
        }

        /// <summary>
        /// Gets all the tiles and returns their IDs
        /// in a string list. Make sure you load the tiles
        /// first.
        /// </summary>
        public IEnumerable<string> GetTileIDs()
        {
            // Loop through all the tiles
            // Return the list
            return GetTiles().Keys.ToList().Select(item => item.Split('_')[1]).ToList();
        }

        /// <summary>
        /// Removes all live tiles from the start menu
        /// or start screen
        /// </summary>
        public async Task RemoveAllAsync()
        {
            // Clear the tile list
            _mPTileList.Clear();
            // Find all the tiles and loop though them all
            foreach (var tile in await SecondaryTile.FindAllAsync())
            {
                // Request a tile delete
                await tile.RequestDeleteAsync();
            }
        }

        /// <summary>
        /// Removes a tile from the users start screen and deletes it from the list
        /// </summary>
        /// <param name="id">Tile ID</param>
        /// <returns>True is successful</returns>
        public async Task<bool> RemoveAsync(string id)
        {
            // The tile that we will access
            SecondaryTile tile;
            // Check if tile exists and get it
            if (_mPTileList.TryGetValue(id, out tile))
            {
                try
                {
                    // Request tile deletion
                    var success = await tile.RequestDeleteAsync();
                    // Remove the tile from the list
                    _mPTileList.Remove(id);
                    // Return weather the task was successful or not
                    return success;
                }
                catch
                {
                    // There was some error, tile not removed
                    return false;
                }
            }

            // Tile does not exist
            return true;
        }

        /// <summary>
        /// Returns if the tile exists
        /// </summary>
        /// <param name="id">Tile ID</param>
        /// <returns>True if the tile exists</returns>
        public bool DoesTileExist(string id)
        {
            // Check if tileid is in list
            return _mPTileList.ContainsKey(id);
        }

        /// <summary>
        /// Creates a tile and pins it to the users screen
        /// </summary>
        /// <param name="tileId">The ID for the tile</param>
        /// <param name="tileTitle">The title that will appear on the tile</param>
        /// <param name="tileParam">Any params that will be passed to the app on launch</param>
        /// <param name="tileImage">Uri to image for the background</param>
        /// <param name="tileForeground">Text to display on tile</param>
        /// <returns></returns>
        public async Task<bool> CreateTileAsync(string tileId, string tileTitle, string tileParam, Uri tileImage, ForegroundText tileForeground)
        {
            // Check if the tile already exists
            if (DoesTileExist(tileId))
                return false;

            await new PinTileDialog(tileId, tileTitle, tileParam, tileImage).ShowAsync();
            return true;
        }
        #endregion
    }
}
