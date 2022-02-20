#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Input;

/// <summary> Interface for raw input handler. </summary>
public interface IInputHandler
{
    /// <summary> Register the input events on the given input device. </summary>
    /// <param name="device"> The device. </param>
    void RegisterInput(IInputDevice device);

    /// <summary> Unregister the input events on the given input device. </summary>
    /// <param name="device"> The device. </param>
    void UnregisterInput(IInputDevice device);
}