
<h1 align="center">
SoundByte
</h1>

<h4 align="center">A <a href="https://soundcloud.com/" target="_blank">SoundCloud</a> &amp; Experimental <a href="https://fanburst.com/" target="_blank">Fanburst</a> Client for Windows 10 &amp; Xbox One.</h4>

<p align="center">
    <a href="https://ci.appveyor.com/project/dominic-maas/soundbyte">
        <img src="https://ci.appveyor.com/api/projects/status/fxf50mr4qamkhybo?svg=true" alt="Build Status">
    </a>
    <a href="https://github.com/DominicMaas/SoundByte/issues">
        <img src="https://img.shields.io/github/issues/dominicmaas/soundbyte.svg" alt="SoundByte Issues">
    </a>
    <a href="https://github.com/DominicMaas/SoundByte/blob/master/LICENSE">
        <img src="https://img.shields.io/github/license/dominicmaas/soundbyte.svg" alt="SoundByte License">
    </a>
    <a href="https://discord.gg/tftSadE">
        <img src="https://img.shields.io/discord/333524708463214594.svg" alt="Chat on Discord">
    </a>
    
</p>


## Introduction
SoundByte is a Universal Windows Platform (UWP) App that connects with the SoundCloud (and Fanburst) API allowing for a user to listen to music from SoundCloud natively. SoundByte is published through the Windows Store for free.

SoundByte has recently been open sourced due to less free time on my end, hopefully by open sourcing SoundByte, the app will continue to be improved and updated.

SoundByte is split into two main projects:
- **`SoundByte.Core`:** This project contains the core API code for the SoundCloud and Fanburst API. Mainily consisits of helper classes when deserializing JSON. Currently this project targets the UWP framework, but in the future it's planned to target a framework that works with both UWP and Xamarin, while also intergrating more of the networking code.

- **`SoundByte.UWP`:** This project contains the main code for SoundByte on Windows 10 / Xbox One. Items such as brushes, converters, view models, models, views, services etc. are all stored here.

SoundByte logic is based around a central XAML/C# file called `MainShell.xaml`/`MainShell.xaml.cs`. This file displays key app elements such as the left hand navigation pane, and mobile navigation bar. It also supports app navigation, and is used to load key app resources at load time.

The `Views` folder contains XAML pages used within the app. Generally there is one xaml page per app page (using visual state triggers to change certain UI elements depending on the platform). The code behind these pages is usually simple, only containing the view model logic and telemetry logic.

The `ViewModels` folder contains all the view models for the app. A view model class will typically extend `INotifyPropertyChanged` and `IDisposable` (although IDisposable is not currently used). These classes usually are linked with a view and contain logic for said view.

The `UserControls` folder contains the XAML and behind code for common user controls within the app. For example the stream item, and notification item.

The `Services` folder contains static services used around the app. Noticable examples are the Playback Service (handles starting songs and playing / pausing songs) and the SoundByte service (used by the app to login / logout, access api resources etc.)

The `Models` folder contains models for the app. The name models may sound a little confusing at first (as these are not empty classes for JSON deserialization - these classes are located in the `SoundByte.Core` project). These classes typically extend `ObservableCollection<item>` and `ISupportIncrementalLoading` and are used for automatic loading of content within list views and grid views.


## Features
- **SoundCloud API:** SoundByte is able to access the SoundCloud API either logged in or logged out. When logged out a user can serarch for music and then play the music. When logged in, a user can like / repost items, add items to their playlist, view their history, likes, stream, created/liked playlists and notifications. When also logged in the user can upload their own music to the SoundCloud API.

- **Fanburst API:** Support for Fanburst is still in early stages, currently a user can search for Fanburst songs and play them. Basic login is also supported, but not currently used. In the future, we plan on extending this API into the rest of the app (such as likes, history etc.) and at the same time, hopefully make the app more modular, allowing for more service intergration in the future.

- **Background Audio:** SoundByte supports playback of audio in the background using the Single Process Background Audio API. This allows the ability to play music when the screen is turned off (phone), when the app is minimised (desktop) or while playing a game (Xbox).

- **Background Notifications:** Initial versions of SoundByte supported background notifications that were provided by a background timer service that ran every 15 minutes. This service would update a temporary list with all new items in the users stream since the last check, and display notifications for these items. This newly open-sourced version of SoundByte no longer supports this notification system due to instability issues. 

## Goals for version 2.1.x
There are a few main goals that I am aiming for the v2.1.x release. These are listed below:
- Improved Mobile Support.
- App Stability Improvements, better code.
- Bring back notification support.
- New logo to align with Windows Store app guidelines.
- Better error messages.
- Fanburst Intergration / start work on an extendable code base - allowing for other services to be added.

## Download
SoundByte can be either downloaded from the Windows Store [here](https://www.microsoft.com/store/apps/9nblggh4xbjg) or downloaded from the build server. Windows 10 Creators Update or newer is required to run SoundByte.

## Development
...

## Credits

- **[Dominic Maas](https://twitter.com/dominicjmaas)**  - *Initial App Development*
- **[Dennis Bednarz](https://twitter.com/DennisBednarz)**  - *Initial App UI/UX Design*

See also the list of [contributors](https://github.com/DominicMaas/SoundByte/contributors) who participated in this project.

## License
MIT License

Copyright (c) 2017 Dominic Maas

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
