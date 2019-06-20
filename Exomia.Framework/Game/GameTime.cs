#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     Current timing used for variable-step (real time) or fixed-step (game time) games.
    /// </summary>
    public sealed class GameTime
    {
        /// <summary>
        ///     The maximum frame time.
        /// </summary>
        private const float MAX_FRAME_TIME = 1000.0f / 60.0f;

        /// <summary>
        ///     The counts per m security.
        /// </summary>
        private readonly double _countsPerMSec = 1000.0 / Stopwatch.Frequency;

        /// <summary>
        ///     The counts per security.
        /// </summary>
        private readonly double _countsPerSec = 1.0 / Stopwatch.Frequency;

        /// <summary>
        ///     The base time.
        /// </summary>
        private long _baseTime;

        /// <summary>
        ///     The curr time.
        /// </summary>
        private long _currTime;

        /// <summary>
        ///     The paused time.
        /// </summary>
        private long _pausedTime;

        /// <summary>
        ///     The previous time.
        /// </summary>
        private long _prevTime;

        /// <summary>
        ///     True if stopped.
        /// </summary>
        private bool _stopped;

        /// <summary>
        ///     The stop time.
        /// </summary>
        private long _stopTime;

        /// <summary>
        ///     this value will be limited to <see cref="MAX_FRAME_TIME" /> (1000.0f / 60.0f)
        /// </summary>
        /// <value>
        ///     The limited delta time milliseconds.
        /// </value>
        public float LimitedDeltaTimeMS { get; private set; }

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
        public float DeltaTimeMS { get; private set; }

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
        public float TotalTimeMS
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
                LimitedDeltaTimeS = LimitedDeltaTimeMS = DeltaTimeS = DeltaTimeMS = 0;
                return;
            }

            _currTime = Stopwatch.GetTimestamp();

            DeltaTimeMS = (float)((_currTime - _prevTime) * _countsPerMSec);
            DeltaTimeS  = DeltaTimeMS * 0.001f;

            LimitedDeltaTimeMS = DeltaTimeMS;
            if (LimitedDeltaTimeMS > MAX_FRAME_TIME)
            {
                LimitedDeltaTimeMS = MAX_FRAME_TIME;
            }
            LimitedDeltaTimeS = LimitedDeltaTimeMS * 0.001f;

            _prevTime = _currTime;
        }
    }
}