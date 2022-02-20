﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;

namespace Exomia.Framework.Core.Game;

/// <summary>
///     Current timing used for variable-step (real time) or fixed-step (game time) games.
/// </summary>
public sealed class GameTime
{
    private const    float  MAX_FRAME_TIME = 1000.0f / 60.0f;
    private readonly double _countsPerMSec = 1000.0 / Stopwatch.Frequency;
    private readonly double _countsPerSec  = 1.0 / Stopwatch.Frequency;
    private          long   _baseTime, _currTime, _pausedTime, _prevTime, _stopTime;
    private          bool   _stopped;

    /// <summary>
    ///     this value will be limited to <see cref="MAX_FRAME_TIME" /> (1000.0f / 60.0f)
    /// </summary>
    /// <value>
    ///     The limited delta time milliseconds.
    /// </value>
    public float LimitedDeltaTimeMs { get; private set; }

    /// <summary>
    ///     this value will be limited to <see cref="MAX_FRAME_TIME" /> (1.0f / 60.0f)
    /// </summary>
    /// <value>
    ///     The limited delta time s.
    /// </value>
    public float LimitedDeltaTimeS { get; private set; }

    /// <summary>
    ///     Gets the delta time milliseconds.
    /// </summary>
    /// <value>
    ///     The delta time milliseconds.
    /// </value>
    public float DeltaTimeMs { get; private set; }

    /// <summary>
    ///     Gets the delta time s.
    /// </summary>
    /// <value>
    ///     The delta time s.
    /// </value>
    public float DeltaTimeS { get; private set; }

    /// <summary>
    ///     Gets the total number of time milliseconds.
    /// </summary>
    /// <value>
    ///     The total number of time milliseconds.
    /// </value>
    public float TotalTimeMs
    {
        get
        {
            if (_stopped)
            {
                return (float)((_stopTime - _pausedTime - _baseTime) * _countsPerMSec);
            }
            return (float)((_currTime - _pausedTime - _baseTime) * _countsPerMSec);
        }
    }

    /// <summary>
    ///     Gets the total number of time s.
    /// </summary>
    /// <value>
    ///     The total number of time s.
    /// </value>
    public float TotalTimeS
    {
        get
        {
            if (_stopped)
            {
                return (float)((_stopTime - _pausedTime - _baseTime) * _countsPerSec);
            }
            return (float)((_currTime - _pausedTime - _baseTime) * _countsPerSec);
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameTime" /> class.
    /// </summary>
    private GameTime()
    {
        _prevTime = _baseTime = Stopwatch.GetTimestamp();
    }

    /// <summary>
    ///     Starts a new.
    /// </summary>
    /// <returns>
    ///     A GameTime.
    /// </returns>
    public static GameTime StartNew()
    {
        return new GameTime();
    }

    /// <summary>
    ///     reset the game time.
    /// </summary>
    public void Reset()
    {
        _prevTime = _baseTime = Stopwatch.GetTimestamp();
        _stopTime = 0;
        _stopped  = false;
    }

    /// <summary>
    ///     start the game time.
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
    ///     stop the game time.
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
    ///     Ticks this object.
    /// </summary>
    public void Tick()
    {
        if (_stopped)
        {
            LimitedDeltaTimeS = LimitedDeltaTimeMs = DeltaTimeS = DeltaTimeMs = 0;
            return;
        }

        _currTime = Stopwatch.GetTimestamp();

        DeltaTimeMs = (float)((_currTime - _prevTime) * _countsPerMSec);
        DeltaTimeS  = DeltaTimeMs * 0.001f;

        LimitedDeltaTimeMs = DeltaTimeMs;
        if (LimitedDeltaTimeMs > MAX_FRAME_TIME)
        {
            LimitedDeltaTimeMs = MAX_FRAME_TIME;
        }
        LimitedDeltaTimeS = LimitedDeltaTimeMs * 0.001f;

        _prevTime = _currTime;
    }
}