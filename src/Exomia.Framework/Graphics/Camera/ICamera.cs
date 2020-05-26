#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Input;
using SharpDX;

namespace Exomia.Framework.Graphics.Camera
{
    /// <summary>
    ///     Interface for a camera.
    /// </summary>
    public interface ICamera : IComponent, IInitializable, IUpdateable, IInputHandler
    {
        /// <summary>
        ///     Occurs when position is changed.
        /// </summary>
        event EventHandler<ICamera>? PositionChanged;

        /// <summary>
        ///     Occurs when the target is changed.
        /// </summary>
        event EventHandler<ICamera>? TargetChanged;

        /// <summary>
        ///     Gets the view matrix.
        /// </summary>
        /// <value>
        ///     The view matrix.
        /// </value>
        Matrix ViewMatrix { get; }

        /// <summary>
        ///     Gets the projection matrix.
        /// </summary>
        /// <value>
        ///     The projection matrix.
        /// </value>
        Matrix ProjectionMatrix { get; }

        /// <summary>
        ///     Gets the view * projection matrix.
        /// </summary>
        /// <value>
        ///     The view * projection matrix.
        /// </value>
        Matrix ViewProjectionMatrix { get; }


        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        /// <value>
        ///     The position.
        /// </value>
        Vector3 Position { get; set; }

        /// <summary>
        ///     Gets or sets the target for the camera to look at.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        Vector3 Target { get; set; }

        /// <summary>
        ///     Gets or sets the up vector.
        /// </summary>
        /// <value>
        ///     The up vector.
        /// </value>
        Vector3 Up { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the camera is focused.
        /// </summary>
        /// <value>
        ///     True if focused, false if not.
        /// </value>
        bool Focused { get; set; }

        /// <summary>
        ///     Gets the frustum.
        /// </summary>
        /// <value>
        ///     The frustum.
        /// </value>
        BoundingFrustum Frustum { get; }

        /// <summary>
        ///     Sets a position.
        /// </summary>
        /// <param name="newPos">     The new position. </param>
        /// <param name="raiseEvent"> (Optional) True to raise event. </param>
        void SetPosition(Vector3 newPos, bool raiseEvent = false);

        /// <summary>
        ///     Adds a component.
        /// </summary>
        /// <param name="component"> The component. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        bool AddComponent(ICameraComponent component);

        /// <summary>
        ///     Gets a component.
        /// </summary>
        /// <param name="name"> The name. </param>
        /// <returns>
        ///     The component.
        /// </returns>
        ICameraComponent? GetComponent(string name);
    }
}