#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Extensions;

/// <summary> A stream extension class. This class cannot be inherited. </summary>
public static class StreamExtensions
{
    /// <summary>
    ///     A Stream extension method that reads from given stream and determines weather the read bytes and given sequence are equal.
    /// </summary>
    /// <param name="stream"> The stream to read from. </param>
    /// <param name="sequence"> The sequence to compare against. </param>
    /// <returns> True if the given and read sequence are equal; false otherwise. </returns>
    public static bool SequenceEqual(this Stream stream, Span<byte> sequence)
    {
        Span<byte> buffer = stackalloc byte[sequence.Length];

        return stream.Read(buffer) == sequence.Length &&
            buffer.SequenceEqual(sequence);
    }
}