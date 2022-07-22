#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Mathematics;

/// <content> The mathematics 2. </content>
public static partial class Math2
{
    /// <summary> Projects a 3D vector from object space into screen space. </summary>
    /// <param name="vector">              The vector to project. </param>
    /// <param name="x">                   The X position of the viewport. </param>
    /// <param name="y">                   The Y position of the viewport. </param>
    /// <param name="width">               The width of the viewport. </param>
    /// <param name="height">              The height of the viewport. </param>
    /// <param name="minZ">                The minimum depth of the viewport. </param>
    /// <param name="maxZ">                The maximum depth of the viewport. </param>
    /// <param name="worldViewProjection"> The combined world-view-projection matrix. </param>
    /// <param name="result">              [out] When the method completes, contains the vector in screen space. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Project(in Vector3    vector,
                               float         x,
                               float         y,
                               float         width,
                               float         height,
                               float         minZ,
                               float         maxZ,
                               in  Matrix4x4 worldViewProjection,
                               out Vector3   result)
    {
        TransformCoordinate(in vector, in worldViewProjection, out Vector3 v);

        result = new Vector3(
            ((1.0f + v.X) * 0.5f * width)  + x,
            ((1.0f - v.Y) * 0.5f * height) + y,
            (v.Z          * (maxZ - minZ)) + minZ);
    }

    /// <summary> Projects a 3D vector from object space into screen space. </summary>
    /// <param name="vector">              The vector to project. </param>
    /// <param name="x">                   The X position of the viewport. </param>
    /// <param name="y">                   The Y position of the viewport. </param>
    /// <param name="width">               The width of the viewport. </param>
    /// <param name="height">              The height of the viewport. </param>
    /// <param name="minZ">                The minimum depth of the viewport. </param>
    /// <param name="maxZ">                The maximum depth of the viewport. </param>
    /// <param name="worldViewProjection"> The combined world-view-projection matrix. </param>
    /// <returns> The vector in screen space. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Project(in Vector3   vector,
                                  float        x,
                                  float        y,
                                  float        width,
                                  float        height,
                                  float        minZ,
                                  float        maxZ,
                                  in Matrix4x4 worldViewProjection)
    {
        Project(in vector, x, y, width, height, minZ, maxZ, in worldViewProjection, out Vector3 result);
        return result;
    }

    /// <summary> Projects a 3D vector from screen space into object space. </summary>
    /// <param name="vector">              The vector to project. </param>
    /// <param name="x">                   The X position of the viewport. </param>
    /// <param name="y">                   The Y position of the viewport. </param>
    /// <param name="width">               The width of the viewport. </param>
    /// <param name="height">              The height of the viewport. </param>
    /// <param name="minZ">                The minimum depth of the viewport. </param>
    /// <param name="maxZ">                The maximum depth of the viewport. </param>
    /// <param name="worldViewProjection"> The combined world-view-projection matrix. </param>
    /// <param name="result">              [out] When the method completes, contains the vector in object space. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Unproject(in Vector3    vector,
                                 float         x,
                                 float         y,
                                 float         width,
                                 float         height,
                                 float         minZ,
                                 float         maxZ,
                                 in  Matrix4x4 worldViewProjection,
                                 out Vector3   result)
    {
        Matrix4x4.Invert(worldViewProjection, out Matrix4x4 matrix);

        Vector3 v;
        v.X = (((vector.X - x)             / width) * 2.0f) - 1.0f;
        v.Y = -((((vector.Y - y) / height) * 2.0f) - 1.0f);
        v.Z = (vector.Z                            - minZ) / (maxZ - minZ);

        TransformCoordinate(in v, in matrix, out result);
    }

    /// <summary> Projects a 3D vector from screen space into object space. </summary>
    /// <param name="vector">              The vector to project. </param>
    /// <param name="x">                   The X position of the viewport. </param>
    /// <param name="y">                   The Y position of the viewport. </param>
    /// <param name="width">               The width of the viewport. </param>
    /// <param name="height">              The height of the viewport. </param>
    /// <param name="minZ">                The minimum depth of the viewport. </param>
    /// <param name="maxZ">                The maximum depth of the viewport. </param>
    /// <param name="worldViewProjection"> The combined world-view-projection matrix. </param>
    /// <returns> The vector in object space. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Unproject(in Vector3   vector,
                                    float        x,
                                    float        y,
                                    float        width,
                                    float        height,
                                    float        minZ,
                                    float        maxZ,
                                    in Matrix4x4 worldViewProjection)
    {
        Unproject(in vector, x, y, width, height, minZ, maxZ, in worldViewProjection, out Vector3 result);
        return result;
    }

    /// <summary> Performs a coordinate transformation using the given <see cref="Matrix4x4" />. </summary>
    /// <param name="coordinate"> The coordinate vector to transform. </param>
    /// <param name="transform">  The transformation <see cref="Matrix4x4" />. </param>
    /// <param name="result">     [out] When the method completes, contains the transformed coordinates. </param>
    /// <remarks>
    ///     A coordinate transform performs the transformation with the assumption that the w component is one. The four
    ///     dimensional vector obtained from the transformation
    ///     operation has each component in the vector divided by the w component. This forces the w component to be one and
    ///     therefore makes the vector homogeneous. The homogeneous
    ///     vector is often preferred when working with coordinates as the w component can safely be ignored.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TransformCoordinate(in Vector3 coordinate, in Matrix4x4 transform, out Vector3 result)
    {
        float w = 1f / ((coordinate.X * transform.M14) + (coordinate.Y * transform.M24) +
            (coordinate.Z             * transform.M34) + transform.M44);

        result.X = ((coordinate.X * transform.M11) + (coordinate.Y * transform.M21) +
            (coordinate.Z         * transform.M31) + transform.M41) * w;
        result.Y = ((coordinate.X * transform.M12) + (coordinate.Y * transform.M22) +
            (coordinate.Z         * transform.M32) + transform.M42) * w;
        result.Z = ((coordinate.X * transform.M13) + (coordinate.Y * transform.M23) +
            (coordinate.Z         * transform.M33) + transform.M43) * w;
    }

    /// <summary> Performs a coordinate transformation using the given <see cref="Matrix4x4" />. </summary>
    /// <param name="coordinate"> The coordinate vector to transform. </param>
    /// <param name="transform">  The transformation <see cref="Matrix4x4" />. </param>
    /// <returns> The transformed coordinates. </returns>
    /// <remarks>
    ///     A coordinate transform performs the transformation with the assumption that the w component is one. The four
    ///     dimensional vector obtained from the transformation
    ///     operation has each component in the vector divided by the w component. This forces the w component to be one and
    ///     therefore makes the vector homogeneous. The homogeneous
    ///     vector is often preferred when working with coordinates as the w component can safely be ignored.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 TransformCoordinate(in Vector3 coordinate, in Matrix4x4 transform)
    {
        TransformCoordinate(in coordinate, in transform, out Vector3 result);
        return result;
    }
}