//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SoundByte.UWP.UserControls
{
    public sealed partial class SearchBox
    {
        /// <summary>
        /// Event handler for binding to the submitted
        /// event.
        /// </summary>
        public event RoutedEventHandler SearchSubmitted;

        /// <summary>
        /// The text currently stored in the 
        /// search box
        /// </summary>
        public string Text
        {
            get => AutoSearchBox.Text;
            set => AutoSearchBox.Text = value;
        }

        public SearchBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the query is submitted from the text box
        /// </summary>
        private void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            SearchBoxSubmitted();
        }

        /// <summary>
        /// Called when the text changes on the text box
        /// This method detects the enter key and then
        /// performs a search.
        /// </summary>
        private void TextAdded(object sender, KeyRoutedEventArgs e)
        {
            // Check if enter key
            if (e.Key == Windows.System.VirtualKey.Enter || e.Key == Windows.System.VirtualKey.GamepadMenu)
                SearchBoxSubmitted();
        }

        private void SearchBoxSubmitted()
        {
            // Create the search arguments
            var args = new SearchEventArgs { Keyword = AutoSearchBox.Text };
            // Call the event handler
            SearchSubmitted?.Invoke(this, args);
        }

        /// <summary>
        /// Holds the keyword that came from 
        /// the search box
        /// </summary>
        public class SearchEventArgs : RoutedEventArgs
        {
            /// <summary>
            /// The Keyword that the user
            /// search for in the searched
            /// box
            /// </summary>
            public string Keyword { get; set; }
        }
    }
}