//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;

namespace SoundByte.Core.API.Exceptions
{
    /// <summary>
    /// Used for exception handling within the app. Supports an error title, 
    /// message and custom image.
    /// </summary>
    public class SoundByteException : Exception
    {
        /// <summary>
        /// Title of the error message
        /// </summary>
        public string ErrorTitle { get; }

        /// <summary>
        /// A description of the error message
        /// </summary>
        public string ErrorDescription { get; }

        /// <summary>
        /// Picture that relates with the error message
        /// </summary>
        public string ErrorGlyph { get; }

        public SoundByteException(string title, string description, string glyph) : base(string.Format("Title: {0}, Description: {1}", title, description))
        {
            ErrorTitle = title;
            ErrorDescription = description;
            ErrorGlyph = glyph;
        }
    }
}
