#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content.Resolver;

/// <summary> Used to mark a content readable class. </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ContentResolverAttribute : Attribute
{
    internal int Order { get; }

    /// <inheritdoc />
    /// <summary> Initializes a new instance of the <see cref="T:Exomia.Framework.Core.Content.Resolver.ContentResolverAttribute" /> class. </summary>
    /// <param name="order"> the order in which the resolvers should be gone through. </param>
    public ContentResolverAttribute(int order)
    {
        Order = order;
    }
}