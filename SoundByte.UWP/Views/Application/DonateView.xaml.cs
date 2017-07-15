//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views.Application
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DonateView
    {
        public DonateView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            TelemetryService.Current.TrackPage("Donate Page");

            // We are loading
            App.IsLoading = true;

            // Get all the products
            var donateProducts = await MonitizeService.Current.GetProductInfoAsync();

            LooseChangePrice.Text = donateProducts.Exists(t => t.Key.ToLower() == "9p3vls5wtft6") ? donateProducts.Find(t => t.Key.ToLower() == "9p3vls5wtft6").Value.Price.FormattedBasePrice : "Unknown";
            SmallCoffeePrice.Text = donateProducts.Exists(t => t.Key.ToLower() == "9msxrvnlnlj7") ? donateProducts.Find(t => t.Key.ToLower() == "9msxrvnlnlj7").Value.Price.FormattedBasePrice : "Unknown";
            RegularCoffeePrice.Text = donateProducts.Exists(t => t.Key.ToLower() == "9nrgs6r2grsz") ? donateProducts.Find(t => t.Key.ToLower() == "9nrgs6r2grsz").Value.Price.FormattedBasePrice : "Unknown";
            LargeCoffeePrice.Text = donateProducts.Exists(t => t.Key.ToLower() == "9pnsd6hskwpk") ? donateProducts.Find(t => t.Key.ToLower() == "9pnsd6hskwpk").Value.Price.FormattedBasePrice : "Unknown";

            // We are not loading now
            App.IsLoading = false;
        }

        private async void PayPalDonate(object sender, RoutedEventArgs e)
        {
            TelemetryService.Current.TrackEvent("Donation Attempt", new Dictionary<string, string> { { "StoreID", "PayPal" } });

            await Launcher.LaunchUriAsync(new Uri(
                "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XJ5XWEXKFCQS6"));
        }

        private async void DonateLooseChange(object sender, RoutedEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            await MonitizeService.Current.PurchaseDonation("9p3vls5wtft6");

            // We are not loading
            App.IsLoading = false;
        }

        private async void DonateSmall(object sender, RoutedEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            await MonitizeService.Current.PurchaseDonation("9msxrvnlnlj7");

            // We are not loading
            App.IsLoading = false;
        }

        private async void DonateRegular(object sender, RoutedEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            await MonitizeService.Current.PurchaseDonation("9nrgs6r2grsz");

            // We are not loading
            App.IsLoading = false;
        }

        private async void DonateLarge(object sender, RoutedEventArgs e)
        {
            // We are loading
            App.IsLoading = true;

            await MonitizeService.Current.PurchaseDonation("9pnsd6hskwpk");

            // We are not loading
            App.IsLoading = false;
        }
    }
}