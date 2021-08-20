#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using IServiceProvider = Exomia.IoC.IServiceProvider;

namespace Exomia.Framework.Core.Scene
{
    /// <summary> A scene builder. This class cannot be inherited. </summary>
    public sealed class SceneBuilder
    {
        private readonly IServiceProvider                        _serviceProvider;
        private readonly List<(bool initialize, Type sceneType)> _sceneCollection = new List<(bool, Type)>(8);

        /// <summary> Initializes a new instance of the <see cref="SceneBuilder"/> class. </summary>
        /// <param name="serviceProvider"> The service provider. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public SceneBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary> Adds Scene to the <see cref="SceneBuilder"/>. </summary>
        /// <typeparam name="TScene"> Type of the scene. </typeparam>
        /// <param name="initialize"> (Optional) True to initialize the scene at startup. </param>
        /// <returns> A <see cref="SceneBuilder"/>. </returns>
        public SceneBuilder Add<TScene>(bool initialize = false) where TScene : SceneBase
        {
            _sceneCollection.Add((initialize, typeof(TScene)));
            return this;
        }

        internal IEnumerable<(bool, SceneBase)> BuildAndClear()
        {
            foreach ((bool initialize, Type? sceneType) in _sceneCollection)
            {
                yield return (initialize, (SceneBase)_serviceProvider.Get(sceneType));
            }

            _sceneCollection.Clear();
        }
    }
}