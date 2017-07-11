//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.UI.Xaml;

namespace SoundByte.UWP.UserControls
{
    /// <summary>
    /// Base item for users, tracks, reposted tracks, playlists, reposted playlists and groups
    /// </summary>
    public sealed partial class SoundByteStreamItem
    {
        #region Variables
        // What type of track this is
        public static readonly DependencyProperty TrackTypeProperty = DependencyProperty.Register("TrackType", typeof(string), typeof(SoundByteStreamItem), null);
        // When this was created
        public static readonly DependencyProperty CreatedProperty = DependencyProperty.Register("Created", typeof(string), typeof(SoundByteStreamItem), null);
        // The track object
        public static readonly DependencyProperty TrackProperty = DependencyProperty.Register("Track", typeof(Core.API.Endpoints.Track), typeof(SoundByteStreamItem), null);
        // The playlist object
        public static readonly DependencyProperty PlaylistProperty = DependencyProperty.Register("Playlist", typeof(Core.API.Endpoints.Playlist), typeof(SoundByteStreamItem), null);
        #endregion

        #region Getters and Setters
        /// <summary>
        /// What is the current type of track (e.g repost, playlist, etc.)
        /// </summary>
        public string TrackType
        {
            get { return GetValue(TrackTypeProperty) as string; }
            set { SetValue(TrackTypeProperty, value); }
        }

        /// <summary>
        /// When the track was created
        /// </summary>
        public string Created
        {
            get { return GetValue(CreatedProperty) as string; }
            set { SetValue(CreatedProperty, value); }
        }

        /// <summary>
        /// The track object 
        /// </summary>
        public Core.API.Endpoints.Track Track
        {
            get { return GetValue(TrackProperty) as Core.API.Endpoints.Track; }
            set
            {
                SetValue(TrackProperty, value);
            }
        }

        /// <summary>
        /// The track object 
        /// </summary>
        public Core.API.Endpoints.Playlist Playlist
        {
            get { return GetValue(PlaylistProperty) as Core.API.Endpoints.Playlist; }
            set { SetValue(PlaylistProperty, value); }
        }
        #endregion

        public SoundByteStreamItem()
        {
            // Load the xaml
            InitializeComponent();

            // Setup the even that is called when the data
            // context chanages.
            DataContextChanged += delegate
            {
                // Switch through all the items
                switch (TrackType)
                {
                    case "track-repost":
                    case "track":
                        VisualStateManager.GoToState(this, "TrackItem", false);
                        break;
                    case "playlist-repost":
                    case "playlist":
                        VisualStateManager.GoToState(this, "PlaylistItem", false);
                        break;
                }
            };
        }
    }
}

