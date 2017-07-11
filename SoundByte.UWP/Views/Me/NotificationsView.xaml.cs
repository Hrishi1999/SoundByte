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

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    /// Displays the users notifications
    /// </summary>
    public sealed partial class NotificationsView
    {
        // The view model
        public ViewModels.NotificationsViewModel ViewModel = new ViewModels.NotificationsViewModel();

        /// <summary>
        /// Load the page and init the xaml
        /// </summary>
        public NotificationsView()
        {
            // Setup the XAML
            InitializeComponent();
            // This page must be cached
            NavigationCacheMode = NavigationCacheMode.Enabled;
            // Set the data context
            DataContext = ViewModel;
            
            // Page has been unloaded from UI
          //  Unloaded += (s, e) =>
          //  {
           //     ViewModel.Dispose();
          //      ViewModel = null;
       //     };

            // Create the view model on load
       //     Loaded += (s, e) => ViewModel = new ViewModels.NotificationsViewModel();
        }

        /// <summary>
        /// Called when the user navigates to the page
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the last frame
            SettingsService.Current.LastFrame = typeof(NotificationsView).FullName;
            // Track event
            TelemetryService.Current.TrackPage("Notifications Page");
        }
    }
}
