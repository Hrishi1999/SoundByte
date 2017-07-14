//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// This page is the main landing page for any user.
    /// This page displays the users stream, the latest/trending tracks,
    /// and the users playlists/likes.
    /// </summary>
    public sealed partial class StreamView
    {
        // The view model
        public StreamViewModel ViewModel = new StreamViewModel();

        /// <summary>
        /// Setup page and init the xaml
        /// </summary>
        public StreamView()
        {
            InitializeComponent();
            // This page must be cached
            NavigationCacheMode = NavigationCacheMode.Enabled;
            // Set the data context
            DataContext = ViewModel;
            // Page has been unloaded from UI
            Unloaded += (s, e) => ViewModel.Dispose();
        }

        /// <summary>
        /// Called when the user navigates to the page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the last visited frame (crash handling)
            SettingsService.Current.LastFrame = typeof(StreamView).FullName;
            // Always clear the backstack when navigating to the main page
            Frame.BackStack.Clear();
            // Store the latest time (for notification task)
            SettingsService.Current.LatestViewedTrack = DateTime.Now;
            // Track Event
            TelemetryService.Current.TrackPage("Stream Page");
        }
    }
}