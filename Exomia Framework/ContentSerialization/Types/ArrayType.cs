#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using Exomia.Framework.ContentSerialization.Exceptions;

namespace Exomia.Framework.ContentSerialization.Types
{
    /// <summary>
    ///     ArrayType class
    /// </summary>
    sealed class ArrayType : IType
    {
        /// <summary>
        ///     constructor EnumType
        /// </summary>
        public ArrayType()
        {
            BaseType = typeof(Array);
        }

        /// <summary>
        ///     TypeName without System
        ///     !ALL UPPER CASE!
        /// </summary>
        public string TypeName
        {
            get { return BaseType.Name.ToUpper(); }
        }

        /// <summary>
        ///     typeof(Array)
        /// </summary>
        public Type BaseType { get; }

        /// <summary>
        ///     <see cref="IType.IsPrimitive()" />
        /// </summary>
        public bool IsPrimitive
        {
            get { return false; }
        }

        /// <summary>
        ///     <see cref="IType.CreateType(string)" />
        /// </summary>
        public Type CreateType(string genericTypeInfo)
        {
            genericTypeInfo.GetInnerType(out string bti, out string gti);

            return ContentSerializer.s_types.TryGetValue(bti, out IType it)
                ? it.CreateType(gti).MakeArrayType()
                : bti.CreateType().MakeArrayType();
        }

        /// <summary>
        ///     <see cref="IType.CreateTypeInfo(Type)" />
        /// </summary>
        public string CreateTypeInfo(Type type)
        {
            Type elementType = type.GetElementType();
            string genericTypeInfo =
                ContentSerializer.s_types.TryGetValue(elementType.Name.ToUpper(), out IType it) ||
                ContentSerializer.s_types.TryGetValue(elementType.BaseType.Name.ToUpper(), out it)
                    ? it.CreateTypeInfo(elementType)
                    : elementType.ToString();
            return $"{TypeName}<{genericTypeInfo}>";
        }

        /// <summary>
        ///     <see cref="IType.Read(CSStreamReader, string, string, string)" />
        /// </summary>
        public object Read(CSStreamReader stream, string key, string genericTypeInfo, string dimensionInfo)
        {
            if (string.IsNullOrEmpty(dimensionInfo))
            {
                throw new CSReaderException(
                    $"ERROR: NO DIMENSION INFO FOUND EXPECTED: ARRAY<GENERIC_TYPE_INFO>(d1,d2,...,dx) -> ARRAY{genericTypeInfo}{dimensionInfo}");
            }
            if (string.IsNullOrEmpty(genericTypeInfo))
            {
                throw new CSReaderException("ERROR: NO GENERIC TYPE INFO DEFINED -> ARRAY<GENERIC_TYPE_INFO>");
            }

            genericTypeInfo.GetInnerType(out string bti, out string gti);

            Type elementType = null;
            Func<CSStreamReader, string, object> readCallback = null;
            if (ContentSerializer.s_types.TryGetValue(bti, out IType it))
            {
                elementType = it.CreateType(gti);
                readCallback = (s, d) =>
                {
                    return it.Read(stream, string.Empty, gti, d);
                };
            }
            else
            {
                elementType = bti.CreateType();
                readCallback = (s, d) =>
                {
                    return ContentSerializer.Read(stream, elementType, string.Empty);
                };
            }

            int[] dimensions = GetArrayDimensionInfo(dimensionInfo);
            Array arr = Array.CreateInstance(elementType, dimensions);
            AddArrayContent(stream, readCallback, arr, dimensions, new int[dimensions.Length], 0);
            stream.ReadTag($"/{key}");

            return arr;
        }

        /// <summary>
        ///     <see cref="IType.Write(Action{string, string}, string, string, object, bool)" />
        /// </summary>
        public void Write(Action<string, string> writeHandler, string tabSpace, string key, object content,
            bool useTypeInfo = true)
        {
            //[key:type]content[/key]
            Type type = content.GetType();
            if (type.IsArray)
            {
                Array arr = (Array)content;
                writeHandler(
                    tabSpace,
                    $"[{key}:{(useTypeInfo ? CreateTypeInfo(type) : string.Empty)}({CreateArrayDimensionInfo(arr)})]");
                ForArrayDimension(
                    writeHandler, tabSpace + ContentSerializer.TABSPACE, arr, type.GetElementType(), 0,
                    new int[arr.Rank]);
                writeHandler(tabSpace, $"[/{(useTypeInfo ? key : string.Empty)}]");
            }
            else { throw new InvalidCastException(nameof(content)); }
        }

        #region WriteHelper

        private static string CreateArrayDimensionInfo(Array arr)
        {
            string info = string.Empty;
            for (int i = 0; i < arr.Rank; i++)
            {
                info += arr.GetLength(i) + ",";
            }
            return info.TrimEnd(',');
        }

        private static void ForArrayDimension(Action<string, string> writeHandler, string tabSpace, Array arr,
            Type elementType, int dimension, int[] indices)
        {
            for (int i = 0; i < arr.GetLength(dimension); i++)
            {
                indices[dimension] = i;
                if (dimension + 1 < arr.Rank)
                {
                    writeHandler(tabSpace, "[:]");
                    ForArrayDimension(
                        writeHandler, tabSpace + ContentSerializer.TABSPACE, arr, elementType, dimension + 1, indices);
                    writeHandler(tabSpace, "[/]");
                }
                else
                {
                    if (ContentSerializer.s_types.TryGetValue(elementType.Name.ToUpper(), out IType it) ||
                        ContentSerializer.s_types.TryGetValue(elementType.BaseType.Name.ToUpper(), out it))
                    {
                        it.Write(writeHandler, tabSpace, string.Empty, arr.GetValue(indices), false);
                    }
                    else
                    {
                        writeHandler(tabSpace, "[:]");
                        ContentSerializer.Write(
                            writeHandler, tabSpace + ContentSerializer.TABSPACE, arr.GetValue(indices),
                            elementType);
                        writeHandler(tabSpace, "[/]");
                    }
                }
            }
        }

        #endregion

        #region ReaderHelper

        private static int[] GetArrayDimensionInfo(string arrayTypeInfo)
        {
            const string start = "(";
            const string end = ")";

            int sIndex = arrayTypeInfo.IndexOf(start, StringComparison.Ordinal);
            if (sIndex == -1)
            {
                throw new CSTypeException("No dimension start definition found in '" + arrayTypeInfo + "'");
            }
            sIndex += start.Length;

            int eIndex = arrayTypeInfo.LastIndexOf(end, StringComparison.Ordinal);
            if (eIndex == -1)
            {
                throw new CSTypeException("No dimension end definition found in '" + arrayTypeInfo + "'");
            }

            string dimensionInfo = arrayTypeInfo.Substring(sIndex, eIndex - sIndex).Trim();

            string[] dimensions = dimensionInfo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int[] buffer = new int[dimensions.Length];
            for (int i = 0; i < dimensions.Length; i++)
            {
                if (!int.TryParse(dimensions[i], out buffer[i]))
                {
                    throw new CSTypeException("Invalid dimension format found in '" + dimensionInfo + "'");
                }
            }
            return buffer;
        }

        private static void AddArrayContent(CSStreamReader stream, Func<CSStreamReader, string, object> readCallback,
            Array arr, int[] dimensions, int[] indices, int currentDimension)
        {
            for (int i = 0; i < dimensions[currentDimension]; i++)
            {
                indices[currentDimension] = i;

                stream.ReadStartTag(out string key, out string dimensionInfo);

                if (currentDimension + 1 < dimensions.Length)
                {
                    AddArrayContent(stream, readCallback, arr, dimensions, indices, currentDimension + 1);
                    stream.ReadTag("/");
                }
                else
                {
                    arr.SetValue(readCallback(stream, dimensionInfo), indices);
                }
            }
        }

        #endregion
    }
}