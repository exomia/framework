using System;
using System.Text;
using Exomia.Framework.ContentSerialization.Exceptions;

namespace Exomia.Framework.ContentSerialization.Types
{
    /// <summary>
    ///     PType{T} class
    /// </summary>
    internal sealed class PrimitiveType<T> : IType where T : struct
    {
        /// <summary>
        ///     constructor PrimitiveType{T}
        /// </summary>
        public PrimitiveType()
        {
            BaseType = typeof(T);
            if (!BaseType.IsPrimitive)
            {
                throw new NotSupportedException("typeof(T) isn't a primitive type -> " + BaseType.FullName);
            }
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
            get { return true; }
        }

        /// <summary>
        ///     <see cref="IType.CreateType(string)" />
        /// </summary>
        public Type CreateType(string genericTypeInfo)
        {
            return BaseType;
        }

        /// <summary>
        ///     <see cref="IType.CreateTypeInfo(Type)" />
        /// </summary>
        public string CreateTypeInfo(Type type)
        {
            return TypeName;
        }

        /// <summary>
        ///     <see cref="IType.Read(CSStreamReader, string, string, string)" />
        /// </summary>
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

        /// <summary>
        ///     <see cref="IType.Write(Action{string, string}, string, string, object, bool)" />
        /// </summary>
        public void Write(Action<string, string> writeHandler, string tabSpace, string key, object content,
            bool useTypeInfo = true)
        {
            //[key:type]content[/key]
            writeHandler(
                tabSpace,
                $"[{key}:{(useTypeInfo ? TypeName : string.Empty)}]{content}[/{(useTypeInfo ? key : string.Empty)}]");
        }
    }
}