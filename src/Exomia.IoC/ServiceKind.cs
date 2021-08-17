#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.IoC
{
    /// <summary> Values that represent the service kind. </summary>
    public enum ServiceKind
    {
        /// <summary> An enum constant representing the transient option. </summary>
        Transient,

        /// <summary> An enum constant representing the singleton option. </summary>
        Singleton
    }
}