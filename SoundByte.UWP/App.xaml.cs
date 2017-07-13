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
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp;
using SoundByte.UWP.Dialogs;
using SoundByte.UWP.Helpers;
using SoundByte.UWP.Services;
using SoundByte.UWP.Views;
using SoundByte.UWP.Views.CoreApp;
using UICompositionAnimations.Lights;

namespace SoundByte.UWP
{
    sealed partial class App
    {
        #region App Setup

        /// <summary>
        ///     This is the main class for this app. This function is the first function
        ///     called and it setups the app analytic (If in release mode), components,
        ///     requested theme and event handlers.
        /// </summary>
        public App()
        {
            // Init XAML Resources
            InitializeComponent();

            // We want to use the controler if on xbox
            if (IsXbox)
                RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            // Check that we are not using the default theme,
            // if not change the requested theme to the users
            // picked theme.
            if (!AccentHelper.IsDefaultTheme)
                RequestedTheme = AccentHelper.ThemeType;

            // Log when the app crashes
            CoreApplication.UnhandledErrorDetected += async (sender, args) =>
            {
                try
                {
                    args.UnhandledError.Propagate();
                }
                catch (Exception e)
                {
                    // Show the exception UI
                    await HandleAppCrashAsync(e);
                }
            };

            // Log when the app crashes
            Current.UnhandledException += async (sender, args) =>
            {
                // We have handled this exception
                args.Handled = true;
                // Show the exception UI
                await HandleAppCrashAsync(args.Exception);
            };

            // Enter and Leaving background handlers
            EnteredBackground += AppEnteredBackground;
            LeavingBackground += AppLeavingBackground;

            // During the transition from foreground to background the
            // memory limit allowed for the application changes. The application
            // has a short time to respond by bringing its memory usage
            // under the new limit.
            MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;

            // After an application is backgrounded it is expected to stay
            // under a memory target to maintain priority to keep running.
            // Subscribe to the event that informs the app of this change.
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;
        }
        #endregion

        #region Key Events
        private void CoreWindowOnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.F11:
                    // Send hit
                    TelemetryService.Current.TrackEvent("Toggle FullScreen");
                    // Toggle between fullscreen or not
                    if (!IsFullScreen)
                        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                    else
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();
                    break;
                case VirtualKey.F12:
                    // Send hit
                    TelemetryService.Current.TrackEvent("Debug Action");
                    
                    // Artifically decrease memory usage, app will be dead after this
                    IsBackground = true;
                    ReduceMemoryUsage();
                    break;
                case VirtualKey.GamepadView:
                    // Send hit
                    TelemetryService.Current.TrackEvent("Xbox Playing Page");
                    // Navigate to the current playing track
                    NavigateTo(typeof(Track));
                    break;
                case VirtualKey.GamepadY:
                    // Send hit
                    TelemetryService.Current.TrackEvent("Xbox Search Page");
                    // Navigate to the search page
                    NavigateTo(typeof(Search));
                    break;
            }
        }
        #endregion

        #region Static App Helpers
        /// <summary>
        ///  Navigate to a certain page using the main shells
        ///  rootfrom navigate method
        /// </summary>
        public static void NavigateTo(Type page, object param = null) => (Window.Current.Content as MainShell)?.RootFrame.Navigate(page, param);
        
        /// <summary>
        /// Go back a page
        /// </summary>
        public static void GoBack()
        {
            var canGoBack = (Window.Current.Content as MainShell)?.RootFrame.CanGoBack;

            if (canGoBack.HasValue && canGoBack.Value)
                ((MainShell) Window.Current.Content)?.RootFrame.GoBack(); 
        }

        /// <summary>
        /// Stops the back event from being called, allowing for manual overiding
        /// </summary>
        public static bool OverrideBackEvent { get; set; }

        /// <summary>
        ///     Is the app currently in the background.
        /// </summary>
        public static bool IsBackground { get; set; }

        /// <summary>
        ///     Is the app running on xbox
        /// </summary>
        public static bool IsXbox => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox";

        /// <summary>
        ///     Is the app runnning on a phone
        /// </summary>
        public static bool IsMobile => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";

        /// <summary>
        ///     Is the app running on desktop
        /// </summary>
        public static bool IsDesktop => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop";

        /// <summary>
        ///     Is the application fullscreen.
        /// </summary>
        public static bool IsFullScreen => ApplicationView.GetForCurrentView().IsFullScreenMode;

        /// <summary>
        ///     Is anything currently loading
        /// </summary>
        public static bool IsLoading
        {
            get
            {
                var loadingRing = (Window.Current.Content as MainShell)?.FindName("LoadingRing") as ProgressRing;
                return loadingRing?.Visibility == Visibility.Visible;
            }

            set
            {
                var loadingRing = (Window.Current.Content as MainShell)?.FindName("LoadingRing") as ProgressRing;
                if (loadingRing != null)
                    loadingRing.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     Does the application currently have access to the internet.
        /// </summary>
        public static bool HasInternet
        {
            get
            {
                try
                {
                    var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                    return connectionProfile != null &&
                           connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #endregion

        #region App Crash Helpers
        private async Task HandleAppCrashAsync(Exception ex)
        {
            // Track the error
            TelemetryService.Current.TrackException(ex);

            try
            {
                if (!IsBackground)
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                    {
                        await new CrashDialog(ex).ShowAsync();
                    });
                }
            }
            catch
            {
                // Blank Catch
            }
        }
        #endregion

        #region Background Handlers

        /// <summary>
        ///     The application is leaving the background.
        /// </summary>
        private async void AppLeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            // Send hit
            TelemetryService.Current.TrackEvent("Leave Background");

            // Mark the transition out of the background state
            IsBackground = false;

            // Restore view content if it was previously unloaded
            if (Window.Current.Content == null)
            {
                // Create / Get the main shell
                var rootShell = await CreateMainShellAsync();

                // Set the root shell as the window content
                Window.Current.Content = rootShell;

                // If on xbox display the screen to the full width and height
                if (IsXbox)
                    ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

                // Activate the window
                Window.Current.Activate();
            }
        }

        /// <summary>
        ///     The application entered the background.
        /// </summary>
        private static void AppEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            // Send hit
            TelemetryService.Current.TrackEvent("Enter Background");

            // Update the variable
            IsBackground = true;
        }

        #endregion

        #region Memory Handlers

        /// <summary>
        ///     Raised when the memory limit for the app is changing, such as when the app
        ///     enters the background.
        /// </summary>
        /// <remarks>
        ///     If the app is using more than the new limit, it must reduce memory within 2 seconds
        ///     on some platforms in order to avoid being suspended or terminated.
        ///     While some platforms will allow the application
        ///     to continue running over the limit, reducing usage in the time
        ///     allotted will enable the best experience across the broadest range of devices.
        /// </remarks>
        private void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            // If app memory usage is over the limit, reduce usage within 2 seconds
            // so that the system does not suspend the app
            if (MemoryManager.AppMemoryUsage < e.NewLimit) return;

            // Send hit
            TelemetryService.Current.TrackEvent("Reducing Memory Usage", new Dictionary<string, string>
            {
                { "method", "MemoryManager_AppMemoryUsageLimitChanging" },
                { "new_limit", e.NewLimit.ToString() },
                { "old_limit", e.OldLimit.ToString() },
                { "current_usage", MemoryManager.AppMemoryUsage.ToString() },
                { "memory_usage_level", MemoryManager.AppMemoryUsageLevel.ToString() }
            });

            // Reduce the memory usage
            ReduceMemoryUsage();
        }

        /// <summary>
        ///     Handle system notifications that the app has increased its
        ///     memory usage level compared to its current target.
        /// </summary>
        /// <remarks>
        ///     The app may have increased its usage or the app may have moved
        ///     to the background and the system lowered the target for the app
        ///     In either case, if the application wants to maintain its priority
        ///     to avoid being suspended before other apps, it may need to reduce
        ///     its memory usage.
        ///     This is not a replacement for handling AppMemoryUsageLimitChanging
        ///     which is critical to ensure the app immediately gets below the new
        ///     limit. However, once the app is allowed to continue running and
        ///     policy is applied, some apps may wish to continue monitoring
        ///     usage to ensure they remain below the limit.
        /// </remarks>
        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            // Obtain the current usage level
            var level = MemoryManager.AppMemoryUsageLevel;

            // Check the usage level to determine whether reducing memory is necessary.
            // Memory usage may have been fine when initially entering the background but
            // the app may have increased its memory usage since then and will need to trim back.
            if (level != AppMemoryUsageLevel.OverLimit && level != AppMemoryUsageLevel.High) return;

            // Send hit
            TelemetryService.Current.TrackEvent("Reducing Memory Usage", new Dictionary<string, string>
            {
                { "method", "MemoryManager_AppMemoryUsageIncreased" },
                { "current_usage", MemoryManager.AppMemoryUsage.ToString() },
                { "memory_usage_level", MemoryManager.AppMemoryUsageLevel.ToString() }
            });

            // Reduce memory usage
            ReduceMemoryUsage();
        }

        /// <summary>
        ///     Reduces application memory usage.
        /// </summary>
        /// <remarks>
        ///     When the app enters the background, receives a memory limit changing
        ///     event, or receives a memory usage increased event, it can
        ///     can optionally unload cached data or even its view content in
        ///     order to reduce memory usage and the chance of being suspended.
        ///     This must be called from multiple event handlers because an application may already
        ///     be in a high memory usage state when entering the background, or it
        ///     may be in a low memory usage state with no need to unload resources yet
        ///     and only enter a higher state later.
        /// </remarks>
        private void ReduceMemoryUsage()
        {
            var memUsage = MemoryManager.AppMemoryUsage;
            var removeUi = false;

            // If the app has caches or other memory it can free, it should do so now.
            // << App can release memory here >>

            // Additionally, if the application is currently
            // in background mode and still has a view with content
            // then the view can be released to save memory and
            // can be recreated again later when leaving the background.
            if (IsBackground && Window.Current != null && Window.Current.Content != null)
            {
                VisualTreeHelper.DisconnectChildrenRecursive(Window.Current.Content);

                // Clear the view content. Note that views should rely on
                // events like Page.Unloaded to further release resources.
                // Release event handlers in views since references can
                // prevent objects from being collected.
                Window.Current.Content = null;

                removeUi = true;
            }

            var shell = Window.Current?.Content as MainShell;

            shell?.RootFrame.Navigate(typeof(BlankPage));

            // Clear the page cache
            if (shell?.RootFrame != null)
            {
                var cacheSize = shell.RootFrame.CacheSize;
                shell.RootFrame.CacheSize = 0;
                shell.RootFrame.CacheSize = cacheSize;
            }

            // Run the GC to collect released resources.
            GC.Collect();

            var diffMemUsage = (memUsage - MemoryManager.AppMemoryUsage);

            TelemetryService.Current.TrackEvent("Memory Collection Completed", new Dictionary<string, string>()
            {
                { "cleaned_memory", diffMemUsage + "kb" },
                { "is_background", IsBackground.ToString() },
                { "remove_ui", removeUi.ToString() },
            });

        }

        #endregion

        /// <summary>
        ///     Create the main app shell and load app logic
        /// </summary>
        /// <returns>The main app shell</returns>
        private async Task<MainShell> CreateMainShellAsync(string path = null)
        {
            // Get the main shell
            var shell = Window.Current.Content as MainShell;
            // If the shell is null, we need to set it up.
            if (shell != null)
            {
                if (!string.IsNullOrEmpty(path))
                    await shell.HandleProtocolAsync(path);

                return shell;
            }

            // Create the main shell
            shell = new MainShell(path);
            // Hook the key pressed event for the global app
            Window.Current.CoreWindow.KeyUp += CoreWindowOnKeyUp;
            // Return the created shell
            return shell;
        }

        #region Launch / Activate Events
        /// <summary>
        ///     Called when the app is activated.
        /// </summary>
        protected override async void OnActivated(IActivatedEventArgs e)
        {
            var path = string.Empty;

            // Handle all the activation protocols that could occure
            switch (e.Kind)
            {
                // We were launched using the protocol
                case ActivationKind.Protocol:
                    var protoArgs = e as ProtocolActivatedEventArgs;
                    if (protoArgs != null) path = protoArgs.Uri.ToString();
                    break;
                case ActivationKind.ToastNotification:
                    var toastArgs = e as IToastNotificationActivatedEventArgs;
                    if (toastArgs != null) path = toastArgs.Argument;
                    break;
            }

            // Create / Get the main shell
            var rootShell = await CreateMainShellAsync(path);

            // Set the root shell as the window content
            Window.Current.Content = rootShell;

            // If on xbox display the screen to the full width and height
            if (IsXbox)
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            // Activate the window
            Window.Current.Activate();
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var path = string.Empty;

            // Handle all the activation protocols that could occure
            if (!string.IsNullOrEmpty(e.TileId))
            {
                path = e.Arguments;
            }

            // Create / Get the main shell
            var rootShell = await CreateMainShellAsync(path);

            // If this is just a prelaunch, don't 
            // actually set the content to the frame.
            if (e.PrelaunchActivated) return;

            // Set the root shell as the window content
            Window.Current.Content = rootShell;

            Window.Current.Content.Lights.Add(new PointerPositionSpotLight
            {
               Active = true
            });

            // If on xbox display the screen to the full width and height
            if (IsXbox)
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            // Activate the window
            Window.Current.Activate();
        }

        #endregion
    }
}