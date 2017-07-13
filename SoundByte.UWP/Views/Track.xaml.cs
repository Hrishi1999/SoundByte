//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SoundByte.UWP.Services;
using UICompositionAnimations.Behaviours;
using UICompositionAnimations.Behaviours.Effects;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// This page handles track playback and connection to
    /// the background audio task.
    /// </summary>
    public sealed partial class Track
    {
        // Main page view model
        public ViewModels.TrackViewModel ViewModel { get; } = new ViewModels.TrackViewModel();

        /// <summary>
        /// Setup page and init the xaml
        /// </summary>
        public Track()
        {
            InitializeComponent();
            // This page must be cached
            NavigationCacheMode = NavigationCacheMode.Enabled;
            // Set the data context
            DataContext = ViewModel;
            // Page has been unloaded from UI
            Unloaded += (s, e) => ViewModel.Dispose();

            SizeChanged += (sender, args) =>
            {
                if (IsEnhanced)
                    ShowOverlay();
                else
                    HideOverlay();
            };
        }

        /// <summary>
        /// Setup the view model, passing in the navigation events.
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Setup view model
            ViewModel.SetupModel();
            // Track Event
            TelemetryService.Current.TrackPage("Now Playing Page");

            if (App.IsDesktop)
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor =
                    new Color {R = 0, G = 0, B = 0, A = 20};
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor =
                    new Color {R = 0, G = 0, B = 0, A = 60};
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = Colors.White;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = Colors.White;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = Colors.White;
            }

            // Hide the overlay for a new session
            HideOverlay();

            // Override the back event button

            if (App.IsXbox)
                FullScreenButton.Visibility = Visibility.Collapsed;
        }

        private void Track_BackRequested(object sender, BackRequestedEventArgs e)
        {
            HideOverlay();
        }

        /// <summary>
        /// Clean the view model
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.CleanModel();

            var textColor = Windows.UI.Xaml.Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.White : Colors.Black;

            if (App.IsDesktop)
            {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor = new Color { R = 0, G = 0, B = 0, A = 20 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor = new Color { R = 0, G = 0, B = 0, A = 60 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = textColor;
            }

            HideOverlay();
        } 

        private bool IsEnhanced { get; set; } = false;

        private void HideOverlay()
        {
            IsEnhanced = false;

            App.OverrideBackEvent = false;
            SystemNavigationManager.GetForCurrentView().BackRequested -= Track_BackRequested;

            ButtonHolder.Visibility = Visibility.Visible;
            ButtonHolder.Offset(0, 0, 450).Fade(1, 250).Start();

            EnhanceButton.Rotate(0, (float)EnhanceButton.ActualWidth / 2, (float)EnhanceButton.ActualHeight / 2, 450).Offset(0, 0, 450).Start();

            var moreInfoAnimation = MoreInfoScreen.Fade(0, 450).Offset(0, (float)RootGrid.ActualHeight, 450);
            moreInfoAnimation.Completed += (o, args) => 
            {
                MoreInfoScreen.Visibility = Visibility.Collapsed;
                MoreInfoPivot.SelectedIndex = 0;
            };
            moreInfoAnimation.Start();

            TrackInfoHolder.Offset(0, 0, 450).Scale(1, 1, 0, 0, 450).Start();

            BlurOverlay.Fade(0, 450).Start();
        }

        private void ShowOverlay()
        {
            TelemetryService.Current.TrackEvent("Show Now Playing Overlay");

            IsEnhanced = true;

            App.OverrideBackEvent = true;
            SystemNavigationManager.GetForCurrentView().BackRequested += Track_BackRequested;

            var buttonHolderShowAnimation = ButtonHolder.Offset(0, 120, 450).Fade(0, 250);
            buttonHolderShowAnimation.Completed += (o, args) => { ButtonHolder.Visibility = Visibility.Collapsed; };
            buttonHolderShowAnimation.Start();

            EnhanceButton.Rotate(180, (float)EnhanceButton.ActualWidth / 2, (float)EnhanceButton.ActualHeight / 2, 450).Offset(0, -1.0f * ((float)RootGrid.ActualHeight - (float)EnhanceButton.ActualHeight - 160), 450).Start();

            MoreInfoScreen.Visibility = Visibility.Visible;
            MoreInfoPivot.SelectedIndex = 0;
            MoreInfoScreen.Fade(1, 450, 150).Offset(0, 0, 450, 150).Start();

            TrackInfoHolder.Offset(0, -1.0f * ((float)RootGrid.ActualHeight - (float)TrackInfoHolder.ActualHeight - 40), 450).Scale(0.8f,0.8f,0,0, 450).Start();

            BlurOverlay.Fade(1, 450).Start();
        }

        /// <summary>
        /// This cannot go in the view model, as we change UI elements A LOT here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTransition(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (IsEnhanced)
            {
                HideOverlay();
            }
            else
            {
                ShowOverlay();
            }
        }

        private void test(object sender, RoutedEventArgs e)
        {

        }
    }
}