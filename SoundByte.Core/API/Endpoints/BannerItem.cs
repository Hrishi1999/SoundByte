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
    /// Used for displaying banner items
    /// </summary>
    [JsonObject]
    public class BannerItem
    {
        /// <summary>
        /// The title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Link to go in the app
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        /// Link to the background image
        /// </summary>
        [JsonProperty("iamge_url")]
        public string ImageUri { get; set; }
    }
}
