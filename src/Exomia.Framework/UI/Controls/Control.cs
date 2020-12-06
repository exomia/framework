#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using Exomia.Framework.Graphics;
using Exomia.Framework.Input;
using Exomia.Framework.UI.Brushes;
using SharpDX;

namespace Exomia.Framework.UI.Controls
{
    /// <summary>
    ///     A control base class.
    /// </summary>
    public abstract class Control : IDisposable
    {
        /// <summary>
        ///     Occurs when Visible Changed.
        /// </summary>
        public event EventHandler<Control>? VisibleChanged;

        /// <summary>
        ///     Occurs when Enabled Changed.
        /// </summary>
        public event EventHandler<Control>? EnabledChanged;

        /// <summary>
        ///     Occurs when Size Changed.
        /// </summary>
        public event EventHandler<Control>? SizeChanged;

        /// <summary>
        ///     Occurs when Location Changed.
        /// </summary>
        public event EventHandler<Control>? LocationChanged;

        /// <summary>
        ///     Occurs when Got Focus.
        /// </summary>
        public event EventHandler<Control>? GotFocus;

        /// <summary>
        ///     Occurs when Lost Focus.
        /// </summary>
        public event EventHandler<Control>? LostFocus;

        /// <summary>
        ///     Occurs when Mouse Entered.
        /// </summary>
        public event UiMouseEventHandler? MouseEntered;

        /// <summary>
        ///     Occurs when Mouse Leave.
        /// </summary>
        public event UiMouseEventHandler? MouseLeaved;

        /// <summary>
        ///     Occurs when Mouse Move.
        /// </summary>
        public event UiMouseEventActionHandler? MouseMove;

        /// <summary>
        ///     Occurs when Mouse Down.
        /// </summary>
        public event UiMouseEventActionHandler? MouseDown;

        /// <summary>
        ///     Occurs when Mouse Up.
        /// </summary>
        public event UiMouseEventActionHandler? MouseUp;

        internal int      _uiListIndex;
        internal Control? _parent = null;

        private readonly DisposeCollector _collector;

        private bool       _isDirty = true;
        private bool       _enabled;
        private bool       _visible;
        private bool       _hasFocus;
        private object?    _tag; 
        private IBrush?    _backgroundBrush;
        private RectangleF _clientRectangle = RectangleF.Empty;

        private protected UiManager? _uiManager;
        
        private protected Margin     _margin        = Margin.Default;
        private protected Padding    _padding       = Padding.Default;

        private protected RectangleF _drawRectangle = RectangleF.Empty;

        private protected float _opacity = 1.0f;
        private protected bool  _isMouseEntered;

        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>
        ///     The tag.
        /// </value>
        public object? Tag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _tag; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _tag = value; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this object is enabled.
        /// </summary>
        /// <value>
        ///     True if enabled, false if not.
        /// </value>
        public bool Enabled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _enabled; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this object is visible.
        /// </summary>
        /// <value>
        ///     True if visible, false if not.
        /// </value>
        public bool Visible
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _visible; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    VisibleChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the opacity.
        /// </summary>
        /// <value>
        ///     The opacity.
        /// </value>
        public float Opacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _opacity; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_opacity != value && value <= 1.0f && value >= 0.0f)
                {
                    _opacity = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the client rectangle.
        /// </summary>
        /// <value>
        ///     The client rectangle.
        /// </value>
        public RectangleF ClientRectangle
        {
            get { return _clientRectangle; }
            set
            {
                if (_clientRectangle != value)
                {
                    // ReSharper disable CompareOfFloatsByEqualityOperator
                    if (value.X != _clientRectangle.X || value.Y != _clientRectangle.Y)
                    {
                        OnLocationChanged();
                    }

                    if (value.Width != _clientRectangle.Width || value.Height != _clientRectangle.Height)
                    {
                        OnSizeChanged();
                    }

                    // ReSharper enable CompareOfFloatsByEqualityOperator
                    _clientRectangle = value;
                    _isDirty         = true;
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this control is dirty.
        /// </summary>
        /// <value>
        ///     True if this control is dirty, false if not.
        /// </value>
        private protected bool IsDirty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _isDirty; }
        }

        /// <summary>
        ///     Gets or sets the background brush.
        /// </summary>
        /// <value>
        ///     The background brush.
        /// </value>
        public IBrush? BackgroundBrush
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _backgroundBrush; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _backgroundBrush = value; }
        }

        /// <summary>
        ///     Gets or sets the margin.
        /// </summary>
        /// <value>
        ///     The margin.
        /// </value>
        public Margin Margin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _margin; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _margin = value; }
        }

        /// <summary>
        ///     Gets or sets the padding.
        /// </summary>
        /// <value>
        ///     The padding.
        /// </value>
        public Padding Padding
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _padding; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _padding = value; }
        }

        /// <summary>
        ///     Gets a value indicating whether this object has focus.
        /// </summary>
        /// <value>
        ///     True if this object has focus, false if not.
        /// </value>
        public bool HasFocus
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _hasFocus; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Control" /> class.
        /// </summary>
        protected Control()
        {
            _collector = new DisposeCollector();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual void SetUiManager(UiManager? manager)
        {
            _uiManager = manager;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal UiManager? GetUiManager()
        {
            return _uiManager;
        }

        /// <summary>
        ///     Sets the focus of this control.
        /// </summary>
        /// <param name="focused"> True to focus this control; false to un focus it. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFocus(bool focused)
        {
            _uiManager!.SetFocusedControl(this, focused);
        }

        /// <summary>
        ///     Adds an <see cref="IDisposable" /> object to the dispose collector.
        /// </summary>
        /// <typeparam name="T"> The <see cref="IDisposable" /> object type. </typeparam>
        /// <param name="obj"> The object to add. </param>
        /// <returns>
        ///     The <paramref name="obj" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T ToDispose<T>(T obj)
        {
            return _collector.Collect(obj);
        }

        /// <summary>
        ///     Executes the size changed action.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnSizeChanged()
        {
            SizeChanged?.Invoke(this);
        }

        /// <summary>
        ///     Executes the location changed action.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnLocationChanged()
        {
            LocationChanged?.Invoke(this);
        }

        /// <summary>
        ///     Executes the lost focus action.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnLostFocus()
        {
            LostFocus?.Invoke(this);
        }

        /// <summary>
        ///     Executes the got focus action.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnGotFocus()
        {
            GotFocus?.Invoke(this);
        }

        /// <summary>
        ///     Raises the mouse entered event.
        /// </summary>
        /// <param name="e"> Event information to send to registered event handlers. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnMouseEntered(in MouseEventArgs e)
        {
            MouseEntered?.Invoke(this, in e);
        }

        /// <summary>
        ///     Raises the mouse leaved event.
        /// </summary>
        /// <param name="e"> Event information to send to registered event handlers. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnMouseLeaved(in MouseEventArgs e)
        {
            MouseLeaved?.Invoke(this, in e);
        }

        /// <summary>
        ///     Raises the mouse move event.
        /// </summary>
        /// <param name="e">           Event information to send to registered event handlers. </param>
        /// <param name="eventAction"> [in,out] The event action. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnMouseMove(in MouseEventArgs e, ref EventAction eventAction)
        {
            MouseMove?.Invoke(this, in e, ref eventAction);
        }

        /// <summary>
        ///     Raises the mouse down event.
        /// </summary>
        /// <param name="e">           Event information to send to registered event handlers. </param>
        /// <param name="eventAction"> [in,out] The event action. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnMouseDown(in MouseEventArgs e, ref EventAction eventAction)
        {
            MouseDown?.Invoke(this, in e, ref eventAction);
        }

        /// <summary>
        ///     Raises the mouse up event.
        /// </summary>
        /// <param name="e">           Event information to send to registered event handlers. </param>
        /// <param name="eventAction"> [in,out] The event action. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnMouseUp(in MouseEventArgs e, ref EventAction eventAction)
        {
            MouseUp?.Invoke(this, in e, ref eventAction);
        }

        /// <summary>
        ///     Starts the drawing of this control.
        ///     This method is followed by calls to <see cref="Draw" /> and <see cref="EndDraw" />.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the <see cref="Draw" /> method should be called, <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool BeginDraw()
        {
            _isDirty |= _parent != null && _parent._isDirty;
            return _visible;
        }

        /// <summary>
        ///     Draws this control.
        /// </summary>
        /// <param name="elapsedSeconds"> The current elapsed time in seconds. </param>
        /// <param name="canvas">         The canvas. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Draw(float elapsedSeconds, Canvas canvas)
        {
            if (_isDirty)
            {
                if (_parent != null)
                {
                    _drawRectangle.X = _parent._drawRectangle.X + _parent._padding.W + _clientRectangle.X + _margin.W;
                    _drawRectangle.Y = _parent._drawRectangle.Y + _parent._padding.N + _clientRectangle.Y + _margin.N;
                    _drawRectangle.Width = _clientRectangle.Width - _parent.Padding.E - _margin.E;
                    _drawRectangle.Height = _clientRectangle.Height - _parent.Padding.S - _margin.S;
                }
                else
                {
                    _drawRectangle = new RectangleF(
                        _clientRectangle.X + _margin.W,
                        _clientRectangle.Y + _margin.N,
                        _clientRectangle.Width - _margin.E,
                        _clientRectangle.Height - _margin.S);
                }
                OnDrawRectangleChanged();
            }
            _backgroundBrush?.Render(canvas, _drawRectangle, _opacity);
        }

        /// <summary>
        ///     Called if the control was dirty and the draw rectangle was changed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnDrawRectangleChanged() { }

        /// <summary>
        ///     Ends the drawing of this control.
        ///     This method is preceded by calls to <see cref="Draw" /> and <see cref="BeginDraw" />.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void EndDraw()
        {
            _isDirty = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InternalSetFocus(bool focused)
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (focused && !_hasFocus)
            {
                OnGotFocus();
                _hasFocus = true;
            }
            else if (!focused && _hasFocus)
            {
                OnLostFocus();
                _hasFocus = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InternalSetMouseEntered(bool entered, in MouseEventArgs e)
        {
            // ReSharper disable once AssignmentInConditionalExpression
            if (entered && !_isMouseEntered) { OnMouseEntered(in e); }
            else if (!entered && _isMouseEntered) { OnMouseLeaved(in e); }

            _isMouseEntered = entered;
        }

        internal virtual bool InternalMouseMove(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (_drawRectangle.Contains(e.Position))
            {
                OnMouseMove(in e, ref eventAction);
                if (!_isMouseEntered)
                {
                    _uiManager!.SetEnteredControl(this, in e);
                }
                return true;
            }

            return false;
        }

        internal virtual bool InternalMouseDown(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (_isMouseEntered)
            {
                OnMouseDown(in e, ref eventAction);
                if (!_hasFocus)
                {
                    _uiManager!.SetFocusedControl(this, true);
                }

                return true;
            }

            return false;
        }

        internal virtual bool InternalMouseUp(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (_isMouseEntered)
            {
                OnMouseUp(in e, ref eventAction);
                return true;
            }

            return false;
        }

        #region IDisposable Support

        private bool _disposed;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                OnDispose(disposing);
                _collector.DisposeAndClear(disposing);
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~Control()
        {
            Dispose(false);
        }

        /// <summary>
        ///     called then the instance is disposing.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}