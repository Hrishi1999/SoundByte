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
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.Globalization;
using Windows.Networking.PushNotifications;
using Windows.Services.Store;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.WindowsAzure.Messaging;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using SoundByte.UWP.Views.Application;
using SoundByte.UWP.Views.CoreApp;
using SoundByte.UWP.Views.Me;
using SoundByte.UWP.Views.Mobile;

namespace SoundByte.UWP
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainShell
    {
        /// <summary>
        /// Used to access the playback service from the UI
        /// </summary>
        public PlaybackService Service => PlaybackService.Current;

        public SoundByteService SoundByteService => SoundByteService.Current;

        public MainShell(string path)
        {
            // Init the XAML
            InitializeComponent();

            // Set the accent color
            AccentHelper.UpdateAccentColor();

            // Amoled Magic
            if (App.IsMobile)
                Application.Current.Resources["ShellBackground"] = new SolidColorBrush(Application.Current.RequestedTheme == ApplicationTheme.Dark ? Colors.Black : Colors.White);

            // Make xbox selection easy to see
            if (App.IsXbox)
                Application.Current.Resources["CircleButtonStyle"] = Application.Current.Resources["XboxCircleButtonStyle"];

            // When the page is loaded (after the following and xaml init)
            // we can perform the async work
            Loaded += async (sender, args) => await PerformAsyncWork(path);

            // This is a dirty to show the now playing
            // bar when a track is played. This method
            // updates the required layout for the now
            // playint bar.
            Service.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName != "CurrentTrack")
                    return;

                if (Service.CurrentTrack == null || !App.IsDesktop || RootFrame.CurrentSourcePageType == typeof(Track))
                    HideNowPlayingBar();
                else
                    ShowNowPlayingBar();
            };

            // Create the blur for desktop
            if (App.IsDesktop)
            {
                CreateShellFrameShadow();
            }
            else
            {
                HideNowPlayingBar();
            }

            if (App.IsXbox)
            {
                RootFrame.Margin = new Thickness { Left = 50 };
                MainSplitView.IsPaneOpen = false;

                // Hide the labels
                SearchXboxTab.Visibility = Visibility.Visible;

                // Center icons
                NavbarScrollViewer.VerticalAlignment = VerticalAlignment.Center;

                // Show backgroudn iamge
                XboxOnlyGrid.Visibility = Visibility.Visible;
                ShellFrame.Background = new SolidColorBrush(Colors.Transparent);
            }

            // Mobile Specific stuff
            if (App.IsMobile)
            {
                RootFrame.Margin = new Thickness {Left = 0, Right = 0, Top = 0, Bottom = 64};
                MainSplitView.IsPaneOpen = false;
                MainSplitView.CompactPaneLength = 0;
                MobileNavigation.Visibility = Visibility.Visible;
                NowPlaying.Visibility = Visibility.Collapsed;
            }
            else
            {
                MobileNavigation.Visibility = Visibility.Collapsed;
            }

            RootFrame.Focus(FocusState.Programmatic);
        }
    
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (App.OverrideBackEvent)
            {
                e.Handled = true;
            }
            else
            {
                if (RootFrame.CanGoBack)
                {
                    RootFrame.GoBack();
                    e.Handled = true;
                }
                else
                {
                    if (RootFrame.SourcePageType == typeof(HomeView)) return;

                    RootFrame.Navigate(typeof(HomeView));
                    e.Handled = true;
                }
            }
        }

        private async Task PerformAsyncWork(string path)
        {
            App.IsLoading = true;
            App.NavigateTo(typeof(BlankPage));

            // Set the app language
            ApplicationLanguages.PrimaryLanguageOverride =
                string.IsNullOrEmpty(SettingsService.Current.CurrentAppLanguage)
                    ? ApplicationLanguages.Languages[0]
                    : SettingsService.Current.CurrentAppLanguage;

            // Set the on back requested event
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            if (App.IsMobile)
            {
                await new MessageDialog("Please Note: SoundByte v2 for Windows Phone is still under active development. The reason why you have this update is because of limitations in the Windows Store. In order to release v2 to desktop and Xbox users, I must also release it to Windows Phone Users.\n\nI will continue to work on SoundByte for Windows Phone as time goes on.\n\nIf you encounter any issues, please contact me at dominic.maas@live.com.", "READ ME").ShowAsync();
            }

            // Navigate to the first page
            await HandleProtocolAsync(path);

            // Test Version and tell user app upgraded
            await HandleNewAppVersion();

            // Clear the unread badge
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

            // The methods below are sorted into try catch groups. many of them can fail, but they are not important
            try
            {
                // Get the store and check for app updates
                var updates = await StoreContext.GetDefault().GetAppAndOptionalStorePackageUpdatesAsync();

                // If we have updates navigate to the update page where we
                // ask the user if they would like to update or not (depending
                // if the update is mandatory or not).
                if (updates.Count > 0)
                    await new PendingUpdateDialog().ShowAsync();
            }
            catch
            {
                // ignored
            }

            try
            {
                // Create and register the notification task
                BackgroundExecutionManager.RemoveAccess();
                await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskHelper.Register("SoundByte_BackgroundTask", new TimeTrigger(15, false));
            }
            catch
            {
                // ignored
            }

            try
            {
                // Create our notification channel and also supply the client id as a
                // tag, this can then be used to send targeted messages. We also supply the 
                // app version and device type.
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                var hub = new NotificationHub("SoundByteAppNotificationHub", "Endpoint=sb://soundbyte.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=WPV48GftvDYzvC26RuQ7laYROd677k6ducBRUaZ9G9o=");

                // If the user logged in
                var login = SoundByteService.Current.IsSoundCloudAccountConnected ? "login_true" : "login_false";

                await hub.RegisterNativeAsync(channel.Uri, new[]
                {
                    SystemInformation.DeviceFamily,
                    SystemInformation.ApplicationVersion.ToString(),
                    login
                });
            }
            catch
            {
                // ignored
            }
        }

        private async Task HandleNewAppVersion()
        {
            var currentAppVersionString = Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build;

            // Get stored app version (this will stay the same when app is updated)
            var storedAppVersionString = SettingsService.Current.AppStoredVersion;

            // Save the new app version
            SettingsService.Current.AppStoredVersion = currentAppVersionString;

            // If the stored version is null, set the temp to 0, and the version to the actual version
            if (!string.IsNullOrEmpty(storedAppVersionString))
            {
                // Convert the current app version
                var currentAppVersion = new Version(currentAppVersionString);
                // Convert the stored app version
                var storedAppVersion = new Version(storedAppVersionString);

                if (currentAppVersion <= storedAppVersion)
                    return;
            }

            // If the stored app version is null, this is the users first time,
            if (string.IsNullOrEmpty(storedAppVersionString))
            {
                App.NavigateTo(typeof(WelcomeView));
            }
            else
            {
                // Show update dialog
                await new AppUpdateDialog().ShowAsync();
            }
        }

        #region Protocol

        public async Task HandleProtocolAsync(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var parser = DeepLinkParser.Create(path);

                    var section = parser.Root.Split('/')[0].ToLower();
                    var page = parser.Root.Split('/')[1].ToLower();

                    App.IsLoading = true;
                    if (section == "core")
                    {
                        switch (page)
                        {
                            case "track":
                                var track = await SoundByteService.Current.GetAsync<Core.API.Endpoints.Track>($"/tracks/{parser["id"]}");

                                var startPlayback = await PlaybackService.Current.StartMediaPlayback(new List<Core.API.Endpoints.Track> { track }, $"Protocol-{track.Id}");

                                if (!startPlayback.success)
                                    await new MessageDialog(startPlayback.message, "Error playing track.").ShowAsync();
                                break;
                            case "playlist":
                                var playlist = await SoundByteService.Current.GetAsync<Core.API.Endpoints.Playlist>($"/playlists/{parser["id"]}");
                                App.NavigateTo(typeof(Playlist), playlist);
                                return;
                            case "user":
                                var user = await SoundByteService.Current.GetAsync<Core.API.Endpoints.User>($"/users/{parser["id"]}");
                                App.NavigateTo(typeof(UserView), user);
                                return;
                        }
                    }
                }
                catch (Exception)
                {
                    await new MessageDialog("The specified protocol is not correct. App will now launch as normal.").ShowAsync();
                }
                App.IsLoading = false;
            }

            RootFrame.Navigate(typeof(HomeView));
        }

        #endregion

        private void NavigateHome(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(HomeView));
        }

        private void NavigateDonate(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(DonateView));
        }

        private void NavigateSettings(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(SettingsView));
        }

        private void NavigateNotifications(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(NotificationsView));
        }

        private void NavigateHistory(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(HistoryView));
        }

     

        private void NavigateLikes(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(LikesView));
        }

        private void NavigateTrack()
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(Track));
        }

        private void NavigateLogin(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(LoginView));
        }

        private void NavigateSets(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(PlaylistsView));
        }

        private void NavigateUserProfile(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(UserView), SoundByteService.Current.CurrentUser);
        }

        private void NavigateSearch(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(Search));
        }

        private void NavigateMobileNavView(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            RootFrame.Navigate(typeof(MobileNavView));
        }

        private async void NavigateCurrentPlaying(object sender, RoutedEventArgs e)
        {
            if (BlockNavigation) return;

            if (PlaybackService.Current.CurrentTrack != null)
            {
                RootFrame.Navigate(typeof(Track));
            }
            else
            {
                await new MessageDialog("No Items are currently playing...").ShowAsync();
            }
        }

        private void ShellFrame_Navigated(object sender, NavigationEventArgs e)
        {
            BlockNavigation = true;

            // Update the side bar
            switch (((Frame)sender).SourcePageType.Name)
            {
                case "HomeView":
                    HomeTab.IsChecked = true;
                    MobileHomeTab.IsChecked = true;
                    break;
                case "Track":
                    UnknownTab.IsChecked = true;
                    NowPlayingTab.IsChecked = true;
                    break;
                case "DonateView":
                    DonateTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
                case "LikesView":
                    LikesTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
                case "PlaylistsView":
                    SetsTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
                case "NotificationsView":
                    NotificationsTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
                case "HistoryView":
                    HistoryTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
                case "LoginView":
                    LoginTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
                case "SettingsView":
                    SettingsTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
                case "Search":
                    SearchXboxTab.IsChecked = true;
                    MobileSearchTab.IsChecked = true;
                    break;
                case "AboutView":
                    SettingsTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
                case "MobileNavView":
                    UnknownTab.IsChecked = true;
                    MenuMobileTab.IsChecked = true;
                    break;
                default:
                    UnknownTab.IsChecked = true;
                    MobileUnkownTab.IsChecked = true;
                    break;
            }

            RootFrame.Focus(FocusState.Keyboard);

            if (((Frame)sender).SourcePageType == typeof(HomeView) || ((Frame)sender).SourcePageType == typeof(MainShell))
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            else
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;

            // Update the UI depending if we are logged in or not
            if (SoundByteService.Current.IsSoundCloudAccountConnected)
                ShowLoginContent();
            else
                ShowLogoutContent();

            if (App.IsDesktop)
            {
                if (((Frame)sender).SourcePageType.Name == "Track")
                {
                    MainSplitView.IsPaneOpen = false;
                    MainSplitView.CompactPaneLength = 0;

                    LoadingRing.Margin = new Thickness { Left = 0, Right = 0, Top = 0, Bottom = 0 };

                    HideNowPlayingBar();

                    MainSplitView.Margin = new Thickness { Bottom = 0, Top = 0 };

                }
                else
                {
                    MainSplitView.IsPaneOpen = true;
                    MainSplitView.CompactPaneLength = 84;

                    if (Service.CurrentTrack == null)
                    {
                        LoadingRing.Margin = new Thickness { Left = 350, Right = 0, Top = 32, Bottom = 0 };

                        HideNowPlayingBar();

                    }
                    else
                    {
                        LoadingRing.Margin = new Thickness { Left = 350, Right = 0, Top = 32, Bottom = 64 };
                        ShowNowPlayingBar();
                    }
                }
            }

            if (App.IsXbox)
            {
                if (((Frame)sender).SourcePageType.Name == "Track")
                {
                    MainSplitView.IsPaneOpen = false;
                    MainSplitView.CompactPaneLength = 0;
                }
                else
                {
                    MainSplitView.IsPaneOpen = true;
                    MainSplitView.CompactPaneLength = 84;

                }
            }

            BlockNavigation = false;
        }

        private void HideNowPlayingBar()
        {
            UnknownTab.IsChecked = true;
            NowPlaying.Visibility = Visibility.Collapsed;
            MainSplitView.Margin = new Thickness { Bottom = 0, Top = 32};
        }

        private void ShowNowPlayingBar()
        {
            NowPlaying.Visibility = Visibility.Visible;
            MainSplitView.Margin = new Thickness { Bottom = 64, Top = 32};
        }

        private void SearchBox_SearchSubmitted(object sender, RoutedEventArgs e)
        {
            App.NavigateTo(typeof(Search), (e as UserControls.SearchBox.SearchEventArgs)?.Keyword);
        }

        // Login and Logout events. This is used to display what pages
        // are visiable to the user.
        private void ShowLoginContent()
        {
            HomeTab.Visibility = Visibility.Visible;
            LikesTab.Visibility = Visibility.Visible;
            SetsTab.Visibility = Visibility.Visible;
            NotificationsTab.Visibility = Visibility.Visible;
            HistoryTab.Visibility = Visibility.Visible;
            LoginTab.Visibility = Visibility.Collapsed;
            UserButton.Visibility = Visibility.Visible;

            MobileHomeTab.IsEnabled = true;

            if (App.IsXbox)
            {
                // Hide buttons we don't want to expose on the xbox version
                SearchTab.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowLogoutContent()
        {
            HomeTab.Visibility = Visibility.Visible;
            LikesTab.Visibility = Visibility.Collapsed;
            SetsTab.Visibility = Visibility.Collapsed;
            NotificationsTab.Visibility = Visibility.Collapsed;
            HistoryTab.Visibility = Visibility.Collapsed;
            LoginTab.Visibility = Visibility.Visible;
            UserButton.Visibility = Visibility.Collapsed;
            MobileHomeTab.IsEnabled = false;

            if (App.IsXbox)
            {
                SearchTab.Visibility = Visibility.Collapsed;
            }
        }

        #region Composition
        private void CreateShellFrameShadow()
        {
            // Get the compositor for this window
            var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            
            // Create a visual element that we will apply the shadow to and attach 
            // to the ShellFrameShadow element.
            var shellFrameShadowVisual = compositor.CreateSpriteVisual();

            // Apply the shadow effects
            var shellDropShadow = compositor.CreateDropShadow();
            shellDropShadow.Offset = new Vector3(0, 0, 0);
            shellDropShadow.BlurRadius = 20;
            shellDropShadow.Color = new Color { A = 52, R = 0, G = 0, B = 0 };

            // Set the element visual
            shellFrameShadowVisual.Shadow = shellDropShadow;
            ElementCompositionPreview.SetElementChildVisual(ShellFrameShadow, shellFrameShadowVisual);

            // Get the shell frame for animation purposes
            var shellFrameVisual = ElementCompositionPreview.GetElementVisual(ShellFrame);

            var shellFrameSizeAnimation = compositor.CreateExpressionAnimation("shellFrameVisual.Size");
            shellFrameSizeAnimation.SetReferenceParameter("shellFrameVisual", shellFrameVisual);
            shellFrameShadowVisual.StartAnimation("Size", shellFrameSizeAnimation);          
        }
        #endregion

        #region Getters and Setters

        /// <summary>
        ///     Used to block navigation from happening when
        ///     updating the UI for sidebar
        /// </summary>
        private bool BlockNavigation { get; set; }

        /// <summary>
        ///     Get the root frame, if no root frame exists,
        ///     we wait 150ms and call the getter again.
        /// </summary>
        public Frame RootFrame
        {
            get
            {
                if (ShellFrame != null) return ShellFrame;

                Task.Delay(TimeSpan.FromMilliseconds(150));

                return RootFrame;
            }
        }

        #endregion

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {

            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }
    }
}