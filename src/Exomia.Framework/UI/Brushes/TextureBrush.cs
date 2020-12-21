#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.UI.Brushes
{
    /// <summary>
    ///     A texture brush. This class cannot be inherited.
    /// </summary>
    public sealed class TextureBrush : IBrush
    {
        private readonly Texture _texture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextureBrush" /> class.
        /// </summary>
        /// <param name="texture"> The texture. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public TextureBrush(Texture texture)
        {
            _texture = texture ?? throw new ArgumentNullException(nameof(texture));
        }

        void IBrush.Render(Canvas canvas, in RectangleF region, float opacity)
        {
            canvas.Draw(_texture, region, null, Color.White, 0, Vector2.Zero, opacity, TextureEffects.None);
        }

        void IBrush.RenderClipped(Canvas canvas, in RectangleF region, in RectangleF visibleRegion, float opacity)
        {
            canvas.Draw(
                _texture, visibleRegion,
                new Rectangle(
                    0, 0,
                    (int)((_texture.Width / region.Width) * visibleRegion.Width),
                    (int)((_texture.Height / region.Height) * visibleRegion.Height)),
                Color.White, 0, Vector2.Zero, opacity, TextureEffects.None);
        }
    }
}