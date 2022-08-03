#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Application;

/// <summary>
///     Current timing used for variable-step (real time) or fixed-step (application time) games.
/// </summary>
public sealed class Time
{
    private static readonly double s_countsPerMSec = 1000.0 / Stopwatch.Frequency;
    private static readonly double s_countsPerSec  = 1.0    / Stopwatch.Frequency;
    private                 long   _baseTime, _currTime, _pausedTime, _prevTime, _stopTime;
    private                 bool   _stopped;
    private                 float  _deltaTimeMs;
    private                 float  _deltaTimeS;

    /// <summary>
    ///     Gets the delta time in milliseconds.
    /// </summary>
    /// <value>
    ///     The delta time in milliseconds.
    /// </value>
    public float DeltaTimeMs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _deltaTimeMs; }
    }

    /// <summary>
    ///     Gets the delta time in seconds.
    /// </summary>
    /// <value>
    ///     The delta time in seconds.
    /// </value>
    public float DeltaTimeS
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _deltaTimeS; }
    }

    /// <summary>
    ///     Gets the total number of time in milliseconds.
    /// </summary>
    /// <value>
    ///     The total number of time in milliseconds.
    /// </value>
    public float TotalTimeMs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_stopped)
            {
                return (float)((_stopTime - _pausedTime - _baseTime) * s_countsPerMSec);
            }
            return (float)((_currTime - _pausedTime - _baseTime) * s_countsPerMSec);
        }
    }

    /// <summary>
    ///     Gets the total number of time in seconds.
    /// </summary>
    /// <value>
    ///     The total number of time in seconds.
    /// </value>
    public float TotalTimeS
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (_stopped)
            {
                return (float)((_stopTime - _pausedTime - _baseTime) * s_countsPerSec);
            }
            return (float)((_currTime - _pausedTime - _baseTime) * s_countsPerSec);
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Time" /> class.
    /// </summary>
    private Time()
    {
        _prevTime = _baseTime = Stopwatch.GetTimestamp();
    }

    /// <summary>
    ///     Initializes a new <see cref="Time" /> instance,
    ///     sets the (delta) time to zero and starts measuring the elapsed time since start and delta time between each ticks.
    /// </summary>
    /// <returns>
    ///     A <see cref="Time" />.
    /// </returns>
    public static Time StartNew()
    {
        return new Time();
    }

    /// <summary>
    ///     reset the time.
    /// </summary>
    public void Reset()
    {
        _prevTime = _baseTime = Stopwatch.GetTimestamp();
        _stopTime = 0;
        _stopped  = false;
    }

    /// <summary>
    ///     start the time.
    /// </summary>
    public void Start()
    {
        _prevTime = _baseTime = Stopwatch.GetTimestamp();
        if (_stopped)
        {
            _pausedTime += _baseTime - _stopTime;
            _stopTime   =  0;
            _stopped    =  false;
        }
        Tick();
    }

    /// <summary>
    ///     stop the time.
    /// </summary>
    public void Stop()
    {
        if (!_stopped)
        {
            _stopTime = Stopwatch.GetTimestamp();
            _stopped  = true;
        }
    }

    /// <summary>
    ///     Perform a tick and update the delta times.
    /// </summary>
    public void Tick()
    {
        if (_stopped)
        {
            _deltaTimeS = _deltaTimeMs = 0;
            return;
        }

        _deltaTimeS = (_deltaTimeMs = (float)(((_currTime = Stopwatch.GetTimestamp()) - _prevTime) * s_countsPerMSec)) * 0.001f;
        _prevTime   = _currTime;
    }
}