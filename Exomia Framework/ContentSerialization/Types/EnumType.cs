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
using System.Text;
using Exomia.Framework.ContentSerialization.Exceptions;

namespace Exomia.Framework.ContentSerialization.Types
{
    /// <summary>
    ///     EnumType class
    /// </summary>
    internal sealed class EnumType : IType
    {
        /// <summary>
        ///     constructor EnumType
        /// </summary>
        public EnumType()
        {
            BaseType = typeof(Enum);
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
            return bti.CreateType();
        }

        /// <summary>
        ///     <see cref="IType.CreateTypeInfo(Type)" />
        /// </summary>
        public string CreateTypeInfo(Type type)
        {
            return $"{TypeName}<{type}>";
        }

        /// <summary>
        ///     <see cref="IType.Read(CSStreamReader, string, string, string)" />
        /// </summary>
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
                            throw new CSReaderException($"ERROR: AN ENUM CAN't BE A GENERIC TYPE -> {genericTypeInfo}");
                        }

                        try
                        {
                            Type enumType = bti.CreateType();
                            if (enumType.IsEnum)
                            {
                                return Enum.Parse(enumType, sb.ToString());
                            }
                            throw new CSReaderException($"ERROR: BASETYPE ISN'T AN ENUM TYPE -> {bti}");
                        }
                        catch { throw; }
                    }
                    case ']':
                        throw new CSReaderException($"ERROR: INVALID CONTENT -> {sb}");
                }

                sb.Append(c);
            }
            throw new CSReaderException($"ERROR: INVALID FILE CONTENT! - > {sb}");
        }

        /// <summary>
        ///     <see cref="IType.Write(Action{string, string}, string, string, object, bool)" />
        /// </summary>
        public void Write(Action<string, string> writeHandler, string tabSpace, string key, object content,
            bool useTypeInfo = true)
        {
            //[key:type]content[/key]
            writeHandler(
                tabSpace,
                $"[{key}:{(useTypeInfo ? CreateTypeInfo(content.GetType()) : string.Empty)}]{content}[/{(useTypeInfo ? key : string.Empty)}]");
        }
    }
}