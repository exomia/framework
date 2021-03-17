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
    ///     PType{T} class.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    sealed class PrimitiveType<T> : IType where T : struct
    {
        /// <inheritdoc />
        public Type BaseType { get; }

        /// <inheritdoc />
        public bool IsPrimitive
        {
            get { return true; }
        }

        /// <inheritdoc />

        public string TypeName
        {
            get { return BaseType.Name.ToUpper(); }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PrimitiveType{T}" /> class.
        /// </summary>
        /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported. </exception>
        public PrimitiveType()
        {
            BaseType = typeof(T);
            if (!BaseType.IsPrimitive)
            {
                throw new NotSupportedException("typeof(T) isn't a primitive type -> " + BaseType.FullName);
            }
        }

        /// <inheritdoc />
        public Type CreateType(string genericTypeInfo)
        {
            return BaseType;
        }

        /// <inheritdoc />
        public string CreateTypeInfo(Type type)
        {
            return TypeName;
        }

        /// <inheritdoc />
        public object Read(CSStreamReader stream, string key, string genericTypeInfo, string dimensionInfo)
        {
            StringBuilder sb = new StringBuilder(32);

            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case '[':
                        {
                            stream.ReadEndTag(key);
                            string content = sb.ToString();
                            try
                            {
                                return Convert.ChangeType(content, BaseType);
                            }
                            catch
                            {
                                throw new InvalidCastException(
                                    $"content '{content}' can't be converted to '{BaseType.FullName}'!");
                            }
                        }
                    case ']':
                    case '\r':
                    case '\n':
                    case '\t':
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
                $"[{key}:{(useTypeInfo ? TypeName : string.Empty)}]{content}[/{(useTypeInfo ? key : string.Empty)}]");
        }
    }
}