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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Exomia.Framework.ContentSerialization.Exceptions;
using Exomia.Framework.ContentSerialization.Readers;
using Exomia.Framework.ContentSerialization.Types;
using Exomia.Framework.ContentSerialization.Writers;
using SharpDX;

namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     ContentSerializer
    /// </summary>
    public static class ContentSerializer
    {
        internal const string TABSPACE = "\t";

        /// <summary>
        ///     DEFAULT_EXTENSION
        /// </summary>
        public const string DEFAULT_EXTENSION = ".ds0";

        private static readonly Dictionary<Type, IContentSerializationReader> s_contentPipeLineReaders =
            new Dictionary<Type, IContentSerializationReader>();

        private static readonly Dictionary<Type, IContentSerializationWriter> s_contentPipeLineWriters =
            new Dictionary<Type, IContentSerializationWriter>();

        internal static Dictionary<string, Assembly> s_assemblies = new Dictionary<string, Assembly>();
        internal static Dictionary<string, IType> s_types = new Dictionary<string, IType>();

        static ContentSerializer()
        {
            #region ADD TYPES 

            IType pt = null;

            pt = new PrimitiveType<bool>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<byte>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<sbyte>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<char>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<short>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<ushort>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<int>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<uint>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<long>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<ulong>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<float>();
            s_types.Add(pt.TypeName, pt);

            pt = new PrimitiveType<double>();
            s_types.Add(pt.TypeName, pt);

            pt = new StringType();
            s_types.Add(pt.TypeName, pt);

            pt = new EnumType();
            s_types.Add(pt.TypeName, pt);

            pt = new ArrayType();
            s_types.Add(pt.TypeName, pt);

            pt = new ListType();
            s_types.Add(pt.TypeName, pt);

            pt = new DictionaryType();
            s_types.Add(pt.TypeName, pt);

            #endregion

            #region Add Assembly & Writers + Readers

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.FullName.StartsWith("System") ||
                    a.FullName.StartsWith("SharpDX") ||
                    a.FullName.StartsWith("ms") ||
                    a.FullName.StartsWith("Xilium.CefGlue")) { continue; }

                foreach (Type t in a.GetTypes())
                {
                    ContentSerializableAttribute attribute = null;
                    if ((attribute = t.GetCustomAttribute<ContentSerializableAttribute>(false)) != null)
                    {
                        AddWriter(t, attribute.Writer);
                        AddReader(t, attribute.Reader);
                    }
                }
            }

            #endregion

            #region SharpDX 

            AddWriter<Vector2>(new Vector2CW());
            AddWriter<Color>(new ColorCW());
            AddWriter<Rectangle>(new RectangleCW());
            AddWriter<RectangleF>(new RectangleFCW());

            AddReader<Vector2>(new Vector2CR());
            AddReader<Color>(new ColorCR());
            AddReader<Rectangle>(new RectangleCR());
            AddReader<RectangleF>(new RectangleFCR());

            #endregion
        }

        /// <summary>
        ///     Adds a new content pipeline reader to the content pipeline
        /// </summary>
        /// <param name="type">the type the reader can read</param>
        /// <param name="reader">IContentSerializationReader</param>
        /// <exception cref="CSReaderException">CSReaderException</exception>
        /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
        public static void AddReader(Type type, IContentSerializationReader reader)
        {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            if (!s_contentPipeLineReaders.ContainsKey(type))
            {
                s_contentPipeLineReaders.Add(type, reader);
                AddAssembly(type.Assembly);
                return;
            }
            throw new CSReaderException(
                "The content pipeline has already registered a reader of the type: '" + type + "'");
        }

        /// <summary>
        ///     Adds a new content pipeline reader to the content pipeline
        /// </summary>
        /// <typeparam name="T">the type the reader can read</typeparam>
        /// <param name="reader">IContentSerializationReader</param>
        /// <exception cref="CSReaderException">CSReaderException</exception>
        /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
        public static void AddReader<T>(IContentSerializationReader reader)
        {
            AddReader(typeof(T), reader);
        }

        /// <summary>
        ///     Adds a new content pipeline writer to the content pipeline
        /// </summary>
        /// <param name="type">the type the writer can write</param>
        /// <param name="writer">IContentSerializationWriter</param>
        /// <exception cref="CSWriterException">CSWriterException</exception>
        /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
        public static void AddWriter(Type type, IContentSerializationWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            if (!s_contentPipeLineWriters.ContainsKey(type))
            {
                s_contentPipeLineWriters.Add(type, writer);
                return;
            }
            throw new CSWriterException(
                "The content pipeline has already registered a writer of the type: '" + type + "'");
        }

        /// <summary>
        ///     Adds a new content pipeline writer to the content pipeline
        /// </summary>
        /// <typeparam name="T">the type the writer can write</typeparam>
        /// <param name="writer">IContentSerializationWriter</param>
        /// <exception cref="CSWriterException">CSWriterException</exception>
        /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
        public static void AddWriter<T>(IContentSerializationWriter writer)
        {
            AddWriter(typeof(T), writer);
        }

        private static void AddAssembly(Assembly assembly)
        {
            string assemblyName = assembly.GetName().Name;
            if (!s_assemblies.ContainsKey(assemblyName))
            {
                s_assemblies.Add(assemblyName, assembly);
            }
        }

        #region ContentWriter

        /// <summary>
        ///     Write a given object into the asset on the file system
        /// </summary>
        /// <typeparam name="T">typeof Object</typeparam>
        /// <param name="assetName">the asset name</param>
        /// <param name="obj">Object</param>
        /// <param name="minify">minify</param>
        /// <exception cref="CSWriterException">CPWriterException</exception>
        /// <exception cref="CSTypeException">CPTypeException</exception>
        /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
        /// <exception cref="NotSupportedException">NotSupportedException</exception>
        public static void Write<T>(string assetName, T obj, bool minify = false) where T : class
        {
            if (obj == null) { throw new ArgumentNullException(nameof(obj)); }
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }

            if (!Path.HasExtension(assetName)) { assetName += DEFAULT_EXTENSION; }

            using (StreamWriter sw = new StreamWriter(assetName, false))
            {
                Action<string, string> writeHandler = minify
                    ? (t, s) => { sw.Write(s); }
                    : new Action<string, string>((t, s) => { sw.WriteLine($"{t}{s}"); });

                writeHandler(string.Empty, $"[{obj.GetType()}]");
                Write(writeHandler, TABSPACE, obj, typeof(T));
                writeHandler(string.Empty, $"[/{obj.GetType()}]");
            }
        }

        internal static void Write(Action<string, string> writeHandler, string tabSpace, object obj, Type type)
        {
            if (obj == null) { return; }

            if (!s_contentPipeLineWriters.TryGetValue(type, out IContentSerializationWriter writer))
            {
                throw new CSWriterException($"The content pipeline has not registered a writer of the type: '{type}'");
            }

            ContentSerializationContext context = new ContentSerializationContext();
            writer.Write(context, obj);

            foreach (KeyValuePair<string, ContentSerializationContextValue> ctxt in context.Content)
            {
                if (ctxt.Value.Object == null) { continue; }

                if (s_types.TryGetValue(ctxt.Value.Type.Name.ToUpper(), out IType it) ||
                    s_types.TryGetValue(ctxt.Value.Type.BaseType.Name.ToUpper(), out it))
                {
                    it.Write(writeHandler, tabSpace, ctxt.Key, ctxt.Value.Object);
                }
                else
                {
                    writeHandler(tabSpace, $"[{ctxt.Key}:{ctxt.Value.Type}]");
                    Write(writeHandler, tabSpace + TABSPACE, ctxt.Value.Object, ctxt.Value.Type);
                    writeHandler(tabSpace, $"[/{ctxt.Key}]");
                }
            }
        }

        #endregion

        #region ContentReader

        /// <summary>
        ///     Reads a object from the given stream
        /// </summary>
        /// <typeparam name="T">typeof object</typeparam>
        /// <param name="stream">Stream </param>
        /// <param name="keepOpen">keep stream open</param>
        /// <returns><c>T</c> Object</returns>
        /// <exception cref="CSReaderException">CSReaderException</exception>
        /// <exception cref="CSTypeException">CSTypeException</exception>
        /// <exception cref="ArgumentNullException">ArgumentNullException</exception>
        /// <exception cref="NotSupportedException">NotSupportedException</exception>
        public static T Read<T>(Stream stream, bool keepOpen = false) where T : class
        {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }

            CSStreamReader sr = new CSStreamReader(stream);

            Type type = typeof(T);
            try
            {
                sr.ReadObjectStartTag(type.ToString());
                return (T)Read(sr, type, type.ToString());
            }
            catch (Exception e)
            {
                throw new CSReaderException($"error near line {sr.Line}", e);
            }
            finally
            {
                if (!keepOpen)
                {
                    sr.Dispose();
                }
            }
        }

        internal static object Read(CSStreamReader stream, Type type, string objKey)
        {
            if (!s_contentPipeLineReaders.TryGetValue(type, out IContentSerializationReader reader))
            {
                throw new CSReaderException($"The content pipeline has no reader of the type '{type}' registered");
            }

            ContentSerializationContext context = new ContentSerializationContext();

            Read(stream, ref context, objKey);

            return reader.Read(context);
        }

        private static void Read(CSStreamReader stream, ref ContentSerializationContext context, string objKey)
        {
            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case '[':
                    {
                        if (stream.ReadStartTag(
                            out string key, out string baseTypeInfo, out string genericTypeInfo,
                            out string dimensionInfo))
                        {
                            if (s_types.TryGetValue(baseTypeInfo, out IType it))
                            {
                                if (it.IsPrimitive)
                                {
                                    context.Set(key, it.Read(stream, key, string.Empty, string.Empty), it.BaseType);
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(genericTypeInfo))
                                    {
                                        throw new CSReaderException(
                                            $"ERROR: NO GENERIC TYPE INFO DEFINED -> {baseTypeInfo}<GENERIC_TYPE_INFO>");
                                    }
                                    context.Set(
                                        key, it.Read(stream, key, genericTypeInfo, dimensionInfo), it.BaseType);
                                }
                            }
                            else
                            {
                                Type type = baseTypeInfo.CreateType();
                                object obj = Read(stream, type, key);
                                context.Set(key, obj, type);
                            }
                        }
                        else
                        {
                            if ($"/{objKey}" != key)
                            {
                                throw new CSReaderException($"ERROR: INVALID ENDTAG DEFINITION! -> {objKey} != {key}");
                            }
                            return;
                        }
                    }
                        break;
                    case '/':
                        throw new CSReaderException($"ERROR: INVALID FILE CONTENT! -> invalid char '{c}'!");
                    case '\n':
                    case '\r':
                    case '\t':
                    case ' ':
                        break;
                    default:
                        Console.WriteLine(
                            $"WARNING: invalid char '{c}' found near line {stream.Line} -> index {stream.Index}!");
                        break;
                }
            }
        }

        #endregion
    }
}