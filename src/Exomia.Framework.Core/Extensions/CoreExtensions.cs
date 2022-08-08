﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content;
using Exomia.Framework.Core.Input;
using Exomia.Framework.Core.Scene;
using Microsoft.Extensions.DependencyInjection;

namespace Exomia.Framework.Core.Extensions;

/// <summary> The core extensions. </summary>
public static class CoreExtensions
{
    /// <summary> An <see cref="IServiceCollection" /> extension method that adds the default content management. </summary>
    /// <param name="serviceCollection"> The serviceCollection to act on. </param>
    /// <param name="configuration"> The configuration. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    public static IServiceCollection AddDefaultContentManagement(this IServiceCollection serviceCollection, ContentManager.Configuration? configuration = null)
    {
        return serviceCollection
           .AddSingleton<IContentManager>(p => new ContentManager(p, configuration ?? new ContentManager.Configuration()));
    }

    /// <summary> An <see cref="IServiceCollection" /> extension method that adds the default scene management. </summary>
    /// <param name="serviceCollection"> The serviceCollection to act on. </param>
    /// <param name="sceneBuilder"> The scene builder. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    public static IServiceCollection AddDefaultSceneManagement(this IServiceCollection serviceCollection, Func<SceneBuilder, SceneBuilder> sceneBuilder)
    {
        return serviceCollection
           .AddSingleton<ISceneManager>(
                p => new SceneManager(
                    p.GetRequiredService<IInputDevice>(),
                    sceneBuilder(new SceneBuilder(p)).BuildAndClear()));
    }
}