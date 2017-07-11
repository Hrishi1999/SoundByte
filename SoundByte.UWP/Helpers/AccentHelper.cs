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
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Helpers
{
    /// <summary>
    /// Helper methods for working with the apps accent color and
    /// selected theme
    /// </summary>
    public static class AccentHelper
    {
        #region Getters and Setters

        /// <summary>
        /// The Adjusted Apps Accent Color
        /// </summary>
        public static Color AccentColor => ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightAccentBrush"]).Color;

        /// <summary>
        /// Is the app currently using the default system theme
        /// </summary>
        public static bool IsDefaultTheme
        {
            get
            {
                switch (SettingsService.Current.ApplicationThemeType)
                {
                    case AppTheme.Default:
                        return true;
                    case AppTheme.Dark:
                        return false;
                    case AppTheme.Light:
                        return false;
                    default:
                        return true;
                }
            }
        }

        /// <summary>
        /// The apps currently picked theme color
        /// </summary>
        public static ApplicationTheme ThemeType
        {
            get
            {
                switch (SettingsService.Current.ApplicationThemeType)
                {
                    case AppTheme.Dark:
                        return ApplicationTheme.Dark;
                    case AppTheme.Light:
                        return ApplicationTheme.Light;
                    case AppTheme.Default:
                        return ApplicationTheme.Dark;
                    default:
                        return ApplicationTheme.Dark;
                }
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Refreshes the stored app accent color
        /// </summary>
        public static async void UpdateAccentColor()
        {
            // Get the stored accent color
            var accentColorStored = SettingsService.Current.AppAccentColor;
            // Where we are going to save the accent color
            Color color;
            // Check that there is a value already stored
            if (!string.IsNullOrEmpty(accentColorStored))
            {
                if (accentColorStored == "ACCENT")
                {
                    // Get the system default accent color
                    color = (Color) Application.Current.Resources["SystemAccentColor"];
                }
                else
                {
                    // Parse the accent color from HEX value
                    color = new Color
                    {
                        A = byte.Parse(accentColorStored.Substring(1, 2), NumberStyles.AllowHexSpecifier),
                        R = byte.Parse(accentColorStored.Substring(3, 2), NumberStyles.AllowHexSpecifier),
                        G = byte.Parse(accentColorStored.Substring(5, 2), NumberStyles.AllowHexSpecifier),
                        B = byte.Parse(accentColorStored.Substring(7, 2), NumberStyles.AllowHexSpecifier)
                    };
                }
            }
            else
            {
                // Set to the default accent color
                color = (Color) Application.Current.Resources["SystemAccentColor"];
            }

            // Update XAML Resources
            ((SolidColorBrush) Application.Current.Resources["AppBarColor"]).Color = color;
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightAccentBrush"]).Color = color;
            ((SolidColorBrush) Application.Current.Resources["SystemControlDisabledAccentBrush"]).Color = color;
            ((SolidColorBrush) Application.Current.Resources["SystemControlForegroundAccentBrush"]).Color = color;
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightAccentBrush"]).Color = color;
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightAltAccentBrush"]).Color = color;
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightAltListAccentHighBrush"]).Color = Color.FromArgb(229, color.R, color.G, color.B);
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightAltListAccentLowBrush"]).Color = Color.FromArgb(153, color.R, color.G, color.B);
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightAltListAccentMediumBrush"]).Color = Color.FromArgb(204, color.R, color.G, color.B);
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightListAccentHighBrush"]).Color = Color.FromArgb(229, color.R, color.G, color.B);
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightListAccentLowBrush"]).Color = Color.FromArgb(153, color.R, color.G, color.B);
            ((SolidColorBrush) Application.Current.Resources["SystemControlHighlightListAccentMediumBrush"]).Color = Color.FromArgb(204, color.R, color.G, color.B);
            ((SolidColorBrush) Application.Current.Resources["SystemControlHyperlinkTextBrush"]).Color = color;
            ((SolidColorBrush) Application.Current.Resources["ContentDialogBorderThemeBrush"]).Color = color;
            ((SolidColorBrush) Application.Current.Resources["JumpListDefaultEnabledBackground"]).Color = color;

            var textColor = Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.White : Colors.Black;

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

            if (App.IsMobile)
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
        }
        #endregion
    }
}