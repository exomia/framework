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
    ///     A padding.
    /// </summary>
    public readonly struct Padding
    {
        /// <summary>
        ///     The default padding equals all sides set to zero.
        /// </summary>
        public static Padding Default = new Padding(0, 0, 0, 0);

        /// <summary>
        ///     The north padding amount.
        /// </summary>
        public readonly int N;

        /// <summary>
        ///     The east padding amount.
        /// </summary>
        public readonly int E;

        /// <summary>
        ///     The south padding amount.
        /// </summary>
        public readonly int S;

        /// <summary>
        ///     The west padding amount.
        /// </summary>
        public readonly int W;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Padding" /> struct.
        /// </summary>
        /// <param name="n"> The north padding amount. </param>
        /// <param name="e"> The east padding amount. </param>
        /// <param name="s"> The south padding amount. </param>
        /// <param name="w"> The west padding amount. </param>
        public Padding(int n, int e, int s, int w)
        {
            N = n < 0 ? 0 : n;
            E = e < 0 ? 0 : e;
            S = s < 0 ? 0 : s;
            W = w < 0 ? 0 : w;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Padding" /> struct.
        /// </summary>
        /// <param name="ns"> The north and south padding amount. </param>
        /// <param name="ew"> The east and west padding amount. </param>
        public Padding(int ns, int ew)
        {
            N = S = ns < 0 ? 0 : ns;
            E = W = ew < 0 ? 0 : ew;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Padding" /> struct.
        /// </summary>
        /// <param name="nesw"> The north, east, south and west padding amount. </param>
        public Padding(int nesw)
        {
            N = E = S = W = nesw < 0 ? 0 : nesw;
        }
    }
}