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

namespace SoundByte.UWP.Converters
{
    /// <summary>
    /// This class takes in a timespac and converts it into a human
    /// readable string.
    /// </summary>
    public class TimeStampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var timeSpan = new TimeSpan();

                if (value is TimeSpan)
                    timeSpan = (TimeSpan) value;

                if (value is int)
                    timeSpan = TimeSpan.FromMilliseconds((int) value);

                if (value is string)
                    timeSpan = TimeSpan.FromMilliseconds(int.Parse(value.ToString()));

                var returnValue = timeSpan.TotalHours < 1.0 ? string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds) : string.Format("{0}:{1:D2}:{2:D2}", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
                
                // Return the formatted value
                return returnValue;
            }
            catch (Exception)
            {
                // There was an error either parsing the value or converting the
                // time stamp. We will show a generic unknown message here.
                return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
