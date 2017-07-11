//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.Globalization;
using Windows.Storage;

namespace SoundByte.UWP.Services
{
    /// <summary>
    /// This class handles all the settings within the app
    /// </summary>
    public class SettingsService
    {
        #region Static Class Setup
        private static SettingsService _mPInstance;

        public static SettingsService Current => _mPInstance ?? (_mPInstance = new SettingsService());

        #endregion

        #region Constant Keys
        private const string SettingsSyncKey = "SoundByte_SettingsSyncEnabled";
        private const string CleanNotificationsKey = "SoundByte_CleanUpNotificationsEnabled";
        private const string ThemeTypeKey = "SoundByte_ThemeType";
        private const string CurrentTrackKey = "SoundByte_TrackID";
        private const string LastFrameKey = "SoundByte_LastFrame";
        private const string LastViewedTrackKey = "SoundByte_LastViewedTack";
        private const string AppStoredVersionKey = "SoundByte_AppStoredVersionKey";
        private const string NotificationSoundKey = "SoundByte_NotificationSoundEnabled";
        private const string NotificationKey = "SoundByte_NotificationsEnabled";
        private const string TrackNumberKey = "SoundByte_TrackNumberEnabled";
        private const string TrackPostKey = "SoundByte_TrackPostEnabled";
        private const string TrackRepostKey = "SoundByte_TrackRepostEnabled";
        private const string PlaylistPostKey = "SoundByte_PlaylistPostEnabled";
        private const string PlaylistRepostKey = "SoundByte_PlaylistRepostEnabled";
        private const string NotificationGroupingKey = "SoundByte_NotificationGroupingEnabled";
        private const string AppAcentColorKey = "SoundByte_AppAcentColor";
        private const string ArtworkQualityKey = "SoundByte_ArtworkQualityColor";
        private const string LanguageKey = "SoundByte_DefaultLanguage";
        #endregion

        #region Getter and Setters

        /// <summary>
        /// How many items at once are we allowed to load
        /// (less for mobile, more for PC)
        /// </summary>
        public static int TrackLimitor => Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar") ? 40 : 60;

        /// <summary>
        /// The apps accent color
        /// </summary>
        public string AppAccentColor
        {
            get => ReadSettingsValue(AppAcentColorKey) as string;
            set => SaveSettingsValue(AppAcentColorKey, value, true);
        }

        /// <summary>
        /// Should the app use high quality images
        /// </summary>
        public bool IsHighQualityArtwork
        {
            get
            {
                var boolVal = ReadSettingsValue(ArtworkQualityKey) as bool?;

                return !boolVal.HasValue || (boolVal.Value);
            }
            set => SaveSettingsValue(ArtworkQualityKey, value, true);
        }

        /// <summary>
        /// Should notifications be grouped
        /// </summary>
        public bool IsNotificationGroupingEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(NotificationGroupingKey) as bool?;

                return boolVal.HasValue && (boolVal.Value);
            }
            set => SaveSettingsValue(NotificationGroupingKey, value, true);
        }

        /// <summary>
        /// Should the user receive notifications about track reposts
        /// </summary>
        public bool IsUserRepostTrackEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(TrackRepostKey) as bool?;

                return !boolVal.HasValue || (boolVal.Value);
            }
            set => SaveSettingsValue(TrackRepostKey, value, true);
        }

        /// <summary>
        /// Should the user receive notifications about playlist reposts
        /// </summary>
        public bool IsUserRepostPlaylistEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(PlaylistRepostKey) as bool?;

                return !boolVal.HasValue || (boolVal.Value);
            }
            set => SaveSettingsValue(PlaylistRepostKey, value, true);
        }

        /// <summary>
        /// Should the user receive notifications about playlist posts
        /// </summary>
        public bool IsUserPostPlaylistEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(PlaylistPostKey) as bool?;

                return !boolVal.HasValue || (boolVal.Value);
            }
            set => SaveSettingsValue(PlaylistPostKey, value, true);
        }

        /// <summary>
        /// Should the user receive notifications about track posts
        /// </summary>
        public bool IsUserPostTrackEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(TrackPostKey) as bool?;

                return !boolVal.HasValue || (boolVal.Value);
            }
            set => SaveSettingsValue(TrackPostKey, value, true);
        }

        /// <summary>
        /// Are track numbers displayed on the live tile
        /// </summary>
        public bool IsTrackNumberEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(TrackNumberKey) as bool?;

                return !boolVal.HasValue || (boolVal.Value);
            }
            set => SaveSettingsValue(TrackNumberKey, value, true);
        }

        /// <summary>
        /// Are notifications enabled for the app
        /// </summary>
        public bool IsNotificationsEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(NotificationKey) as bool?;

                return !boolVal.HasValue || (boolVal.Value);
            }
            set => SaveSettingsValue(NotificationKey, value, true);
        }

        /// <summary>
        /// Should the app display notification sounds
        /// </summary>
        public bool IsNotificationSoundEnabled
        {
            get
            {
                var boolVal = ReadSettingsValue(NotificationSoundKey) as bool?;

                return boolVal.HasValue && (boolVal.Value);
            }
            set => SaveSettingsValue(NotificationSoundKey, value, true);
        }

        /// <summary>
        /// The last stored app version
        /// </summary>
        public string AppStoredVersion
        {
            get => ReadSettingsValue(AppStoredVersionKey, true) as string;
            set => SaveSettingsValue(AppStoredVersionKey, value);
        }

        /// <summary>
        ///  The user saved language for the app
        /// </summary>
        public string CurrentAppLanguage
        {
            get => ReadSettingsValue(LanguageKey, true) as string;
            set => SaveSettingsValue(LanguageKey, value);
        }

        /// <summary>
        /// The latest viewed track in the user stream
        /// </summary>
        public DateTime LatestViewedTrack
        {
            get
            {
                var stringVal = ReadSettingsValue(LastViewedTrackKey, true) as string;

                if (string.IsNullOrEmpty(stringVal))
                {
                    return (DateTime.UtcNow);
                }

                try
                {
                    return (DateTime.Parse(stringVal));
                }
                catch (FormatException)
                {
                    return (DateTime.UtcNow);
                }
            }
            set => SaveSettingsValue(LastViewedTrackKey, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The last active frame in the window
        /// </summary>
        public string LastFrame
        {
            get => ReadSettingsValue(LastFrameKey, true) as string;
            set => SaveSettingsValue(LastFrameKey, value);
        }

        /// <summary>
        /// Gets the application theme type
        /// </summary>
        public AppTheme ApplicationThemeType
        {
            get
            {
                var stringVal = ReadSettingsValue(ThemeTypeKey) as string;

                if (string.IsNullOrEmpty(stringVal))
                {
                    return (AppTheme.Default);
                }

                try
                {
                    var enumVal = (AppTheme)Enum.Parse(typeof(AppTheme), stringVal);
                    return enumVal;
                }
                catch
                {
                    return (AppTheme.Default);
                }

            }
            set => SaveSettingsValue(ThemeTypeKey, value.ToString(), true);
        }

        /// <summary>
        /// The currently playing track in the background task
        /// </summary>
        public int? CurrentPlayingTrack
        {
            get => (ReadSettingsValue(CurrentTrackKey, true) as int?);
            set
            {
                if (value == -1)
                {
                    SaveSettingsValue(CurrentTrackKey, null);
                    return;
                }
                SaveSettingsValue(CurrentTrackKey, value);
            }
        }

        /// <summary>
        /// Gets if settings syncing is enabled or not
        /// </summary>
        public bool IsSyncSettingsEnabled
        {
            get => ReadBoolSetting(ReadSettingsValue(SettingsSyncKey, true) as bool?, true);
            set => SaveSettingsValue(SettingsSyncKey, value);
        }

        /// <summary>
        /// Gets if the app should display cleanup notifications
        /// </summary>
        public bool IsCleanUpNotificationsEnabled
        {
            get => ReadBoolSetting(ReadSettingsValue(CleanNotificationsKey) as bool?, false);
            set => SaveSettingsValue(CleanNotificationsKey, value, true);
        }
        #endregion

        #region Settings Helpers
        /// <summary>
        /// Used to Return bool values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static bool ReadBoolSetting(bool? value, bool defaultValue) => value ?? defaultValue;

        /// <summary>
        /// Reads a settings value. This method will check the roaming data to see if anything is saved first
        /// </summary>
        /// <param name="key">Key to look for</param>
        /// <param name="forceLocal"></param>
        /// <returns>Saved object</returns>
        private object ReadSettingsValue(string key, bool forceLocal = false)
        {
            // Check if the force local flag is enabled
            if (forceLocal)
                return GetLocalValue(key);

            // Check if sync is enabled
            if (!IsSyncSettingsEnabled) return GetLocalValue(key);
            // Get remote value
            var remoteValue = GetRemoteValue(key);
            // Return the remote value if it exists
            return remoteValue ?? GetLocalValue(key);
        }


        /// <summary>
        /// Gets a remote value
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Object or null</returns>
        private static object GetRemoteValue(string key)
        {
            return ApplicationData.Current.RoamingSettings.Values.ContainsKey(key) ? ApplicationData.Current.RoamingSettings.Values[key] : null;
        }

        /// <summary>
        /// Gets a local value
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Object or null</returns>
        private static object GetLocalValue(string key)
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(key) ? ApplicationData.Current.LocalSettings.Values[key] : null;
        }

        /// <summary>
        /// Save a key value pair in settings. Create if it doesn't exist
        /// </summary>
        /// <param name="key">Used to find the value at a later state</param>
        /// <param name="value">what to save</param>
        /// <param name="canSync">should this value save online? (If user has enabled syncing)</param>
        private void SaveSettingsValue(string key, object value, bool canSync = false)
        {
            // Check if this value supports remote syncing
            if (canSync)
            {
                if (IsSyncSettingsEnabled)
                {
                    if (!ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
                    {
                        // Create new value
                        ApplicationData.Current.RoamingSettings.Values.Add(key, value);
                        return;
                    }
                    else
                    {
                        // Edit existing value
                        ApplicationData.Current.RoamingSettings.Values[key] = value;
                        return;
                    }
                }
            }

            // Store the value locally
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                // Add a new value
                ApplicationData.Current.LocalSettings.Values.Add(key, value);
            }
            else
            {
                // Edit existing value
                ApplicationData.Current.LocalSettings.Values[key] = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// The possible states for the app theme
    /// </summary>
    public enum AppTheme
    {
        Default,
        Light,
        Dark
    }
}
