#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.ContentSerialization.Exceptions;

namespace Exomia.Framework.Core.ContentSerialization.Types;

internal sealed class ListType : IType
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

    /// <summary> Initializes a new instance of the <see cref="ListType" /> class. </summary>
    public ListType()
    {
        BaseType = typeof(List<>);
    }

    /// <inheritdoc />
    public Type CreateType(string genericTypeInfo)
    {
        genericTypeInfo.GetInnerType(out string bti, out string gti);

        return ContentSerializer.Types.TryGetValue(bti, out IType? it)
            ? BaseType.MakeGenericType(it.CreateType(gti))
            : BaseType.MakeGenericType(bti.CreateType());
    }

    /// <inheritdoc />
    public string CreateTypeInfo(Type type)
    {
        Type[] gArgs = type.GetGenericArguments();
        if (gArgs.Length == 1)
        {
            string genericTypeInfo =
                ContentSerializer.Types.TryGetValue(gArgs[0].Name.ToUpper(), out IType? it)
                || ContentSerializer.Types.TryGetValue(
                    (gArgs[0].BaseType ?? throw new NullReferenceException()).Name.ToUpper(),
                    out it)
                    ? it.CreateTypeInfo(gArgs[0])
                    : gArgs[0].ToString();

            return $"{TypeName}<{genericTypeInfo}>";
        }
        throw new NotSupportedException(type.ToString());
    }

    /// <inheritdoc />
    public object Read(CsStreamReader stream, string key, string genericTypeInfo, string dimensionInfo)
    {
        if (string.IsNullOrEmpty(dimensionInfo))
        {
            throw new CsReaderException(
                $"ERROR: NO DIMENSION INFO FOUND EXPECTED: LIST<GENERIC_TYPE_INFO>(count) -> LIST{genericTypeInfo}{dimensionInfo}");
        }
        if (string.IsNullOrEmpty(genericTypeInfo))
        {
            throw new CsReaderException("ERROR: NO GENERIC TYPE INFO DEFINED -> LIST<GENERIC_TYPE_INFO>");
        }

        genericTypeInfo.GetInnerType(out string bti, out string gti);

        Type                                 elementType;
        Func<CsStreamReader, string, object> readCallback;
        if (ContentSerializer.Types.TryGetValue(bti, out IType? it))
        {
            elementType  = it.CreateType(gti);
            readCallback = (s, d) => it.Read(stream, string.Empty, gti, d);
        }
        else
        {
            elementType  = bti.CreateType();
            readCallback = (s, d) => ContentSerializer.Read(stream, elementType, string.Empty);
        }

        int count = GetListCount(dimensionInfo);

        Type   list = BaseType.MakeGenericType(elementType);
        object obj  = System.Activator.CreateInstance(list, count)!;
        AddListContent(stream, readCallback, (dynamic)obj, count);
        stream.ReadTag($"/{key}");
        return obj;
    }

    /// <inheritdoc />
    public void Write(Action<string, string> writeHandler,
                      string                 tabSpace,
                      string                 key,
                      object                 content,
                      bool                   useTypeInfo = true)
    {
        //[key:type]content[/key]
        Write(writeHandler, tabSpace, key, (dynamic)content, useTypeInfo);
    }

    #region WriteHelper

    private static void ForeachListDimension<T>(Action<string, string> writeHandler, string tabSpace, List<T> list)
    {
        foreach (T entry in list)
        {
            Type elementType = entry!.GetType();
            if (ContentSerializer.Types.TryGetValue(elementType.Name.ToUpper(),           out IType? it) ||
                ContentSerializer.Types.TryGetValue(elementType.BaseType!.Name.ToUpper(), out it))
            {
                it.Write(writeHandler, tabSpace, string.Empty, entry, false);
            }
            else
            {
                writeHandler(tabSpace, "[:]");
                ContentSerializer.Write(writeHandler, tabSpace + ContentSerializer.TABSPACE, entry, elementType);
                writeHandler(tabSpace, "[/]");
            }
        }
    }

    #endregion

    private void Write<T>(Action<string, string> writeHandler,
                          string                 tabSpace,
                          string                 key,
                          List<T>                content,
                          bool                   useTypeInfo = true)
    {
        writeHandler(
            tabSpace,
            $"[{key}:{(useTypeInfo ? CreateTypeInfo(content.GetType()) : string.Empty)}({content.Count})]");
        ForeachListDimension(writeHandler, tabSpace + ContentSerializer.TABSPACE, content);
        writeHandler(tabSpace, $"[/{(useTypeInfo ? key : string.Empty)}]");
    }

    #region ReaderHelper

    private static int GetListCount(string listTypeInfo)
    {
        string start = "(";
        string end   = ")";

        int sIndex = listTypeInfo.IndexOf(start, StringComparison.InvariantCultureIgnoreCase);
        if (sIndex == -1)
        {
            throw new CsTypeException($"No dimension start definition found in '{listTypeInfo}'");
        }
        sIndex += start.Length;

        int eIndex = listTypeInfo.LastIndexOf(end, StringComparison.InvariantCultureIgnoreCase);
        if (eIndex == -1)
        {
            throw new CsTypeException($"No dimension end definition found in '{listTypeInfo}'");
        }

        string dimensionInfo = listTypeInfo.Substring(sIndex, eIndex - sIndex).Trim();

        if (!int.TryParse(dimensionInfo, out int buffer))
        {
            throw new CsTypeException($"Invalid dimension format found in '{dimensionInfo}'");
        }
        return buffer;
    }

    private static void AddListContent<T>(CsStreamReader                       stream,
                                          Func<CsStreamReader, string, object> readCallback,
                                          List<T>                              list,
                                          int                                  count)
    {
        for (int i = 0; i < count; i++)
        {
            stream.ReadStartTag(out string _, out string dimensionInfo);

            list.Add((dynamic)readCallback(stream, dimensionInfo));
        }
    }

    #endregion
}