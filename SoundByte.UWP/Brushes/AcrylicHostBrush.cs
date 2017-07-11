//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using SoundByte.UWP.Common;

namespace SoundByte.UWP.Brushes
{
    public class AcrylicHostBrush : AcrylicBrushBase
    {
        public AcrylicHostBrush()
        {
        }

        protected override BackdropBrushType GetBrushType()
        {
            return BackdropBrushType.HostBackdrop;
        }
    }
}
