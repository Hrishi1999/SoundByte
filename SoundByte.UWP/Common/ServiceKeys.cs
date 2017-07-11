//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System.Collections.Generic;
using Windows.Storage;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Common
{
    /// <summary>
    /// This class contains any keys used by the app. For example
    /// client IDs and client secrets.
    /// </summary>
    public static class ServiceKeys
    {
        public static string GoogleAnalyticsTrackerId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.GA") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.GA"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("ga");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.GA", liveKey);

                return liveKey;
            }
        } 

        public static string HockeyAppClientId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.HAC") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.HAC"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("hac");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.HAC", liveKey);

                return liveKey;
            }
        }

        public static string AzureMobileCenterClientId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.AMCC") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.AMCC"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("amcc");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.AMCC", liveKey);

                return liveKey;
            }
        }

        public static string SoundCloudClientId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.SCC") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.SCC"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("scc");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.SCC", liveKey);

                return liveKey;
            }
        } 

        public static string SoundCloudClientSecret
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.SCS") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.SCS"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("scs");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.SCS", liveKey);

                return liveKey;
            }
        } 

        public static string FanburstClientId
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.FBC") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.FBC"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("fbc");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.FBC", liveKey);

                return liveKey;
            }
        }

        public static string FanburstClientSecret
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.FBS") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.FBS"] : null;

                if (key != null)
                    return key.ToString();

                var liveKey = SoundByteService.Current.GetSoundByteKey("fbs");
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.FBS", liveKey);

                return liveKey;
            }
        }

        public static List<string> SoundCloudPlaybackClientIds
        {
            get
            {
                // Check if the key has been stored locally
                var key = ApplicationData.Current.LocalSettings.Values.ContainsKey("SoundByte.Keys.SCPI") ? ApplicationData.Current.LocalSettings.Values["SoundByte.Keys.SCPI"] as List<string> : null;

                if (key != null)
                    return key;

                var liveKey = SoundByteService.Current.GetSoundBytePlaybackKeys();
                ApplicationData.Current.LocalSettings.Values.Add("SoundByte.Keys.SCPI", liveKey);

                return liveKey;
            }
        }
    }
}
