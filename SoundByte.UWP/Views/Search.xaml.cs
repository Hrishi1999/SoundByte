//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// This page lets the user search for tracks/playlists/people 
    /// within SoundCloud.
    /// </summary>
    public sealed partial class Search
    {
        // The view model for the page
        public ViewModels.SearchViewModel ViewModel = new ViewModels.SearchViewModel();

        /// <summary>
        /// Setup the page
        /// </summary>
        public Search()
        {
            // Initialize XAML Components
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
        /// <param name="e">Args</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the last visited frame (crash handling)
            SettingsService.Current.LastFrame = typeof(Search).FullName;
            // Set the search string
            ViewModel.SearchQuery = e.Parameter != null ? e.Parameter as string : string.Empty;
            // Track Event
            TelemetryService.Current.TrackPage("Search Page");
        }
    }
}
