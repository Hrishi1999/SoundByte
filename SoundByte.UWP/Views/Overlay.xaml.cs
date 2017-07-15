//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using SoundByte.UWP.Converters;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Overlay
    {
        public BaseViewModel ViewModel { get; } = new BaseViewModel();

        private readonly CoreDispatcher _dispatcher;

        public Overlay()
        {
            InitializeComponent();

            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            ViewModel.Service.PropertyChanged += Service_PropertyChanged;

            // Set the accent color
            AccentHelper.UpdateAccentColor();

            BackgroundImage.Blur(40).Start();

            BackgroundImage.Source = new BitmapImage(new Uri(ArtworkConverter.ConvertObjectToImage(ViewModel.Service.CurrentTrack)));

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("Compact Overlay Page");
        }

        private async void Service_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTrack")
            {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
                {
                    BackgroundImage.Source = new BitmapImage(new Uri(ArtworkConverter.ConvertObjectToImage(ViewModel.Service.CurrentTrack)));
                });
            }
        }
    }
}
