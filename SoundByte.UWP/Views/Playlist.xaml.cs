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
    /// Displays a playlist
    /// </summary>
    public sealed partial class Playlist
    {
        // Page View Model
        public readonly ViewModels.PlaylistViewModel ViewModel = new ViewModels.PlaylistViewModel();

        public Playlist()
        {
            // Setup the XAML
            InitializeComponent();
            // This page must be cached
            NavigationCacheMode = NavigationCacheMode.Enabled;
            // Set the data context
            DataContext = ViewModel;
        }

        /// <summary>
        /// Called when the user navigates to the page
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Make sure the view is ready for the user
            await ViewModel.SetupView(e.Parameter as Core.API.Endpoints.Playlist);
            // Track Event
            TelemetryService.Current.TrackPage("Playlist Page");
        }
    }
}
