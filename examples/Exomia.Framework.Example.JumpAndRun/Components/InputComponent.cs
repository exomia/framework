#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.ECS.Attributes;

namespace Exomia.Framework.Example.JumpAndRun.Components
{
    [EntityComponentConfiguration(PoolSize = 8)]
    sealed class InputComponent
    {
        public bool Left  { get; set; }
        public bool Right { get; set; }
        public bool Jump  { get; set; }
    }
}