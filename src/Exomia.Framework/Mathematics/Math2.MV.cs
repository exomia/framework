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
    /// <content> The mathematics 2. </content>
    public static partial class Math2
    {
        /// <summary> calculates a transform matrix. </summary>
        /// <param name="position">  position. </param>
        /// <param name="origin">    origin. </param>
        /// <param name="scale">     scale. </param>
        /// <param name="rotation">  rotation. </param>
        /// <param name="transform"> [out] out transform matrix. </param>
        public static void CalculateTransformMatrix(in Vector2    position,
                                                    in Vector2    origin,
                                                    in Vector2    scale,
                                                    float         rotation,
                                                    out Matrix4x4 transform)
        {
            transform = Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0) *
                        Matrix4x4.CreateRotationZ(rotation) *
                        Matrix4x4.CreateScale(scale.X, scale.Y, 0.0f) *
                        Matrix4x4.CreateTranslation(position.X, position.Y, 0);
        }

        /// <summary> calculates a transform matrix. </summary>
        /// <param name="position"> position. </param>
        /// <param name="origin">   origin. </param>
        /// <param name="scale">    scale. </param>
        /// <param name="rotation"> rotation. </param>
        /// <returns> transform matrix. </returns>
        public static Matrix4x4 CalculateTransformMatrix(in Vector2 position,
                                                         in Vector2 origin,
                                                         in Vector2 scale,
                                                         float      rotation)
        {
            return Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0) *
                   Matrix4x4.CreateRotationZ(rotation) *
                   Matrix4x4.CreateScale(scale.X, scale.Y, 0.0f) *
                   Matrix4x4.CreateTranslation(position.X, position.Y, 0);
        }

        /// <summary> creates an axis aligned bounding box. </summary>
        /// <param name="transform"> transform. </param>
        /// <param name="width">     width. </param>
        /// <param name="height">    height. </param>
        /// <param name="aabb">      [out] out aabb. </param>
        public static void CreateAABB(in Matrix4x4 transform, float width, float height, out RectangleF aabb)
        {
            Vector2 leftTop     = Vector2.Transform(Vector2.Zero,               transform);
            Vector2 rightTop    = Vector2.Transform(new Vector2(width, 0),      transform);
            Vector2 leftBottom  = Vector2.Transform(new Vector2(0,     height), transform);
            Vector2 rightBottom = Vector2.Transform(new Vector2(width, height), transform);

            Vector2 min = new Vector2(
                Math.Min(leftTop.X, Math.Min(rightTop.X, Math.Min(leftBottom.X, rightBottom.X))),
                Math.Min(leftTop.Y, Math.Min(rightTop.Y, Math.Min(leftBottom.Y, rightBottom.Y))));
            Vector2 max = new Vector2(
                Math.Max(leftTop.X, Math.Max(rightTop.X, Math.Max(leftBottom.X, rightBottom.X))),
                Math.Max(leftTop.Y, Math.Max(rightTop.Y, Math.Max(leftBottom.Y, rightBottom.Y))));

            aabb = new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
        }

        /// <summary> creates an axis aligned bounding box. </summary>
        /// <param name="transform"> transform. </param>
        /// <param name="width">     width. </param>
        /// <param name="height">    height. </param>
        /// <returns> axis aligned bounding box. </returns>
        public static RectangleF CreateAABB(in Matrix4x4 transform, float width, float height)
        {
            Vector2 leftTop     = Vector2.Transform(Vector2.Zero,               transform);
            Vector2 rightTop    = Vector2.Transform(new Vector2(width, 0),      transform);
            Vector2 leftBottom  = Vector2.Transform(new Vector2(0,     height), transform);
            Vector2 rightBottom = Vector2.Transform(new Vector2(width, height), transform);

            Vector2 min = new Vector2(
                Math.Min(leftTop.X, Math.Min(rightTop.X, Math.Min(leftBottom.X, rightBottom.X))),
                Math.Min(leftTop.Y, Math.Min(rightTop.Y, Math.Min(leftBottom.Y, rightBottom.Y))));
            Vector2 max = new Vector2(
                Math.Max(leftTop.X, Math.Max(rightTop.X, Math.Max(leftBottom.X, rightBottom.X))),
                Math.Max(leftTop.Y, Math.Max(rightTop.Y, Math.Max(leftBottom.Y, rightBottom.Y))));

            return new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
        }

        /// <summary> creates an axis aligned bounding box. </summary>
        /// <param name="position"> position. </param>
        /// <param name="origin">   origin. </param>
        /// <param name="scale">    scale. </param>
        /// <param name="rotation"> rotation. </param>
        /// <param name="width">    width. </param>
        /// <param name="height">   height. </param>
        /// <returns> axis aligned bounding box. </returns>
        public static RectangleF CreateAABB(in Vector2 position,
                                            in Vector2 origin,
                                            in Vector2 scale,
                                            float      rotation,
                                            float      width,
                                            float      height)
        {
            return CreateAABB(CalculateTransformMatrix(position, origin, scale, rotation), width, height);
        }
    }
}