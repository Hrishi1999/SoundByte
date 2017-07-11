//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.UI.Xaml.Data;
using SoundByte.Core.API.Endpoints;

namespace SoundByte.UWP.Converters
{
    /// <summary>
    /// The artwork converter is used to get the best quality
    /// and correct artwork for the provided resource. This was
    /// previously done in the endpoint classes them selves, but to 
    /// reduce app size and code reuse, the methods have been moved here.
    /// 
    /// By default this class should be used whenever you are trying to 
    /// display an image in xaml. There is also a static method that can be
    /// used in code when accessing the image there.
    /// </summary>
    public class ArtworkConverter : IValueConverter
    {
        /// <summary>
        /// Pass in a endpoint object to retreive its appropiate image. Currently
        /// supported objects are user, playlist and track.
        /// </summary>
        /// <param name="value">Endpoint object</param>
        /// <returns>A string to the object</returns>
        public static string ConvertObjectToImage(object value)
        {
            if (value == null)
                return "";

            // Grab the source object type
            var sourceType = value.GetType();

            // Check that we can use this object
            if (!(sourceType == typeof(Track) || sourceType == typeof(Playlist) || sourceType == typeof(User)))
                throw new ArgumentException($"Expected object to convert is either Track, Playlist or User. {sourceType} was passed instead.", nameof(value));

            // Switch between all the options
            switch (sourceType.Name)
            {
                case "Track":
                    return GetTrackImage(value as Track);
                case "User":
                    return GetUserImage(value as User);
                case "Playlist":
                    return GetPlaylistImage(value as Playlist);
            }

            // If we reach here, something went wrong, this should never happen
            throw new ArgumentException($"Expected object to convert is either Track, Playlist or User. {sourceType} was passed instead.", nameof(value));
        }

        #region Image Getters
        /// <summary>
        /// Get the image for the track
        /// </summary>
        /// <param name="track">The track to get an image for</param>
        /// <returns>A url to the image</returns>
        private static string GetTrackImage(Track track)
        { 
            // This is a youtube link, do nothing
            if (track.ServiceType == ServiceType.YouTube)
                return track.ArtworkLink;

            // If there is no uri, return the users image
            if (string.IsNullOrEmpty(track.ArtworkLink))
                return GetUserImage(track.User);

            // Check if this image supports high resolution
            if (track.ArtworkLink.Contains("large"))
            {
                // If the user has enabled high quality artwork, return a 
                // high quality version, otherwise return a normal quality 
                // version.
                return Services.SettingsService.Current.IsHighQualityArtwork ? track.ArtworkLink.Replace("large", "t500x500") : track.ArtworkLink.Replace("large", "t300x300");
            }
           
            // This image does not support high resoultion
            return track.ArtworkLink; 
        }

        /// <summary>
        /// Get the image for the user
        /// </summary>
        /// <param name="user">The user to get an image for</param>
        /// <returns>A url to the image</returns>
        private static string GetUserImage(User user)
        {
            // If there is no uri, return the default image image
            if (string.IsNullOrEmpty(user.ArtworkLink))
                return "http://a1.sndcdn.com/images/default_avatar_large.png";

            // If the avatar is defaut, just return it
            if (user.ArtworkLink.Contains("default_avatar"))
                return user.ArtworkLink;

            // Check if this image supports high resolution
            if (user.ArtworkLink.Contains("large"))
            {
                // If the user has enabled high quality artwork, return a
                // high quality version, otherwise return a normal quality 
                // version.
                return Services.SettingsService.Current.IsHighQualityArtwork ? user.ArtworkLink.Replace("large", "t500x500") : user.ArtworkLink.Replace("large", "t300x300");
            }

            // This image does not support high resoultion
            return user.ArtworkLink;
        }

        /// <summary>
        /// Get the image for the playlist
        /// </summary>
        /// <param name="playlist">The playlist to get an image for</param>
        /// <returns>A url to the image</returns>
        private static string GetPlaylistImage(Playlist playlist)
        {
            // If there is no uri, return the users image
            if (string.IsNullOrEmpty(playlist.ArtworkLink))
                return GetUserImage(playlist.User);

            // Check if this image supports high resolution
            if (playlist.ArtworkLink.Contains("large"))
            {
                // If the user has enabled high quality artwork, return a 
                // high quality version, otherwise return a normal quality 
                // version.
                return Services.SettingsService.Current.IsHighQualityArtwork ? playlist.ArtworkLink.Replace("large", "t500x500") : playlist.ArtworkLink.Replace("large", "t300x300");
            }

            // This image does not support high resoultion
            return playlist.ArtworkLink;
        }
        #endregion

        #region Base Converter Methods
        /// <summary>
        /// Used for converting within XAML
        /// </summary>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Just call the static method
            return ConvertObjectToImage(value);
        }
        #endregion

        #region Unused
        /// <summary>
        /// We do not convert back as it is not possible in this case
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}