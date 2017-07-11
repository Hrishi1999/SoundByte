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
using Newtonsoft.Json;

namespace SoundByte.Core.API.Endpoints
{
    /// <summary>
    /// Represents a playlist within the SoundCloud API
    /// </summary>
    [JsonObject]
    public class Playlist
    {
        /// <summary>
        /// How long is the total playlists
        /// </summary>
        [JsonProperty("duration")]
        public int Duration { get; set; }

        /// <summary>
        /// This title of this playlist
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// The genre of this playlist
        /// </summary>
        [JsonProperty("genre")]
        public string Genre { get; set; }

        /// <summary>
        /// Used for updating the items within a users playlist
        /// </summary>
        [JsonProperty("secret_token")]
        public string SecretToken { get; set; }

        /// <summary>
        /// The description for this track
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// A list of track objects that are within this playlist
        /// </summary>
        [JsonProperty("tracks")]
        public List<Track> Tracks { get; set; }

        /// <summary>
        /// Internal ID of this object.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The date and time of this objects creation
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The artwork URI for this object
        /// </summary>
        [JsonProperty("artwork_url")]
        public string ArtworkLink { get; set; }

        /// <summary>
        /// The user who posted this track
        /// </summary>
        [JsonProperty("user")]
        public User User { get; set; }
      
        /// <summary>
        /// Used by SoundByte to determine if the track is in a set
        /// </summary>
        public bool IsTrackInInternalSet { get; set; }

        /// <summary>
        /// The number of likes that this playlist has
        /// </summary>
        [JsonProperty("likes_count")]
        public int? LikesCount { get; set; }

        /// <summary>
        /// The number of tracks in this playlist
        /// </summary>
        [JsonProperty("track_count")]
        public int? TrackCount { get; set; }
    }
}
