#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;

namespace Exomia.Framework.Core.Application;

public abstract unsafe partial class Application
{
    private void RenderloopVariableTime(Time time)
    {
        while (!_shutdown)
        {
            _doEvents();

            if (!_isRunning)
            {
                Thread.Sleep(16);
                continue;
            }

            Update(time);

            if (BeginFrame())
            {
                Render(time);
                EndFrame();
            }

            time.Tick();
        }

        _isShutdownCompleted.Set();
    }

    private void RenderloopFixedTime(Time time)
    {
        Stopwatch stopwatch = new Stopwatch();

        while (!_shutdown)
        {
            stopwatch.Restart();

            _doEvents();

            if (!_isRunning)
            {
                Thread.Sleep(16);
                continue;
            }

            Update(time);

            if (BeginFrame())
            {
                Render(time);
                EndFrame();
            }

            //SLEEP
            while (TargetElapsedTime - FIXED_TIMESTAMP_THRESHOLD > stopwatch.Elapsed.TotalMilliseconds)
            {
                Thread.Yield();
            }

            //IDLE
            while (stopwatch.Elapsed.TotalMilliseconds < TargetElapsedTime) { }

            time.Tick();
        }

        _isShutdownCompleted.Set();
    }
}