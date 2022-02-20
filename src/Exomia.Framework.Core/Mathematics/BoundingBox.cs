#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Core.Mathematics;

/// <summary> Represents an axis-aligned bounding box in three dimensional space. </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BoundingBox
{
    /// <summary> The minimum point of the box. </summary>
    public Vector3 Minimum;

    /// <summary> The maximum point of the box. </summary>
    public Vector3 Maximum;

    /// <summary> Returns the width of the bounding box. </summary>
    /// <value> The width. </value>
    public readonly float Width
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return Maximum.X - Minimum.X; }
    }

    /// <summary> Returns the height of the bounding box. </summary>
    /// <value> The height. </value>
    public readonly float Height
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return Maximum.Y - Minimum.Y; }
    }

    /// <summary> Returns the height of the bounding box. </summary>
    /// <value> The depth. </value>
    public readonly float Depth
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return Maximum.Z - Minimum.Z; }
    }

    /// <summary> Returns the size of the bounding box. </summary>
    /// <value> The size. </value>
    public readonly Vector3 Size
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return Maximum - Minimum; }
    }

    /// <summary> Returns the size of the bounding box. </summary>
    /// <value> The center. </value>
    public readonly Vector3 Center
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return (Maximum + Minimum) * 0.5f; }
    }

    /// <summary> Initializes a new instance of the <see cref="BoundingBox" /> struct. </summary>
    /// <param name="minimum"> The minimum vertex of the bounding box. </param>
    /// <param name="maximum"> The maximum vertex of the bounding box. </param>
    public BoundingBox(Vector3 minimum, Vector3 maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    /// <summary> Returns a hash code for this instance. </summary>
    /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
    public readonly override int GetHashCode()
    {
        unchecked
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();

            // ReSharper enable NonReadonlyMemberInGetHashCode
        }
    }

    /// <summary> Determines whether the specified <see cref="BoundingBox" /> is equal to this instance. </summary>
    /// <param name="value"> The <see cref="BoundingBox" /> to compare with this instance. </param>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="BoundingBox" /> is equal to this instance;
    ///     otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(in BoundingBox value)
    {
        return Minimum == value.Minimum && Maximum == value.Maximum;
    }

    /// <summary> Determines whether the specified <see cref="System.Object" /> is equal to this instance. </summary>
    /// <param name="value"> The <see cref="System.Object" /> to compare with this instance. </param>
    /// <returns>
    ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
    ///     otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override bool Equals(object? value)
    {
        return value is BoundingBox other && Equals(in other);
    }

    /// <inheritdoc />
    public readonly override string ToString()
    {
        return string.Format(
            CultureInfo.CurrentCulture,
            "Minimum:{0} Maximum:{1}",
            Minimum.ToString(),
            Maximum.ToString());
    }

    /// <summary> Constructs a <see cref="BoundingBox" /> that fully contains the given points. </summary>
    /// <param name="points"> The points that will be contained by the box. </param>
    /// <param name="result"> [out] When the method completes, contains the newly constructed bounding box. </param>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="points" /> is <c>null</c>. </exception>
    public static void FromPoints(Vector3[] points, out BoundingBox result)
    {
        if (points == null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        Vector3 min = new(float.MaxValue);
        Vector3 max = new(float.MinValue);

        for (int i = 0; i < points.Length; ++i)
        {
            min = Vector3.Min(min, points[i]);
            max = Vector3.Max(max, points[i]);
        }

        result = new BoundingBox(min, max);
    }

    /// <summary> Constructs a <see cref="BoundingBox" /> that fully contains the given points. </summary>
    /// <param name="points"> The points that will be contained by the box. </param>
    /// <returns> The newly constructed bounding box. </returns>
    /// <exception cref="ArgumentNullException"> Thrown when <paramref name="points" /> is <c>null</c>. </exception>
    public static BoundingBox FromPoints(Vector3[] points)
    {
        if (points == null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        Vector3 min = new(float.MaxValue);
        Vector3 max = new(float.MinValue);

        for (int i = 0; i < points.Length; ++i)
        {
            min = Vector3.Min(min, points[i]);
            max = Vector3.Max(max, points[i]);
        }

        return new BoundingBox(min, max);
    }

    /// <summary> Constructs a <see cref="BoundingBox" /> from a given sphere. </summary>
    /// <param name="sphere"> The sphere that will designate the extents of the box. </param>
    /// <param name="result"> [out] When the method completes, contains the newly constructed bounding box. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FromSphere(in BoundingSphere sphere, out BoundingBox result)
    {
        result.Minimum = new Vector3(
            sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius);
        result.Maximum = new Vector3(
            sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius);
    }

    /// <summary> Constructs a <see cref="BoundingBox" /> from a given sphere. </summary>
    /// <param name="sphere"> The sphere that will designate the extents of the box. </param>
    /// <returns> The newly constructed bounding box. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox FromSphere(in BoundingSphere sphere)
    {
        BoundingBox box;
        box.Minimum = new Vector3(
            sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius);
        box.Maximum = new Vector3(
            sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius);
        return box;
    }

    /// <summary>
    ///     Constructs a <see cref="BoundingBox" /> that is as large as the total combined area of the two specified
    ///     boxes.
    /// </summary>
    /// <param name="value1"> The first box to merge. </param>
    /// <param name="value2"> The second box to merge. </param>
    /// <param name="result"> [out] When the method completes, contains the newly constructed bounding box. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Merge(in BoundingBox value1, in BoundingBox value2, out BoundingBox result)
    {
        result.Minimum = Vector3.Min(value1.Minimum, value2.Minimum);
        result.Maximum = Vector3.Max(value1.Maximum, value2.Maximum);
    }

    /// <summary>
    ///     Constructs a <see cref="BoundingBox" /> that is as large as the total combined area of the two specified
    ///     boxes.
    /// </summary>
    /// <param name="value1"> The first box to merge. </param>
    /// <param name="value2"> The second box to merge. </param>
    /// <returns> The newly constructed bounding box. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BoundingBox Merge(in BoundingBox value1, in BoundingBox value2)
    {
        BoundingBox box;
        box.Minimum = Vector3.Min(value1.Minimum, value2.Minimum);
        box.Maximum = Vector3.Max(value1.Maximum, value2.Maximum);
        return box;
    }

    /// <summary> Tests for equality between two objects. </summary>
    /// <param name="left">  The first value to compare. </param>
    /// <param name="right"> The second value to compare. </param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> has the same value as <paramref name="right" />;
    ///     otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in BoundingBox left, in BoundingBox right)
    {
        return left.Equals(in right);
    }

    /// <summary> Tests for inequality between two objects. </summary>
    /// <param name="left">  The first value to compare. </param>
    /// <param name="right"> The second value to compare. </param>
    /// <returns>
    ///     <c>true</c> if <paramref name="left" /> has a different value than
    ///     <paramref name="right" />; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in BoundingBox left, in BoundingBox right)
    {
        return !left.Equals(in right);
    }

    /// <summary> Retrieves the eight corners of the bounding box. </summary>
    /// <returns> An array of points representing the eight corners of the bounding box. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3[] GetCorners()
    {
        Vector3[] corners = new Vector3[8];
        GetCorners(corners);
        return corners;
    }

    /// <summary> Retrieves the eight corners of the bounding box. </summary>
    /// <param name="corners"> The corners. </param>
    public readonly void GetCorners(Vector3[] corners)
    {
        corners[0] = new Vector3(Minimum.X, Maximum.Y, Maximum.Z);
        corners[1] = new Vector3(Maximum.X, Maximum.Y, Maximum.Z);
        corners[2] = new Vector3(Maximum.X, Minimum.Y, Maximum.Z);
        corners[3] = new Vector3(Minimum.X, Minimum.Y, Maximum.Z);
        corners[4] = new Vector3(Minimum.X, Maximum.Y, Minimum.Z);
        corners[5] = new Vector3(Maximum.X, Maximum.Y, Minimum.Z);
        corners[6] = new Vector3(Maximum.X, Minimum.Y, Minimum.Z);
        corners[7] = new Vector3(Minimum.X, Minimum.Y, Minimum.Z);
    }

    /// <summary> Determines if there is an intersection between the current object and a <see cref="Ray" />. </summary>
    /// <param name="ray">      The ray to test. </param>
    /// <param name="distance">
    ///     [out] When the method completes, contains the distance of the intersection, or 0 if there was
    ///     no intersection.
    /// </param>
    /// <returns> Whether the two objects intersected. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(in Ray ray, out float distance)
    {
        return Collision.RayIntersectsBox(in ray, in this, out distance);
    }

    /// <summary> Determines if there is an intersection between the current object and a <see cref="Ray" />. </summary>
    /// <param name="ray">   The ray to test. </param>
    /// <param name="point">
    ///     [out] When the method completes, contains the point of intersection, or
    ///     <see cref="Vector3.Zero" /> if there was no intersection.
    /// </param>
    /// <returns> Whether the two objects intersected. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(in Ray ray, out Vector3 point)
    {
        return Collision.RayIntersectsBox(in ray, in this, out point);
    }

    /// <summary>
    ///     Determines if there is an intersection between the current object and a
    ///     <see cref="Plane" />.
    /// </summary>
    /// <param name="plane"> The plane to test. </param>
    /// <returns> Whether the two objects intersected. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly PlaneIntersectionType Intersects(in Plane plane)
    {
        return Collision.PlaneIntersectsBox(in plane, in this);
    }

    /// <summary> Determines if there is an intersection between the current object and a <see cref="BoundingBox" />. </summary>
    /// <param name="box"> The box to test. </param>
    /// <returns> Whether the two objects intersected. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(in BoundingBox box)
    {
        return Collision.BoxIntersectsBox(in this, in box);
    }

    /// <summary> Determines if there is an intersection between the current object and a <see cref="BoundingSphere" />. </summary>
    /// <param name="sphere"> The sphere to test. </param>
    /// <returns> Whether the two objects intersected. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(in BoundingSphere sphere)
    {
        return Collision.BoxIntersectsSphere(in this, in sphere);
    }

    /// <summary> Determines whether the current objects contains a point. </summary>
    /// <param name="point"> The point to test. </param>
    /// <returns> Whether the box contains the point. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Contains(in Vector3 point)
    {
        return Collision.BoxContainsPoint(in this, in point);
    }

    /// <summary> Determines whether the current objects contains a <see cref="BoundingBox" />. </summary>
    /// <param name="box"> The box to test. </param>
    /// <returns> The type of containment the two objects have. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ContainmentType Contains(in BoundingBox box)
    {
        return Collision.BoxContainsBox(in this, in box);
    }

    /// <summary> Determines whether the current objects contains a <see cref="BoundingSphere" />. </summary>
    /// <param name="sphere"> The sphere to test. </param>
    /// <returns> The type of containment the two objects have. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ContainmentType Contains(in BoundingSphere sphere)
    {
        return Collision.BoxContainsSphere(in this, in sphere);
    }
}