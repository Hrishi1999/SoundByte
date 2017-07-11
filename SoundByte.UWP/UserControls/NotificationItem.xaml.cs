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
    public sealed partial class NotificationItem
    {
        #region Variables
        public static readonly DependencyProperty _trackDataProperty = DependencyProperty.Register("TrackData", typeof(Core.API.Endpoints.Track), typeof(NotificationItem), null);
        public static readonly DependencyProperty _playlistDataProperty = DependencyProperty.Register("PlaylistData", typeof(Core.API.Endpoints.Playlist), typeof(NotificationItem), null);
        public static readonly DependencyProperty _userDataProperty = DependencyProperty.Register("UserData", typeof(Core.API.Endpoints.User), typeof(NotificationItem), null);
        public static readonly DependencyProperty _commentProperty = DependencyProperty.Register("CommentData", typeof(Core.API.Endpoints.Comment), typeof(NotificationItem), null);
        public static readonly DependencyProperty _creationProperty = DependencyProperty.Register("Creation", typeof(string), typeof(NotificationItem), null);
        public static readonly DependencyProperty _collectionTypeProperty = DependencyProperty.Register("CollectionType", typeof(string), typeof(NotificationItem), null);
        #endregion

        #region Page Setup
        /// <summary>
        /// Called when this item is created
        /// </summary>
        public NotificationItem()
        {
            // Load the xaml
            InitializeComponent();

            // Setup the even that is called when the data
            // context chanages.
            DataContextChanged += delegate
            {
                // Switch through all the items
                switch (CollectionType)
                {
                    case "track-like":
                        RootContentPane.ContentTemplate = Resources["TrackLikeView"] as DataTemplate;
                        break;
                    case "playlist-like":
                        RootContentPane.ContentTemplate = Resources["PlaylistLikeView"] as DataTemplate;
                        break;
                    case "comment":
                        RootContentPane.ContentTemplate = Resources["TrackCommentView"] as DataTemplate;
                        break;
                    case "affiliation":
                        RootContentPane.ContentTemplate = Resources["UserFollowView"] as DataTemplate;
                        break;
                    default:
                        RootContentPane.ContentTemplate = Resources["UnknownView"] as DataTemplate;
                        break;
                }
            };
        }
        #endregion

        #region Getters and Setters
        /// <summary>
        /// The track object
        /// </summary>
        public Core.API.Endpoints.Track TrackData
        {
            get { return GetValue(_trackDataProperty) as Core.API.Endpoints.Track; }
            set { SetValue(_trackDataProperty, value); }
        }

        /// <summary>
        /// The playlist object
        /// </summary>
        public Core.API.Endpoints.Playlist PlaylistData
        {
            get { return GetValue(_playlistDataProperty) as Core.API.Endpoints.Playlist; }
            set { SetValue(_playlistDataProperty, value); }
        }

        /// <summary>
        /// The user object
        /// </summary>
        public Core.API.Endpoints.User UserData
        {
            get { return GetValue(_userDataProperty) as Core.API.Endpoints.User; }
            set { SetValue(_userDataProperty, value); }
        }

        /// <summary>
        /// The comment object
        /// </summary>
        public Core.API.Endpoints.Comment CommentData
        {
            get { return GetValue(_commentProperty) as Core.API.Endpoints.Comment; }
            set { SetValue(_commentProperty, value); }
        }

        /// <summary>
        /// When this item was created
        /// </summary>
        public string Creation
        {
            get { return GetValue(_creationProperty) as string; }
            set { SetValue(_creationProperty, value); }
        }

        /// <summary>
        /// What kind of object is this
        /// </summary>
        public string CollectionType
        {
            get { return GetValue(_collectionTypeProperty) as string; }
            set { SetValue(_collectionTypeProperty, value); }
        }
        #endregion
    }
}
