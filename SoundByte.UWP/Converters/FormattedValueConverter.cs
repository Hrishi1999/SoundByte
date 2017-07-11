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
    /// Converts a nullable into to a human readable string
    /// </summary>
    public class FormattedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Get our value
            var inValue = value as int?;

            // Does this null int have a value
            if (inValue.HasValue)
            {
                // Return the actual value of 0 if there is none
                return inValue.Value == 0 ? "0" : NumberFormatHelper.GetFormattedLargeNumber(inValue.Value);
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
