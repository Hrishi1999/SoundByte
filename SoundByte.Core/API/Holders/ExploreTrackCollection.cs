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
    [JsonObject]
    public class ExploreTrackCollection
    {
        /// <summary>
        /// The track object
        /// </summary>
        [JsonProperty("track")]
        public Endpoints.Track Track { get; set; }
    }

    /// <summary>
    /// Used when deserlizing charts. Provided is the list of tracks and 
    /// the uri to the next list.
    /// </summary>
    [JsonObject]
    public class ExploreTrackHolder
    {
        /// <summary>
        /// The list of items
        /// </summary>
        [JsonProperty("collection")]
        public List<ExploreTrackCollection> Items { get; set; }

        /// <summary>
        /// Next item in the list
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}