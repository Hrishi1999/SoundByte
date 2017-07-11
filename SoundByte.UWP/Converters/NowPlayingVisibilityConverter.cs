//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SoundByte.UWP.Converters
{
    /// <summary>
    /// This converter is used to show or hide the now playing
    /// UI depending on if an item is playing or we are on the
    /// track page.
    /// </summary>
    public class NowPlayingVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Check to see if we are on the track page
            var isTrackPage = ((MainShell) Window.Current.Content)?.RootFrame.CurrentSourcePageType == typeof(Views.Track);

            if (value == null || isTrackPage)
                return Visibility.Collapsed;
            
            return Visibility.Visible;
        }

        /// <summary>
        /// We can not convert back in this case
        /// </summary>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
