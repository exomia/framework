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

using System;
using Exomia.Framework.Game;

namespace Exomia.Framework.Tools
{
    /// <summary>
    /// </summary>
    /// <param name="timer">sender instance</param>
    public delegate void TimerEvent(Timer2 timer);

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class Timer2 : IUpdateable
    {
        /// <inheritdoc />
        public event EventHandler<EventArgs> EnabledChanged;

        /// <summary>
        /// </summary>
        public event TimerEvent TimerFinished;

        /// <summary>
        /// </summary>
        public event TimerEvent TimerTicked;

        /// <inheritdoc />
        public event EventHandler<EventArgs> UpdateOrderChanged;

        private readonly uint _maxIterations;
        private float _elapsedTime;

        private bool _enabled;
        private int _updateOrder;

        /// <summary>
        ///     Gets the current iteration or 0 if maxIteration = 0
        /// </summary>
        public uint CurrentIteration { get; private set; }

        /// <inheritdoc />
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the timer tick(time in ms after a timer Tick occurs)
        /// </summary>
        public float TimerTick { get; set; }

        /// <inheritdoc />
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer2" /> class.
        /// </summary>
        /// <param name="tick">time in ms after a timer Tick occurs</param>
        /// <param name="maxIterations">set the max iteration count for this timer or 0 for unlimited</param>
        public Timer2(float tick, uint maxIterations = 0)
        {
            TimerTick      = tick;
            _maxIterations = maxIterations;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Exomia.Framework.Tools.Timer2" /> class.
        /// </summary>
        /// <param name="tick">time in ms after a timer Tick occurs</param>
        /// <param name="tickCallback">callback for each tick event</param>
        /// <param name="maxIterations">set the max iteration count for this timer or 0 for unlimited</param>
        public Timer2(float tick, TimerEvent tickCallback, uint maxIterations = 0)
            : this(tick, maxIterations)
        {
            TimerTicked += tickCallback;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Exomia.Framework.Tools.Timer2" /> class.
        /// </summary>
        /// <param name="tick">time in ms after a timer Tick occurs</param>
        /// <param name="tickCallback">callback for each tick event</param>
        /// <param name="finishedCallback">callback for timer finished</param>
        /// <param name="maxIterations">set the max iteration count for this timer or 0 for unlimited</param>
        public Timer2(float tick, TimerEvent tickCallback, TimerEvent finishedCallback, uint maxIterations)
            : this(tick, tickCallback, maxIterations)
        {
            TimerFinished += finishedCallback;
        }

        /// <inheritdoc />
        public void Update(GameTime gameTime)
        {
            if (!_enabled) { return; }

            _elapsedTime += gameTime.DeltaTimeMS;
            if (_elapsedTime >= TimerTick)
            {
                _elapsedTime -= TimerTick;
                TimerTicked?.Invoke(this);

                if (_maxIterations > 0 && ++CurrentIteration >= _maxIterations)
                {
                    _enabled = false;
                    TimerFinished?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// </summary>
        public void Reset()
        {
            _elapsedTime     = 0;
            CurrentIteration = 0;
            _enabled         = true;
        }
    }
}