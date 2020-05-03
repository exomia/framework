#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Game;

namespace Exomia.Framework.Components
{
    /// <summary>
    ///     A model 3 d viewer component. This class cannot be inherited.
    /// </summary>
    public sealed class Model3DViewerComponent : DrawableComponent
    {
        private readonly string _modelAsset;

        /// <inheritdoc />
        public Model3DViewerComponent(string modelAsset, string name)
            : base(name)
        {
            _modelAsset = modelAsset;
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void OnLoadContent(IServiceRegistry registry) { }
    }
}