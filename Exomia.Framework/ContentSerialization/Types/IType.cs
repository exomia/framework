#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
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

namespace Exomia.Framework.ContentSerialization.Types
{
    /// <summary>
    ///     Interface for type.
    /// </summary>
    interface IType
    {
        /// <summary>
        ///     Gets the type of the base.
        /// </summary>
        /// <value>
        ///     The type of the base.
        /// </value>
        Type BaseType { get; }

        /// <summary>
        ///     Gets a value indicating whether this object is primitive.
        /// </summary>
        /// <value>
        ///     True if this object is primitive, false if not.
        /// </value>
        bool IsPrimitive { get; }

        /// <summary>
        ///     Gets the name of the type.
        /// </summary>
        /// <value>
        ///     The name of the type.
        /// </value>
        string TypeName { get; }

        /// <summary>
        ///     Creates a type.
        /// </summary>
        /// <param name="genericTypeInfo"> Information describing the generic type. </param>
        /// <returns>
        ///     The new type.
        /// </returns>
        Type CreateType(string genericTypeInfo);

        /// <summary>
        ///     Creates type information.
        /// </summary>
        /// <param name="type"> The type. </param>
        /// <returns>
        ///     The new type information.
        /// </returns>
        string CreateTypeInfo(Type type);

        /// <summary>
        ///     Reads.
        /// </summary>
        /// <param name="stream">          The stream. </param>
        /// <param name="key">             The key. </param>
        /// <param name="genericTypeInfo"> Information describing the generic type. </param>
        /// <param name="dimensionInfo">   Information describing the dimension. </param>
        /// <returns>
        ///     An object.
        /// </returns>
        object Read(CSStreamReader stream, string key, string genericTypeInfo, string dimensionInfo);

        /// <summary>
        ///     Writes.
        /// </summary>
        /// <param name="writeHandler"> The write handler. </param>
        /// <param name="tabSpace">     The tab space. </param>
        /// <param name="key">          The key. </param>
        /// <param name="content">      The content. </param>
        /// <param name="useTypeInfo">  (Optional) True to use type information. </param>
        void Write(Action<string, string> writeHandler, string tabSpace, string key, object content,
                   bool                   useTypeInfo = true);
    }
}