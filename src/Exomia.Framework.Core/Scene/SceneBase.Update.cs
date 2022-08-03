#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core.Scene;

public abstract partial class SceneBase
{
    /// <summary> Update the scene (logic) for the frame. </summary>
    /// <param name="time"> The time. </param>
    public virtual void Update(Time time)
    {
        lock (_updateableComponents)
        {
            _currentlyUpdateableComponents.AddRange(_updateableComponents);
        }

        for (int i = 0; i < _currentlyUpdateableComponents.Count; i++)
        {
            IUpdateable updateable = _currentlyUpdateableComponents[i];
            if (updateable.Enabled)
            {
                updateable.Update(time);
            }
        }

        _currentlyUpdateableComponents.Clear();
    }
}