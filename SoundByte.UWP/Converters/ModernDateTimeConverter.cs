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
using SoundByte.UWP.Helpers;

namespace SoundByte.UWP.Converters
{
    /// <summary>
    /// A modern version of the datetime converter that supports the new
    /// type of datetime and handles error cleanly.
    /// </summary>
    public class ModernDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "Unknown";

            try
            {
                var inputDate = DateTime.Parse(value.ToString());

                return NumberFormatHelper.GetTimeDateString(inputDate, true);

            }
            catch
            {
                return "Unknown";
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Not yet, hahahah
            throw new NotImplementedException();
        }
    }
}
