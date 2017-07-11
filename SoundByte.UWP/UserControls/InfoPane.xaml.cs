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
    /// This control is used to show friendly messages 
    /// within the app
    /// </summary>
    public sealed partial class InfoPane
    {
        #region Binding Variables
        private static readonly DependencyProperty _textProperty = DependencyProperty.Register("Text", typeof(string), typeof(InfoPane), null);
        private static readonly DependencyProperty _glyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(InfoPane), null);
        private static readonly DependencyProperty _headerProperty = DependencyProperty.Register("Header", typeof(string), typeof(InfoPane), null);
        #endregion

        #region Page Setup
        /// <summary>
        /// Load the XAML part of the user control
        /// </summary>
        public InfoPane() { InitializeComponent(); }
        #endregion

        #region Getters and Setters
        /// <summary>
        /// The title to show on the error control
        /// </summary>
        public string Header
        {
            get => GetValue(_headerProperty) as string;
            set => SetValue(_headerProperty, value);
        }

        /// <summary>
        /// The icon to show on the control
        /// </summary>
        public string Glyph
        {
            get => GetValue(_glyphProperty) as string;
            set => SetValue(_glyphProperty, value);
        }

        /// <summary>
        /// The text to show on the error control
        /// </summary>
        public string Text
        {
            get => GetValue(_textProperty) as string;
            set => SetValue(_textProperty, value);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Shows a message on the screen
        /// </summary>
        /// <param name="header">The title of the message</param>
        /// <param name="text">The text of the message</param>
        /// <param name="glyph">The picture to show</param>
        /// <param name="showButton">Should we display the close button</param>
        public void ShowMessage(string header, string text, string glyph, bool showButton = true)
        {
            // Update the needed variables
            Header = header;
            Text = text;
            Glyph = glyph;

            // Logic to show or hide the buton
            CloseButton.Visibility = (showButton ? Visibility.Visible : Visibility.Collapsed);

            // Show the control
            Visibility = Visibility.Visible;
            Opacity = 1;
        }

        /// <summary>
        /// Closes the pane
        /// </summary>
        private void ClosePane(object sender, RoutedEventArgs e)
        {
            // Hide the pane
            Visibility = Visibility.Collapsed;
            Opacity = 0;
        }
        #endregion
    }
}
