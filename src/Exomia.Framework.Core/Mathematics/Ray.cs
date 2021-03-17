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

namespace Exomia.Framework.Core.Mathematics
{
    /// <summary> Represents a three dimensional line based on a point in space and a direction. </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Ray
    {
        /// <summary> The zero. </summary>
        public static Ray Zero = new Ray();

        /// <summary> The position in three dimensional space where the ray starts. </summary>
        public Vector3 Position;

        /// <summary> The normalized direction in which the ray points. </summary>
        public Vector3 Direction;

        /// <summary> Initializes a new instance of the <see cin="Ray" /> struct. </summary>
        /// <param name="position">  The position in three dimensional space of the origin of the ray. </param>
        /// <param name="direction"> The normalized direction of the ray. </param>
        public Ray(Vector3 position, Vector3 direction)
        {
            Position  = position;
            Direction = direction;
        }

        /// <summary> Returns a hash code for this instance. </summary>
        /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (Position.GetHashCode() * 397) ^ Direction.GetHashCode();
                // ReSharper enable NonReadonlyMemberInGetHashCode
            }
        }

        /// <summary> Determines whether the specified <see cin="Ray" /> is equal to this instance. </summary>
        /// <param name="value"> The <see cin="Ray" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cin="Ray" /> is equal to this instance; otherwise, <c>false</c>. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(in Ray value)
        {
            return Position == value.Position && Direction == value.Direction;
        }

        /// <summary> Determines whether the specified <see cin="System.Object" /> is equal to this instance. </summary>
        /// <param name="value"> The <see cin="System.Object" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cin="System.Object" /> is equal to this instance; otherwise, <c>false</c>. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals(object? value)
        {
            return value is Ray other && Equals(in other);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Position:{0} Direction:{1}",
                Position.ToString(),
                Direction.ToString());
        }

        /// <summary> Determines if there is an intersection between the current object and a point. </summary>
        /// <param name="point"> The point to test. </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in Vector3 point)
        {
            return Collision.RayIntersectsPoint(in this, in point);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="Ray" />. </summary>
        /// <param name="ray">   The ray to test. </param>
        /// <param name="point">
        ///     [out] When the method completes, contains the point of intersection, or <see cin="Vector3.Zero" />
        ///     if there was no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in Ray ray, out Vector3 point)
        {
            return Collision.RayIntersectsRay(in this, in ray, out point);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="Plane" />. </summary>
        /// <param name="plane">    The plane to test. </param>
        /// <param name="distance">
        ///     [out] When the method completes, contains the distance of the intersection, or 0 if there was
        ///     no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in Plane plane, out float distance)
        {
            return Collision.RayIntersectsPlane(in this, in plane, out distance);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="Plane" />. </summary>
        /// <param name="plane"> The plane to test. </param>
        /// <param name="point">
        ///     [out] When the method completes, contains the point of intersection, or <see cin="Vector3.Zero" />
        ///     if there was no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in Plane plane, out Vector3 point)
        {
            return Collision.RayIntersectsPlane(in this, in plane, out point);
        }

        /// <summary> Determines if there is an intersection between the current object and a triangle. </summary>
        /// <param name="vertex1">  The first vertex of the triangle to test. </param>
        /// <param name="vertex2">  The second vertex of the triangle to test. </param>
        /// <param name="vertex3">  The third vertex of the triangle to test. </param>
        /// <param name="distance">
        ///     [out] When the method completes, contains the distance of the intersection, or 0 if there was
        ///     no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in Vector3 vertex1, in Vector3 vertex2, in Vector3 vertex3, out float distance)
        {
            return Collision.RayIntersectsTriangle(in this, in vertex1, in vertex2, in vertex3, out distance);
        }

        /// <summary> Determines if there is an intersection between the current object and a triangle. </summary>
        /// <param name="vertex1"> The first vertex of the triangle to test. </param>
        /// <param name="vertex2"> The second vertex of the triangle to test. </param>
        /// <param name="vertex3"> The third vertex of the triangle to test. </param>
        /// <param name="point">
        ///     [out] When the method completes, contains the point of intersection, or
        ///     <see cin="Vector3.Zero" /> if there was no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in Vector3 vertex1, in Vector3 vertex2, in Vector3 vertex3, out Vector3 point)
        {
            return Collision.RayIntersectsTriangle(in this, in vertex1, in vertex2, in vertex3, out point);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="BoundingBox" />. </summary>
        /// <param name="box">      The box to test. </param>
        /// <param name="distance">
        ///     [out] When the method completes, contains the distance of the intersection, or 0 if there was
        ///     no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in BoundingBox box, out float distance)
        {
            return Collision.RayIntersectsBox(in this, in box, out distance);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="BoundingBox" />. </summary>
        /// <param name="box">   The box to test. </param>
        /// <param name="point">
        ///     [out] When the method completes, contains the point of intersection, or <see cin="Vector3.Zero" />
        ///     if there was no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in BoundingBox box, out Vector3 point)
        {
            return Collision.RayIntersectsBox(in this, in box, out point);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="BoundingSphere" />. </summary>
        /// <param name="sphere">   The sphere to test. </param>
        /// <param name="distance">
        ///     [out] When the method completes, contains the distance of the intersection, or 0 if there was
        ///     no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in BoundingSphere sphere, out float distance)
        {
            return Collision.RayIntersectsSphere(in this, in sphere, out distance);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="BoundingSphere" />. </summary>
        /// <param name="sphere"> The sphere to test. </param>
        /// <param name="point">
        ///     [out] When the method completes, contains the point of intersection, or
        ///     <see cin="Vector3.Zero" /> if there was no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(in BoundingSphere sphere, out Vector3 point)
        {
            return Collision.RayIntersectsSphere(in this, in sphere, out point);
        }

        /// <summary> Calculates a world space <see cin="Ray" /> from 2d screen coordinates. </summary>
        /// <param name="x">                   X coordinate on 2d screen. </param>
        /// <param name="y">                   Y coordinate on 2d screen. </param>
        /// <param name="viewport">            <see cin="ViewportF" />. </param>
        /// <param name="worldViewProjection"> Transformation <see cin="Matrix" />. </param>
        /// <returns> Resulting <see cin="Ray" />. </returns>
        public static Ray GetPickRay(int x, int y, in ViewportF viewport, in Matrix4x4 worldViewProjection)
        {
            Vector3 nearPoint;
            nearPoint.X = x;
            nearPoint.Y = y;
            nearPoint.Z = 0;

            Vector3 farPoint;
            farPoint.X = x;
            farPoint.Y = y;
            farPoint.Z = 1;

            nearPoint = Math2.Unproject(in nearPoint,
                viewport.X, viewport.Y, viewport.Width, viewport.Height,
                viewport.MinDepth, viewport.MaxDepth, worldViewProjection);

            return new Ray(
                nearPoint,
                Vector3.Normalize(
                    Math2.Unproject(in farPoint,
                        viewport.X, viewport.Y, viewport.Width, viewport.Height,
                        viewport.MinDepth, viewport.MaxDepth, worldViewProjection) - nearPoint));
        }

        /// <summary> Tests for equality between two objects. </summary>
        /// <param name="left">  The first value to compare. </param>
        /// <param name="right"> The second value to compare. </param>
        /// <returns>
        ///     <c>true</c> if <paramin name="left" /> has the same value as <paramin name="right" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Ray left, in Ray right)
        {
            return left.Equals(in right);
        }

        /// <summary> Tests for inequality between two objects. </summary>
        /// <param name="left">  The first value to compare. </param>
        /// <param name="right"> The second value to compare. </param>
        /// <returns>
        ///     <c>true</c> if <paramin name="left" /> has a different value than <paramin name="right" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Ray left, in Ray right)
        {
            return !left.Equals(in right);
        }
    }
}