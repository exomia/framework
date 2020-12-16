#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.UI
{
    /// <summary>
    ///     A margin.
    /// </summary>
    public readonly struct Margin
    {
        /// <summary>
        ///     The default margin equals all sides set to zero.
        /// </summary>
        public static Margin Default = new Margin(0, 0, 0, 0);

        /// <summary>
        ///     The north margin amount.
        /// </summary>
        public readonly int N;

        /// <summary>
        ///     The east margin amount.
        /// </summary>
        public readonly int E;

        /// <summary>
        ///     The south margin amount.
        /// </summary>
        public readonly int S;

        /// <summary>
        ///     The west margin amount.
        /// </summary>
        public readonly int W;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Margin" /> struct.
        /// </summary>
        /// <param name="n"> The north margin amount. </param>
        /// <param name="e"> The east margin amount. </param>
        /// <param name="s"> The south margin amount. </param>
        /// <param name="w"> The west margin amount. </param>
        public Margin(int n, int e, int s, int w)
        {
            N = n < 0 ? 0 : n;
            E = e < 0 ? 0 : e;
            S = s < 0 ? 0 : s;
            W = w < 0 ? 0 : w;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Margin" /> struct.
        /// </summary>
        /// <param name="ns"> The north and south margin amount. </param>
        /// <param name="ew"> The east and west margin amount. </param>
        public Margin(int ns, int ew)
        {
            N = S = ns < 0 ? 0 : ns;
            E = W = ew < 0 ? 0 : ew;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Margin" /> struct.
        /// </summary>
        /// <param name="nesw"> The north, east, south and west margin amount. </param>
        public Margin(int nesw)
        {
            N = E = S = W = nesw < 0 ? 0 : nesw;
        }
    }
}