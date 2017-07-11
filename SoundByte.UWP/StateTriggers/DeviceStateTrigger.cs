//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.UI.Xaml;

namespace SoundByte.UWP.StateTriggers
{
    public class DeviceStateTrigger : StateTriggerBase
    {
        private string _deviceFamily;

        public string DeviceFamily
        {
            get => _deviceFamily;
            set
            {
                _deviceFamily = value;
                SetActive(_deviceFamily == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily);
            }
        }
    }
}
