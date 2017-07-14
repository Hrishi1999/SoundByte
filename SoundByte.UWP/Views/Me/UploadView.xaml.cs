//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using SoundByte.UWP.Services;
using SoundByte.UWP.ViewModels;

namespace SoundByte.UWP.Views.Me
{
    /// <summary>
    /// Page used by user to upload tracks
    /// </summary>
    public sealed partial class UploadView
    {
        // The main view model
        public BaseViewModel ViewModel { get; } = new BaseViewModel();

        private StorageFile _imageFile;
        private StorageFile _musicFile;
        InMemoryRandomAccessStream _audioStream = null;
        CancellationTokenSource _cancelToken;

        public UploadView()
        {
            // Initialize XAML Components
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            // Set the data context
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsService.Current.LastFrame = typeof(UploadView).FullName;

            // Track Event
            TelemetryService.Current.TrackPage("Upload Page");
        }

        private async void UploadButton_Tapped(object sender, RoutedEventArgs e)
        {
            UploadButton.IsEnabled = false;
            CancelButton.IsEnabled = false;

            if (_musicFile == null && _audioStream == null || string.IsNullOrEmpty(Trackname.Text) || string.IsNullOrEmpty(TrackTags.Text))
            {
                UploadButton.IsEnabled = true;
                CancelButton.IsEnabled = true;

                await new MessageDialog("Make sure that you have picked a file and filled in the required information.", "Upload Error").ShowAsync();
                return;
            }

            try
            {
                // Create a http client
                using (var httpClient = new HttpClient(new HttpBaseProtocolFilter { AutomaticDecompression = true }))
                {
                    // Close the connection automaticly
                    // httpClient.DefaultRequestHeaders.ConnectionClose = true;
                    // Add the user agent
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new Windows.Web.Http.Headers.HttpProductInfoHeaderValue("SoundByte", Windows.ApplicationModel.Package.Current.Id.Version.Major + "." + Windows.ApplicationModel.Package.Current.Id.Version.Minor + "." + Windows.ApplicationModel.Package.Current.Id.Version.Build));
                    // Set the media that we want to recieve
                    httpClient.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
                    // Add oauth token
                    httpClient.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("OAuth", SoundByteService.Current.SoundCloudToken.AccessToken);
                    // Set the muiltipart form
                    var formContent = new HttpMultipartFormDataContent();

                    if (_musicFile != null)
                    {
                        // Get the track stream
                        var trackStream = await _musicFile.OpenReadAsync();
                        // Create a ByteArrayContent to store the array in a byte array
                        var trackContent = new HttpStreamContent(trackStream);
                        // Set the content type
                        trackContent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("application/octet-stream");
                        // Add the image to the form
                        formContent.Add(trackContent, "track[asset_data]", _musicFile.Name);
                    }
                    else if (_audioStream != null)
                    {
                        // Create a ByteArrayContent to store the array in a byte array
                        var trackContent = new HttpStreamContent(_audioStream);
                        // Set the content type
                        trackContent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("application/octet-stream");
                        // Add the image to the form
                        formContent.Add(trackContent, "track[asset_data]", "stream.mp3");
                    }
                    else
                    {
                        await new MessageDialog("No Track File Specified.", "Upload Error").ShowAsync();
                        return;
                    }

                    // Add the title (in a UTF8 byte array)
                    formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(Trackname.Text).AsBuffer()), "track[title]");

                    // Add the tags (only if the tags exist in a comma seperated array)
                    if (!string.IsNullOrEmpty(TrackTags.Text))
                    {
                        // Get all the tags
                        var tagArray = TrackTags.Text.Split(',');
                        // Check that the array is greater than 0
                        if (tagArray.Length > 0)
                        {
                            formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(string.Join(",", tagArray)).AsBuffer()), "track[tag_list]");
                        }
                    }

                    // Add the description (only if the description exists)
                    string trackDeskText;
                    TrackDescription.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out trackDeskText);

                    if (!string.IsNullOrEmpty(trackDeskText))
                        formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(trackDeskText).AsBuffer()), "track[description]");

                    // Add the Track Artwork (if exists)
                    if (_imageFile != null)
                    {
                        // Get the track art stream
                        var trackArtStream = await _imageFile.OpenReadAsync();

                        // Create a ByteArrayContent to store the array in a byte array
                        var trackArtContent = new HttpStreamContent(trackArtStream);
                        // Set the content type
                        trackArtContent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue(_imageFile.ContentType);
                        // Add the image to the form
                        formContent.Add(trackArtContent, "track[artwork_data]", _imageFile.Name);
                    }

                    // Add the track sharing type
                    formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes((PrivacyBox.SelectedItem as ComboBoxItem)?.Content?.ToString().ToLower()).AsBuffer()), "track[sharing]");


                    // Add buy link
                    if (!string.IsNullOrEmpty(PurchaseBox.Text))
                        formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(PurchaseBox.Text).AsBuffer()), "track[purchase_url]");

                    // Add video link
                    if (!string.IsNullOrEmpty(VideoBox.Text))
                        formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(VideoBox.Text).AsBuffer()), "track[video_url]");

                    // Add Release Date
                    formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(ReleaseDateBox.Date.Year.ToString()).AsBuffer()), "track[release_year]");
                    formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(ReleaseDateBox.Date.Month.ToString()).AsBuffer()), "track[release_month]");
                    formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(ReleaseDateBox.Date.Day.ToString()).AsBuffer()), "track[release_day]");

                    // Add release number
                    if (!string.IsNullOrEmpty(RecordNumBox.Text))
                        formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(RecordNumBox.Text).AsBuffer()), "track[release]");

                    // Add ISRC number
                    if (!string.IsNullOrEmpty(ISRCBox.Text))
                        formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(ISRCBox.Text).AsBuffer()), "track[isrc]");

                    // Add BPM number
                    if (!string.IsNullOrEmpty(BPMBox.Text))
                        formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(BPMBox.Text).AsBuffer()), "track[bpm]");

                    // Add licence
                    formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes((LicenseBox.SelectedItem as ComboBoxItem)?.Content?.ToString().ToLower().Replace(' ', '-')).AsBuffer()), "track[license]");

                    // Add type
                    formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes((TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString().ToLower().Replace(' ', '-')).AsBuffer()), "track[type]");

                    // Add genre number
                    if (!string.IsNullOrEmpty(GenreBox.Text))
                        formContent.Add(new HttpBufferContent(Encoding.UTF8.GetBytes(GenreBox.Text).AsBuffer()), "track[genre]");

                    // Add Downloadble
                    formContent.Add(
                        downloadSwitch.IsOn
                            ? new HttpBufferContent(Encoding.UTF8.GetBytes("true").AsBuffer())
                            : new HttpBufferContent(Encoding.UTF8.GetBytes("false").AsBuffer()), "track[downloadable]");

                    var progressCallback = new Progress<HttpProgress>(p =>
                    {
                        if (!p.TotalBytesToSend.HasValue) return;

                        Progressbar.Maximum = p.TotalBytesToSend.Value;
                        Progressbar.Value = p.BytesSent;
                    });

                    _cancelToken = new CancellationTokenSource();

                    // Post the data
                    var uploadResponse = await httpClient.PostAsync(new Uri("https://api.soundcloud.com/tracks?oauth_token=" + SoundByteService.Current.SoundCloudToken.AccessToken + "&client_id=" + Common.ServiceKeys.SoundCloudClientId + "&client_secret=" + Common.ServiceKeys.SoundCloudClientSecret), formContent).AsTask(_cancelToken.Token, progressCallback);

                    if (uploadResponse.IsSuccessStatusCode)
                    {
                        await new MessageDialog("Your track has been uploaded and SoundCloud is currently processing it.", "Upload Complete").ShowAsync();
                    }
                    else
                    {
                        UploadButton.IsEnabled = true; CancelButton.IsEnabled = true;

                        await new MessageDialog("An Error Occured while trying to upload your track. Please try again later.", "Upload Error").ShowAsync();

                        return;
                    }
                }

                UploadButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
            }
            catch
            {
                UploadButton.IsEnabled = true; CancelButton.IsEnabled = true;
                _imageFile = null;
                _imageFile = null;
                Trackname.Text = string.Empty;
                TrackDescription.Document.SetText(Windows.UI.Text.TextSetOptions.None, string.Empty);
                TrackTags.Text = string.Empty;

                await new MessageDialog("An Error Occured while trying to upload your track. Please try again later.", "Upload Error").ShowAsync();
            }
        }

        private void CancelButton_Tapped(object sender, RoutedEventArgs e)
        {
            if (_cancelToken != null)
            {
                _cancelToken.Cancel();
                UploadButton.IsEnabled = true; CancelButton.IsEnabled = true;
                _imageFile = null;
                _imageFile = null;
                Trackname.Text = string.Empty;
                TrackDescription.Document.SetText(Windows.UI.Text.TextSetOptions.None, string.Empty);
                TrackTags.Text = string.Empty;
                return;
            }

            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                App.NavigateTo(typeof(HomeView));
            }
        }

        private async void TrackImageBrowser_Tapped(object sender, RoutedEventArgs e)
        {
            TrackImageBrowser.IsEnabled = false;

            var imagePicker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };

            imagePicker.FileTypeFilter.Add(".png");
            imagePicker.FileTypeFilter.Add(".jpg");
            imagePicker.FileTypeFilter.Add(".jpeg");

            var imageFile = await imagePicker.PickSingleFileAsync();

            if (imageFile != null)
            {
                var bitmapImage = new BitmapImage();


                _imageFile = imageFile;
                await bitmapImage.SetSourceAsync(await _imageFile.OpenReadAsync());
                ArtworkPreviewImage.Source = bitmapImage;
                ArtworkPreviewImage.Visibility = Visibility.Visible;
                TrackImageBrowser.IsEnabled = true;
            }
            else
            {
                ArtworkPreviewImage.Visibility = Visibility.Collapsed;
                ArtworkPreviewImage.Source = null;
                TrackImageBrowser.IsEnabled = true;
            }
        }

        private async void UploadRecording_Tapped(object sender, RoutedEventArgs e)
        {
            UploadRecording.IsEnabled = false;

            var musicPicker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary
            };

            musicPicker.FileTypeFilter.Add(".aiff");
            musicPicker.FileTypeFilter.Add(".wav");
            musicPicker.FileTypeFilter.Add(".flac");
            musicPicker.FileTypeFilter.Add(".alac");
            musicPicker.FileTypeFilter.Add(".ogg");
            musicPicker.FileTypeFilter.Add(".mp2");
            musicPicker.FileTypeFilter.Add(".mp3");
            musicPicker.FileTypeFilter.Add(".acc");
            musicPicker.FileTypeFilter.Add(".amr");
            musicPicker.FileTypeFilter.Add(".wma");

            var musicFile = await musicPicker.PickSingleFileAsync();

            if (musicFile != null)
            {
                var trackProps = await musicFile.GetBasicPropertiesAsync();
                var trackPropsMusic = await musicFile.Properties.GetMusicPropertiesAsync();

                // Track size test
                if (trackProps.Size >= 5368709120 || trackPropsMusic.Duration >= TimeSpan.FromMinutes(405))
                {
                    await new MessageDialog("Pick a file that is under 5gbs or is less than 6 hours and 45 minutes long", "Upload Error").ShowAsync();
                    return;
                }

                _musicFile = musicFile;

                Trackname.Text = musicFile.DisplayName;
                UploadRecording.IsEnabled = true;
            }
            else
            {
                UploadRecording.IsEnabled = true;
            }
        }
    }
}
