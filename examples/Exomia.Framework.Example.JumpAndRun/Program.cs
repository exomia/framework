#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Example.JumpAndRun
{
    class Program
    {
        private static void Main(string[] args)
        {
            using (Game.Game g = new JumpAndRunGame())
            {
                g.Run();
            }
        }
    }
}