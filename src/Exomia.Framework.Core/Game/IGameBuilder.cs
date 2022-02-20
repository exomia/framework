#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.IoC;
using IServiceProvider = Exomia.IoC.IServiceProvider;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Game;

/// <summary> Interface for game builder. </summary>
public interface IGameBuilder : IDisposable
{
    IGameBuilder ConfigureServices(Action<IServiceCollection>                       configureDelegate);
    IGameBuilder Configure<TConfiguration>(Action<IServiceProvider, TConfiguration> configureDelegate) where TConfiguration : class;
    T            RegisterDisposable<T>(T                                            disposable) where T : IDisposable;
    TGame        Build<TGame>() where TGame : Game;
}