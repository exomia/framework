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

namespace Exomia.Framework.UI.Controls
{
    using static UiConstants;

    /// <summary>
    ///     A label.
    /// </summary>
    public class Label : Control
    {
        private bool          _isDirty = true;
        private Vector2       _offset;
        private string        _text;
        private SpriteFont    _font;
        private TextAlignment _alignment = TextAlignment.TopLeft;

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string Text
        {
            get { return _text; }
            set
            {
                _text    = value;
                _isDirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the font.
        /// </summary>
        /// <value>
        ///     The font.
        /// </value>
        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font    = value;
                _isDirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the alignment.
        /// </summary>
        /// <value>
        ///     The alignment.
        /// </value>
        public TextAlignment TextAlignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
                _isDirty   = true;
            }
        }

        /// <summary>
        ///     Gets or sets the color of the foreground.
        /// </summary>
        /// <value>
        ///     The color of the foreground.
        /// </value>
        public Color ForegroundColor { get; set; } = Color.Black;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        /// <param name="font"> The font. </param>
        /// <param name="text"> The text. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public Label(SpriteFont font, string? text)
        {
            _font = font ?? throw new ArgumentNullException(nameof(font));
            _text = text ?? string.Empty;
        }

        /// <inheritdoc />
        public override void Draw(float elapsedSeconds, Canvas canvas)
        {
            base.Draw(elapsedSeconds, canvas);

            if (_isDirty)
            {
                CalculateTextOffset();
                _isDirty = false;
            }

            canvas.DrawText(Font, Text, _drawRectangle.TopLeft + _offset, ForegroundColor);
        }

        private void CalculateTextOffset()
        {
            Vector2 size = Font.MeasureText(Text);

            int flags = (int)TextAlignment;
            if ((flags & TEXT_ALIGN_TOP) == TEXT_ALIGN_TOP)
            {
                _offset.Y = 0;
            }
            else if ((flags & TEXT_ALIGN_MIDDLE) == TEXT_ALIGN_MIDDLE)
            {
                _offset.Y = (_drawRectangle.Height * 0.5f) - (size.Y * 0.5f);
            }
            else if ((flags & TEXT_ALIGN_BOTTOM) == TEXT_ALIGN_BOTTOM)
            {
                _offset.Y = _drawRectangle.Height - size.Y;
            }

            if ((flags & TEXT_ALIGN_LEFT) == TEXT_ALIGN_LEFT)
            {
                _offset.X = 0;
            }
            else if ((flags & TEXT_ALIGN_CENTER) == TEXT_ALIGN_CENTER)
            {
                _offset.X = (_drawRectangle.Width * 0.5f) - (size.X * 0.5f);
            }
            else if ((flags & TEXT_ALIGN_RIGHT) == TEXT_ALIGN_RIGHT)
            {
                _offset.X = _drawRectangle.Width - size.X;
            }
        }
    }
}