#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Numerics;

namespace Exomia.Framework.Mathematics
{
    /// <summary>
    ///     Contains static methods to help in determining intersections, containment, etc.
    /// </summary>
    public static class Collision
    {
        /// <summary>
        ///     Determines the closest point between a point and a triangle.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="vertex1">The first vertex to test.</param>
        /// <param name="vertex2">The second vertex to test.</param>
        /// <param name="vertex3">The third vertex to test.</param>
        /// <param name="result">When the method completes, contains the closest point between the two objects.</param>
        public static void ClosestPointPointTriangle(in  Vector3 point,
                                                     in  Vector3 vertex1,
                                                     in  Vector3 vertex2,
                                                     in  Vector3 vertex3,
                                                     out Vector3 result)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 136

            //Check if P in vertex region outside A
            Vector3 ab = vertex2 - vertex1;
            Vector3 ac = vertex3 - vertex1;
            Vector3 ap = point - vertex1;

            float d1 = Vector3.Dot(ab, ap);
            float d2 = Vector3.Dot(ac, ap);
            if (d1 <= 0.0f && d2 <= 0.0f)
            {
                result = vertex1; //Barycentric coordinates (1,0,0)
                return;
            }

            //Check if P in vertex region outside B
            Vector3 bp = point - vertex2;
            float   d3 = Vector3.Dot(ab, bp);
            float   d4 = Vector3.Dot(ac, bp);
            if (d3 >= 0.0f && d4 <= d3)
            {
                result = vertex2; // Barycentric coordinates (0,1,0)
                return;
            }

            //Check if P in edge region of AB, if so return projection of P onto AB
            float vc = d1 * d4 - d3 * d2;
            if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
            {
                float v = d1 / (d1 - d3);
                result = vertex1 + v * ab; //Barycentric coordinates (1-v,v,0)
                return;
            }

            //Check if P in vertex region outside C
            Vector3 cp = point - vertex3;
            float   d5 = Vector3.Dot(ab, cp);
            float   d6 = Vector3.Dot(ac, cp);
            if (d6 >= 0.0f && d5 <= d6)
            {
                result = vertex3; //Barycentric coordinates (0,0,1)
                return;
            }

            //Check if P in edge region of AC, if so return projection of P onto AC
            float vb = d5 * d2 - d1 * d6;
            if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
            {
                float w = d2 / (d2 - d6);
                result = vertex1 + w * ac; //Barycentric coordinates (1-w,0,w)
                return;
            }

            //Check if P in edge region of BC, if so return projection of P onto BC
            float va = d3 * d6 - d5 * d4;
            if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
            {
                float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
                result = vertex2 + w * (vertex3 - vertex2); //Barycentric coordinates (0,1-w,w)
                return;
            }

            //P inside face region. Compute Q through its Barycentric coordinates (u,v,w)
            float denom = 1.0f / (va + vb + vc);
            float v2    = vb * denom;
            float w2    = vc * denom;
            result = vertex1 + ab * v2 + ac * w2; //= u*vertex1 + v*vertex2 + w*vertex3, u = va * denom = 1.0f - v - w
        }

        /// <summary>
        ///     Determines the closest point between a <see cin="Plane" /> and a point.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="point">The point to test.</param>
        /// <param name="result">When the method completes, contains the closest point between the two objects.</param>
        public static void ClosestPointPlanePoint(in Plane plane, in Vector3 point, out Vector3 result)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 126

            result = point - ((Vector3.Dot(plane.Normal, point) - plane.D) * plane.Normal);
        }

        /// <summary>
        ///     Determines the closest point between a <see cin="BoundingBox" /> and a point.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <param name="point">The point to test.</param>
        /// <param name="result">When the method completes, contains the closest point between the two objects.</param>
        public static void ClosestPointBoxPoint(in BoundingBox box, in Vector3 point, out Vector3 result)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 130

            result = Vector3.Min(Vector3.Max(point, box.Minimum), box.Maximum);
        }

        /// <summary>
        ///     Determines the closest point between a <see cin="BoundingSphere" /> and a point.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="point">The point to test.</param>
        /// <param name="result">
        ///     When the method completes, contains the closest point between the two objects;
        ///     or, if the point is directly in the center of the sphere, contains <see cin="Vector3.Zero" />.
        /// </param>
        public static void ClosestPointSpherePoint(in BoundingSphere sphere, in Vector3 point, out Vector3 result)
        {
            result = (Vector3.Normalize(Vector3.Subtract(point, sphere.Center)) * sphere.Radius) + sphere.Center;
        }

        /// <summary>
        ///     Determines the closest point between a <see cin="BoundingSphere" /> and a <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="sphere1">The first sphere to test.</param>
        /// <param name="sphere2">The second sphere to test.</param>
        /// <param name="result">
        ///     When the method completes, contains the closest point between the two objects;
        ///     or, if the point is directly in the center of the sphere, contains <see cin="Vector3.Zero" />.
        /// </param>
        /// <remarks>
        ///     If the two spheres are overlapping, but not directly on top of each other, the closest point
        ///     is the 'closest' point of intersection. This can also be considered is the deepest point of
        ///     intersection.
        /// </remarks>
        public static void ClosestPointSphereSphere(in  BoundingSphere sphere1,
                                                    in  BoundingSphere sphere2,
                                                    out Vector3        result)
        {
            result = (Vector3.Normalize(Vector3.Subtract(sphere2.Center, sphere1.Center)) * sphere1.Radius) + sphere1.Center;
        }

        /// <summary>
        ///     Determines the distance between a <see cin="Plane" /> and a point.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="point">The point to test.</param>
        /// <returns>The distance between the two objects.</returns>
        public static float DistancePlanePoint(in Plane plane, in Vector3 point)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 127
            return Vector3.Dot(plane.Normal, point) - plane.D;
        }

        /// <summary>
        ///     Determines the distance between a <see cin="BoundingBox" /> and a point.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <param name="point">The point to test.</param>
        /// <returns>The distance between the two objects.</returns>
        public static float DistanceBoxPoint(in BoundingBox box, in Vector3 point)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 131

            float distance = 0f;

            if (point.X < box.Minimum.X)
            {
                distance += (box.Minimum.X - point.X) * (box.Minimum.X - point.X);
            }

            if (point.X > box.Maximum.X)
            {
                distance += (point.X - box.Maximum.X) * (point.X - box.Maximum.X);
            }

            if (point.Y < box.Minimum.Y)
            {
                distance += (box.Minimum.Y - point.Y) * (box.Minimum.Y - point.Y);
            }

            if (point.Y > box.Maximum.Y)
            {
                distance += (point.Y - box.Maximum.Y) * (point.Y - box.Maximum.Y);
            }

            if (point.Z < box.Minimum.Z)
            {
                distance += (box.Minimum.Z - point.Z) * (box.Minimum.Z - point.Z);
            }

            if (point.Z > box.Maximum.Z)
            {
                distance += (point.Z - box.Maximum.Z) * (point.Z - box.Maximum.Z);
            }

            return MathF.Sqrt(distance);
        }

        /// <summary>
        ///     Determines the distance between a <see cin="BoundingBox" /> and a <see cin="BoundingBox" />.
        /// </summary>
        /// <param name="box1">The first box to test.</param>
        /// <param name="box2">The second box to test.</param>
        /// <returns>The distance between the two objects.</returns>
        public static float DistanceBoxBox(in BoundingBox box1, in BoundingBox box2)
        {
            float distance = 0f;

            //Distance for X.
            if (box1.Minimum.X > box2.Maximum.X)
            {
                float delta = box2.Maximum.X - box1.Minimum.X;
                distance += delta * delta;
            }
            else if (box2.Minimum.X > box1.Maximum.X)
            {
                float delta = box1.Maximum.X - box2.Minimum.X;
                distance += delta * delta;
            }

            //Distance for Y.
            if (box1.Minimum.Y > box2.Maximum.Y)
            {
                float delta = box2.Maximum.Y - box1.Minimum.Y;
                distance += delta * delta;
            }
            else if (box2.Minimum.Y > box1.Maximum.Y)
            {
                float delta = box1.Maximum.Y - box2.Minimum.Y;
                distance += delta * delta;
            }

            //Distance for Z.
            if (box1.Minimum.Z > box2.Maximum.Z)
            {
                float delta = box2.Maximum.Z - box1.Minimum.Z;
                distance += delta * delta;
            }
            else if (box2.Minimum.Z > box1.Maximum.Z)
            {
                float delta = box1.Maximum.Z - box2.Minimum.Z;
                distance += delta * delta;
            }

            return MathF.Sqrt(distance);
        }

        /// <summary>
        ///     Determines the distance between a <see cin="BoundingSphere" /> and a point.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="point">The point to test.</param>
        /// <returns>The distance between the two objects.</returns>
        public static float DistanceSpherePoint(in BoundingSphere sphere, in Vector3 point)
        {
            return Math.Max(Vector3.Distance(sphere.Center, point) - sphere.Radius, 0f);
        }

        /// <summary>
        ///     Determines the distance between a <see cin="BoundingSphere" /> and a <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="sphere1">The first sphere to test.</param>
        /// <param name="sphere2">The second sphere to test.</param>
        /// <returns>The distance between the two objects.</returns>
        public static float DistanceSphereSphere(in BoundingSphere sphere1, in BoundingSphere sphere2)
        {
            return Math.Max(Vector3.Distance(sphere1.Center, sphere2.Center) - (sphere1.Radius + sphere2.Radius), 0f);
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a point.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="point">The point to test.</param>
        /// <returns>Whether the two objects intersect.</returns>
        public static bool RayIntersectsPoint(in Ray ray, in Vector3 point)
        {
            Vector3 m = Vector3.Subtract(ray.Position, point);
            float   b = Vector3.Dot(m, ray.Direction);
            float   c = Vector3.Dot(m, m) - Math2.ZERO_TOLERANCE;

            if (c > 0f && b > 0f)
            {
                return false;
            }
            return b * b - c >= 0f;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a <see cin="Ray" />.
        /// </summary>
        /// <param name="ray1">The first ray to test.</param>
        /// <param name="ray2">The second ray to test.</param>
        /// <param name="point">
        ///     When the method completes, contains the point of intersection,
        ///     or <see cin="Vector3.Zero" /> if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersect.</returns>
        /// <remarks>
        ///     This method performs a ray vs ray intersection test based on the following formula
        ///     from Goldman.
        ///     <code>s = det([o_2 - o_1, d_2, d_1 x d_2]) / ||d_1 x d_2||^2</code>
        ///     <code>t = det([o_2 - o_1, d_1, d_1 x d_2]) / ||d_1 x d_2||^2</code>
        ///     Where o_1 is the position of the first ray, o_2 is the position of the second ray,
        ///     d_1 is the normalized direction of the first ray, d_2 is the normalized direction
        ///     of the second ray, det denotes the determinant of a matrix, x denotes the cross
        ///     product, [ ] denotes a matrix, and || || denotes the length or magnitude of a vector.
        /// </remarks>
        public static bool RayIntersectsRay(in Ray ray1, in Ray ray2, out Vector3 point)
        {
            //Source: Real-Time Rendering, Third Edition
            //inerence: Page 780

            Vector3 cross       = Vector3.Cross(ray1.Direction, ray2.Direction);
            float   denominator = cross.Length();

            //Lines are parallel.
            if (Math2.IsZero(denominator))
            {
                //Lines are parallel and on top of each other.
                if (Math2.NearEqual(ray2.Position.X, ray1.Position.X) &&
                    Math2.NearEqual(ray2.Position.Y, ray1.Position.Y) &&
                    Math2.NearEqual(ray2.Position.Z, ray1.Position.Z))
                {
                    point = Vector3.Zero;
                    return true;
                }
            }

            //3x3 matrix for the first ray.
            float m11 = ray2.Position.X - ray1.Position.X;
            float m12 = ray2.Position.Y - ray1.Position.Y;
            float m13 = ray2.Position.Z - ray1.Position.Z;
            float m21 = ray2.Direction.X;
            float m22 = ray2.Direction.Y;
            float m23 = ray2.Direction.Z;
            float m31 = cross.X;
            float m32 = cross.Y;
            float m33 = cross.Z;

            //Determinant of first matrix.
            float dets =
                m11 * m22 * m33 +
                m12 * m23 * m31 +
                m13 * m21 * m32 -
                m11 * m23 * m32 -
                m12 * m21 * m33 -
                m13 * m22 * m31;

            //3x3 matrix for the second ray.
            m21 = ray1.Direction.X;
            m22 = ray1.Direction.Y;
            m23 = ray1.Direction.Z;

            denominator *= denominator;
            //t values of the point of intersection.
            float s = dets / denominator;
            float t = (m11 * m22 * m33 +
                       m12 * m23 * m31 +
                       m13 * m21 * m32 -
                       m11 * m23 * m32 -
                       m12 * m21 * m33 -
                       m13 * m22 * m31) / denominator;

            //The points of intersection.
            point = ray1.Position + (s * ray1.Direction);
            Vector3 point2 = ray2.Position + (t * ray2.Direction);

            //If the points are not equal, no intersection has occurred.
            if (Math2.NearEqual(point2.X, point.X) &&
                Math2.NearEqual(point2.Y, point.Y) &&
                Math2.NearEqual(point2.Z, point.Z))
            {
                return true;
            }

            point = Vector3.Zero;
            return false;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a <see cin="Plane" />.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="plane">The plane to test.</param>
        /// <param name="distance">
        ///     When the method completes, contains the distance of the intersection,
        ///     or 0 if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersect.</returns>
        public static bool RayIntersectsPlane(in Ray ray, in Plane plane, out float distance)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 175

            float direction = Vector3.Dot(plane.Normal, ray.Direction);

            if (Math2.IsNotZero(direction))
            {
                distance = (-plane.D - Vector3.Dot(plane.Normal, ray.Position)) / direction;

                if (distance < 0f)
                {
                    distance = 0f;
                    return false;
                }

                return true;
            }

            distance = 0f;
            return false;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a <see cin="Plane" />.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="plane">The plane to test</param>
        /// <param name="point">
        ///     When the method completes, contains the point of intersection,
        ///     or <see cin="Vector3.Zero" /> if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool RayIntersectsPlane(in Ray ray, in Plane plane, out Vector3 point)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 175

            if (RayIntersectsPlane(in ray, in plane, out float distance))
            {
                point = ray.Position + (ray.Direction * distance);
                return true;
            }

            point = Vector3.Zero;
            return false;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a triangle.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <param name="distance">
        ///     When the method completes, contains the distance of the intersection,
        ///     or 0 if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersected.</returns>
        /// <remarks>
        ///     This method tests if the ray intersects either the front or back of the triangle.
        ///     If the ray is parallel to the triangle's plane, no intersection is assumed to have
        ///     happened. If the intersection of the ray and the triangle is behind the origin of
        ///     the ray, no intersection is assumed to have happened. In both cases of assumptions,
        ///     this method returns false.
        /// </remarks>
        public static bool RayIntersectsTriangle(in  Ray     ray,
                                                 in  Vector3 vertex1,
                                                 in  Vector3 vertex2,
                                                 in  Vector3 vertex3,
                                                 out float   distance)
        {
            //Source: Fast Minimum Storage Ray / Triangle Intersection
            //inerence: http://www.cs.virginia.edu/~gfx/Courses/2003/ImageSynthesis/papers/Acceleration/Fast%20MinimumStorage%20RayTriangle%20Intersection.pdf

            Vector3 edge1 = vertex2 - vertex1;
            Vector3 edge2 = vertex3 - vertex1;

            Vector3 directioncrossedge2 = Vector3.Cross(ray.Direction, edge2);
            float   dets                = Vector3.Dot(edge1, directioncrossedge2);

            //If the ray is parallel to the triangle plane, there is no collision.
            //This also means that we are not culling, the ray may hit both the
            //back and the front of the triangle.
            if (Math2.IsZero(dets))
            {
                distance = 0f;
                return false;
            }

            float idets = 1.0f / dets;

            //Calculate the U parameter of the intersection point.
            Vector3 distanceVector = ray.Position - vertex1;

            float triangleU = Vector3.Dot(distanceVector, directioncrossedge2);
            triangleU *= idets;

            //Make sure it is inside the triangle.
            if (triangleU < 0f || triangleU > 1f)
            {
                distance = 0f;
                return false;
            }

            //Calculate the V parameter of the intersection point.
            Vector3 distancecrossedge1 = Vector3.Cross(distanceVector, edge1);
            float   triangleV          = Vector3.Dot(ray.Direction, distancecrossedge1) * idets;

            //Make sure it is inside the triangle.
            if (triangleV < 0f || triangleU + triangleV > 1f)
            {
                distance = 0f;
                return false;
            }

            //Compute the distance along the ray to the triangle.
            distance = Vector3.Dot(edge2, distancecrossedge1) * idets;

            //Is the triangle behind the ray origin?
            if (distance < 0f)
            {
                distance = 0f;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a triangle.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <param name="point">
        ///     When the method completes, contains the point of intersection,
        ///     or <see cin="Vector3.Zero" /> if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool RayIntersectsTriangle(in  Ray     ray,
                                                 in  Vector3 vertex1,
                                                 in  Vector3 vertex2,
                                                 in  Vector3 vertex3,
                                                 out Vector3 point)
        {
            if (RayIntersectsTriangle(in ray, in vertex1, in vertex2, in vertex3, out float distance))
            {
                point = ray.Position + (ray.Direction * distance);
                return true;
            }

            point = Vector3.Zero;
            return false;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a <see cin="BoundingBox" />.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="box">The box to test.</param>
        /// <param name="distance">
        ///     When the method completes, contains the distance of the intersection,
        ///     or 0 if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool RayIntersectsBox(in Ray ray, in BoundingBox box, out float distance)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 179

            distance = 0.0f;
            float tmax = float.MaxValue;

            if (Math2.IsZero(ray.Direction.X))
            {
                if (ray.Position.X < box.Minimum.X || ray.Position.X > box.Maximum.X)
                {
                    distance = 0f;
                    return false;
                }
            }
            else
            {
                float inverse = 1.0f / ray.Direction.X;
                float t1      = (box.Minimum.X - ray.Position.X) * inverse;
                float t2      = (box.Maximum.X - ray.Position.X) * inverse;

                if (t1 > t2)
                {
                    float temp = t1;
                    t1 = t2;
                    t2 = temp;
                }
                distance = Math.Max(t1, distance);
                tmax     = Math.Min(t2, tmax);

                if (distance > tmax)
                {
                    distance = 0f;
                    return false;
                }
            }

            if (Math2.IsZero(ray.Direction.Y))
            {
                if (ray.Position.Y < box.Minimum.Y || ray.Position.Y > box.Maximum.Y)
                {
                    distance = 0f;
                    return false;
                }
            }
            else
            {
                float inverse = 1.0f / ray.Direction.Y;
                float t1      = (box.Minimum.Y - ray.Position.Y) * inverse;
                float t2      = (box.Maximum.Y - ray.Position.Y) * inverse;

                if (t1 > t2)
                {
                    float temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                distance = Math.Max(t1, distance);
                tmax     = Math.Min(t2, tmax);

                if (distance > tmax)
                {
                    distance = 0f;
                    return false;
                }
            }

            if (Math2.IsZero(ray.Direction.Z))
            {
                if (ray.Position.Z < box.Minimum.Z || ray.Position.Z > box.Maximum.Z)
                {
                    distance = 0f;
                    return false;
                }
            }
            else
            {
                float inverse = 1.0f / ray.Direction.Z;
                float t1      = (box.Minimum.Z - ray.Position.Z) * inverse;
                float t2      = (box.Maximum.Z - ray.Position.Z) * inverse;

                if (t1 > t2)
                {
                    float temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                distance = Math.Max(t1, distance);
                tmax     = Math.Min(t2, tmax);

                if (distance > tmax)
                {
                    distance = 0f;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a <see cin="Plane" />.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="box">The box to test.</param>
        /// <param name="point">
        ///     When the method completes, contains the point of intersection,
        ///     or <see cin="Vector3.Zero" /> if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool RayIntersectsBox(in Ray ray, in BoundingBox box, out Vector3 point)
        {
            if (RayIntersectsBox(in ray, in box, out float distance))
            {
                point = ray.Position + (ray.Direction * distance);
                return true;
            }

            point = Vector3.Zero;
            return false;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="distance">
        ///     When the method completes, contains the distance of the intersection,
        ///     or 0 if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool RayIntersectsSphere(in Ray ray, in BoundingSphere sphere, out float distance)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 177

            Vector3 m = Vector3.Subtract(ray.Position, sphere.Center);

            float b = Vector3.Dot(m, ray.Direction);
            float c = Vector3.Dot(m, m) - (sphere.Radius * sphere.Radius);

            if (c > 0f && b > 0f)
            {
                distance = 0f;
                return false;
            }

            float discriminant = b * b - c;

            if (discriminant < 0f)
            {
                distance = 0f;
                return false;
            }

            distance = -b - MathF.Sqrt(discriminant);

            if (distance < 0f)
            {
                distance = 0f;
            }

            return true;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Ray" /> and a <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="ray">The ray to test.</param>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="point">
        ///     When the method completes, contains the point of intersection,
        ///     or <see cin="Vector3.Zero" /> if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool RayIntersectsSphere(in Ray ray, in BoundingSphere sphere, out Vector3 point)
        {
            if (RayIntersectsSphere(in ray, in sphere, out float distance))
            {
                point = ray.Position + (ray.Direction * distance);
                return true;
            }

            point = Vector3.Zero;
            return false;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Plane" /> and a point.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="point">The point to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static PlaneIntersectionType PlaneIntersectsPoint(in Plane plane, in Vector3 point)
        {
            float distance = Vector3.Dot(plane.Normal, point) + plane.D;

            if (distance > 0f)
            {
                return PlaneIntersectionType.Front;
            }

            return distance < 0f
                ? PlaneIntersectionType.Back
                : PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Plane" /> and a <see cin="Plane" />.
        /// </summary>
        /// <param name="plane1">The first plane to test.</param>
        /// <param name="plane2">The second plane to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool PlaneIntersectsPlane(in Plane plane1, in Plane plane2)
        {
            Vector3 direction = Vector3.Cross(plane1.Normal, plane2.Normal);
            return Math2.IsNotZero(Vector3.Dot(direction, direction));
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Plane" /> and a <see cin="Plane" />.
        /// </summary>
        /// <param name="plane1">The first plane to test.</param>
        /// <param name="plane2">The second plane to test.</param>
        /// <param name="line">
        ///     When the method completes, contains the line of intersection
        ///     as a <see cin="Ray" />, or a zero ray if there was no intersection.
        /// </param>
        /// <returns>Whether the two objects intersected.</returns>
        /// <remarks>
        ///     Although a ray is set to have an origin, the ray returned by this method is really
        ///     a line in three dimensions which has no real origin. The ray is considered valid when
        ///     both the positive direction is used and when the negative direction is used.
        /// </remarks>
        public static bool PlaneIntersectsPlane(in Plane plane1, in Plane plane2, out Ray line)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 207

            Vector3 direction = Vector3.Cross(plane1.Normal, plane2.Normal);
            if (Math2.IsNotZero(Vector3.Dot(direction, direction)))
            {
                line.Position  = Vector3.Cross(plane1.D * plane2.Normal - plane2.D * plane1.Normal, direction);
                line.Direction = Vector3.Normalize(direction);

                return true;
            }

            line = Ray.Zero;
            return false;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Plane" /> and a triangle.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static PlaneIntersectionType PlaneIntersectsTriangle(in Plane   plane,
                                                                    in Vector3 vertex1,
                                                                    in Vector3 vertex2,
                                                                    in Vector3 vertex3)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 207

            PlaneIntersectionType test1 = PlaneIntersectsPoint(in plane, in vertex1);
            PlaneIntersectionType test2 = PlaneIntersectsPoint(in plane, in vertex2);
            PlaneIntersectionType test3 = PlaneIntersectsPoint(in plane, in vertex3);

            if (test1 == PlaneIntersectionType.Front &&
                test2 == PlaneIntersectionType.Front &&
                test3 == PlaneIntersectionType.Front)
            {
                return PlaneIntersectionType.Front;
            }

            if (test1 == PlaneIntersectionType.Back &&
                test2 == PlaneIntersectionType.Back &&
                test3 == PlaneIntersectionType.Back)
            {
                return PlaneIntersectionType.Back;
            }

            return PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Plane" /> and a <see cin="BoundingBox" />.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="box">The box to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static PlaneIntersectionType PlaneIntersectsBox(in Plane plane, in BoundingBox box)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 161

            Vector3 max;
            max.X = (plane.Normal.X >= 0.0f)
                ? box.Minimum.X
                : box.Maximum.X;
            max.Y = (plane.Normal.Y >= 0.0f)
                ? box.Minimum.Y
                : box.Maximum.Y;
            max.Z = (plane.Normal.Z >= 0.0f)
                ? box.Minimum.Z
                : box.Maximum.Z;

            Vector3 min;
            min.X = (plane.Normal.X >= 0.0f)
                ? box.Maximum.X
                : box.Minimum.X;
            min.Y = (plane.Normal.Y >= 0.0f)
                ? box.Maximum.Y
                : box.Minimum.Y;
            min.Z = (plane.Normal.Z >= 0.0f)
                ? box.Maximum.Z
                : box.Minimum.Z;

            if (Vector3.Dot(plane.Normal, max) + plane.D > 0.0f)
            {
                return PlaneIntersectionType.Front;
            }

            if (Vector3.Dot(plane.Normal, min) + plane.D < 0.0f)
            {
                return PlaneIntersectionType.Back;
            }

            return PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="Plane" /> and a <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static PlaneIntersectionType PlaneIntersectsSphere(in Plane plane, in BoundingSphere sphere)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 160

            float distance = Vector3.Dot(plane.Normal, sphere.Center) + plane.D;

            if (distance > sphere.Radius)
            {
                return PlaneIntersectionType.Front;
            }

            if (distance < -sphere.Radius)
            {
                return PlaneIntersectionType.Back;
            }

            return PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="BoundingBox" /> and a <see cin="BoundingBox" />.
        /// </summary>
        /// <param name="box1">The first box to test.</param>
        /// <param name="box2">The second box to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool BoxIntersectsBox(in BoundingBox box1, in BoundingBox box2)
        {
            if (box1.Minimum.X > box2.Maximum.X || box2.Minimum.X > box1.Maximum.X)
            {
                return false;
            }

            if (box1.Minimum.Y > box2.Maximum.Y || box2.Minimum.Y > box1.Maximum.Y)
            {
                return false;
            }

            if (box1.Minimum.Z > box2.Maximum.Z || box2.Minimum.Z > box1.Maximum.Z)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="BoundingBox" /> and a <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool BoxIntersectsSphere(in BoundingBox box, in BoundingSphere sphere)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 166

            return Vector3.DistanceSquared(sphere.Center,
                Vector3.Clamp(sphere.Center, box.Minimum, box.Maximum)) <= sphere.Radius * sphere.Radius;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="BoundingSphere" /> and a triangle.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool SphereIntersectsTriangle(in BoundingSphere sphere,
                                                    in Vector3        vertex1,
                                                    in Vector3        vertex2,
                                                    in Vector3        vertex3)
        {
            //Source: Real-Time Collision Detection by Christer Ericson
            //inerence: Page 167

            ClosestPointPointTriangle(in sphere.Center, in vertex1, in vertex2, in vertex3, out Vector3 point);
            Vector3 v = point - sphere.Center;
            return Vector3.Dot(v, v) <= sphere.Radius * sphere.Radius;
        }

        /// <summary>
        ///     Determines whether there is an intersection between a <see cin="BoundingSphere" /> and a
        ///     <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="sphere1">First sphere to test.</param>
        /// <param name="sphere2">Second sphere to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public static bool SphereIntersectsSphere(in BoundingSphere sphere1, in BoundingSphere sphere2)
        {
            float radiisum = sphere1.Radius + sphere2.Radius;
            return Vector3.DistanceSquared(sphere1.Center, sphere2.Center) <= radiisum * radiisum;
        }

        /// <summary> Determines whether a <see cin="BoundingBox" /> contains a point. </summary>
        /// <param name="box">   The box to test. </param>
        /// <param name="point"> The point to test. </param>
        /// <returns> True if the box contains the point; false otherwise. </returns>
        public static bool BoxContainsPoint(in BoundingBox box, in Vector3 point)
        {
            return box.Minimum.X <= point.X && box.Maximum.X >= point.X &&
                   box.Minimum.Y <= point.Y && box.Maximum.Y >= point.Y &&
                   box.Minimum.Z <= point.Z && box.Maximum.Z >= point.Z;
        }

        /// <summary>
        ///     Determines whether a <see cin="BoundingBox" /> contains a <see cin="BoundingBox" />.
        /// </summary>
        /// <param name="box1">The first box to test.</param>
        /// <param name="box2">The second box to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public static ContainmentType BoxContainsBox(in BoundingBox box1, in BoundingBox box2)
        {
            if (box1.Maximum.X < box2.Minimum.X ||
                box1.Minimum.X > box2.Maximum.X ||
                box1.Maximum.Y < box2.Minimum.Y ||
                box1.Minimum.Y > box2.Maximum.Y ||
                box1.Maximum.Z < box2.Minimum.Z ||
                box1.Minimum.Z > box2.Maximum.Z)
            {
                return ContainmentType.Disjoint;
            }

            if (box1.Minimum.X <= box2.Minimum.X &&
                box2.Maximum.X <= box1.Maximum.X &&
                box1.Minimum.Y <= box2.Minimum.Y &&
                box2.Maximum.Y <= box1.Maximum.Y &&
                box1.Minimum.Z <= box2.Minimum.Z &&
                box2.Maximum.Z <= box1.Maximum.Z)
            {
                return ContainmentType.Contains;
            }

            return ContainmentType.Intersects;
        }

        /// <summary>
        ///     Determines whether a <see cin="BoundingBox" /> contains a <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="box">The box to test.</param>
        /// <param name="sphere">The sphere to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public static ContainmentType BoxContainsSphere(in BoundingBox box, in BoundingSphere sphere)
        {
            if (Vector3.DistanceSquared(sphere.Center, Vector3.Clamp(sphere.Center, box.Minimum, box.Maximum)) > sphere.Radius * sphere.Radius)
            {
                return ContainmentType.Disjoint;
            }

            if ((((box.Minimum.X + sphere.Radius <= sphere.Center.X) &&
                  (sphere.Center.X <= box.Maximum.X - sphere.Radius)) &&
                 ((box.Maximum.X - box.Minimum.X > sphere.Radius) &&
                  (box.Minimum.Y + sphere.Radius <= sphere.Center.Y))) &&
                (((sphere.Center.Y <= box.Maximum.Y - sphere.Radius) &&
                  (box.Maximum.Y - box.Minimum.Y > sphere.Radius)) &&
                 (((box.Minimum.Z + sphere.Radius <= sphere.Center.Z) &&
                   (sphere.Center.Z <= box.Maximum.Z - sphere.Radius)) &&
                  (box.Maximum.Z - box.Minimum.Z > sphere.Radius))))
            {
                return ContainmentType.Contains;
            }

            return ContainmentType.Intersects;
        }

        /// <summary> Determines whether a <see cin="BoundingSphere" /> contains a point. </summary>
        /// <param name="sphere"> The sphere to test. </param>
        /// <param name="point">  The point to test. </param>
        /// <returns> True if the sphere contains the point; false otherwise. </returns>
        public static bool SphereContainsPoint(in BoundingSphere sphere, in Vector3 point)
        {
            return Vector3.DistanceSquared(point, sphere.Center) <= sphere.Radius * sphere.Radius;
        }

        /// <summary>
        ///     Determines whether a <see cin="BoundingSphere" /> contains a triangle.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="vertex1">The first vertex of the triangle to test.</param>
        /// <param name="vertex2">The second vertex of the triangle to test.</param>
        /// <param name="vertex3">The third vertex of the triangle to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public static ContainmentType SphereContainsTriangle(in BoundingSphere sphere,
                                                             in Vector3        vertex1,
                                                             in Vector3        vertex2,
                                                             in Vector3        vertex3)
        {
            //Source: Jorgy343
            //inerence: None

            if (SphereContainsPoint(in sphere, in vertex1) &&
                SphereContainsPoint(in sphere, in vertex2) &&
                SphereContainsPoint(in sphere, in vertex3))
            {
                return ContainmentType.Contains;
            }

            if (SphereIntersectsTriangle(in sphere, in vertex1, in vertex2, in vertex3))
            {
                return ContainmentType.Intersects;
            }

            return ContainmentType.Disjoint;
        }

        /// <summary>
        ///     Determines whether a <see cin="BoundingSphere" /> contains a <see cin="BoundingBox" />.
        /// </summary>
        /// <param name="sphere">The sphere to test.</param>
        /// <param name="box">The box to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public static ContainmentType SphereContainsBox(in BoundingSphere sphere, in BoundingBox box)
        {
            if (!BoxIntersectsSphere(in box, in sphere))
            {
                return ContainmentType.Disjoint;
            }

            float radiussquared = sphere.Radius * sphere.Radius;

            Vector3 vector;
            vector.X = sphere.Center.X - box.Minimum.X;
            vector.Y = sphere.Center.Y - box.Minimum.Y;
            vector.Z = sphere.Center.Z - box.Minimum.Z;

            if (vector.LengthSquared() > radiussquared)
            {
                return ContainmentType.Intersects;
            }

            vector.X = sphere.Center.X - box.Maximum.X;
            vector.Y = sphere.Center.Y - box.Maximum.Y;
            vector.Z = sphere.Center.Z - box.Maximum.Z;

            if (vector.LengthSquared() > radiussquared)
            {
                return ContainmentType.Intersects;
            }

            vector.X = sphere.Center.X - box.Maximum.X;
            vector.Y = sphere.Center.Y - box.Minimum.Y;
            vector.Z = sphere.Center.Z - box.Maximum.Z;

            if (vector.LengthSquared() > radiussquared)
            {
                return ContainmentType.Intersects;
            }

            vector.X = sphere.Center.X - box.Minimum.X;
            vector.Y = sphere.Center.Y - box.Minimum.Y;
            vector.Z = sphere.Center.Z - box.Maximum.Z;

            if (vector.LengthSquared() > radiussquared)
            {
                return ContainmentType.Intersects;
            }

            vector.X = sphere.Center.X - box.Minimum.X;
            vector.Y = sphere.Center.Y - box.Maximum.Y;
            vector.Z = sphere.Center.Z - box.Minimum.Z;

            if (vector.LengthSquared() > radiussquared)
            {
                return ContainmentType.Intersects;
            }

            vector.X = sphere.Center.X - box.Maximum.X;
            vector.Y = sphere.Center.Y - box.Maximum.Y;
            vector.Z = sphere.Center.Z - box.Minimum.Z;

            if (vector.LengthSquared() > radiussquared)
            {
                return ContainmentType.Intersects;
            }

            vector.X = sphere.Center.X - box.Maximum.X;
            vector.Y = sphere.Center.Y - box.Minimum.Y;
            vector.Z = sphere.Center.Z - box.Minimum.Z;

            if (vector.LengthSquared() > radiussquared)
            {
                return ContainmentType.Intersects;
            }

            vector.X = sphere.Center.X - box.Minimum.X;
            vector.Y = sphere.Center.Y - box.Minimum.Y;
            vector.Z = sphere.Center.Z - box.Minimum.Z;

            if (vector.LengthSquared() > radiussquared)
            {
                return ContainmentType.Intersects;
            }

            return ContainmentType.Contains;
        }

        /// <summary>
        ///     Determines whether a <see cin="BoundingSphere" /> contains a <see cin="BoundingSphere" />.
        /// </summary>
        /// <param name="sphere1">The first sphere to test.</param>
        /// <param name="sphere2">The second sphere to test.</param>
        /// <returns>The type of containment the two objects have.</returns>
        public static ContainmentType SphereContainsSphere(in BoundingSphere sphere1, in BoundingSphere sphere2)
        {
            float distance = Vector3.Distance(sphere1.Center, sphere2.Center);

            if (sphere1.Radius + sphere2.Radius < distance)
            {
                return ContainmentType.Disjoint;
            }

            if (sphere1.Radius - sphere2.Radius < distance)
            {
                return ContainmentType.Intersects;
            }

            return ContainmentType.Contains;
        }
    }
}