#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Mathematics
{
    /// <summary>
    ///     Defines the viewport dimensions using float coordinates for (X,Y,Width,Height).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ViewportF
    {
        /// <summary>
        ///     Position of the pixel coordinate of the upper-left corner of the viewport.
        /// </summary>
        public float X;

        /// <summary>
        ///     Position of the pixel coordinate of the upper-left corner of the viewport.
        /// </summary>
        public float Y;

        /// <summary>
        ///     Width dimension of the viewport.
        /// </summary>
        public float Width;

        /// <summary>
        ///     Height dimension of the viewport.
        /// </summary>
        public float Height;

        /// <summary>
        ///     Gets or sets the minimum depth of the clip volume.
        /// </summary>
        public float MinDepth;

        /// <summary>
        ///     Gets or sets the maximum depth of the clip volume.
        /// </summary>
        public float MaxDepth;

        /// <summary>
        ///     Gets the aspect ratio used by the viewport.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public readonly float AspectRatio
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!Math2.IsZero(Height))
                {
                    return Width / Height;
                }

                return 0f;
            }
        }

        /// <summary>
        ///     Gets the size of this resource.
        /// </summary>
        /// <value>The bounds.</value>
        public RectangleF Bounds
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get { return new(X, Y, Width, Height); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X      = value.X;
                Y      = value.Y;
                Width  = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewportF" /> struct.
        /// </summary>
        /// <param name="x">The x coordinate of the upper-left corner of the viewport in pixels.</param>
        /// <param name="y">The y coordinate of the upper-left corner of the viewport in pixels.</param>
        /// <param name="width">The width of the viewport in pixels.</param>
        /// <param name="height">The height of the viewport in pixels.</param>
        public ViewportF(float x, float y, float width, float height)
        {
            X        = x;
            Y        = y;
            Width    = width;
            Height   = height;
            MinDepth = 0f;
            MaxDepth = 1f;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewportF" /> struct.
        /// </summary>
        /// <param name="x">The x coordinate of the upper-left corner of the viewport in pixels.</param>
        /// <param name="y">The y coordinate of the upper-left corner of the viewport in pixels.</param>
        /// <param name="width">The width of the viewport in pixels.</param>
        /// <param name="height">The height of the viewport in pixels.</param>
        /// <param name="minDepth">The minimum depth of the clip volume.</param>
        /// <param name="maxDepth">The maximum depth of the clip volume.</param>
        public ViewportF(float x, float y, float width, float height, float minDepth, float maxDepth)
        {
            X        = x;
            Y        = y;
            Width    = width;
            Height   = height;
            MinDepth = minDepth;
            MaxDepth = maxDepth;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewportF" /> struct.
        /// </summary>
        /// <param name="bounds">A bounding box that defines the location and size of the viewport in a render target.</param>
        public ViewportF(RectangleF bounds)
        {
            X        = bounds.X;
            Y        = bounds.Y;
            Width    = bounds.Width;
            Height   = bounds.Height;
            MinDepth = 0f;
            MaxDepth = 1f;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                hashCode = (hashCode * 397) ^ MinDepth.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxDepth.GetHashCode();
                return hashCode;

                // ReSharper enable NonReadonlyMemberInGetHashCode
            }
        }

        /// <summary>
        ///     Determines whether the specified <see cref="ViewportF" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="ViewportF" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="ViewportF" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(in ViewportF other)
        {
            return Math2.NearEqual(X,        other.X) &&
                   Math2.NearEqual(Y,        other.Y) &&
                   Math2.NearEqual(Width,    other.Width) &&
                   Math2.NearEqual(Height,   other.Height) &&
                   Math2.NearEqual(MinDepth, other.MinDepth) &&
                   Math2.NearEqual(MaxDepth, other.MaxDepth);
        }

        /// <summary>
        ///     Determines whether the specified object is equal to this instance.
        /// </summary>
        /// <param name="value">The object to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals(object? value)
        {
            return value is ViewportF other && Equals(in other);
        }
        
        /// <inheritdoc/>
        public override readonly string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{{X:{0} Y:{1} Width:{2} Height:{3} MinDepth:{4} MaxDepth:{5}}}",
                X.ToString(CultureInfo.CurrentCulture),
                Y.ToString(CultureInfo.CurrentCulture),
                Width.ToString(CultureInfo.CurrentCulture),
                Height.ToString(CultureInfo.CurrentCulture),
                MinDepth.ToString(CultureInfo.CurrentCulture),
                MaxDepth.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        ///     Projects a 3D vector from object space into screen space.
        /// </summary>
        /// <param name="source">The vector to project.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="view">The view matrix.</param>
        /// <param name="world">The world matrix.</param>
        /// <returns>The projected vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector3 Project(in Vector3   source,
                                        in Matrix4x4 projection,
                                        in Matrix4x4 view,
                                        in Matrix4x4 world)
        {
            Project(in source, world * view * projection, out Vector3 vector);
            return vector;
        }

        /// <summary>
        ///     Projects a 3D vector from object space into screen space.
        /// </summary>
        /// <param name="source">The vector to project.</param>
        /// <param name="matrix">A combined WorldViewProjection matrix.</param>
        /// <param name="vector">The projected vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Project(in Vector3 source, in Matrix4x4 matrix, out Vector3 vector)
        {
            vector = Vector3.Transform(source, matrix);
            float a = source.X * matrix.M14 + source.Y * matrix.M24 + source.Z * matrix.M34 + matrix.M44;

            if (!Math2.IsOne(a))
            {
                vector /= a;
            }

            vector.X = (vector.X + 1f) * 0.5f * Width + X;
            vector.Y = (-vector.Y + 1f) * 0.5f * Height + Y;
            vector.Z = vector.Z * (MaxDepth - MinDepth) + MinDepth;
        }

        /// <summary>
        ///     Converts a screen space point into a corresponding point in world space.
        /// </summary>
        /// <param name="source">The vector to project.</param>
        /// <param name="projection">The projection matrix.</param>
        /// <param name="view">The view matrix.</param>
        /// <param name="world">The world matrix.</param>
        /// <returns>The unprojected Vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector3 Unproject(in Vector3   source,
                                          in Matrix4x4 projection,
                                          in Matrix4x4 view,
                                          in Matrix4x4 world)
        {
            Matrix4x4.Invert(world * view * projection, out Matrix4x4 matrix);
            Unproject(in source, in matrix, out Vector3 vector);
            return vector;
        }

        /// <summary>
        ///     Converts a screen space point into a corresponding point in world space.
        /// </summary>
        /// <param name="source">The vector to project.</param>
        /// <param name="matrix">An inverted combined WorldViewProjection matrix.</param>
        /// <param name="vector">The unprojected vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Unproject(in Vector3 source, in Matrix4x4 matrix, out Vector3 vector)
        {
            vector.X = (source.X - X) / Width * 2f - 1f;
            vector.Y = -((source.Y - Y) / Height * 2f - 1f);
            vector.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);

            float a = vector.X * matrix.M14 + vector.Y * matrix.M24 + vector.Z * matrix.M34 + matrix.M44;
            vector = Vector3.Transform(vector, matrix);

            if (!Math2.IsOne(a))
            {
                vector /= a;
            }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in ViewportF left, in ViewportF right)
        {
            return left.Equals(in right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in ViewportF left, in ViewportF right)
        {
            return !left.Equals(in right);
        }

        /// <summary>
        ///     Performs an explicit conversion from <see cref="Viewport" /> to <see cref="ViewportF" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ViewportF(in Viewport value)
        {
            return new(value.X, value.Y, value.Width, value.Height, value.MinDepth, value.MaxDepth);
        }
    }
}