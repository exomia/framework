#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.BasicSetup
{
    /// <summary>
    ///     A program. This class cannot be inherited.
    /// </summary>
    sealed class Program
    {
        /// <summary>
        ///     Main entry-point for this application.
        /// </summary>
        private static void Main()
        {
            using(MyGame game = new MyGame())
			{
				game.Run();
			}
        }
    }
}