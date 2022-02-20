#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Reflection;
using Exomia.Framework.Core.ContentSerialization.Exceptions;
using Exomia.Framework.Core.ContentSerialization.Readers;
using Exomia.Framework.Core.ContentSerialization.Types;
using Exomia.Framework.Core.ContentSerialization.Writers;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.ContentSerialization;

/// <summary> An object for persisting content data. </summary>
public static class ContentSerializer
{
    /// <summary> DEFAULT_EXTENSION. </summary>
    public const string DEFAULT_EXTENSION = ".e0";

    internal const string TABSPACE = "\t";

    internal static Dictionary<string, Assembly> Assemblies = new Dictionary<string, Assembly>();
    internal static Dictionary<string, IType>    Types      = new Dictionary<string, IType>();

    private static readonly Dictionary<Type, IContentSerializationReader> s_contentPipeLineReaders = new Dictionary<Type, IContentSerializationReader>();
    private static readonly Dictionary<Type, IContentSerializationWriter> s_contentPipeLineWriters = new Dictionary<Type, IContentSerializationWriter>();

    static ContentSerializer()
    {
        #region ADD TYPES

        IType pt = new PrimitiveType<bool>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<byte>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<sbyte>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<char>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<short>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<ushort>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<int>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<uint>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<long>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<ulong>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<float>();
        Types.Add(pt.TypeName, pt);

        pt = new PrimitiveType<double>();
        Types.Add(pt.TypeName, pt);

        pt = new StringType();
        Types.Add(pt.TypeName, pt);

        pt = new EnumType();
        Types.Add(pt.TypeName, pt);

        pt = new ArrayType();
        Types.Add(pt.TypeName, pt);

        pt = new ListType();
        Types.Add(pt.TypeName, pt);

        pt = new DictionaryType();
        Types.Add(pt.TypeName, pt);

        #endregion

        #region Add Assembly & Writers + Readers

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (a.FullName == null ||
                a.FullName.StartsWith("System") ||
                a.FullName.StartsWith("ms")) { continue; }

            foreach (Type t in a.GetTypes())
            {
                ContentSerializableAttribute? attribute;
                if ((attribute = t.GetCustomAttribute<ContentSerializableAttribute>(false)) != null)
                {
                    AddWriter(t, attribute.Writer);
                    AddReader(t, attribute.Reader);
                }
            }
        }

        #endregion

        #region Defaults

        AddWriter<Vector3>(new Vector3ContentSerializationWriter());
        AddWriter<Vector2>(new Vector2ContentSerializationWriter());
        AddWriter<VkColor>(new ColorContentSerializationWriter());
        AddWriter<Rectangle>(new RectangleContentSerializationWriter());
        AddWriter<RectangleF>(new RectangleFContentSerializationWriter());

        AddReader<Vector3>(new Vector3ContentSerializationReader());
        AddReader<Vector2>(new Vector2ContentSerializationReader());
        AddReader<VkColor>(new ColorContentSerializationReader());
        AddReader<Rectangle>(new RectangleContentSerializationReader());
        AddReader<RectangleF>(new RectangleFContentSerializationReader());

        #endregion
    }

    /// <summary> Adds a new <see cref="IContentSerializationReader" /> to the content pipeline. </summary>
    /// <param name="type">   The type the reader can read. </param>
    /// <param name="reader"> The <see cref="IContentSerializationReader" />. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <exception cref="CsReaderException">     Thrown when a Create struct Reader error condition occurs. </exception>
    public static void AddReader(Type type, IContentSerializationReader reader)
    {
        if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
        if (!s_contentPipeLineReaders.ContainsKey(type))
        {
            s_contentPipeLineReaders.Add(type, reader);
            AddAssembly(type.Assembly);
            return;
        }
        throw new CsReaderException(
            "The content pipeline has already registered a reader of the type: '" + type + "'");
    }

    /// <summary> Adds a new content pipeline reader to the content pipeline. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="reader"> The <see cref="IContentSerializationReader" />. </param>
    public static void AddReader<T>(IContentSerializationReader reader)
    {
        AddReader(typeof(T), reader);
    }

    /// <summary> Adds a new <see cref="IContentSerializationWriter" /> to the content pipeline. </summary>
    /// <param name="type">   The type the writer can write. </param>
    /// <param name="writer"> The <see cref="IContentSerializationWriter" />. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <exception cref="CsWriterException">     Thrown when a Create struct Writer error condition occurs. </exception>
    public static void AddWriter(Type type, IContentSerializationWriter writer)
    {
        if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
        if (!s_contentPipeLineWriters.ContainsKey(type))
        {
            s_contentPipeLineWriters.Add(type, writer);
            return;
        }
        throw new CsWriterException(
            "The content pipeline has already registered a writer of the type: '" + type + "'");
    }

    /// <summary> Adds a new content pipeline writer to the content pipeline. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="writer"> The <see cref="IContentSerializationWriter" />. </param>
    public static void AddWriter<T>(IContentSerializationWriter writer)
    {
        AddWriter(typeof(T), writer);
    }

    private static void AddAssembly(Assembly assembly)
    {
        string? assemblyName = assembly.GetName().Name;
        if (assemblyName != null && !Assemblies.ContainsKey(assemblyName))
        {
            Assemblies.Add(assemblyName, assembly);
        }
    }

    #region ContentWriter

    /// <summary> Write a given object into the asset on the file system. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="assetName"> Name of the asset. </param>
    /// <param name="obj">       The object. </param>
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

    internal static void Write(Action<string, string> writeHandler, string tabSpace, object? obj, Type type)
    {
        if (obj == null) { return; }

        if (!s_contentPipeLineWriters.TryGetValue(type, out IContentSerializationWriter? writer))
        {
            throw new CsWriterException($"The content pipeline has not registered a writer of the type: '{type}'");
        }

        ContentSerializationContext context = new ContentSerializationContext();
        writer.Write(context, obj);

        foreach ((string key, ContentSerializationContextValue value) in context.Content)
        {
            if (value.Object == null) { continue; }

            if (Types.TryGetValue(value.Type.Name.ToUpper(),           out IType? it) ||
                Types.TryGetValue(value.Type.BaseType!.Name.ToUpper(), out it))
            {
                it.Write(writeHandler, tabSpace, key, value.Object);
            }
            else
            {
                writeHandler(tabSpace, $"[{key}:{value.Type}]");
                Write(writeHandler, tabSpace + TABSPACE, value.Object, value.Type);
                writeHandler(tabSpace, $"[/{key}]");
            }
        }
    }

    #endregion

    #region ContentReader

    /// <summary> Reads an object from the given stream. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="stream">   The stream. </param>
    /// <param name="keepOpen"> (Optional) True to keep open. </param>
    /// <returns> A T. </returns>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <exception cref="CsReaderException">     Thrown when a Create struct Reader error condition occurs. </exception>
    public static T Read<T>(Stream stream, bool keepOpen = false) where T : class
    {
        if (stream == null) { throw new ArgumentNullException(nameof(stream)); }

        CsStreamReader sr = new CsStreamReader(stream);

        Type type = typeof(T);
        try
        {
            sr.ReadObjectStartTag(type.ToString());
            return (T)Read(sr, type, type.ToString());
        }
        catch (Exception e)
        {
            throw new CsReaderException($"error near line {sr.Line}", e);
        }
        finally
        {
            if (!keepOpen)
            {
                sr.Dispose();
            }
        }
    }

    internal static object Read(CsStreamReader stream, Type type, string objKey)
    {
        if (!s_contentPipeLineReaders.TryGetValue(type, out IContentSerializationReader? reader))
        {
            throw new CsReaderException($"The content pipeline has no reader of the type '{type}' registered");
        }

        ContentSerializationContext context = new ContentSerializationContext();
        Read(stream, ref context, objKey);
        return reader.Read(context);
    }

    private static void Read(CsStreamReader stream, ref ContentSerializationContext context, string objKey)
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
                        if (Types.TryGetValue(baseTypeInfo, out IType? it))
                        {
                            if (it.IsPrimitive)
                            {
                                context.Set(key, it.Read(stream, key, string.Empty, string.Empty), it.BaseType);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(genericTypeInfo))
                                {
                                    throw new CsReaderException(
                                        $"ERROR: No generic type info defined -> {baseTypeInfo}<GENERIC_TYPE_INFO>");
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
                            throw new CsReaderException(
                                $"ERROR: Invalid end tag definition! -> {objKey} != {key}");
                        }
                        return;
                    }
                }
                    break;
                case '/':
                    throw new CsReaderException(
                        $"ERROR: Invalid file content char '{c}' at ({stream.Line}:{stream.Index})!");
                case '\n':
                case '\r':
                case '\t':
                case ' ':
                    break;
                default:
                    Console.WriteLine(
                        $"WARNING: invalid char '{c}' found near ({stream.Line}:{stream.Index})!");
                    break;
            }
        }
    }

    #endregion
}