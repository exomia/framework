#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization.Types;

internal interface IType
{
    /// <summary> Gets the type of the base. </summary>
    /// <value> The type of the base. </value>
    Type BaseType { get; }

    /// <summary> Gets a value indicating whether this object is primitive. </summary>
    /// <value> True if this object is primitive, false if not. </value>
    bool IsPrimitive { get; }

    /// <summary> Gets the name of the type. </summary>
    /// <value> The name of the type. </value>
    string TypeName { get; }

    /// <summary> Creates a type. </summary>
    /// <param name="genericTypeInfo"> Information describing the generic type. </param>
    /// <returns> The new type. </returns>
    Type CreateType(string genericTypeInfo);

    /// <summary> Creates type information. </summary>
    /// <param name="type"> The type. </param>
    /// <returns> The new type information. </returns>
    string CreateTypeInfo(Type type);

    /// <summary> Reads. </summary>
    /// <param name="stream">          The stream. </param>
    /// <param name="key">             The key. </param>
    /// <param name="genericTypeInfo"> Information describing the generic type. </param>
    /// <param name="dimensionInfo">   Information describing the dimension. </param>
    /// <returns> An object. </returns>
    object Read(CsStreamReader stream, string key, string genericTypeInfo, string dimensionInfo);

    /// <summary> Writes. </summary>
    /// <param name="writeHandler"> The write handler. </param>
    /// <param name="tabSpace">     The tab space. </param>
    /// <param name="key">          The key. </param>
    /// <param name="content">      The content. </param>
    /// <param name="useTypeInfo">  (Optional) True to use type information. </param>
    void Write(Action<string, string> writeHandler,
               string                 tabSpace,
               string                 key,
               object                 content,
               bool                   useTypeInfo = true);
}