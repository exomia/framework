#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Game;

namespace Exomia.Framework
{
    /// <summary>
    ///     A renderer.
    /// </summary>
    public abstract class Renderer : IComponent, IInitializable, IDrawable, IDisposable
    {
        /// <summary>
        ///     Occurs when the <see cref="DrawOrder" /> property changes.
        /// </summary>
        public event EventHandler? DrawOrderChanged;

        /// <summary>
        ///     Occurs when the <see cref="Visible" /> property changes.
        /// </summary>
        public event EventHandler? VisibleChanged;

        private int  _drawOrder;
        private bool _visible;

        /// <inheritdoc />
        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder != value)
                {
                    _drawOrder = value;
                    DrawOrderChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    VisibleChanged?.Invoke();
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Renderer" /> class.
        /// </summary>
        /// <param name="name"> name. </param>
        protected Renderer(string name)
        {
            Name = name;
        }

        /// <inheritdoc />
        public abstract void Dispose();

        /// <inheritdoc />
        public virtual bool BeginDraw()
        {
            return _visible;
        }

        /// <inheritdoc />
        public virtual void Draw(GameTime gameTime) { }

        /// <inheritdoc />
        public virtual void EndDraw() { }

        /// <inheritdoc />
        public virtual void Initialize(IServiceRegistry registry) { }
    }
}