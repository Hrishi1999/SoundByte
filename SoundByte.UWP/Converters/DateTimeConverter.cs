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
    /// This class takes in a DateTime object and converts it into
    /// a human readable form.
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
        /// <summary>
        /// This function takes in a dattime object and 
        /// </summary>
        /// <returns>A human readable date time object</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                // Return the formatted DateTime 
                return NumberFormatHelper.GetTimeDateString((DateTime)value, true);
            }
            catch (Exception)
            {
                // There was an error either parsing the value or converting the
                // date time. We will show a generic unknown message here.
                return "Unknown";
            }
        }

        /// <summary>
        /// This function is not needed and should not be used.
        /// It returns the current date time just in case it is
        /// called.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
