#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Game;

namespace Exomia.Framework
{
    /// <summary>
    ///     A drawable game component.
    /// </summary>
    public abstract class DrawableComponent : Component, IDrawable
    {
        /// <summary>
        ///     Occurs when the <see cref="DrawOrder" /> property changes.
        /// </summary>
        public event EventHandler? DrawOrderChanged;

        /// <summary>
        ///     Occurs when the <see cref="Visible" /> property changes.
        /// </summary>
        public event EventHandler? VisibleChanged;

        /// <summary>
        ///     The draw order.
        /// </summary>
        private int _drawOrder;

        /// <summary>
        ///     True to show, false to hide.
        /// </summary>
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

        /// <inheritdoc />
        protected DrawableComponent(string name)
            : base(name) { }

        /// <inheritdoc />
        public virtual bool BeginDraw()
        {
            return _visible;
        }

        /// <inheritdoc />
        public abstract void Draw(GameTime gameTime);

        /// <inheritdoc />
        public virtual void EndDraw() { }
    }
}