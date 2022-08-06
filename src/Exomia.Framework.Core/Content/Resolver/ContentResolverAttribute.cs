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
    /// <summary> The lower the number, the earlier the resolver will get called. </summary>
    public int Order { get; init; } = 1;
}