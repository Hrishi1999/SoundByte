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
    /// Small class for holding comments
    /// </summary>
    [JsonObject]
    public class CommentListHolder
    {
        /// <summary>
        /// List of comments
        /// </summary>
        [JsonProperty("collection")]
        public List<Endpoints.Comment> Items { get; set; }

        /// <summary>
        /// Next items in the list
        /// </summary>
        [JsonProperty("next_href")]
        public string NextList { get; set; }
    }
}