#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Tools;

namespace Exomia.Framework.Core.Application;

public abstract partial class Application
{
    /// <summary> Adds a timer. </summary>
    /// <param name="tick"> The tick. </param>
    /// <param name="enabled"> True to enable, false to disable. </param>
    /// <param name="maxIterations"> (Optional) The maximum iterations. </param>
    /// <param name="removeAfterFinished"> (Optional) True if remove after finished. </param>
    /// <returns> A Timer2. </returns>
    public Timer2 AddTimer(float tick, bool enabled, uint maxIterations = 0, bool removeAfterFinished = false)
    {
        Timer2 timer = Add(new Timer2(tick, maxIterations) { Enabled = enabled });
        if (removeAfterFinished)
        {
            timer.TimerFinished += sender => { Remove(sender); };
        }
        return timer;
    }

    /// <summary> Adds a timer. </summary>
    /// <param name="tick"> The tick. </param>
    /// <param name="enabled"> True to enable, false to disable. </param>
    /// <param name="tickCallback"> The tick callback. </param>
    /// <param name="maxIterations"> (Optional) The maximum iterations. </param>
    /// <param name="removeAfterFinished"> (Optional) True if remove after finished. </param>
    /// <returns> A Timer2. </returns>
    public Timer2 AddTimer(float                tick,
                           bool                 enabled,
                           EventHandler<Timer2> tickCallback,
                           uint                 maxIterations       = 0,
                           bool                 removeAfterFinished = false)
    {
        Timer2 timer = Add(new Timer2(tick, tickCallback, maxIterations) { Enabled = enabled });
        if (removeAfterFinished)
        {
            timer.TimerFinished += sender => { Remove(sender); };
        }
        return timer;
    }

    /// <summary> Adds a timer. </summary>
    /// <param name="tick"> The tick. </param>
    /// <param name="enabled"> True to enable, false to disable. </param>
    /// <param name="tickCallback"> The tick callback. </param>
    /// <param name="finishedCallback"> The finished callback. </param>
    /// <param name="maxIterations"> The maximum iterations. </param>
    /// <param name="removeAfterFinished"> (Optional) True if remove after finished. </param>
    /// <returns> A Timer2. </returns>
    public Timer2 AddTimer(float                tick,
                           bool                 enabled,
                           EventHandler<Timer2> tickCallback,
                           EventHandler<Timer2> finishedCallback,
                           uint                 maxIterations,
                           bool                 removeAfterFinished = false)
    {
        Timer2 timer = Add(new Timer2(tick, tickCallback, finishedCallback, maxIterations) { Enabled = enabled });
        if (removeAfterFinished)
        {
            timer.TimerFinished += sender => { Remove(sender); };
        }
        return timer;
    }
}