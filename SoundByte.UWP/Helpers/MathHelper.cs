//*********************************************************
// Copyright (c) Dominic Maas. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using System.Collections.Generic;
using System.Linq;

namespace SoundByte.UWP.Helpers
{
    /// <summary>
    /// General math helper functions
    /// </summary>
    public static class MathHelper
    {
        public static List<int> AverageOut(List<int> numbers)
        {
            return numbers.Select((t, i) => (t + numbers[i++] + numbers[i++] + numbers[i]) / 4).ToList();
        }
    }
}
