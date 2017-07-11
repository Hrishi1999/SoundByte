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
using GoogleAnalytics;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Push;
using Microsoft.HockeyApp;

namespace SoundByte.UWP.Services
{
    /// <summary>
    /// This class handles global app telemetry to all telemetry services
    /// connected to this app. (Application Insights, HockeyApp, Google Analytics).
    /// </summary>
    public class TelemetryService
    {
        private Tracker GoogleAnalyticsClient { get; }

        #region Service Setup
        private static TelemetryService _instance;
        public static TelemetryService Current => _instance ?? (_instance = new TelemetryService());
        #endregion

        /// <summary>
        /// Setup the telemetry providers
        /// </summary>
        private TelemetryService()
        {
            try
            {
                // Setup Google Analytics
                AnalyticsManager.Current.DispatchPeriod = TimeSpan.Zero; // Immediate mode, sends hits immediately
                AnalyticsManager.Current.AutoAppLifetimeMonitoring = true; // Handle suspend/resume and empty hit batched hits on suspend
                AnalyticsManager.Current.AppOptOut = false;
                AnalyticsManager.Current.IsEnabled = true;
                AnalyticsManager.Current.IsDebug = false;
                GoogleAnalyticsClient = AnalyticsManager.Current.CreateTracker(Common.ServiceKeys.GoogleAnalyticsTrackerId);

                // Used for crash reporting
                HockeyClient.Current.Configure(Common.ServiceKeys.HockeyAppClientId);

                // Azure Mobile Aalytics and push support
                MobileCenter.Start(Common.ServiceKeys.AzureMobileCenterClientId, typeof(Analytics), typeof(Push));

#if DEBUG
                // Disable this on debug
                AnalyticsManager.Current.AppOptOut = true;
                MobileCenter.Enabled = false;
#endif
            }
            catch
            {
                // ignored
            }  
        }
    
        public void TrackPage(string pageName)
        {
            try
            {
                GoogleAnalyticsClient.ScreenName = pageName;
                GoogleAnalyticsClient.Send(HitBuilder.CreateScreenView().Build());
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="properties"></param>
        public void TrackEvent(string eventName, Dictionary<string, string> properties = null)
        {
            try
            {
                // Send a hit to Google Analytics
                GoogleAnalyticsClient.Send(HitBuilder.CreateCustomEvent("App", "Action", eventName).Build());

                // Send a hit to azure
                Analytics.TrackEvent(eventName, properties);
            }
            catch
            {
                // ignored
            }

            System.Diagnostics.Debug.WriteLine(properties != null
                ? $"[{eventName}]:\n{string.Join(Environment.NewLine, properties.Select(kvp => kvp.Key + ": " + kvp.Value.ToString()))}\n"
                : $"[{eventName}]\n");
        }

        public void TrackException(Exception exception)
        {
            try
            {
                HockeyClient.Current.TrackException(exception);
            }
            catch
            {
                // ignored
            }
        }
    }
}
