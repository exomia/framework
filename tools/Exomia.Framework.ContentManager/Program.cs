#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager;

/// <summary>
///     A program.
/// </summary>
static class Program
{
    [STAThread]
    private static void Main()
    {
        Directory.CreateDirectory("temp");
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}