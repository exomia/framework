#pragma warning disable 1591
using System;
using Exomia.Framework.Game;

namespace Exomia.Framework.Tools
{
    public delegate void TimerEvent(Timer2 timer);

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class Timer2 : IUpdateable
    {
        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> EnabledChanged;

        public event TimerEvent TimerTicked;
        public event TimerEvent TimerFinished;

        private bool _enabled;
        private float _elapsedTime;

        private readonly uint _maxIterations;
        private int _updateOrder;

        #endregion

        #region Properties

        #region Statics

        #endregion

        /// <summary>
        ///     Gets or sets the timer tick(time in ms after a timer Tick occurs)
        /// </summary>
        public float TimerTick { get; set; }

        /// <summary>
        ///     Gets or sets the enabled state
        /// </summary>
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
        ///     Gets or sets the update order
        /// </summary>
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
        ///     Gets the current iteration or 0 if maxIteration = 0
        /// </summary>
        public uint CurrentIteration { get; private set; }

        #endregion

        #region Constructors

        #region Statics

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer2" /> class.
        /// </summary>
        /// <param name="tick">time in ms after a timer Tick occurs</param>
        /// <param name="maxIterations">set the max iteration count for this timer or 0 for unlimited</param>
        public Timer2(float tick, uint maxIterations = 0)
        {
            TimerTick = tick;
            _maxIterations = maxIterations;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer2" /> class.
        /// </summary>
        /// <param name="tick">time in ms after a timer Tick occurs</param>
        /// <param name="tickCallback">callback for each tick event</param>
        /// <param name="maxIterations">set the max iteration count for this timer or 0 for unlimited</param>
        public Timer2(float tick, TimerEvent tickCallback, uint maxIterations = 0)
            : this(tick, maxIterations)
        {
            TimerTicked += tickCallback;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Timer2" /> class.
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

        #endregion

        #region Methods

        #region Statics

        #endregion

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

        public void Reset()
        {
            _elapsedTime = 0;
            CurrentIteration = 0;
            _enabled = true;
        }

        #endregion
    }
}