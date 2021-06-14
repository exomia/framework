#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Text;
using System.Text.RegularExpressions;
using Exomia.Framework.Core.ContentSerialization.Exceptions;

namespace Exomia.Framework.Core.ContentSerialization
{
    internal static class CsExtensions
    {
        private static readonly Regex s_innerTypeMatcher = new Regex(
            "^<([A-Za-z][A-Za-z0-9.+,\\s`]+)(?:(<[A-Za-z0-9.,+\\s`<>]+>)?)>$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex s_valueInnerTypeMatcher = new Regex(
            "^<([A-Za-z][A-Za-z0-9]+),(?:[\\s]*)?([A-Za-z][A-Za-z0-9.`+]+)(?:(<[A-Za-z0-9.,\\s<>`+]+>)?)>$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex s_keyInfoMatcher = new Regex(
            "^([A-Za-z][A-Za-z0-9]*):([A-Za-z][A-Za-z0-9.,+\\s`]+)(?:(<[A-Za-z0-9.,+\\s`<>]+>)(\\([0-9,]+\\))?)?$",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex s_kvInfoMatcher = new Regex(
            "^([a-zA-Z0-9-]+)?:(\\([0-9,]+\\))?$", RegexOptions.Compiled | RegexOptions.Singleline);

        internal static Type CreateType(this string typeInfo)
        {
            Type? t = Type.GetType(typeInfo);
            if (t != null) { return t; }

            foreach (var (_, value) in ContentSerializer.Assemblies)
            {
                t = Type.GetType(typeInfo + ", " + value.FullName);
                if (t != null) { return t; }
            }
            throw new CsTypeException("Can't create type of: '" + typeInfo + "'");
        }

        internal static void GetInnerType(this string typeInfo, out string baseTypeInfo, out string genericTypeInfo)
        {
            Match match = s_innerTypeMatcher.Match(typeInfo);
            if (!match.Success)
            {
                throw new CsReaderException($"ERROR: TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");
            }

            baseTypeInfo = match.Groups[1].Success
                ? match.Groups[1].Value
                : throw new CsReaderException($"ERROR: TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");

            genericTypeInfo = match.Groups[2].Success
                ? match.Groups[2].Value
                : string.Empty;
        }

        internal static void GetKeyValueInnerType(this string typeInfo,
                                                  out  string keyBaseTypeInfo,
                                                  out  string valueBaseTypeInfo,
                                                  out  string valueGenericTypeInfo)
        {
            Match match = s_valueInnerTypeMatcher.Match(typeInfo);
            if (!match.Success)
            {
                throw new CsReaderException($"ERROR: KEY VALUE TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");
            }

            keyBaseTypeInfo = match.Groups[1].Success
                ? match.Groups[1].Value
                : throw new CsReaderException($"ERROR: KEY VALUE TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");

            valueBaseTypeInfo = match.Groups[2].Success
                ? match.Groups[2].Value
                : throw new CsReaderException($"ERROR: KEY VALUE TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");

            valueGenericTypeInfo = match.Groups[3].Success
                ? match.Groups[3].Value
                : string.Empty;
        }

        internal static void ReadEndTag(this CsStreamReader stream, string key)
        {
            StringBuilder sb = new StringBuilder(32);

            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case ']':
                    {
                        string buffer = sb.ToString();
                        if ($"/{key}" == buffer)
                        {
                            return;
                        }
                        throw new CsReaderException(
                            $"ERROR: INVALID END TAG DEFINITION! -> '{buffer}' != '/{key}'");
                    }
                    case '\n':
                    case '\r':
                    case '[':
                    case '\t':
                        throw new CsReaderException($"ERROR: INVALID END TAG DEFINITION! -> invalid char '{c}'");
                }
                sb.Append(c);
            }
            throw new CsReaderException($"ERROR: NO END TAG FOUND! -> {sb}");
        }

        internal static void ReadObjectStartTag(this CsStreamReader stream, string key)
        {
            StringBuilder sb = new StringBuilder(128);

            while (stream.ReadChar(out char c))
            {
                sb.Append(c);
                switch (c)
                {
                    case ']':
                    {
                        string buffer = sb.ToString();
                        if (buffer == $"[{key}]") { return; }
                        throw new CsReaderException($"ERROR: INVALID START TAG FOUND! -> '{buffer}' != '[{key}]'");
                    }
                    case '\n':
                    case '\r':
                    case '\t':
                        throw new CsReaderException($"ERROR: INVALID KEY:INFO DEFINITION! -> invalid char '{c}'");
                }
            }
            throw new CsReaderException($"ERROR: NO KEY FOUND! -> {sb}");
        }

        internal static bool ReadStartTag(this CsStreamReader stream,
                                          out  string         key,
                                          out  string         baseTypeInfo,
                                          out  string         genericTypeInfo,
                                          out  string         dimensionInfo)
        {
            StringBuilder sb = new StringBuilder(128);

            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case ']':
                    {
                        string buffer = sb.ToString();

                        Match match = s_keyInfoMatcher.Match(buffer);
                        if (!match.Success)
                        {
                            key             = buffer;
                            baseTypeInfo    = string.Empty;
                            genericTypeInfo = string.Empty;
                            dimensionInfo   = string.Empty;
                            return false;
                        }

                        key = match.Groups[1].Success
                            ? match.Groups[1].Value
                            : throw new CsReaderException(
                                $"ERROR: KEY:INFO DOES NOT MATCH CONDITIONS! -> '{buffer}'");

                        baseTypeInfo = match.Groups[2].Success
                            ? match.Groups[2].Value
                            : throw new CsReaderException(
                                $"ERROR: BASE TYPE DOES NOT MATCH CONDITIONS! -> '{buffer}'");

                        genericTypeInfo = match.Groups[3].Success
                            ? match.Groups[3].Value
                            : string.Empty;

                        dimensionInfo = match.Groups[4].Success
                            ? match.Groups[4].Value
                            : string.Empty;
                        return true;
                    }
                    case '\n':
                    case '[':
                    case '\r':
                    case '\t':
                        throw new CsReaderException($"ERROR: INVALID KEY:INFO DEFINITION! -> invalid char '{c}'");
                }
                sb.Append(c);
            }
            throw new CsReaderException($"ERROR: NO KEY FOUND! -> {sb}");
        }

        internal static void ReadStartTag(this CsStreamReader stream, out string key, out string dimensionInfo)
        {
            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case '[':
                    {
                        stream.ReadStartTagInner(out key, out dimensionInfo);
                        return;
                    }
                    case '/':
                        throw new CsReaderException($"ERROR: INVALID TAG! -> invalid char '{c}'!");
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
            throw new CsReaderException("ERROR: NO START TAG FOUND -> \'[:]\'");
        }

        internal static void ReadTag(this CsStreamReader stream, string content)
        {
            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case '[':
                    {
                        stream.ReadTagInner(content);
                        return;
                    }
                    case '/':
                        throw new CsReaderException($"ERROR: INVALID TAG! -> invalid char '{c}'!");
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
            throw new CsReaderException($"ERROR: NO TAG FOUND -> '{content}'");
        }

        private static void ReadStartTagInner(this CsStreamReader stream, out string key, out string dimensionInfo)
        {
            StringBuilder sb = new StringBuilder(128);

            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case ']':
                    {
                        string buffer = sb.ToString();

                        Match match = s_kvInfoMatcher.Match(buffer);
                        if (!match.Success)
                        {
                            dimensionInfo = string.Empty;
                            throw new CsReaderException($"ERROR: INVALID START TAG DEFINITION! -> '{buffer}'");
                        }

                        key = match.Groups[1].Success
                            ? match.Groups[1].Value
                            : string.Empty;

                        dimensionInfo = match.Groups[2].Success
                            ? match.Groups[2].Value
                            : string.Empty;

                        return;
                    }
                    case '\n':
                    case '[':
                    case '\r':
                    case '\t':
                        throw new CsReaderException($"ERROR: INVALID START DEFINITION! -> invalid char '{c}'");
                }
                sb.Append(c);
            }
            throw new CsReaderException($"ERROR: NO KEY FOUND! -> {sb}");
        }

        private static void ReadTagInner(this CsStreamReader stream, string content)
        {
            StringBuilder sb = new StringBuilder(128);

            while (stream.ReadChar(out char c))
            {
                switch (c)
                {
                    case ']':
                    {
                        string buffer = sb.ToString();
                        if (buffer == content) { return; }
                        throw new CsReaderException($"ERROR: INVALID TAG DEFINITION! -> '{buffer}' != '{content}'");
                    }
                    case '\n':
                    case '[':
                    case '\r':
                    case '\t':
                        throw new CsReaderException($"ERROR: INVALID TAG DEFINITION! -> invalid char '{c}'");
                }
                sb.Append(c);
            }
            throw new CsReaderException($"ERROR: NO KEY FOUND! -> '{sb}'");
        }
    }
}