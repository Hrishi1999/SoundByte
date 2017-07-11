//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Windows.UI;
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;

namespace SoundByte.UWP.Common
{
    /// <summary>
    /// Code Sourced from: http://juniperphoton.net/2017/05/17/encapsulate-a-acrylicbrush-for-uwp-in-creators-update/
    /// </summary>
    public enum BackdropBrushType
    {
        Backdrop,
        HostBackdrop
    }

    /// <summary>
    /// Code Sourced from: http://juniperphoton.net/2017/05/17/encapsulate-a-acrylicbrush-for-uwp-in-creators-update/
    /// </summary>
    public class CompositionBrushBuilder
    {
        private const string SourceKey = "Source";

        private float _backdropFactor = 0.5f;
        private float _tintColorFactor = 0.5f;
        private float _blurAmount = 2f;
        private Color _tintColor = Colors.Black;
        private BackdropBrushType _brushType = BackdropBrushType.Backdrop;

        public CompositionBrushBuilder(BackdropBrushType type)
        {
            _brushType = type;
        }

        public CompositionBrushBuilder SetBackdropFactor(float factor)
        {
            _backdropFactor = factor;
            return this;
        }

        public CompositionBrushBuilder SetTintColorFactor(float factor)
        {
            _tintColorFactor = factor;
            return this;
        }

        public CompositionBrushBuilder SetTintColor(Color color)
        {
            _tintColor = color;
            return this;
        }

        public CompositionBrushBuilder SetBlurAmount(float blur)
        {
            _blurAmount = blur;
            return this;
        }

        private CompositionEffectBrush CreateBlurEffect(Compositor compositor)
        {
            var effect = new GaussianBlurEffect()
            {
                BlurAmount = _blurAmount,
                BorderMode = EffectBorderMode.Soft,
                Optimization = EffectOptimization.Quality,       
                Source = new ArithmeticCompositeEffect
                {
                    MultiplyAmount = 0,
                    Source1Amount = _backdropFactor,
                    Source2Amount = _tintColorFactor,
                    Source1 = new CompositionEffectSourceParameter(SourceKey),
                    Source2 = new ColorSourceEffect()
                    {
                        Color = _tintColor
                    }
                }
            };

            var effectFactory = compositor.CreateEffectFactory(effect);
            var effectBrush = effectFactory.CreateBrush();
            return effectBrush;
        }

        public CompositionEffectBrush Build(Compositor compositor)
        {
            var effectBrush = CreateBlurEffect(compositor);
            CompositionBackdropBrush backdropBrush;
            switch (_brushType)
            {
                case BackdropBrushType.Backdrop:
                    backdropBrush = compositor.CreateBackdropBrush();
                    break;
                case BackdropBrushType.HostBackdrop:
                default:
                    backdropBrush = compositor.CreateHostBackdropBrush();
                    break;
            }
            effectBrush.SetSourceParameter(SourceKey, backdropBrush);
            return effectBrush;
        }
    }
}
