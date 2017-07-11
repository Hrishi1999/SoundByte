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
    /// A Soundcloud user
    /// </summary>
    [JsonObject]
    public class User
    {
        /// <summary>
        /// The ID of the user
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The users username
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets the url of the users picture
        /// </summary>
        [JsonProperty("avatar_url")]
        public string ArtworkLink { get; set; }

        /// <summary>
        /// The country that the user is from
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// A link to this users profile
        /// </summary>
        [JsonProperty("permalink_url")]
        public string PermalinkUri { get; set; }

        /// <summary>
        /// About the user
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// The number of tracks the user has uploaded
        /// </summary>
        [JsonProperty("track_count")]
        public int? TrackCount { get; set; }

        /// <summary>
        /// The amount of playlists the user owns
        /// </summary>
        [JsonProperty("playlist_count")]
        public int? PlaylistCount { get; set; }

        /// <summary>
        /// The amount of followers this user has
        /// </summary>
        [JsonProperty("followers_count")]
        public int? FollowersCount { get; set; }

        /// <summary>
        /// The amount of followings this user has
        /// </summary>
        [JsonProperty("followings_count")]
        public int? FollowingsCount { get; set; }
    }
}
