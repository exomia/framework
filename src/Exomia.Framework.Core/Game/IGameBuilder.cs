#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Microsoft.Extensions.DependencyInjection;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Game;

/// <summary> Interface for game builder. </summary>
public interface IGameBuilder : IDisposable
{
    /// <summary> Configure options. </summary>
    /// <typeparam name="TConfiguration"> Type of the configuration. </typeparam>
    /// <param name="configure"> The configure delegate. </param>
    /// <returns> An <see cref="IGameBuilder" />. </returns>
    IGameBuilder Configure<TConfiguration>(Action<TConfiguration, IServiceProvider> configure) where TConfiguration : class;

    /// <summary> Configure services. </summary>
    /// <param name="configure"> The configure delegate. </param>
    /// <returns> An <see cref="IGameBuilder" />. </returns>
    IGameBuilder ConfigureServices(Action<IServiceCollection> configure);

    /// <summary> Registers the disposable described by disposable. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="disposable"> The disposable. </param>
    /// <returns> A T. </returns>
    T RegisterDisposable<T>(T disposable) where T : IDisposable;

    /// <summary> Builds the <typeparamref name="TGame" /> with its dependencies. </summary>
    /// <typeparam name="TGame"> Type of the game. </typeparam>
    /// <returns> A <typeparamref name="TGame" />. </returns>
    TGame Build<TGame>() where TGame : Game;
}