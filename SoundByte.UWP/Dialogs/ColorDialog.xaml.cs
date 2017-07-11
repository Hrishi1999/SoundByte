//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class ColorDialog
    {
        public ColorDialog()
        {
            InitializeComponent();

            // Set the accent color
            ColorPicker.Color = AccentHelper.AccentColor;
        }

        public void Apply()
        {
            SettingsService.Current.AppAccentColor = ColorPicker.Color.ToString();
            AccentHelper.UpdateAccentColor();
            Hide();
        }
    }
}
