#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#pragma warning disable 1591

using System.Diagnostics;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     Current timing used for variable-step (real time) or fixed-step (game time) games.
    /// </summary>
    public sealed class GameTime
    {
        private const float MAX_FRAME_TIME = 1000.0f / 60.0f;
        private readonly double _countsPerMSec = 1000.0 / Stopwatch.Frequency;

        private readonly double _countsPerSec = 1.0 / Stopwatch.Frequency;

        private long _baseTime;
        private long _currTime;
        private long _pausedTime;
        private long _prevTime;

        private bool _stopped;
        private long _stopTime;

        public float AbsoluteDeltaTimeMS { get; private set; }

        public float AbsoluteDeltaTimeS { get; private set; }

        public float DeltaTimeMS { get; private set; }

        public float DeltaTimeS { get; private set; }

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
        public GameTime()
        {
            _baseTime = Stopwatch.GetTimestamp();
        }

        /// <summary>
        ///     reset the game time
        /// </summary>
        public void Reset()
        {
            _prevTime = _baseTime = Stopwatch.GetTimestamp();
            _stopTime = 0;
            _stopped  = false;
        }

        /// <summary>
        ///     start the game time
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
        ///     stop the game time
        /// </summary>
        public void Stop()
        {
            if (!_stopped)
            {
                _stopTime = Stopwatch.GetTimestamp();
                _stopped  = true;
            }
        }

        public void Tick()
        {
            if (_stopped)
            {
                AbsoluteDeltaTimeS = AbsoluteDeltaTimeMS = DeltaTimeS = DeltaTimeMS = 0;
                return;
            }
            _currTime   = Stopwatch.GetTimestamp();
            DeltaTimeMS = (float)((_currTime - _prevTime) * _countsPerMSec);

            if (DeltaTimeMS < 0) { DeltaTimeMS = 0; }

            AbsoluteDeltaTimeMS = DeltaTimeMS;
            AbsoluteDeltaTimeS  = DeltaTimeMS / 1000.0f;

            if (DeltaTimeMS > MAX_FRAME_TIME)
            {
                DeltaTimeMS = MAX_FRAME_TIME;
            }
            DeltaTimeS = DeltaTimeMS / 1000.0f;
            _prevTime  = _currTime;
        }
    }
}