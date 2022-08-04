#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Core.Mathematics;

/// <summary>
///     Defines a frustum which can be used in frustum culling, zoom to Extents (zoom to fit) operations, (matrix, frustum, camera) interchange,
///     and many kind of intersection testing.
/// </summary>
[StructLayout(LayoutKind.Explicit, Pack = 4)]
public unsafe struct BoundingFrustum
{
    private const int PLANE_SIZE = 4 * 4;

    // ATTENTION: DO NOT REORDER THE PLANES (left, right, top, bottom, near, far)
    [FieldOffset(PLANE_SIZE * 0)]
    private Plane _pLeft;

    [FieldOffset(PLANE_SIZE * 1)]
    private Plane _pRight;

    [FieldOffset(PLANE_SIZE * 2)]
    private Plane _pTop;

    [FieldOffset(PLANE_SIZE * 3)]
    private Plane _pBottom;

    [FieldOffset(PLANE_SIZE * 4)]
    private Plane _pNear;

    [FieldOffset(PLANE_SIZE * 5)]
    private Plane _pFar;

    [FieldOffset(PLANE_SIZE * 6)]
    private Matrix4x4 _pMatrix;

    /// <summary> Gets the Matrix that describes this bounding frustum. </summary>
    /// <value> The matrix. </value>
    public Matrix4x4 Matrix
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get { return _pMatrix; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _pMatrix = value;
            GetPlanesFromMatrix(in _pMatrix, out _pNear, out _pFar, out _pLeft, out _pRight, out _pTop, out _pBottom);
        }
    }

    /// <summary> Gets the near plane of the BoundingFrustum. </summary>
    /// <value> The near. </value>
    public readonly Plane Near
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _pNear; }
    }

    /// <summary> Gets the far plane of the BoundingFrustum. </summary>
    /// <value> The far. </value>
    public readonly Plane Far
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _pFar; }
    }

    /// <summary> Gets the left plane of the BoundingFrustum. </summary>
    /// <value> The left. </value>
    public readonly Plane Left
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _pLeft; }
    }

    /// <summary> Gets the right plane of the BoundingFrustum. </summary>
    /// <value> The right. </value>
    public readonly Plane Right
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _pRight; }
    }

    /// <summary> Gets the top plane of the BoundingFrustum. </summary>
    /// <value> The top. </value>
    public readonly Plane Top
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _pTop; }
    }

    /// <summary> Gets the bottom plane of the BoundingFrustum. </summary>
    /// <value> The bottom. </value>
    public readonly Plane Bottom
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _pBottom; }
    }

    /// <summary> Indicate whether the current BoundingFrustrum is Orthographic. </summary>
    /// <value> <c> true </c> if the current BoundingFrustrum is Orthographic; <c> false </c> otherwise. </value>
    public readonly bool IsOrthographic
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return (_pLeft.Normal == -_pRight.Normal) && (_pTop.Normal == -_pBottom.Normal); }
    }

    /// <summary> Creates a new instance of BoundingFrustum. </summary>
    /// <param name="matrix"> Combined matrix that usually takes view × projection matrix. </param>
    public BoundingFrustum(Matrix4x4 matrix)
    {
        _pMatrix = matrix;
        GetPlanesFromMatrix(in _pMatrix, out _pNear, out _pFar, out _pLeft, out _pRight, out _pTop, out _pBottom);
    }

    /// <summary> Returns a hash code for this instance. </summary>
    /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return _pMatrix.GetHashCode();
    }

    /// <summary> Determines whether the specified <see cref="BoundingFrustum" /> is equal to this instance. </summary>
    /// <param name="other"> The <see cref="BoundingFrustum" /> to compare with this instance. </param>
    /// <returns>
    ///     <c> true </c> if the specified <see cref="BoundingFrustum" /> is equal to this instance; <c> false </c> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(in BoundingFrustum other)
    {
        return _pMatrix == other._pMatrix;
    }

    /// <summary> Determines whether the specified <see cref="System.Object" /> is equal to this instance. </summary>
    /// <param name="obj"> The <see cref="System.Object" /> to compare with this instance. </param>
    /// <returns> <c> true </c> if the specified <see cref="System.Object" /> is equal to this instance; <c> false </c> otherwise. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override bool Equals(object? obj)
    {
        return obj is BoundingFrustum other && Equals(in other);
    }

    /// <summary> Creates a new frustum relaying on perspective camera parameters. </summary>
    /// <param name="cameraPos"> The camera pos. </param>
    /// <param name="lookDir"> The look dir. </param>
    /// <param name="upDir"> Up dir. </param>
    /// <param name="fov"> The fov. </param>
    /// <param name="zNear"> The zNear. </param>
    /// <param name="zFar"> The zFar. </param>
    /// <param name="aspect"> The aspect. </param>
    /// <returns> The bounding frustum calculated from perspective camera. </returns>
    public static BoundingFrustum FromCamera(in Vector3 cameraPos, in Vector3 lookDir, in Vector3 upDir, float fov, float zNear, float zFar, float aspect)
    {
        //http://knol.google.com/k/view-frustum

        Vector3 look = Vector3.Normalize(lookDir);
        Vector3 up   = Vector3.Normalize(upDir);

        Vector3 nearCenter     = cameraPos + look * zNear;
        Vector3 farCenter      = cameraPos + look * zFar;
        float   nearHalfHeight = zNear          * MathF.Tan(fov * 0.5f);
        float   farHalfHeight  = zFar           * MathF.Tan(fov * 0.5f);
        float   nearHalfWidth  = nearHalfHeight * aspect;
        float   farHalfWidth   = farHalfHeight  * aspect;

        Vector3 rightDir = Vector3.Normalize(Vector3.Cross(up, look));
        Vector3 near1    = nearCenter - nearHalfHeight * up + nearHalfWidth  * rightDir;
        Vector3 near2    = nearCenter                       + nearHalfHeight * up + nearHalfWidth * rightDir;
        Vector3 near3    = nearCenter + nearHalfHeight * up - nearHalfWidth  * rightDir;
        Vector3 near4    = nearCenter                       - nearHalfHeight * up - nearHalfWidth * rightDir;
        Vector3 far1     = farCenter - farHalfHeight * up   + farHalfWidth   * rightDir;
        Vector3 far2     = farCenter                        + farHalfHeight  * up + farHalfWidth * rightDir;
        Vector3 far3     = farCenter + farHalfHeight * up   - farHalfWidth   * rightDir;
        Vector3 far4     = farCenter                        - farHalfHeight  * up - farHalfWidth * rightDir;

        static Plane CreateNormalizedPlane(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            float x1      = point2.X  - point1.X;
            float y1      = point2.Y  - point1.Y;
            float z1      = point2.Z  - point1.Z;
            float x2      = point3.X  - point1.X;
            float y2      = point3.Y  - point1.Y;
            float z2      = point3.Z  - point1.Z;
            float yz      = (y1 * z2) - (z1 * y2);
            float xz      = (z1 * x2) - (x1 * z2);
            float xy      = (x1 * y2) - (y1 * x2);
            float invPyth = 1.0f / MathF.Sqrt((yz * yz) + (xz * xz) + (xy * xy));

            Plane plane;
            plane.Normal.X = yz * invPyth;
            plane.Normal.Y = xz * invPyth;
            plane.Normal.Z = xy * invPyth;
            plane.D        = -((plane.Normal.X * point1.X) + (plane.Normal.Y * point1.Y) + (plane.Normal.Z * point1.Z));
            return Plane.Normalize(plane);
        }

        BoundingFrustum result;
        result._pNear   = CreateNormalizedPlane(near1, near2, near3);
        result._pFar    = CreateNormalizedPlane(far3,  far2,  far1);
        result._pLeft   = CreateNormalizedPlane(near4, near3, far3);
        result._pRight  = CreateNormalizedPlane(far1,  far2,  near2);
        result._pTop    = CreateNormalizedPlane(near2, far2,  far3);
        result._pBottom = CreateNormalizedPlane(far4,  far1,  near1);
        result._pMatrix = Matrix4x4.CreateLookAt(cameraPos, cameraPos + look * 10, up) * Matrix4x4.CreatePerspectiveFieldOfView(fov, aspect, zNear, zFar);
        return result;
    }

    /// <summary> Returns one of the 6 planes related to this frustum. </summary>
    /// <param name="index"> Plane index 0-5 (left, right, top, bottom, near, far) </param>
    /// <returns> The plane. </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Plane GetPlane2(int index)
    {
#if DEBUG
        if (index >= 6) { throw new ArgumentOutOfRangeException(nameof(index)); }
#endif
#pragma warning disable 8509
        return index switch
#pragma warning restore 8509
        {
            0 => _pLeft,
            1 => _pRight,
            2 => _pTop,
            3 => _pBottom,
            4 => _pNear,
            5 => _pFar
        };
    }

    /// <summary>
    ///     Returns the 8 corners of the frustum, element0 is Near1 (near right down corner)
    ///     element1 is Near2 (near right top corner), element2 is Near3 (near Left top corner), element3 is Near4 (near Left
    ///     down corner), element4 is Far1 (far right down corner),
    ///     element5 is Far2 (far right top corner), element6 is Far3 (far left top corner), element7 is Far4 (far left down corner)
    /// </summary>
    /// <returns> The 8 corners of the frustum. </returns>
    public readonly Vector3[] GetCorners()
    {
        Vector3[] corners = new Vector3[8];
        GetCorners(corners);
        return corners;
    }

    /// <summary>
    ///     Returns the 8 corners of the frustum, element0 is Near1 (near right down corner)
    ///     element1 is Near2 (near right top corner), element2 is Near3 (near Left top corner), element3 is Near4 (near Left
    ///     down corner), element4 is Far1 (far right down corner),
    ///     element5 is Far2 (far right top corner), element6 is Far3 (far left top corner), element7 is Far4 (far left down corner)
    /// </summary>
    /// <param name="corners"> The corners. </param>
    public readonly void GetCorners(Vector3[] corners)
    {
        corners[0] = Get3PlanesInterPoint(in _pNear, in _pBottom, in _pRight); //Near1
        corners[1] = Get3PlanesInterPoint(in _pNear, in _pTop,    in _pRight); //Near2
        corners[2] = Get3PlanesInterPoint(in _pNear, in _pTop,    in _pLeft);  //Near3
        corners[3] = Get3PlanesInterPoint(in _pNear, in _pBottom, in _pLeft);  //Near3
        corners[4] = Get3PlanesInterPoint(in _pFar,  in _pBottom, in _pRight); //Far1
        corners[5] = Get3PlanesInterPoint(in _pFar,  in _pTop,    in _pRight); //Far2
        corners[6] = Get3PlanesInterPoint(in _pFar,  in _pTop,    in _pLeft);  //Far3
        corners[7] = Get3PlanesInterPoint(in _pFar,  in _pBottom, in _pLeft);  //Far3
    }

    /// <summary> Extracts perspective camera parameters from the frustum, doesn't work with orthographic frustums. </summary>
    /// <param name="position"> [out] The position. </param>
    /// <param name="lookAt"> [out] The look at. </param>
    /// <param name="up"> [out] The up. </param>
    /// <param name="fov"> [out] The fov. </param>
    /// <param name="aspectRatio"> [out] The aspect ratio. </param>
    /// <param name="zNear"> [out] The zNear. </param>
    /// <param name="zFar"> [out] The zFar. </param>
    /// ###
    /// <returns> Perspective camera parameters from the frustum. </returns>
    public readonly void GetCameraParams(out Vector3 position, out Vector3 lookAt, out Vector3 up, out float fov, out float aspectRatio, out float zNear, out float zFar)
    {
        Vector3[] corners = GetCorners();
        position    = Get3PlanesInterPoint(in _pRight, in _pTop, in _pLeft);
        lookAt      = _pNear.Normal;
        up          = Vector3.Normalize(Vector3.Cross(_pRight.Normal, _pNear.Normal));
        fov         = (Math2.PI_OVER_TWO - MathF.Acos(Vector3.Dot(_pNear.Normal, _pTop.Normal))) * 2;
        aspectRatio = (corners[6] - corners[5]).Length()                                         / (corners[4] - corners[5]).Length();
        zNear       = (position + (_pNear.Normal * _pNear.D)).Length();
        zFar        = (position + (_pFar.Normal  * _pFar.D)).Length();
    }

    /// <summary> Checks whether a point lay inside, intersects or lay outside the frustum. </summary>
    /// <param name="point"> The point. </param>
    /// <returns> Type of the containment. </returns>
    public readonly ContainmentType Contains(in Vector3 point)
    {
        PlaneIntersectionType result = PlaneIntersectionType.Front;
        fixed (BoundingFrustum* ptr = &this)
        {
            for (int i = 0; i < 6; i++)
            {
                ref readonly Plane    plane       = ref *(((Plane*)ptr) + i);
                PlaneIntersectionType planeResult = Collision.PlaneIntersectsPoint(in plane, in point);
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (planeResult)
                {
                    case PlaneIntersectionType.Back:
                        return ContainmentType.Disjoint;
                    case PlaneIntersectionType.Intersecting:
                        result = PlaneIntersectionType.Intersecting;
                        break;
                }
            }
        }
        return result switch
        {
            PlaneIntersectionType.Intersecting => ContainmentType.Intersects,
            _                                  => ContainmentType.Contains
        };
    }

    /// <summary> Determines the intersection relationship between the frustum and a bounding box. </summary>
    /// <param name="box"> The box. </param>
    /// <returns> Type of the containment. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ContainmentType Contains(in BoundingBox box)
    {
        ContainmentType result = ContainmentType.Contains;
        fixed (BoundingFrustum* ptr = &this)
        {
            for (int i = 0; i < 6; i++)
            {
                ref readonly Plane plane = ref *(((Plane*)ptr) + i);
                GetBoxToPlanePVertexNVertex(in box, in plane.Normal, out Vector3 p, out Vector3 n);
                if (Collision.PlaneIntersectsPoint(in plane, in p) == PlaneIntersectionType.Back)
                {
                    return ContainmentType.Disjoint;
                }
                if (Collision.PlaneIntersectsPoint(in plane, in n) == PlaneIntersectionType.Back)
                {
                    result = ContainmentType.Intersects;
                }
            }
        }
        return result;
    }

    /// <summary> Determines the intersection relationship between the frustum and a bounding box. </summary>
    /// <param name="box"> The box. </param>
    /// <param name="result"> [out] Type of the containment. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Contains(in BoundingBox box, out ContainmentType result)
    {
        result = Contains(in box);
    }

    /// <summary> Determines the intersection relationship between the frustum and a bounding sphere. </summary>
    /// <param name="sphere"> The sphere. </param>
    /// <returns> Type of the containment. </returns>
    public readonly ContainmentType Contains(in BoundingSphere sphere)
    {
        PlaneIntersectionType result = PlaneIntersectionType.Front;
        fixed (BoundingFrustum* ptr = &this)
        {
            for (int i = 0; i < 6; i++)
            {
                ref readonly Plane    plane       = ref *(((Plane*)ptr) + i);
                PlaneIntersectionType planeResult = Collision.PlaneIntersectsSphere(in plane, in sphere);
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (planeResult)
                {
                    case PlaneIntersectionType.Back:
                        return ContainmentType.Disjoint;
                    case PlaneIntersectionType.Intersecting:
                        result = PlaneIntersectionType.Intersecting;
                        break;
                }
            }
        }
        return result switch
        {
            PlaneIntersectionType.Intersecting => ContainmentType.Intersects,
            _                                  => ContainmentType.Contains
        };
    }

    /// <summary> Determines the intersection relationship between the frustum and a bounding sphere. </summary>
    /// <param name="sphere"> The sphere. </param>
    /// <param name="result"> [out] Type of the containment. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Contains(in BoundingSphere sphere, out ContainmentType result)
    {
        result = Contains(in sphere);
    }

    /// <summary> Determines the intersection relationship between the frustum and another bounding frustum. </summary>
    /// <param name="frustum"> The frustum. </param>
    /// <returns> Type of the containment. </returns>
    public readonly ContainmentType Contains(in BoundingFrustum frustum)
    {
        PlaneIntersectionType result = PlaneIntersectionType.Front;
        fixed (BoundingFrustum* ptr = &this)
        {
            for (int i = 0; i < 6; i++)
            {
                ref readonly Plane    plane       = ref *(((Plane*)ptr) + i);
                PlaneIntersectionType planeResult = Intersects(in plane);
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (planeResult)
                {
                    case PlaneIntersectionType.Back:
                        return ContainmentType.Disjoint;
                    case PlaneIntersectionType.Intersecting:
                        result = PlaneIntersectionType.Intersecting;
                        break;
                }
            }
        }
        return result switch
        {
            PlaneIntersectionType.Intersecting => ContainmentType.Intersects,
            _                                  => ContainmentType.Contains
        };
    }

    /// <summary> Determines the intersection relationship between the frustum and another bounding frustum. </summary>
    /// <param name="frustum"> The frustum. </param>
    /// <param name="result"> [out] Type of the containment. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Contains(in BoundingFrustum frustum, out ContainmentType result)
    {
        result = Contains(in frustum);
    }

    /// <summary> Checks whether the current BoundingFrustum intersects a BoundingSphere. </summary>
    /// <param name="sphere"> The sphere. </param>
    /// <returns> Type of the containment. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(in BoundingSphere sphere)
    {
        return Contains(in sphere) != ContainmentType.Disjoint;
    }

    /// <summary> Checks whether the current BoundingFrustum intersects a BoundingSphere. </summary>
    /// <param name="sphere"> The sphere. </param>
    /// <param name="result"> [out] Set to <c> true </c> if the current BoundingFrustum intersects a BoundingSphere. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Intersects(in BoundingSphere sphere, out bool result)
    {
        result = Contains(in sphere) != ContainmentType.Disjoint;
    }

    /// <summary> Checks whether the current BoundingFrustum intersects a BoundingBox. </summary>
    /// <param name="box"> The box. </param>
    /// <returns> <c> true </c> if the current BoundingFrustum intersects a BoundingSphere. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(in BoundingBox box)
    {
        return Contains(in box) != ContainmentType.Disjoint;
    }

    /// <summary> Checks whether the current BoundingFrustum intersects a BoundingBox. </summary>
    /// <param name="box"> The box. </param>
    /// <param name="result"> [out] <c> true </c> if the current BoundingFrustum intersects a BoundingSphere. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Intersects(in BoundingBox box, out bool result)
    {
        result = Contains(in box) != ContainmentType.Disjoint;
    }

    /// <summary> Checks whether the current BoundingFrustum intersects the specified Plane. </summary>
    /// <param name="plane"> The plane. </param>
    /// <returns> Plane intersection type. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly PlaneIntersectionType Intersects(in Plane plane)
    {
        return Collision.PlaneIntersectsPoints(in plane, GetCorners());
    }

    /// <summary> Checks whether the current BoundingFrustum intersects the specified Plane. </summary>
    /// <param name="plane"> The plane. </param>
    /// <param name="result"> [out] Plane intersection type. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Intersects(in Plane plane, out PlaneIntersectionType result)
    {
        result = Collision.PlaneIntersectsPoints(in plane, GetCorners());
    }

    /// <summary> Get the width of the frustum at specified depth. </summary>
    /// <param name="depth"> the depth at which to calculate frustum width. </param>
    /// <returns> With of the frustum at the specified depth. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly float GetWidthAtDepth(float depth)
    {
        return MathF.Tan((Math2.PI_OVER_TWO - MathF.Acos(Vector3.Dot(_pNear.Normal, _pLeft.Normal)))) * depth * 2;
    }

    /// <summary> Get the height of the frustum at specified depth. </summary>
    /// <param name="depth"> the depth at which to calculate frustum height. </param>
    /// <returns> Height of the frustum at the specified depth. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly float GetHeightAtDepth(float depth)
    {
        return MathF.Tan((Math2.PI_OVER_TWO - MathF.Acos(Vector3.Dot(_pNear.Normal, _pTop.Normal)))) * depth * 2;
    }

    /// <summary> Checks whether the current BoundingFrustum intersects the specified Ray. </summary>
    /// <param name="ray"> The ray. </param>
    /// <returns> <c> true </c> if the current BoundingFrustum intersects the specified Ray. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(in Ray ray)
    {
        return Intersects(in ray, out _, out _);
    }

    /// <summary> Checks whether the current BoundingFrustum intersects the specified Ray. </summary>
    /// <param name="ray"> The Ray to check for intersection with. </param>
    /// <param name="inDistance"> [out] The distance at which the ray enters the frustum if there is an intersection and the ray starts outside the frustum. </param>
    /// <param name="outDistance"> [out] The distance at which the ray exits the frustum if there is an intersection. </param>
    /// <returns> <c> true </c> if the current BoundingFrustum intersects the specified Ray. </returns>
    public readonly bool Intersects(in Ray ray, out float? inDistance, out float? outDistance)
    {
        if (Contains(ray.Position) != ContainmentType.Disjoint)
        {
            float nearestPlaneDistance = float.MaxValue;
            fixed (BoundingFrustum* ptr = &this)
            {
                for (int i = 0; i < 6; i++)
                {
                    ref readonly Plane plane = ref *(((Plane*)ptr) + i);
                    if (Collision.RayIntersectsPlane(in ray, in plane, out float distance) && distance < nearestPlaneDistance)
                    {
                        nearestPlaneDistance = distance;
                    }
                }
            }

            inDistance  = nearestPlaneDistance;
            outDistance = null;
            return true;
        }

        //We will find the two points at which the ray enters and exists the frustum
        //These two points make a line which center inside the frustum if the ray intersects it
        //Or outside the frustum if the ray intersects frustum planes outside it.
        float minDist = float.MaxValue;
        float maxDist = float.MinValue;
        fixed (BoundingFrustum* ptr = &this)
        {
            for (int i = 0; i < 6; i++)
            {
                ref readonly Plane plane = ref *(((Plane*)ptr) + i);
                if (Collision.RayIntersectsPlane(in ray, in plane, out float distance))
                {
                    minDist = Math.Min(minDist, distance);
                    maxDist = Math.Max(maxDist, distance);
                }
            }
        }

        Vector3 center = ((ray.Position + ray.Direction * minDist) + (ray.Position + ray.Direction * maxDist)) * 0.5f;
        if (Contains(in center) != ContainmentType.Disjoint)
        {
            inDistance  = minDist;
            outDistance = maxDist;
            return true;
        }

        inDistance  = null;
        outDistance = null;
        return false;
    }

    /// <summary>
    ///     Get the distance which when added to camera position along the lookat direction will do the effect of zoom to extents (zoom to fit) operation,
    ///     so all the passed points will fit in the current view.
    ///     If the returned value is positive, the camera will move toward the lookat direction (ZoomIn).
    ///     If the returned value is negative, the camera will move in the reverse direction of the lookat direction (ZoomOut).
    /// </summary>
    /// <param name="points"> The points. </param>
    /// <returns> The zoom to fit distance. </returns>
    public readonly float GetZoomToExtentsShiftDistance(Vector3[] points)
    {
        float vSin                        = MathF.Sin(((Math2.PI_OVER_TWO - MathF.Acos(Vector3.Dot(_pNear.Normal, _pTop.Normal)))));
        float horizontalToVerticalMapping = vSin / MathF.Sin(((Math2.PI_OVER_TWO - MathF.Acos(Vector3.Dot(_pNear.Normal, _pLeft.Normal)))));

        BoundingFrustum ioFrustrum = GetInsideOutClone();

        float maxPointDist = float.MinValue;
        for (int i = 0; i < points.Length; i++)
        {
            float pointDist = Collision.DistancePlanePoint(in ioFrustrum._pTop, in points[i]);
            pointDist    = Math.Max(pointDist,    Collision.DistancePlanePoint(in ioFrustrum._pBottom, in points[i]));
            pointDist    = Math.Max(pointDist,    Collision.DistancePlanePoint(in ioFrustrum._pLeft,  in points[i]) * horizontalToVerticalMapping);
            pointDist    = Math.Max(pointDist,    Collision.DistancePlanePoint(in ioFrustrum._pRight, in points[i]) * horizontalToVerticalMapping);
            maxPointDist = Math.Max(maxPointDist, pointDist);
        }
        return -maxPointDist / vSin;
    }

    /// <summary>
    ///     Get the distance which when added to camera position along the lookat direction will do the effect of zoom to extents (zoom to fit) operation,
    ///     so all the passed points will fit in the current view.
    ///     If the returned value is positive, the camera will move toward the lookat direction (ZoomIn).
    ///     If the returned value is negative, the camera will move in the reverse direction of the lookat direction (ZoomOut).
    /// </summary>
    /// <param name="boundingBox"> The bounding box. </param>
    /// <returns> The zoom to fit distance. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly float GetZoomToExtentsShiftDistance(in BoundingBox boundingBox)
    {
        return GetZoomToExtentsShiftDistance(boundingBox.GetCorners());
    }

    /// <summary>
    ///     Get the vector shift which when added to camera position will do the effect of zoom to extents (zoom to fit) operation,
    ///     so all the passed points will fit in the current view.
    /// </summary>
    /// <param name="points"> The points. </param>
    /// <returns> The zoom to fit vector. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3 GetZoomToExtentsShiftVector(Vector3[] points)
    {
        return GetZoomToExtentsShiftDistance(points) * _pNear.Normal;
    }

    /// <summary>
    ///     Get the vector shift which when added to camera position will do the effect of zoom to extents (zoom to fit) operation,
    ///     so all the passed points will fit in the current view.
    /// </summary>
    /// <param name="boundingBox"> The bounding box. </param>
    /// <returns> The zoom to fit vector. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector3 GetZoomToExtentsShiftVector(in BoundingBox boundingBox)
    {
        return GetZoomToExtentsShiftDistance(boundingBox.GetCorners()) * _pNear.Normal;
    }

    private static void GetPlanesFromMatrix(in Matrix4x4 matrix, out Plane near, out Plane far, out Plane left, out Plane right, out Plane top, out Plane bottom)
    {
        //http://www.chadvernon.com/blog/resources/directx9/frustum-culling/

        // Left plane
        left.Normal.X = matrix.M14 + matrix.M11;
        left.Normal.Y = matrix.M24 + matrix.M21;
        left.Normal.Z = matrix.M34 + matrix.M31;
        left.D        = matrix.M44 + matrix.M41;
        left          = Plane.Normalize(left);

        // Right plane
        right.Normal.X = matrix.M14 - matrix.M11;
        right.Normal.Y = matrix.M24 - matrix.M21;
        right.Normal.Z = matrix.M34 - matrix.M31;
        right.D        = matrix.M44 - matrix.M41;
        right          = Plane.Normalize(right);

        // Top plane
        top.Normal.X = matrix.M14 - matrix.M12;
        top.Normal.Y = matrix.M24 - matrix.M22;
        top.Normal.Z = matrix.M34 - matrix.M32;
        top.D        = matrix.M44 - matrix.M42;
        top          = Plane.Normalize(top);

        // Bottom plane
        bottom.Normal.X = matrix.M14 + matrix.M12;
        bottom.Normal.Y = matrix.M24 + matrix.M22;
        bottom.Normal.Z = matrix.M34 + matrix.M32;
        bottom.D        = matrix.M44 + matrix.M42;
        bottom          = Plane.Normalize(bottom);

        // Near plane
        near.Normal.X = matrix.M13;
        near.Normal.Y = matrix.M23;
        near.Normal.Z = matrix.M33;
        near.D        = matrix.M43;
        near          = Plane.Normalize(near);

        // Far plane
        far.Normal.X = matrix.M14 - matrix.M13;
        far.Normal.Y = matrix.M24 - matrix.M23;
        far.Normal.Z = matrix.M34 - matrix.M33;
        far.D        = matrix.M44 - matrix.M43;
        far          = Plane.Normalize(far);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector3 Get3PlanesInterPoint(in Plane p1, in Plane p2, in Plane p3)
    {
        //P = -d1 * N2xN3 / N1.N2xN3 - d2 * N3xN1 / N2.N3xN1 - d3 * N1xN2 / N3.N1xN2 
        return -p1.D * Vector3.Cross(p2.Normal, p3.Normal) / Vector3.Dot(p1.Normal, Vector3.Cross(p2.Normal, p3.Normal))
          - p2.D     * Vector3.Cross(p3.Normal, p1.Normal) / Vector3.Dot(p2.Normal, Vector3.Cross(p3.Normal, p1.Normal))
          - p3.D     * Vector3.Cross(p1.Normal, p2.Normal) / Vector3.Dot(p3.Normal, Vector3.Cross(p1.Normal, p2.Normal));
    }

    private static void GetBoxToPlanePVertexNVertex(in BoundingBox box, in Vector3 planeNormal, out Vector3 p, out Vector3 n)
    {
        p = box.Minimum;
        if (planeNormal.X >= 0)
        {
            p.X = box.Maximum.X;
        }
        if (planeNormal.Y >= 0)
        {
            p.Y = box.Maximum.Y;
        }
        if (planeNormal.Z >= 0)
        {
            p.Z = box.Maximum.Z;
        }

        n = box.Maximum;
        if (planeNormal.X >= 0)
        {
            n.X = box.Minimum.X;
        }
        if (planeNormal.Y >= 0)
        {
            n.Y = box.Minimum.Y;
        }
        if (planeNormal.Z >= 0)
        {
            n.Z = box.Minimum.Z;
        }
    }

    private readonly BoundingFrustum GetInsideOutClone()
    {
        BoundingFrustum frustum = this;
        frustum._pNear.Normal   = -frustum._pNear.Normal;
        frustum._pFar.Normal    = -frustum._pFar.Normal;
        frustum._pLeft.Normal   = -frustum._pLeft.Normal;
        frustum._pRight.Normal  = -frustum._pRight.Normal;
        frustum._pTop.Normal    = -frustum._pTop.Normal;
        frustum._pBottom.Normal = -frustum._pBottom.Normal;
        return frustum;
    }

    /// <summary> Implements the operator ==. </summary>
    /// <param name="left"> The left. </param>
    /// <param name="right"> The right. </param>
    /// <returns> The result of the operator. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in BoundingFrustum left, in BoundingFrustum right)
    {
        return left.Equals(in right);
    }

    /// <summary> Implements the operator !=. </summary>
    /// <param name="left"> The left. </param>
    /// <param name="right"> The right. </param>
    /// <returns> The result of the operator. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in BoundingFrustum left, in BoundingFrustum right)
    {
        return !left.Equals(in right);
    }
}