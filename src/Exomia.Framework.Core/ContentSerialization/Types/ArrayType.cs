#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.ContentSerialization.Exceptions;

namespace Exomia.Framework.Core.ContentSerialization.Types
{
    internal sealed class ArrayType : IType
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

        /// <summary> Initializes a new instance of the <see cref="ArrayType" /> class. </summary>
        public ArrayType()
        {
            BaseType = typeof(Array);
        }

        /// <inheritdoc />
        public Type CreateType(string genericTypeInfo)
        {
            genericTypeInfo.GetInnerType(out string bti, out string gti);

            return ContentSerializer.Types.TryGetValue(bti, out IType? it)
                ? it.CreateType(gti).MakeArrayType()
                : bti.CreateType().MakeArrayType();
        }

        /// <inheritdoc />
        public string CreateTypeInfo(Type type)
        {
            Type elementType = type.GetElementType() ?? throw new NullReferenceException();
            string genericTypeInfo =
                ContentSerializer.Types.TryGetValue(elementType.Name.ToUpper(), out IType? it) ||
                ContentSerializer.Types.TryGetValue(
                    (elementType.BaseType ?? throw new NullReferenceException()).Name.ToUpper(),
                    out it)
                    ? it.CreateTypeInfo(elementType)
                    : elementType.ToString();
            return $"{TypeName}<{genericTypeInfo}>";
        }

        /// <inheritdoc />
        public object Read(CsStreamReader stream, string key, string genericTypeInfo, string dimensionInfo)
        {
            if (string.IsNullOrEmpty(dimensionInfo))
            {
                throw new CsReaderException(
                    $"ERROR: NO DIMENSION INFO FOUND EXPECTED: ARRAY<GENERIC_TYPE_INFO>(d1,d2,...,dx) -> ARRAY{genericTypeInfo}{dimensionInfo}");
            }
            if (string.IsNullOrEmpty(genericTypeInfo))
            {
                throw new CsReaderException("ERROR: NO GENERIC TYPE INFO DEFINED -> ARRAY<GENERIC_TYPE_INFO>");
            }

            genericTypeInfo.GetInnerType(out string bti, out string gti);

            Type                                 elementType;
            Func<CsStreamReader, string, object> readCallback;
            if (ContentSerializer.Types.TryGetValue(bti, out IType? it))
            {
                elementType  = it.CreateType(gti);
                readCallback = (s, d) => it.Read(stream, string.Empty, gti, d);
            }
            else
            {
                elementType  = bti.CreateType();
                readCallback = (s, d) => ContentSerializer.Read(stream, elementType, string.Empty);
            }

            int[] dimensions = GetArrayDimensionInfo(dimensionInfo);
            Array arr        = Array.CreateInstance(elementType, dimensions);
            AddArrayContent(stream, readCallback, arr, dimensions, new int[dimensions.Length], 0);
            stream.ReadTag($"/{key}");

            return arr;
        }

        /// <inheritdoc />
        public void Write(Action<string, string> writeHandler,
                          string                 tabSpace,
                          string                 key,
                          object                 content,
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
                    writeHandler, tabSpace + ContentSerializer.TABSPACE, arr, type.GetElementType()!, 0,
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

        private static void ForArrayDimension(Action<string, string> writeHandler,
                                              string                 tabSpace,
                                              Array                  arr,
                                              Type                   elementType,
                                              int                    dimension,
                                              int[]                  indices)
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
                    if (ContentSerializer.Types.TryGetValue(elementType.Name.ToUpper(), out IType? it) ||
                        ContentSerializer.Types.TryGetValue(
                            (elementType.BaseType ?? throw new NullReferenceException())
                            .Name.ToUpper(), out it))
                    {
                        it.Write(writeHandler, tabSpace, string.Empty, arr.GetValue(indices)!, false);
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
            const string START = "(";
            const string END   = ")";

            int sIndex = arrayTypeInfo.IndexOf(START, StringComparison.Ordinal);
            if (sIndex == -1)
            {
                throw new CsTypeException($"No dimension start definition found in '{arrayTypeInfo}'");
            }
            sIndex += START.Length;

            int eIndex = arrayTypeInfo.LastIndexOf(END, StringComparison.Ordinal);
            if (eIndex == -1)
            {
                throw new CsTypeException($"No dimension end definition found in '{arrayTypeInfo}'");
            }

            string dimensionInfo = arrayTypeInfo.Substring(sIndex, eIndex - sIndex).Trim();

            string[] dimensions = dimensionInfo.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            int[] buffer = new int[dimensions.Length];
            for (int i = 0; i < dimensions.Length; i++)
            {
                if (!int.TryParse(dimensions[i], out buffer[i]))
                {
                    throw new CsTypeException($"Invalid dimension format found in '{dimensionInfo}'");
                }
            }
            return buffer;
        }

        private static void AddArrayContent(CsStreamReader                       stream,
                                            Func<CsStreamReader, string, object> readCallback,
                                            Array                                arr,
                                            int[]                                dimensions,
                                            int[]                                indices,
                                            int                                  currentDimension)
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