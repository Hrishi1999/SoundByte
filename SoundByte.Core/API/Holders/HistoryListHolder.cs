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
    /// Holds a track
    /// </summary>
    [JsonObject]
    public class HistoryBootstrap
    {
        /// <summary>
        /// A playlist
        /// </summary>
        [JsonProperty("track")]
        public Endpoints.Track Track { get; set; }
    }

    [JsonObject]
    public class HistoryListHolder
    {
        [JsonProperty("collection")]
        public List<HistoryBootstrap> Tracks { get; set; }

        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}
