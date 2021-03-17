#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Core
{
    /// <summary>
    ///     An interface to run an object.
    /// </summary>
    public interface IRunnable : IDisposable
    {
        /// <summary>
        ///     return true if the object is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     starts the run
        /// </summary>
        void Run();

        /// <summary>
        ///     shutdown the run
        /// </summary>
        void Shutdown();
    }
}