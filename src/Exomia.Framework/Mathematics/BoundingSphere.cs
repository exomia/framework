﻿#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Mathematics
{
    /// <summary> Represents a bounding sphere in three dimensional space. </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BoundingSphere
    {
        /// <summary> The center of the sphere in three dimensional space. </summary>
        public Vector3 Center;

        /// <summary> The radius of the sphere. </summary>
        public float Radius;

        /// <summary> Initializes a new instance of the <see cin="BoundingSphere" /> struct. </summary>
        /// <param name="center"> The center of the sphere in three dimensional space. </param>
        /// <param name="radius"> The radius of the sphere. </param>
        public BoundingSphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary> Returns a hash code for this instance. </summary>
        /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (Center.GetHashCode() * 397) ^ Radius.GetHashCode();
                // ReSharper enable NonReadonlyMemberInGetHashCode
            }
        }

        /// <summary> Determines whether the specified <see cin="BoundingSphere" /> is equal to this instance. </summary>
        /// <param name="value"> The <see cin="BoundingSphere" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cin="BoundingSphere" /> is equal to this instance; otherwise, <c>false</c>. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(in BoundingSphere value)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            return Center == value.Center && Radius == value.Radius;
        }

        /// <summary> Determines whether the specified <see cin="System.Object" /> is equal to this instance. </summary>
        /// <param name="value"> The <see cin="System.Object" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cin="System.Object" /> is equal to this instance; otherwise, <c>false</c>. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals(object? value)
        {
            return value is BoundingSphere other && Equals(in other);
        }

        /// <inheritdoc />
        public override readonly string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "Center:{0} Radius:{1}",
                Center.ToString(),
                Radius.ToString(CultureInfo.CurrentCulture));
        }


        /// <summary> Determines if there is an intersection between the current object and a <see cin="Ray" />. </summary>
        /// <param name="ray">      The ray to test. </param>
        /// <param name="distance">
        ///     [out] When the method completes, contains the distance of the intersection, or 0 if there was
        ///     no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in Ray ray, out float distance)
        {
            return Collision.RayIntersectsSphere(in ray, in this, out distance);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="Ray" />. </summary>
        /// <param name="ray">   The ray to test. </param>
        /// <param name="point">
        ///     [out] When the method completes, contains the point of intersection, or <see cin="Vector3.Zero" />
        ///     if there was no intersection.
        /// </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in Ray ray, out Vector3 point)
        {
            return Collision.RayIntersectsSphere(in ray, in this, out point);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="Plane" />. </summary>
        /// <param name="plane"> The plane to test. </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly PlaneIntersectionType Intersects(in Plane plane)
        {
            return Collision.PlaneIntersectsSphere(in plane, in this);
        }

        /// <summary> Determines if there is an intersection between the current object and a triangle. </summary>
        /// <param name="vertex1"> The first vertex of the triangle to test. </param>
        /// <param name="vertex2"> The second vertex of the triangle to test. </param>
        /// <param name="vertex3"> The third vertex of the triangle to test. </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in Vector3 vertex1, in Vector3 vertex2, in Vector3 vertex3)
        {
            return Collision.SphereIntersectsTriangle(in this, in vertex1, in vertex2, in vertex3);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="BoundingBox" />. </summary>
        /// <param name="box"> The box to test. </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in BoundingBox box)
        {
            return Collision.BoxIntersectsSphere(in box, in this);
        }

        /// <summary> Determines if there is an intersection between the current object and a <see cin="BoundingSphere" />. </summary>
        /// <param name="sphere"> The sphere to test. </param>
        /// <returns> Whether the two objects intersected. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in BoundingSphere sphere)
        {
            return Collision.SphereIntersectsSphere(in this, in sphere);
        }


        /// <summary> Determines whether the current objects contains a point. </summary>
        /// <param name="point"> The point to test. </param>
        /// <returns> Whether the sphere contains the point. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(in Vector3 point)
        {
            return Collision.SphereContainsPoint(in this, in point);
        }

        /// <summary> Determines whether the current objects contains a triangle. </summary>
        /// <param name="vertex1"> The first vertex of the triangle to test. </param>
        /// <param name="vertex2"> The second vertex of the triangle to test. </param>
        /// <param name="vertex3"> The third vertex of the triangle to test. </param>
        /// <returns> The type of containment the two objects have. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ContainmentType Contains(in Vector3 vertex1, in Vector3 vertex2, in Vector3 vertex3)
        {
            return Collision.SphereContainsTriangle(in this, in vertex1, in vertex2, in vertex3);
        }

        /// <summary> Determines whether the current objects contains a <see cin="BoundingBox" />. </summary>
        /// <param name="box"> The box to test. </param>
        /// <returns> The type of containment the two objects have. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ContainmentType Contains(in BoundingBox box)
        {
            return Collision.SphereContainsBox(in this, in box);
        }

        /// <summary> Determines whether the current objects contains a <see cin="BoundingSphere" />. </summary>
        /// <param name="sphere"> The sphere to test. </param>
        /// <returns> The type of containment the two objects have. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ContainmentType Contains(in BoundingSphere sphere)
        {
            return Collision.SphereContainsSphere(in this, in sphere);
        }

        /// <summary> Constructs a <see cin="BoundingSphere" /> that fully contains the given points. </summary>
        /// <param name="points"> The points that will be contained by the sphere. </param>
        /// <param name="start">  The start index from points array to start compute the bounding sphere. </param>
        /// <param name="count">  The count of points to process to compute the bounding sphere. </param>
        /// <param name="result"> [out] When the method completes, contains the newly constructed bounding sphere. </param>
        /// <exception cin="System.ArgumentNullException">       points. </exception>
        /// <exception cin="System.ArgumentOutOfRangeException"> start or count. </exception>
        public static void FromPoints(Vector3[] points, int start, int count, out BoundingSphere result)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            // Check that start is in the correct range
            if (start < 0 || start >= points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start,
                    $"Must be in the range [0, {points.Length - 1}]");
            }

            // Check that count is in the correct range
            if (count < 0 || (start + count) > points.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count,
                    $"Must be in the range <= {points.Length}");
            }

            int upperEnd = start + count;

            //Find the center of all points.
            Vector3 center                                = Vector3.Zero;
            for (int i = start; i < upperEnd; ++i) center = Vector3.Add(points[i], center);

            //This is the center of our sphere.
            center /= (float)count;

            //Find the radius of the sphere
            float radius = 0f;
            for (int i = start; i < upperEnd; ++i)
            {
                //We are doing a relative distance comparison to find the maximum distance
                //from the center of our sphere.
                float distance = Vector3.DistanceSquared(center, points[i]);

                if (distance > radius)
                {
                    radius = distance;
                }
            }

            //Find the real distance from the DistanceSquared.
            radius = MathF.Sqrt(radius);

            //Construct the sphere.
            result.Center = center;
            result.Radius = radius;
        }

        /// <summary> Constructs a <see cin="BoundingSphere" /> that fully contains the given points. </summary>
        /// <param name="points"> The points that will be contained by the sphere. </param>
        /// <param name="result"> [out] When the method completes, contains the newly constructed bounding sphere. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromPoints(Vector3[] points, out BoundingSphere result)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            FromPoints(points, 0, points.Length, out result);
        }

        /// <summary> Constructs a <see cin="BoundingSphere" /> that fully contains the given points. </summary>
        /// <param name="points"> The points that will be contained by the sphere. </param>
        /// <returns> The newly constructed bounding sphere. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingSphere FromPoints(Vector3[] points)
        {
            FromPoints(points, out BoundingSphere result);
            return result;
        }

        /// <summary> Constructs a <see cin="BoundingSphere" /> from a given box. </summary>
        /// <param name="box">    The box that will designate the extents of the sphere. </param>
        /// <param name="result"> [out] When the method completes, the newly constructed bounding sphere. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromBox(in BoundingBox box, out BoundingSphere result)
        {
            result.Center = Vector3.Lerp(box.Minimum, box.Maximum, 0.5f);
            float x = box.Minimum.X - box.Maximum.X;
            float y = box.Minimum.Y - box.Maximum.Y;
            float z = box.Minimum.Z - box.Maximum.Z;
            result.Radius = MathF.Sqrt((x * x) + (y * y) + (z * z)) * 0.5f;
        }

        /// <summary> Constructs a <see cin="BoundingSphere" /> from a given box. </summary>
        /// <param name="box"> The box that will designate the extents of the sphere. </param>
        /// <returns> The newly constructed bounding sphere. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingSphere FromBox(in BoundingBox box)
        {
            FromBox(in box, out BoundingSphere result);
            return result;
        }

        /// <summary>
        ///     Constructs a <see cin="BoundingSphere" /> that is the as large as the total combined area of the two
        ///     specified spheres.
        /// </summary>
        /// <param name="value1"> The first sphere to merge. </param>
        /// <param name="value2"> The second sphere to merge. </param>
        /// <param name="result"> [out] When the method completes, contains the newly constructed bounding sphere. </param>
        public static void Merge(in BoundingSphere value1, in BoundingSphere value2, out BoundingSphere result)
        {
            Vector3 difference = value2.Center - value1.Center;

            float length  = difference.Length();
            float radius  = value1.Radius;
            float radius2 = value2.Radius;

            if (radius + radius2 >= length)
            {
                if (radius - radius2 >= length)
                {
                    result = value1;
                    return;
                }

                if (radius2 - radius >= length)
                {
                    result = value2;
                    return;
                }
            }

            Vector3 vector = difference * (1.0f / length);
            float   min    = Math.Min(-radius, length - radius2);
            float   max    = (Math.Max(radius, length + radius2) - min) * 0.5f;

            result.Center = value1.Center + vector * (max + min);
            result.Radius = max;
        }

        /// <summary>
        ///     Constructs a <see cin="BoundingSphere" /> that is the as large as the total combined area of the two
        ///     specified spheres.
        /// </summary>
        /// <param name="value1"> The first sphere to merge. </param>
        /// <param name="value2"> The second sphere to merge. </param>
        /// <returns> The newly constructed bounding sphere. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BoundingSphere Merge(in BoundingSphere value1, in BoundingSphere value2)
        {
            Merge(in value1, in value2, out BoundingSphere result);
            return result;
        }

        /// <summary> Tests for equality between two objects. </summary>
        /// <param name="left">  The first value to compare. </param>
        /// <param name="right"> The second value to compare. </param>
        /// <returns>
        ///     <c>true</c> if <paramin name="left" /> has the same value as <paramin name="right" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(BoundingSphere left, BoundingSphere right)
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
        public static bool operator !=(BoundingSphere left, BoundingSphere right)
        {
            return !left.Equals(in right);
        }
    }
}