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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.ApplicationModel;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;
using SoundByte.UWP.Services;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    /// This page is used to login the user to SoundCloud so we can access their stream etc.
    /// </summary>
    public sealed partial class LoginView
    {
        public LoginView()
        {
            // Load the XAML page
            InitializeComponent();

            LoginWebView.NavigationStarting += (sender, args) =>
            {
                LoadingSection.Visibility = Visibility.Visible;
            };

            LoginWebView.NavigationCompleted += (sender, args) =>
            {
                LoadingSection.Visibility = Visibility.Collapsed;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get the account type
            var accountType = e.Parameter?.ToString();

            if (string.IsNullOrEmpty(accountType))
                accountType = "soundcloud";

            TelemetryService.Current.TrackPage("Login Page");

            // Generate State (for security)
            var stateVerification = new Random().Next(0, 100000000).ToString("D8");

            // Generate Application callback URI
            var callback = Uri.EscapeUriString("http://localhost/soundbyte");

            // Create the URI
            var connectUri = new Uri(accountType == "soundcloud"
                ? $"https://soundcloud.com/connect?scope=non-expiring&client_id={Common.ServiceKeys.SoundCloudClientId}&response_type=code&display=popup&redirect_uri={callback}&state={stateVerification}"
                : $"https://fanburst.com/oauth/authorize?client_id={Common.ServiceKeys.FanburstClientId}&response_type=code&redirect_uri={callback}&state={stateVerification}");

            // Navigate to the connect URI
            LoginWebView.Navigate(connectUri);

            // Handle new window requests, if a new window is requested, just navigate on the 
            // current page. 
            LoginWebView.NewWindowRequested += (view, eventArgs) =>
            {
                eventArgs.Handled = true;
                LoginWebView.Navigate(eventArgs.Uri);
            };

            // Called when the webview is going to navigate to another 
            // Uri.
            LoginWebView.NavigationStarting += async (view, eventArgs) =>
            {
                // If we are navigating to google, let the user know that google login is not supported
                if (eventArgs.Uri.Host == "accounts.google.com")
                {
                    // Cancel the page load and hide the loading panel
                    eventArgs.Cancel = true;
                    LoadingSection.Visibility = Visibility.Collapsed;
                    TelemetryService.Current.TrackEvent("Google Sign in Attempt");
                    await new MessageDialog("Google Account sign in is not supported. Please instead signin with a Facebook or SoundCloud account.", "Sign in Error").ShowAsync();
                }

                // We worry about localhost addresses are they are directed towards us.
                if (eventArgs.Uri.Host == "localhost")
                {
                    // Cancel the navigation, (as localhost does not exist).
                    eventArgs.Cancel = true;

                    // Parse the URL for work
                    // ReSharper disable once CollectionNeverUpdated.Local
                    var parser = new QueryParameterCollection(eventArgs.Uri);

                    // First we just check that the state equals (to make sure the url was not hijacked)
                    var state = parser.FirstOrDefault(x => x.Key == "state").Value;

                    // The state does not match
                    if (string.IsNullOrEmpty(state) || state.TrimEnd('#') != stateVerification)
                    {
                        // Display the error to the user
                        await new MessageDialog("State Verfication Failed. This could be caused by another process intercepting the SoundByte login procedure. Sigin has been canceled to protect your privacy.", "Sign in Error").ShowAsync();
                        TelemetryService.Current.TrackEvent("State Verfication Failed");
                        // Close
                        LoadingSection.Visibility = Visibility.Collapsed;
                        App.GoBack();
                        return;
                    }

                    // We have an error
                    if (parser.FirstOrDefault(x => x.Key == "error").Value != null)
                    {
                        var type = parser.FirstOrDefault(x => x.Key == "error").Value;
                        var reason = parser.FirstOrDefault(x => x.Key == "error_description").Value;

                        // The user denied the request
                        if (type == "access_denied")
                        {
                            LoadingSection.Visibility = Visibility.Collapsed;
                            App.GoBack();
                            return;
                        }

                        // Display the error to the user
                        await new MessageDialog(reason, "Sign in Error").ShowAsync();

                        // Close
                        LoadingSection.Visibility = Visibility.Collapsed;
                        App.GoBack();
                        return;
                    }

                    // Get the code from the url
                    if (parser.FirstOrDefault(x => x.Key == "code").Value != null)
                    {
                        var code = parser.FirstOrDefault(x => x.Key == "code").Value;

                        // Create a http client to get the token
                        using (var httpClient = new HttpClient())
                        {
                            // Set the user agent string
                            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte", Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build));

                            // Get all the params
                            var parameters = new Dictionary<string, string>
                            {
                                {"client_id", accountType == "soundcloud" ? Common.ServiceKeys.SoundCloudClientId : Common.ServiceKeys.FanburstClientId},
                                {"client_secret", accountType == "soundcloud" ? Common.ServiceKeys.SoundCloudClientSecret :  Common.ServiceKeys.FanburstClientSecret},
                                {"grant_type", "authorization_code"},
                                {"redirect_uri", callback.ToString()},
                                {"code", code}
                            };

                            var encodedContent = new FormUrlEncodedContent(parameters);

                            // Post to the soundcloud API
                            using (var postQuery = await httpClient.PostAsync(accountType == "soundcloud" ? "https://api.soundcloud.com/oauth2/token" : "https://fanburst.com/oauth/token", encodedContent))
                            {
                                // Check if the post was successful
                                if (postQuery.IsSuccessStatusCode)
                                {
                                    // Get the stream
                                    using (var stream = await postQuery.Content.ReadAsStreamAsync())
                                    {
                                        // Read the stream
                                        using (var streamReader = new StreamReader(stream))
                                        {
                                            // Get the text from the stream
                                            using (var textReader = new JsonTextReader(streamReader))
                                            {
                                                // Used to get the data from JSON
                                                var serializer = new JsonSerializer
                                                {
                                                    NullValueHandling = NullValueHandling.Ignore
                                                };

                                                // Get the class from the json
                                                var response = serializer.Deserialize<SoundByteService.Token>(textReader);

                                                // Create the password vault
                                                var vault = new PasswordVault();

                                                if (accountType == "soundcloud")
                                                {
                                                    // Store the values in the vault
                                                    vault.Add(new PasswordCredential("SoundByte.SoundCloud", "Token", response.AccessToken.ToString()));
                                                    vault.Add(new PasswordCredential("SoundByte.SoundCloud", "Scope", response.Scope.ToString()));
                                                }
                                                else
                                                {
                                                    // Store the values in the vault
                                                    vault.Add(new PasswordCredential("SoundByte.FanBurst", "Token", response.AccessToken.ToString()));
                                                }

                                                LoadingSection.Visibility = Visibility.Collapsed;
                                                TelemetryService.Current.TrackEvent("Login Successful", new Dictionary<string, string>()
                                                {
                                                    { "service", accountType }
                                                });
                                                App.NavigateTo(typeof(Home));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // Display the error to the user
                                    await new MessageDialog("Token Error. Try again later.", "Sign in Error").ShowAsync();

                                    // Close
                                    LoadingSection.Visibility = Visibility.Collapsed;
                                    App.GoBack();
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
