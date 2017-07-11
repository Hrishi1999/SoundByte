//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Newtonsoft.Json;

namespace SoundByte.Core.API.Endpoints
{
    /// <summary>
    /// A stream collection containing all items that may be on the users stream
    /// </summary>
    [JsonObject]
    public class StreamItem
    {
        /// <summary>
        /// Track detail
        /// </summary>
        [JsonProperty("track")]
        public Track Track { get; set; }

        /// <summary>
        /// User detail
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }

        /// <summary>
        /// Playlist detail
        /// </summary>
        [JsonProperty("playlist")]
        public Playlist Playlist { get; set; }

        /// <summary>
        /// When this object was created
        /// </summary>
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// What type of object this is
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
