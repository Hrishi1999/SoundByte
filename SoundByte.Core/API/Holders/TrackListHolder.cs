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
    /// Holds a list of track
    /// </summary>
    [JsonObject]
    public class TrackListHolder
    {
        /// <summary>
        /// Collection of tracks
        /// </summary>
        [JsonProperty("collection")]
        public List<Endpoints.Track> Tracks { get; set; }

        /// <summary>
        /// The next list of items
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}
