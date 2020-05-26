#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using SharpDX;

namespace Exomia.Framework.Graphics.Camera
{
    /// <summary>
    ///     A camera base.
    /// </summary>
    public sealed class Camera3D : ICamera, IDisposable
    {
        /// <inheritdoc />
        public event EventHandler? EnabledChanged;

        /// <inheritdoc />
        public event EventHandler? UpdateOrderChanged;

        /// <inheritdoc />
        public event EventHandler<ICamera>? PositionChanged;

        /// <inheritdoc />
        public event EventHandler<ICamera>? TargetChanged;

        private readonly List<ICameraComponent>           _components;
        private readonly List<IUpdateableCameraComponent> _updateableCameraComponents;
        private readonly Matrix                           _projectionMatrix;

        private string _name = string.Empty;

        private bool _isInitialized,
                     _enabled,
                     _focused         = true,
                     _viewMatrixDirty = true;

        private int               _updateOrder;
        private Matrix            _viewMatrix, _viewProjectionMatrix;
        private Vector3           _position, _target, _up;
        private BoundingFrustum   _frustum;
        private IServiceRegistry? _registry;

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <inheritdoc />
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc />
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    UpdateOrderChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc />
        public Matrix ViewMatrix
        {
            get { return _viewMatrix; }
        }

        /// <inheritdoc />
        public Matrix ProjectionMatrix
        {
            get { return _projectionMatrix; }
        }
        

        /// <inheritdoc />
        public Matrix ViewProjectionMatrix
        {
            get { return _viewProjectionMatrix; }
        }

        /// <inheritdoc />
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position        = value;
                    _viewMatrixDirty = true;
                    PositionChanged?.Invoke(this);
                }
            }
        }

        /// <inheritdoc />
        public Vector3 Target
        {
            get { return _target; }
            set
            {
                if (_target != value)
                {
                    _target          = value;
                    _viewMatrixDirty = true;
                    TargetChanged?.Invoke(this);
                }
            }
        }

        /// <inheritdoc />
        public Vector3 Up
        {
            get { return _up; }
            set
            {
                if (_up != value)
                {
                    _up              = value;
                    _viewMatrixDirty = true;
                }
            }
        }

        /// <inheritdoc />
        public bool Focused
        {
            get { return _focused; }
            set { _focused = value; }
        }

        /// <inheritdoc />
        public BoundingFrustum Frustum
        {
            get { return _frustum; }
            set { _frustum = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Camera3D" /> class.
        /// </summary>
        /// <param name="position">    The position. </param>
        /// <param name="target">      Target for the. </param>
        /// <param name="up">          The up. </param>
        /// <param name="aspectRatio"> The aspect ratio. </param>
        /// <param name="components">  A variable-length parameters list containing components. </param>
        public Camera3D(Vector3                   position,
                        Vector3                   target,
                        Vector3                   up,
                        float                     aspectRatio,
                        params ICameraComponent[] components)
            : this(position, target, up, aspectRatio, MathUtil.PiOverFour, 0.1f, 10000f, components) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Camera3D" /> class.
        /// </summary>
        /// <param name="position">    The position. </param>
        /// <param name="target">      Target for the. </param>
        /// <param name="up">          The up. </param>
        /// <param name="aspectRatio"> The aspect ratio. </param>
        /// <param name="fieldOfView"> The field of view. </param>
        /// <param name="components">  A variable-length parameters list containing components. </param>
        public Camera3D(Vector3                   position,
                        Vector3                   target,
                        Vector3                   up,
                        float                     aspectRatio,
                        float                     fieldOfView,
                        params ICameraComponent[] components)
            : this(position, target, up, aspectRatio, fieldOfView, 0.1f, 10000f, components) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Camera3D" /> class.
        /// </summary>
        /// <param name="position">    The position. </param>
        /// <param name="target">      Target for the. </param>
        /// <param name="up">          The up. </param>
        /// <param name="aspectRatio"> The aspect ratio. </param>
        /// <param name="zNear">       The near. </param>
        /// <param name="zFar">        The far. </param>
        /// <param name="components">  A variable-length parameters list containing components. </param>
        public Camera3D(Vector3                   position,
                        Vector3                   target,
                        Vector3                   up,
                        float                     aspectRatio,
                        float                     zNear,
                        float                     zFar,
                        params ICameraComponent[] components)
            : this(position, target, up, aspectRatio, MathUtil.PiOverFour, zNear, zFar, components) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Camera3D" /> class.
        /// </summary>
        /// <param name="position">    The position. </param>
        /// <param name="target">      Target for the. </param>
        /// <param name="up">          The up. </param>
        /// <param name="aspectRatio"> The aspect ratio. </param>
        /// <param name="fieldOfView"> The field of view. </param>
        /// <param name="zNear">       The near. </param>
        /// <param name="zFar">        The far. </param>
        /// <param name="components">  A variable-length parameters list containing components. </param>
        public Camera3D(Vector3                   position,
                        Vector3                   target,
                        Vector3                   up,
                        float                     aspectRatio,
                        float                     fieldOfView,
                        float                     zNear,
                        float                     zFar,
                        params ICameraComponent[] components)
        {
            _position = position;
            _target   = target;
            _up       = up;

            _projectionMatrix = Matrix.PerspectiveFovLH(fieldOfView, aspectRatio, zNear, zFar);

            _frustum = new BoundingFrustum(_viewProjectionMatrix = _viewMatrix * _projectionMatrix);

            _components                 = new List<ICameraComponent>(components.Length);
            _updateableCameraComponents = new List<IUpdateableCameraComponent>(components.Length);

            foreach (ICameraComponent component in components)
            {
                AddComponent(component);
            }
        }

        /// <inheritdoc />
        public void SetPosition(Vector3 newPos, bool raiseEvent = false)
        {
            if (raiseEvent)
            {
                Position = newPos;
            }
            else
            {
                _position        = newPos;
                _viewMatrixDirty = true;
            }
        }

        /// <inheritdoc />
        public bool AddComponent(ICameraComponent component)
        {
            if (!_components.Contains(component))
            {
                if (_isInitialized)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    if (component is IInitializableCameraComponent c)
                    {
                        c.Initialize(_registry!, this);
                    }
                }
                _components.Add(component);

                if (component is IUpdateableCameraComponent updateableCameraComponent)
                {
                    _updateableCameraComponents.Add(updateableCameraComponent);
                }

                return true;
            }
            return false;
        }

        /// <inheritdoc />
        public ICameraComponent? GetComponent(string name)
        {
            for (int c = 0; c < _components.Count; c++)
            {
                ICameraComponent controller = _components[c];
                if (controller.Name == name) { return controller; }
            }
            return null;
        }

        /// <inheritdoc />
        public void Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                _registry      = registry;

                foreach (ICameraComponent component in _components)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    if (component is IInitializableCameraComponent c)
                    {
                        c.Initialize(registry, this);
                    }
                }
            }
        }

        /// <inheritdoc />
        void IInputHandler.RegisterInput(IInputDevice device)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is IInputHandler inputHandler)
                {
                    inputHandler.RegisterInput(device);
                }
            }
        }

        /// <inheritdoc />
        void IInputHandler.UnregisterInput(IInputDevice device)
        {
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is IInputHandler inputHandler)
                {
                    inputHandler.UnregisterInput(device);
                }
            }
        }

        /// <inheritdoc />
        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _updateableCameraComponents.Count; i++)
            {
                _updateableCameraComponents[i].Update(gameTime, this);
            }

            if (_viewMatrixDirty)
            {
                _viewMatrixDirty = false;
                _viewMatrix      = Matrix.LookAtLH(_position, _target, _up);
                _frustum         = new BoundingFrustum(_viewProjectionMatrix = _viewMatrix * _projectionMatrix);
            }
        }

        #region IDisposable Support

        /// <summary>
        ///     true if the instance is already disposed; false otherwise
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (ICameraComponent component in _components)
                    {
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        if (component is IDisposableCameraComponent c)
                        {
                            c.Dispose(this);
                        }
                    }
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~Camera3D()
        {
            Dispose(false);
        }

        #endregion
    }
}