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
    /// This class represents a comment object within the SoundCloud API
    /// </summary>
    [JsonObject]
    public class Comment
    {
        /// <summary>
        /// Comment body
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// The date and time that this comment was posted.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Object ID
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The track that this comment was posted on
        /// </summary>
        [JsonProperty("track")]
        public Track Track { get; set; }

        /// <summary>
        /// At what time in the track was this posted
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// The user who posted this comment
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
