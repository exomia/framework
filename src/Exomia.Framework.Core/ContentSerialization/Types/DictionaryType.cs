﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.ContentSerialization.Exceptions;

namespace Exomia.Framework.Core.ContentSerialization.Types
{
    internal sealed class DictionaryType : IType
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

        /// <summary> Initializes a new instance of the <see cref="DictionaryType" /> class. </summary>
        public DictionaryType()
        {
            BaseType = typeof(Dictionary<,>);
        }

        /// <inheritdoc />
        public Type CreateType(string genericTypeInfo)
        {
            genericTypeInfo.GetKeyValueInnerType(
                out string keyBaseTypeInfo, out string valueBaseTypeInfo, out string valueGenericTypeInfo);

            if (!ContentSerializer.Types.TryGetValue(keyBaseTypeInfo, out IType? itk) || !itk.IsPrimitive)
            {
                throw new NotSupportedException($"ERROR: INVALID KEY TYPE FOUND IN -> '{genericTypeInfo}'");
            }

            return ContentSerializer.Types.TryGetValue(valueBaseTypeInfo, out IType? itv)
                ? BaseType.MakeGenericType(itk.CreateType(string.Empty), itv.CreateType(valueGenericTypeInfo))
                : BaseType.MakeGenericType(itk.CreateType(string.Empty), valueBaseTypeInfo.CreateType());
        }

        /// <inheritdoc />
        public string CreateTypeInfo(Type type)
        {
            Type[] gArgs = type.GetGenericArguments();
            if (gArgs.Length == 2)
            {
                string genericTypeInfo1 =
                    ContentSerializer.Types.TryGetValue(gArgs[0].Name.ToUpper(), out IType? itk)
                        ? itk.CreateTypeInfo(gArgs[0])
                        : throw new NotSupportedException(
                            $"the type of '{gArgs[0]}' is not supported as dictionary key");
                string genericTypeInfo2 =
                    ContentSerializer.Types.TryGetValue(gArgs[1].Name.ToUpper(), out IType? itv) ||
                    ContentSerializer.Types.TryGetValue(
                        (gArgs[1].BaseType ?? throw new NullReferenceException()).Name.ToUpper(),
                        out itv)
                        ? itv.CreateTypeInfo(gArgs[1])
                        : gArgs[1].ToString();

                return $"{TypeName}<{genericTypeInfo1}, {genericTypeInfo2}>";
            }
            throw new NotSupportedException(type.ToString());
        }

        /// <inheritdoc />
        public object Read(CsStreamReader stream, string key, string genericTypeInfo, string dimensionInfo)
        {
            if (string.IsNullOrEmpty(dimensionInfo))
            {
                throw new CsReaderException(
                    $"ERROR: NO DIMENSION INFO FOUND EXPECTED: DICTIONARY<GENERIC_TYPE_INFO1, GENERIC_TYPE_INFO2>(count) -> DICTIONARY{genericTypeInfo}{dimensionInfo}");
            }
            if (string.IsNullOrEmpty(genericTypeInfo))
            {
                throw new CsReaderException(
                    "ERROR: NO GENERIC TYPE INFO DEFINED -> DICTIONARY<GENERIC_TYPE_INFO1, GENERIC_TYPE_INFO2>");
            }

            // ReSharper disable IdentifierTypo
            genericTypeInfo.GetKeyValueInnerType(out string kbti, out string vbti, out string vgti);
            // ReSharper enable IdentifierTypo

            if (!ContentSerializer.Types.TryGetValue(kbti, out IType? itk) || !itk.IsPrimitive)
            {
                throw new NotSupportedException($"ERROR: INVALID KEY TYPE FOUND IN -> '{genericTypeInfo}'");
            }

            Type                                 valueType;
            Func<CsStreamReader, string, object> readCallback;
            if (ContentSerializer.Types.TryGetValue(vbti, out IType? it))
            {
                valueType    = it.CreateType(vgti);
                readCallback = (s, d) => it.Read(stream, string.Empty, vgti, d);
            }
            else
            {
                valueType    = vbti.CreateType();
                readCallback = (s, d) => ContentSerializer.Read(stream, valueType, string.Empty);
            }

            int count = GetDictionaryCount(dimensionInfo);

            Type   dic = BaseType.MakeGenericType(itk.CreateType(string.Empty), valueType);
            object obj = System.Activator.CreateInstance(dic, count) ?? throw new NullReferenceException(dic.ToString());
            AddDictionaryContent(stream, readCallback, (dynamic)obj, count);
            stream.ReadTag($"/{key}");
            return obj;
        }

        /// <inheritdoc />
        public void Write(Action<string, string> writeHandler,
                          string                 tabSpace,
                          string                 key,
                          object                 content,
                          bool                   useTypeInfo = true)
        {
            //[key:type]content[/key]
            Write(writeHandler, tabSpace, key, (dynamic)content, useTypeInfo);
        }

        private void Write<TKey, TValue>(Action<string, string>   writeHandler,
                                         string                   tabSpace,
                                         string                   key,
                                         Dictionary<TKey, TValue> content,
                                         bool                     useTypeInfo = true)
            where TKey : notnull
        {
            writeHandler(
                tabSpace,
                $"[{key}:{(useTypeInfo ? CreateTypeInfo(content.GetType()) : string.Empty)}({content.Count})]");
            ForeachDictionaryDimension(writeHandler, tabSpace + ContentSerializer.TABSPACE, content);
            writeHandler(tabSpace, $"[/{(useTypeInfo ? key : string.Empty)}]");
        }

        #region WriteHelper

        private static void ForeachDictionaryDimension<TKey, TValue>(Action<string, string>   writeHandler,
                                                                     string                   tabSpace,
                                                                     Dictionary<TKey, TValue> dic)
            where TKey : notnull
        {
            foreach ((TKey key, TValue value) in dic)
            {
                Type elementType = value!.GetType();
                if (ContentSerializer.Types.TryGetValue(elementType.Name.ToUpper(), out IType? it) ||
                    ContentSerializer.Types.TryGetValue(
                        (elementType.BaseType ?? throw new NullReferenceException()).Name.ToUpper(),
                        out it))
                {
                    it.Write(writeHandler, tabSpace, key.ToString()!, value, false);
                }
                else
                {
                    writeHandler(tabSpace, $"[{key}:]");
                    ContentSerializer.Write(
                        writeHandler, tabSpace + ContentSerializer.TABSPACE, value, elementType);
                    writeHandler(tabSpace, "[/]");
                }
            }
        }

        #endregion

        #region ReaderHelper

        private static int GetDictionaryCount(string dictionaryTypeInfo)
        {
            const string START = "(";
            const string END   = ")";

            int sIndex = dictionaryTypeInfo.IndexOf(START, StringComparison.Ordinal);
            if (sIndex == -1)
            {
                throw new CsTypeException($"No dimension start definition found in '{dictionaryTypeInfo}'");
            }
            sIndex += START.Length;

            int eIndex = dictionaryTypeInfo.LastIndexOf(END, StringComparison.Ordinal);
            if (eIndex == -1)
            {
                throw new CsTypeException($"No dimension end definition found in '{dictionaryTypeInfo}'");
            }

            string dimensionInfo = dictionaryTypeInfo.Substring(sIndex, eIndex - sIndex).Trim();

            if (!int.TryParse(dimensionInfo, out int buffer))
            {
                throw new CsTypeException($"Invalid dimension format found in '{dimensionInfo}'");
            }
            return buffer;
        }

        private static void AddDictionaryContent<TKey, TValue>(CsStreamReader                       stream,
                                                               Func<CsStreamReader, string, object> readCallback,
                                                               Dictionary<TKey, TValue>             dic,
                                                               int                                  count)
            where TKey : notnull
        {
            for (int i = 0; i < count; i++)
            {
                stream.ReadStartTag(out string key, out string dimensionInfo);

                dic.Add((TKey)Convert.ChangeType(key, typeof(TKey)), (dynamic)readCallback(stream, dimensionInfo));
            }
        }

        #endregion
    }
}