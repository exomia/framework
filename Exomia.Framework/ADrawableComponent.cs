#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using Exomia.Framework.Game;

namespace Exomia.Framework
{
    /// <summary>
    ///     A drawable game component.
    /// </summary>
    public abstract class ADrawableComponent : AComponent, IDrawable
    {
        /// <summary>
        ///     Occurs when the <see cref="DrawOrder" /> property changes.
        /// </summary>
        public event EventHandler DrawOrderChanged;

        /// <summary>
        ///     Occurs when the <see cref="Visible" /> property changes.
        /// </summary>
        public event EventHandler VisibleChanged;

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
        protected ADrawableComponent(string name)
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