//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.UI.Xaml;
using SoundByte.UWP.Views.General;

namespace SoundByte.UWP.Dialogs
{
    public sealed partial class AppUpdateDialog
    {
        public AppUpdateDialog()
        {
            InitializeComponent();
        }

        private void NavigateWhatNew(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(WhatsNewView));
            Hide();
        }
    }
}
