#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using Exomia.Framework.ContentSerialization.Exceptions;

namespace Exomia.Framework.ContentSerialization.Types
{
    /// <summary>
    ///     A list type. This class cannot be inherited.
    /// </summary>
    sealed class ListType : IType
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
        ///     Initializes a new instance of the <see cref="ListType" /> class.
        /// </summary>
        public ListType()
        {
            BaseType = typeof(List<>);
        }

        /// <inheritdoc />
        public Type CreateType(string genericTypeInfo)
        {
            genericTypeInfo.GetInnerType(out string bti, out string gti);

            return ContentSerializer.s_types.TryGetValue(bti, out IType it)
                ? BaseType.MakeGenericType(it.CreateType(gti))
                : BaseType.MakeGenericType(bti.CreateType());
        }

        /// <inheritdoc />
        public string CreateTypeInfo(Type type)
        {
            Type[] gArgs = type.GetGenericArguments();
            if (gArgs.Length == 1)
            {
                string genericTypeInfo =
                    ContentSerializer.s_types.TryGetValue(gArgs[0].Name.ToUpper(), out IType it)
                 || ContentSerializer.s_types.TryGetValue(
                        (gArgs[0].BaseType ?? throw new NullReferenceException()).Name.ToUpper(),
                        out it)
                        ? it.CreateTypeInfo(gArgs[0])
                        : gArgs[0].ToString();

                return $"{TypeName}<{genericTypeInfo}>";
            }
            throw new NotSupportedException(type.ToString());
        }

        /// <inheritdoc />
        public object Read(CSStreamReader stream, string key, string genericTypeInfo, string dimensionInfo)
        {
            if (string.IsNullOrEmpty(dimensionInfo))
            {
                throw new CSReaderException(
                    $"ERROR: NO DIMENSION INFO FOUND EXPECTED: LIST<GENERIC_TYPE_INFO>(count) -> LIST{genericTypeInfo}{dimensionInfo}");
            }
            if (string.IsNullOrEmpty(genericTypeInfo))
            {
                throw new CSReaderException("ERROR: NO GENERIC TYPE INFO DEFINED -> LIST<GENERIC_TYPE_INFO>");
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

            int count = GetListCount(dimensionInfo);

            Type   list = BaseType.MakeGenericType(elementType);
            object obj  = System.Activator.CreateInstance(list, count);
            AddListContent(stream, readCallback, (dynamic)obj, count);
            stream.ReadTag($"/{key}");
            return obj;
        }

        /// <inheritdoc />
        public void Write(Action<string, string> writeHandler, string tabSpace, string key, object content,
                          bool                   useTypeInfo = true)
        {
            //[key:type]content[/key]
            Write(writeHandler, tabSpace, key, (dynamic)content, useTypeInfo);
        }

        #region WriteHelper

        /// <summary>
        ///     Foreach list dimension.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="writeHandler"> The write handler. </param>
        /// <param name="tabSpace">     The tab space. </param>
        /// <param name="list">         The list. </param>
        private static void ForeachListDimension<T>(Action<string, string> writeHandler, string tabSpace, List<T> list)
        {
            foreach (T entry in list)
            {
                Type elementType = entry.GetType();
                if (ContentSerializer.s_types.TryGetValue(elementType.Name.ToUpper(), out IType it) ||
                    ContentSerializer.s_types.TryGetValue(elementType.BaseType.Name.ToUpper(), out it))
                {
                    it.Write(writeHandler, tabSpace, string.Empty, entry, false);
                }
                else
                {
                    writeHandler(tabSpace, "[:]");
                    ContentSerializer.Write(writeHandler, tabSpace + ContentSerializer.TABSPACE, entry, elementType);
                    writeHandler(tabSpace, "[/]");
                }
            }
        }

        #endregion

        /// <summary>
        ///     Writes.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="writeHandler"> The write handler. </param>
        /// <param name="tabSpace">     The tab space. </param>
        /// <param name="key">          The key. </param>
        /// <param name="content">      The content. </param>
        /// <param name="useTypeInfo">  (Optional) True to use type information. </param>
        private void Write<T>(Action<string, string> writeHandler, string tabSpace, string key, List<T> content,
                              bool                   useTypeInfo = true)
        {
            writeHandler(
                tabSpace,
                $"[{key}:{(useTypeInfo ? CreateTypeInfo(content.GetType()) : string.Empty)}({content.Count})]");
            ForeachListDimension(writeHandler, tabSpace + ContentSerializer.TABSPACE, content);
            writeHandler(tabSpace, $"[/{(useTypeInfo ? key : string.Empty)}]");
        }

        #region ReaderHelper

        /// <summary>
        ///     Gets list count.
        /// </summary>
        /// <param name="listTypeInfo"> Information describing the list type. </param>
        /// <returns>
        ///     The list count.
        /// </returns>
        /// <exception cref="CSTypeException"> Thrown when a Create struct Type error condition occurs. </exception>
        private static int GetListCount(string listTypeInfo)
        {
            string start = "(";
            string end   = ")";

            int sIndex = listTypeInfo.IndexOf(start);
            if (sIndex == -1)
            {
                throw new CSTypeException("No dimension start definition found in '" + listTypeInfo + "'");
            }
            sIndex += start.Length;

            int eIndex = listTypeInfo.LastIndexOf(end);
            if (eIndex == -1)
            {
                throw new CSTypeException("No dimension end definition found in '" + listTypeInfo + "'");
            }

            string dimensionInfo = listTypeInfo.Substring(sIndex, eIndex - sIndex).Trim();

            if (!int.TryParse(dimensionInfo, out int buffer))
            {
                throw new CSTypeException("Invalid dimension format found in '" + dimensionInfo + "'");
            }
            return buffer;
        }

        /// <summary>
        ///     Adds a list content.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="stream">       The stream. </param>
        /// <param name="readCallback"> The read callback. </param>
        /// <param name="list">         The list. </param>
        /// <param name="count">        Number of. </param>
        private static void AddListContent<T>(CSStreamReader stream, Func<CSStreamReader, string, object> readCallback,
                                              List<T>        list,   int                                  count)
        {
            for (int i = 0; i < count; i++)
            {
                stream.ReadStartTag(out string key, out string dimensionInfo);

                list.Add((dynamic)readCallback(stream, dimensionInfo));
            }
        }

        #endregion
    }
}