#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

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
    ///     An object for persisting content data.
    /// </summary>
    public static class ContentSerializer
    {
        /// <summary>
        ///     The tabspace.
        /// </summary>
        internal const string TABSPACE = "\t";

        /// <summary>
        ///     DEFAULT_EXTENSION.
        /// </summary>
        public const string DEFAULT_EXTENSION = ".e0";

        /// <summary>
        ///     The content pipe line readers.
        /// </summary>
        private static readonly Dictionary<Type, IContentSerializationReader> s_contentPipeLineReaders =
            new Dictionary<Type, IContentSerializationReader>();

        /// <summary>
        ///     The content pipe line writers.
        /// </summary>
        private static readonly Dictionary<Type, IContentSerializationWriter> s_contentPipeLineWriters =
            new Dictionary<Type, IContentSerializationWriter>();

        /// <summary>
        ///     The assemblies.
        /// </summary>
        internal static Dictionary<string, Assembly> s_assemblies = new Dictionary<string, Assembly>();

        /// <summary>
        ///     The types.
        /// </summary>
        internal static Dictionary<string, IType> s_types = new Dictionary<string, IType>();

        /// <summary>
        ///     Initializes static members of the <see cref="ContentSerializer" /> class.
        /// </summary>
        static ContentSerializer()
        {
            #region ADD TYPES 

            IType pt = new PrimitiveType<bool>();
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
                if (a.FullName.StartsWith("System")  ||
                    a.FullName.StartsWith("SharpDX") ||
                    a.FullName.StartsWith("ms")      ||
                    a.FullName.StartsWith("Xilium.CefGlue")) { continue; }

                foreach (Type t in a.GetTypes())
                {
                    ContentSerializableAttribute attribute;
                    if ((attribute = t.GetCustomAttribute<ContentSerializableAttribute>(false)) != null)
                    {
                        AddWriter(t, attribute.Writer);
                        AddReader(t, attribute.Reader);
                    }
                }
            }

            #endregion

            #region SharpDX 

            AddWriter<Vector3>(new Vector3CW());
            AddWriter<Vector2>(new Vector2CW());
            AddWriter<Color>(new ColorCW());
            AddWriter<Rectangle>(new RectangleCW());
            AddWriter<RectangleF>(new RectangleFCW());

            AddReader<Vector3>(new Vector3CR());
            AddReader<Vector2>(new Vector2CR());
            AddReader<Color>(new ColorCR());
            AddReader<Rectangle>(new RectangleCR());
            AddReader<RectangleF>(new RectangleFCR());

            #endregion
        }

        /// <summary>
        ///     Adds a new content pipeline reader to the content pipeline.
        /// </summary>
        /// <param name="type">   the type the reader can read. </param>
        /// <param name="reader"> IContentSerializationReader. </param>
        /// <exception cref="ArgumentNullException"> ArgumentNullException. </exception>
        /// <exception cref="CSReaderException">     CSReaderException. </exception>
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
        ///     Adds a new content pipeline reader to the content pipeline.
        /// </summary>
        /// <typeparam name="T"> the type the reader can read. </typeparam>
        /// <param name="reader"> IContentSerializationReader. </param>
        public static void AddReader<T>(IContentSerializationReader reader)
        {
            AddReader(typeof(T), reader);
        }

        /// <summary>
        ///     Adds a new content pipeline writer to the content pipeline.
        /// </summary>
        /// <param name="type">   the type the writer can write. </param>
        /// <param name="writer"> IContentSerializationWriter. </param>
        /// <exception cref="ArgumentNullException"> ArgumentNullException. </exception>
        /// <exception cref="CSWriterException">     CSWriterException. </exception>
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
        ///     Adds a new content pipeline writer to the content pipeline.
        /// </summary>
        /// <typeparam name="T"> the type the writer can write. </typeparam>
        /// <param name="writer"> IContentSerializationWriter. </param>
        public static void AddWriter<T>(IContentSerializationWriter writer)
        {
            AddWriter(typeof(T), writer);
        }

        /// <summary>
        ///     Adds an assembly.
        /// </summary>
        /// <param name="assembly"> The assembly. </param>
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
        ///     Write a given object into the asset on the file system.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="assetName"> Name of the asset. </param>
        /// <param name="obj">       Object. </param>
        /// <param name="minify">    (Optional) True to minify. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public static void Write<T>(string assetName, T obj, bool minify = false) where T : class
        {
            if (obj == null) { throw new ArgumentNullException(nameof(obj)); }
            if (string.IsNullOrEmpty(assetName)) { throw new ArgumentNullException(nameof(assetName)); }

            if (!Path.HasExtension(assetName) || Path.GetExtension(assetName) != DEFAULT_EXTENSION)
            {
                assetName += DEFAULT_EXTENSION;
            }
            using (FileStream fs = new FileStream(assetName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Action<string, string> writeHandler = minify

                        // ReSharper disable once AccessToDisposedClosure
                        ? (t, s) => { sw.Write(s); }

                        // ReSharper disable once AccessToDisposedClosure
                        : new Action<string, string>((t, s) => { sw.WriteLine($"{t}{s}"); });

                    writeHandler(string.Empty, $"[{obj.GetType()}]");
                    Write(writeHandler, TABSPACE, obj, typeof(T));
                    writeHandler(string.Empty, $"[/{obj.GetType()}]");
                }
            }
        }

        /// <summary>
        ///     Write a given object into the asset on the file system.
        /// </summary>
        /// <param name="writeHandler"> The write handler. </param>
        /// <param name="tabSpace">     The tab space. </param>
        /// <param name="obj">          Object. </param>
        /// <param name="type">         the type the reader can read. </param>
        /// <exception cref="CSWriterException">
        ///     Thrown when a Create struct Writer error condition
        ///     occurs.
        /// </exception>
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
        ///     Reads a object from the given stream.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="stream">   Stream. </param>
        /// <param name="keepOpen"> (Optional) True to keep open. </param>
        /// <returns>
        ///     A T.
        /// </returns>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
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

        /// <summary>
        ///     Reads a object from the given stream.
        /// </summary>
        /// <param name="stream"> Stream. </param>
        /// <param name="type">   the type the reader can read. </param>
        /// <param name="objKey"> The object key. </param>
        /// <returns>
        ///     <c>T</c> Object.
        /// </returns>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
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

        /// <summary>
        ///     Reads a object from the given stream.
        /// </summary>
        /// <param name="stream">  Stream. </param>
        /// <param name="context"> [in,out] The context. </param>
        /// <param name="objKey">  The object key. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
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
                                    Type   type = baseTypeInfo.CreateType();
                                    object obj  = Read(stream, type, key);
                                    context.Set(key, obj, type);
                                }
                            }
                            else
                            {
                                if ($"/{objKey}" != key)
                                {
                                    throw new CSReaderException(
                                        $"ERROR: INVALID ENDTAG DEFINITION! -> {objKey} != {key}");
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