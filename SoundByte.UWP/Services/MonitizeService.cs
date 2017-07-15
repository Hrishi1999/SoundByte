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
using System.Linq;
using System.Threading.Tasks;
using Windows.Services.Store;
using Windows.UI.Popups;

namespace SoundByte.UWP.Services
{
    public class MonitizeService
    {
        // Store Context used to access the store.
        private readonly StoreContext _storeContext;
        // Private class instance
        private static MonitizeService _instance;

        private MonitizeService()
        {
            _storeContext = StoreContext.GetDefault();
        }

        public async Task<bool> PurchaseDonation(string storeId)
        {
            TelemetryService.Current.TrackEvent("Donation Attempt", new Dictionary<string, string> { { "StoreID", storeId } });

            // Get the item
            var item = (await GetProductInfoAsync()).FirstOrDefault(x => x.Key.ToLower() == storeId).Value;
            // Request to purchase the item
            var result = await item.RequestPurchaseAsync();

            // Check if the purchase was successful
            if (result.Status == StorePurchaseStatus.Succeeded)
            {
                TelemetryService.Current.TrackEvent("Donation Successful", new Dictionary<string, string> { { "StoreID", storeId } });

                await new MessageDialog("Thank you for your donation!", "SoundByte").ShowAsync();
            }
            else
            {
                TelemetryService.Current.TrackEvent("Donation Failed", new Dictionary<string, string> { { "StoreID", storeId }, { "Reason", result.ExtendedError.Message } });

                await new MessageDialog("Your account has not been charged:\n" + result.ExtendedError.Message, "SoundByte").ShowAsync();
            }

            return true;
        }

        public async Task<List<KeyValuePair<string, StoreProduct>>> GetProductInfoAsync()
        {
            var list = new List<KeyValuePair<string, StoreProduct>>();

            // Specify the kinds of add-ons to retrieve.
            var filterList = new List<string> { "Durable", "Consumable", "UnmanagedConsumable" };

            // Specify the Store IDs of the products to retrieve.
            var storeIds = new[]
            {
                "9nrgs6r2grsz", // Regular Coffee
                "9p3vls5wtft6", // Loose Change
                "9msxrvnlnlj7", // Small Coffee
                "9pnsd6hskwpk"  // Large Coffee
            };

            var results = await _storeContext.GetStoreProductsAsync(filterList, storeIds);

            if (results.ExtendedError != null)
            {
                await new MessageDialog(results.ExtendedError.Message).ShowAsync();
                return list;
            }

            list.AddRange(results.Products);

            return list;
        }

        /// <summary>
        /// Sets up the App Monetize class, when the app is run in debug mode,
        /// it used the Store Proxy xml file. When built in release mode it
        /// uses the stores license information file.
        /// </summary>
        public static MonitizeService Current => _instance ?? (_instance = new MonitizeService());
    }
}
