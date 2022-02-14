#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime;

namespace Exomia.Framework.Core.Game
{
    /// <summary> A game builder extensions. </summary>
    public static class GameBuilderExtensions
    {
        /// <summary> An <see cref="IGameBuilder" /> extension method that use latency mode. </summary>
        /// <param name="builder">       The builder to act on. </param>
        /// <param name="gcLatencyMode"> (Optional) The GC latency mode. </param>
        /// <returns> An <see cref="IGameBuilder" />. </returns>
        public static IGameBuilder UseLatencyMode(this IGameBuilder builder, GCLatencyMode gcLatencyMode = GCLatencyMode.LowLatency)
        {
            GCSettings.LatencyMode = gcLatencyMode;
            return builder;
        }
    }
}