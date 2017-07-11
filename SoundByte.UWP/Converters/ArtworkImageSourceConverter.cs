//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace SoundByte.UWP.Converters
{
    /// <summary>
    /// The artwork converter is used to get the best quality
    /// and correct artwork for the provided resource. This was
    /// previously done in the endpoint classes them selves, but to 
    /// reduce app size and code reuse, the methods have been moved here.
    /// 
    /// This converter calls the Artwork converter, but change the return type 
    /// to an image source.
    /// </summary>
    public class ArtworkImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var imageUri = ArtworkConverter.ConvertObjectToImage(value);

            if (string.IsNullOrEmpty(imageUri))
            {
                return null;
            }
            else
            {
                return new BitmapImage(new Uri(imageUri));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
