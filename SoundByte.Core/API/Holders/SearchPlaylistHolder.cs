//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System.Collections.Generic;
using Newtonsoft.Json;

namespace SoundByte.Core.API.Holders
{
    /// <summary>
    /// Holds a playlist
    /// </summary>
    [JsonObject]
    public class LikePlaylistBootstrap
    {
        /// <summary>
        /// A playlist
        /// </summary>
        [JsonProperty("playlist")]
        public Endpoints.Playlist Playlist { get; set; }
    }

    /// <summary>
    /// Holds the users playlists
    /// </summary>
    [JsonObject]
    public class PlaylistHolder
    {
        /// <summary>
        /// List of sub playlists
        /// </summary>
        [JsonProperty("collection")]
        public List<LikePlaylistBootstrap> Playlists { get; set; }

        /// <summary>
        /// The next list of items
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }

    /// <summary>
    /// Holder for searched playlist items
    /// </summary>
    [JsonObject]
    public class SearchPlaylistHolder
    {
        /// <summary>
        /// List of playlists
        /// </summary>
        [JsonProperty("collection")]
        public List<Endpoints.Playlist> Playlists { get; set; }

        /// <summary>
        /// The next list of items
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}
