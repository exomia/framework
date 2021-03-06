﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.Content
{
    /// <summary>
    ///     used to mark a content readable class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ContentReadableAttribute : Attribute
    {
        internal IContentReader Reader { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentReadableAttribute" /> class.
        /// </summary>
        /// <param name="reader">
        ///     the content reader type
        ///     <see cref="T:Exomia.Framework.Content.IContentReader" />
        /// </param>
        /// <exception cref="TypeLoadException"> Thrown when a Type Load error condition occurs. </exception>
        public ContentReadableAttribute(Type reader)
        {
            Reader = System.Activator.CreateInstance(reader) as IContentReader ??
                     throw new TypeLoadException(
                         $"can not create an instance of {nameof(IContentReader)} from type: {reader.AssemblyQualifiedName}");
        }
    }
}