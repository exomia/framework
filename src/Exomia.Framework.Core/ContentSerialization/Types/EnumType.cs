#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Text;
using Exomia.Framework.Core.ContentSerialization.Exceptions;

namespace Exomia.Framework.Core.ContentSerialization.Types
{
    /// <summary>
    ///     An enum type. This class cannot be inherited.
    /// </summary>
    sealed class EnumType : IType
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
        ///     Initializes a new instance of the <see cref="EnumType" /> class.
        /// </summary>
        public EnumType()
        {
            BaseType = typeof(Enum);
        }

        /// <inheritdoc />
        public Type CreateType(string genericTypeInfo)
        {
            genericTypeInfo.GetInnerType(out string bti, out _);
            return bti.CreateType();
        }

        /// <inheritdoc />
        public string CreateTypeInfo(Type type)
        {
            return $"{TypeName}<{type}>";
        }

        /// <inheritdoc />
        public object Read(CSStreamReader stream, string key, string genericTypeInfo, string dimensionInfo)
        {
            StringBuilder sb = new StringBuilder(128);

            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case '[':
                        {
                            stream.ReadEndTag(key);

                            genericTypeInfo.GetInnerType(out string bti, out string gti);
                            if (!string.IsNullOrEmpty(gti))
                            {
                                throw new CSReaderException(
                                    $"ERROR: AN ENUM CAN't BE A GENERIC TYPE -> {genericTypeInfo}");
                            }

                            Type enumType = bti.CreateType();
                            if (enumType.IsEnum)
                            {
                                return Enum.Parse(enumType, sb.ToString());
                            }
                            throw new CSReaderException($"ERROR: BASE TYPE ISN'T AN ENUM TYPE -> {bti}");
                        }
                    case ']':
                        throw new CSReaderException($"ERROR: INVALID CONTENT -> {sb}");
                }

                sb.Append(c);
            }
            throw new CSReaderException($"ERROR: INVALID FILE CONTENT! - > {sb}");
        }

        /// <inheritdoc />
        public void Write(Action<string, string> writeHandler,
                          string                 tabSpace,
                          string                 key,
                          object                 content,
                          bool                   useTypeInfo = true)
        {
            //[key:type]content[/key]
            writeHandler(
                tabSpace,
                $"[{key}:{(useTypeInfo ? CreateTypeInfo(content.GetType()) : string.Empty)}]{content}[/{(useTypeInfo ? key : string.Empty)}]");
        }
    }
}