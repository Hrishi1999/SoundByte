//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System.Runtime.Serialization;

namespace SoundByte.Core.API.Endpoints
{
    /// <summary>
    /// Info about the current build
    /// </summary>
    [DataContract]
    public class BuildInfo
    {
        /// <summary>
        /// The branch that this was compliled from
        /// </summary>
        [DataMember(Name = "build_branch")]
        public string BuildBranch { get; set; }

        /// <summary>
        /// The build number
        /// </summary>
        [DataMember(Name = "build_no")]
        public string BuildNumber { get; set; }

        /// <summary>
        /// The time this was built
        /// </summary>
        [DataMember(Name = "build_time")]
        public string BuildTime { get; set; }
    }
}
