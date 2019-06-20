#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.ContentSerialization.Exceptions;

namespace Exomia.Framework.ContentSerialization.Types
{
    /// <summary>
    ///     ArrayType class.
    /// </summary>
    sealed class ArrayType : IType
    {
        /// <inheritdoc />
        public Type BaseType { get; }

        /// <inheritdoc />
        public bool IsPrimitive
        {
            get { return false; }
        }

        /// <inheritdoc />
        public string TypeName
        {
            get { return BaseType.Name.ToUpper(); }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArrayType" /> class.
        /// </summary>
        public ArrayType()
        {
            BaseType = typeof(Array);
        }

        /// <inheritdoc />
        public Type CreateType(string genericTypeInfo)
        {
            genericTypeInfo.GetInnerType(out string bti, out string gti);

            return ContentSerializer.s_types.TryGetValue(bti, out IType it)
                ? it.CreateType(gti).MakeArrayType()
                : bti.CreateType().MakeArrayType();
        }

        /// <inheritdoc />
        public string CreateTypeInfo(Type type)
        {
            Type elementType = type.GetElementType() ?? throw new NullReferenceException();
            string genericTypeInfo =
                ContentSerializer.s_types.TryGetValue(elementType.Name.ToUpper(), out IType it) ||
                ContentSerializer.s_types.TryGetValue(
                    (elementType.BaseType ?? throw new NullReferenceException()).Name.ToUpper(),
                    out it)
                    ? it.CreateTypeInfo(elementType)
                    : elementType.ToString();
            return $"{TypeName}<{genericTypeInfo}>";
        }

        /// <inheritdoc />
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

            Type                                 elementType;
            Func<CSStreamReader, string, object> readCallback;
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
            Array arr        = Array.CreateInstance(elementType, dimensions);
            AddArrayContent(stream, readCallback, arr, dimensions, new int[dimensions.Length], 0);
            stream.ReadTag($"/{key}");

            return arr;
        }

        /// <inheritdoc />
        public void Write(Action<string, string> writeHandler, string tabSpace, string key, object content,
                          bool                   useTypeInfo = true)
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

        /// <summary>
        ///     Creates array dimension information.
        /// </summary>
        /// <param name="arr"> The array. </param>
        /// <returns>
        ///     The new array dimension information.
        /// </returns>
        private static string CreateArrayDimensionInfo(Array arr)
        {
            string info = string.Empty;
            for (int i = 0; i < arr.Rank; i++)
            {
                info += arr.GetLength(i) + ",";
            }
            return info.TrimEnd(',');
        }

        /// <summary>
        ///     For array dimension.
        /// </summary>
        /// <param name="writeHandler"> The write handler. </param>
        /// <param name="tabSpace">     The tab space. </param>
        /// <param name="arr">          The array. </param>
        /// <param name="elementType">  Type of the element. </param>
        /// <param name="dimension">    The dimension. </param>
        /// <param name="indices">      The indices. </param>
        /// <exception cref="NullReferenceException"> Thrown when a value was unexpectedly null. </exception>
        private static void ForArrayDimension(Action<string, string> writeHandler, string tabSpace,  Array arr,
                                              Type                   elementType,  int    dimension, int[] indices)
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
                        ContentSerializer.s_types.TryGetValue(
                            (elementType.BaseType ?? throw new NullReferenceException())
                            .Name.ToUpper(), out it))
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

        /// <summary>
        ///     Gets array dimension information.
        /// </summary>
        /// <param name="arrayTypeInfo"> Information describing the array type. </param>
        /// <returns>
        ///     An array of int.
        /// </returns>
        /// <exception cref="CSTypeException"> Thrown when a Create struct Type error condition occurs. </exception>
        private static int[] GetArrayDimensionInfo(string arrayTypeInfo)
        {
            const string START = "(";
            const string END   = ")";

            int sIndex = arrayTypeInfo.IndexOf(START, StringComparison.Ordinal);
            if (sIndex == -1)
            {
                throw new CSTypeException("No dimension start definition found in '" + arrayTypeInfo + "'");
            }
            sIndex += START.Length;

            int eIndex = arrayTypeInfo.LastIndexOf(END, StringComparison.Ordinal);
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

        /// <summary>
        ///     Adds an array content.
        /// </summary>
        /// <param name="stream">           The stream. </param>
        /// <param name="readCallback">     The read callback. </param>
        /// <param name="arr">              The array. </param>
        /// <param name="dimensions">       The dimensions. </param>
        /// <param name="indices">          The indices. </param>
        /// <param name="currentDimension"> The current dimension. </param>
        private static void AddArrayContent(CSStreamReader stream, Func<CSStreamReader, string, object> readCallback,
                                            Array          arr,    int[]                                dimensions,
                                            int[]          indices,
                                            int            currentDimension)
        {
            for (int i = 0; i < dimensions[currentDimension]; i++)
            {
                indices[currentDimension] = i;

                stream.ReadStartTag(out _, out string dimensionInfo);

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