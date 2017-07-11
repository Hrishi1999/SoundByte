//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

namespace SoundByte.UWP.Helpers
{
    /// <summary>
    /// This class handles text related helper functions
    /// </summary>
    public static class TextHelper
    {
        /// <summary>
        /// Cleans a string ready to be used in a
        /// XML document.
        /// </summary>
        /// <param name="input">The string to clean</param>
        /// <returns>The cleaned string</returns>
        public static string CleanXmlString(string input)
        {
            // Clean and return the string
            return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }
    }
}
