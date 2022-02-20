﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Text;
using Exomia.Framework.Core.ContentSerialization.Exceptions;

namespace Exomia.Framework.Core.ContentSerialization.Types;

internal sealed class StringType : IType
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

    /// <summary> Initializes a new instance of the <see cref="StringType" /> class. </summary>
    public StringType()
    {
        BaseType = typeof(string);
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
    public object Read(CsStreamReader stream, string key, string genericTypeInfo, string dimensionInfo)
    {
        StringBuilder sb = new StringBuilder(128);

        while (stream.ReadChar(out char c))
        {
            switch (c)
            {
                //ESCAPE
                case '\\':
                {
                    if (!stream.ReadChar(out c))
                    {
                        throw new CsReaderException($"ERROR: UNEXPECTED END OF FILE! - > {sb}");
                    }
                }
                    break;
                case '[':
                {
                    stream.ReadEndTag(key);
                    return sb.ToString();
                }
                case ']':
                    throw new CsReaderException($"ERROR: INVALID CONTENT -> {sb}");
            }

            sb.Append(c);
        }
        throw new CsReaderException($"ERROR: INVALID FILE CONTENT! - > {sb}");
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
            $"[{key}:{(useTypeInfo ? TypeName : string.Empty)}]{content.ToString()!.Replace("\\", "\\\\").Replace("[", "\\[").Replace("]", "\\]")}[/{(useTypeInfo ? key : string.Empty)}]");
    }
}