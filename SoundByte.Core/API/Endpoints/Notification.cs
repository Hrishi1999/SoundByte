//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Newtonsoft.Json;

namespace SoundByte.Core.API.Endpoints
{
    /// <summary>
    /// A user notification
    /// </summary>
    [JsonObject]
    public class Notification
    {
        /// <summary>
        /// Whent this object was created
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// What type of object this is
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// User detail
        /// </summary>
        [JsonProperty("user")]
        public Endpoints.User User { get; set; }

        /// <summary>
        /// track detail
        /// </summary>
        [JsonProperty("track")]
        public Endpoints.Track Track { get; set; }

        /// <summary>
        /// Playlist detail
        /// </summary>
        [JsonProperty("playlist")]
        public Endpoints.Playlist Playlist { get; set; }

        /// <summary>
        /// Comment detail
        /// </summary>
        [JsonProperty("comment")]
        public Endpoints.Comment Comment { get; set; }
    }
}
