#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.ContentManager.Attributes
{
    /// <summary>
    ///     Attribute for choices. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ChoicesAttribute : Attribute
    {
        /// <summary>
        ///     Gets the entries.
        /// </summary>
        /// <value>
        ///     The entries.
        /// </value>
        public object[] Entries { get; }

        /// <inheritdoc />
        public ChoicesAttribute(params object[] entries)
        {
            Entries = entries;
        }
    }
}