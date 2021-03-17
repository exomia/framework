#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Exomia.Framework.Core.ContentSerialization.Exceptions;

namespace Exomia.Framework.Core.ContentSerialization
{
    /// <summary>
    ///     A create struct extensions.
    /// </summary>
    static class CSExtensions
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

        /// <summary>
        ///     A string extension method that creates a type.
        /// </summary>
        /// <param name="typeInfo"> The typeInfo to act on. </param>
        /// <returns>
        ///     The new type.
        /// </returns>
        /// <exception cref="CSTypeException"> Thrown when a Create struct Type error condition occurs. </exception>
        internal static Type CreateType(this string typeInfo)
        {
            Type t = Type.GetType(typeInfo);
            if (t != null) { return t; }

            foreach (KeyValuePair<string, Assembly> pair in ContentSerializer.s_assemblies)
            {
                t = Type.GetType(typeInfo + ", " + pair.Value.FullName);
                if (t != null) { return t; }
            }
            throw new CSTypeException("Can't create type of: '" + typeInfo + "'");
        }

        /// <summary>
        ///     A string extension method that gets inner type.
        /// </summary>
        /// <param name="typeInfo">        The typeInfo to act on. </param>
        /// <param name="baseTypeInfo">    [out] Information describing the base type. </param>
        /// <param name="genericTypeInfo"> [out] Information describing the generic type. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        internal static void GetInnerType(this string typeInfo, out string baseTypeInfo, out string genericTypeInfo)
        {
            Match match = s_innerTypeMatcher.Match(typeInfo);
            if (!match.Success)
            {
                throw new CSReaderException($"ERROR: TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");
            }

            baseTypeInfo = match.Groups[1].Success
                ? match.Groups[1].Value
                : throw new CSReaderException($"ERROR: TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");

            genericTypeInfo = match.Groups[2].Success
                ? match.Groups[2].Value
                : string.Empty;
        }

        /// <summary>
        ///     A string extension method that gets key value inner type.
        /// </summary>
        /// <param name="typeInfo">             The typeInfo to act on. </param>
        /// <param name="keyBaseTypeInfo">      [out] Information describing the key base type. </param>
        /// <param name="valueBaseTypeInfo">    [out] Information describing the value base type. </param>
        /// <param name="valueGenericTypeInfo"> [out] Information describing the value generic type. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        internal static void GetKeyValueInnerType(this string typeInfo,
                                                  out  string keyBaseTypeInfo,
                                                  out  string valueBaseTypeInfo,
                                                  out  string valueGenericTypeInfo)
        {
            Match match = s_valueInnerTypeMatcher.Match(typeInfo);
            if (!match.Success)
            {
                throw new CSReaderException($"ERROR: KEY VALUE TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");
            }

            keyBaseTypeInfo = match.Groups[1].Success
                ? match.Groups[1].Value
                : throw new CSReaderException($"ERROR: KEY VALUE TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");

            valueBaseTypeInfo = match.Groups[2].Success
                ? match.Groups[2].Value
                : throw new CSReaderException($"ERROR: KEY VALUE TYPE INFO DOES NOT MATCH CONDITIONS! -> {typeInfo}");

            valueGenericTypeInfo = match.Groups[3].Success
                ? match.Groups[3].Value
                : string.Empty;
        }

        /// <summary>
        ///     A CSStreamReader extension method that reads end tag.
        /// </summary>
        /// <param name="stream"> The stream to act on. </param>
        /// <param name="key">    [out] The key. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        internal static void ReadEndTag(this CSStreamReader stream, string key)
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
                            throw new CSReaderException(
                                $"ERROR: INVALID END TAG DEFINITION! -> '{buffer}' != '/{key}'");
                        }
                    case '\n':
                    case '\r':
                    case '[':
                    case '\t':
                        throw new CSReaderException($"ERROR: INVALID END TAG DEFINITION! -> invalid char '{c}'");
                }
                sb.Append(c);
            }
            throw new CSReaderException($"ERROR: NO END TAG FOUND! -> {sb}");
        }

        /// <summary>
        ///     A CSStreamReader extension method that reads object start tag.
        /// </summary>
        /// <param name="stream"> The stream to act on. </param>
        /// <param name="key">    [out] The key. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        internal static void ReadObjectStartTag(this CSStreamReader stream, string key)
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
                            throw new CSReaderException($"ERROR: INVALID START TAG FOUND! -> '{buffer}' != '[{key}]'");
                        }
                    case '\n':
                    case '\r':
                    case '\t':
                        throw new CSReaderException($"ERROR: INVALID KEY:INFO DEFINITION! -> invalid char '{c}'");
                }
            }
            throw new CSReaderException($"ERROR: NO KEY FOUND! -> {sb}");
        }

        /// <summary>
        ///     A CSStreamReader extension method that reads start tag.
        /// </summary>
        /// <param name="stream">          The stream to act on. </param>
        /// <param name="key">             [out] The key. </param>
        /// <param name="baseTypeInfo">    [out] Information describing the base type. </param>
        /// <param name="genericTypeInfo"> [out] Information describing the generic type. </param>
        /// <param name="dimensionInfo">   [out] Information describing the dimension. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        internal static bool ReadStartTag(this CSStreamReader stream,
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
                                : throw new CSReaderException(
                                    $"ERROR: KEY:INFO DOES NOT MATCH CONDITIONS! -> '{buffer}'");

                            baseTypeInfo = match.Groups[2].Success
                                ? match.Groups[2].Value
                                : throw new CSReaderException(
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
                        throw new CSReaderException($"ERROR: INVALID KEY:INFO DEFINITION! -> invalid char '{c}'");
                }
                sb.Append(c);
            }
            throw new CSReaderException($"ERROR: NO KEY FOUND! -> {sb}");
        }

        /// <summary>
        ///     A CSStreamReader extension method that reads start tag.
        /// </summary>
        /// <param name="stream">        The stream to act on. </param>
        /// <param name="key">           [out] The key. </param>
        /// <param name="dimensionInfo"> [out] Information describing the dimension. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        internal static void ReadStartTag(this CSStreamReader stream, out string key, out string dimensionInfo)
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
                        throw new CSReaderException($"ERROR: INVALID TAG! -> invalid char '{c}'!");
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
            throw new CSReaderException("ERROR: NO START TAG FOUND -> \'[:]\'");
        }

        /// <summary>
        ///     A CSStreamReader extension method that reads a tag.
        /// </summary>
        /// <param name="stream">  The stream to act on. </param>
        /// <param name="content"> The content. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        internal static void ReadTag(this CSStreamReader stream, string content)
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
                        throw new CSReaderException($"ERROR: INVALID TAG! -> invalid char '{c}'!");
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
            throw new CSReaderException($"ERROR: NO TAG FOUND -> '{content}'");
        }

        /// <summary>
        ///     A CSStreamReader extension method that reads start tag inner.
        /// </summary>
        /// <param name="stream">        The stream to act on. </param>
        /// <param name="key">           [out] The key. </param>
        /// <param name="dimensionInfo"> [out] Information describing the dimension. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        private static void ReadStartTagInner(this CSStreamReader stream, out string key, out string dimensionInfo)
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
                                throw new CSReaderException($"ERROR: INVALID START TAG DEFINITION! -> '{buffer}'");
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
                        throw new CSReaderException($"ERROR: INVALID START DEFINITION! -> invalid char '{c}'");
                }
                sb.Append(c);
            }
            throw new CSReaderException($"ERROR: NO KEY FOUND! -> {sb}");
        }

        /// <summary>
        ///     A CSStreamReader extension method that reads tag inner.
        /// </summary>
        /// <param name="stream">  The stream to act on. </param>
        /// <param name="content"> The content. </param>
        /// <exception cref="CSReaderException">
        ///     Thrown when a Create struct Reader error condition
        ///     occurs.
        /// </exception>
        private static void ReadTagInner(this CSStreamReader stream, string content)
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
                            throw new CSReaderException($"ERROR: INVALID TAG DEFINITION! -> '{buffer}' != '{content}'");
                        }
                    case '\n':
                    case '[':
                    case '\r':
                    case '\t':
                        throw new CSReaderException($"ERROR: INVALID TAG DEFINITION! -> invalid char '{c}'");
                }
                sb.Append(c);
            }
            throw new CSReaderException($"ERROR: NO KEY FOUND! -> '{sb}'");
        }
    }
}