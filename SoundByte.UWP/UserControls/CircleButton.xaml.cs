//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.UI.Xaml;

namespace SoundByte.UWP.UserControls
{
    public sealed partial class CircleButton
    {
        public delegate void ClickEventHandler(object sender, RoutedEventArgs e);

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(CircleButton), null);
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register("Glyph", typeof(string), typeof(CircleButton), null);

        /// <summary>
        /// The label to show on the button
        /// </summary>
        public string Label
        {
            get => GetValue(LabelProperty) as string;
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Handles the button click event
        /// </summary>
        public event ClickEventHandler Click;
      //  {
           // get => (ClickEventHandler)GetValue(ClickProperty);
          //  set => SetValue(ClickProperty, value);
      //  }

        /// <summary>
        /// The glyph to show on the button
        /// </summary>
        public string Glyph
        {
            get => this.GetValue(GlyphProperty) as string;
            set => this.SetValue(GlyphProperty, value);
        }

        public CircleButton()
        {
            this.InitializeComponent();

            MainButton.Click += (sender, args) =>
            {
                Click?.Invoke(sender, args);
            };
        }
    }
}
