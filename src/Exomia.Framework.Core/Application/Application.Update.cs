#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Application;

public abstract partial class Application
{
    /// <summary> Update the application (logic) for the frame. </summary>
    /// <param name="time"> The time. </param>
    protected virtual void Update(Time time)
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