#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Graphics;
using Exomia.Framework.Input;

namespace Exomia.Framework.UI.Controls
{
    /// <summary>
    ///     A button.
    /// </summary>
    public class Button : Label
    {
        /// <summary>
        ///     Occurs when Mouse Down.
        /// </summary>
        public event UiMouseEventActionHandler? MouseClick;

        private bool _isMouseDown;

        /// <inheritdoc />
        public Button(SpriteFont font, string? text)
            : base(font, text) { }

        /// <inheritdoc />
        protected override void OnMouseDown(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (!_isMouseDown)
            {
                _isMouseDown = true;
            }
            base.OnMouseDown(in e, ref eventAction);
        }

        /// <inheritdoc />
        protected override void OnMouseUp(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                MouseClick?.Invoke(this, in e, ref eventAction);
            }
            base.OnMouseUp(in e, ref eventAction);
        }

        /// <inheritdoc />
        protected override void OnMouseLeaved(in MouseEventArgs e)
        {
            _isMouseDown = false;
            base.OnMouseLeaved(in e);
        }
    }
}