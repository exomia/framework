#pragma warning disable 1591

using System.Diagnostics;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     Current timing used for variable-step (real time) or fixed-step (game time) games.
    /// </summary>
    public sealed class GameTime
    {
        #region Constants

        private const float MAX_FRAME_TIME = 1000.0f / 60.0f;

        #endregion

        #region Constructors

        #region Statics

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameTime" /> class.
        /// </summary>
        public GameTime()
        {
            _baseTime = Stopwatch.GetTimestamp();
        }

        #endregion

        #region Variables

        #region Statics

        #endregion

        private readonly double _countsPerSec = 1.0 / Stopwatch.Frequency;
        private readonly double _countsPerMSec = 1000.0 / Stopwatch.Frequency;

        private long _baseTime;
        private long _pausedTime;
        private long _stopTime;
        private long _prevTime;
        private long _currTime;

        private bool _stopped;

        #endregion

        #region Properties

        #region Statics

        #endregion

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

        public float DeltaTimeS { get; private set; }

        public float DeltaTimeMS { get; private set; }

        public float AbsoluteDeltaTimeMS { get; private set; }

        public float AbsoluteDeltaTimeS { get; private set; }

        #endregion

        #region Methods

        #region Statics

        #endregion

        /// <summary>
        ///     reset the gametime
        /// </summary>
        public void Reset()
        {
            long curTime = Stopwatch.GetTimestamp();
            _baseTime = curTime;
            _prevTime = curTime;
            _stopTime = 0;
            _stopped = false;
        }

        /// <summary>
        ///     start the gametime
        /// </summary>
        public void Start()
        {
            long startTime = Stopwatch.GetTimestamp();
            _prevTime = _baseTime;
            if (_stopped)
            {
                _pausedTime += _baseTime - _stopTime;
                _stopTime = 0;
                _stopped = false;
            }
            Tick();
        }

        /// <summary>
        ///     stop the gametime
        /// </summary>
        public void Stop()
        {
            if (!_stopped)
            {
                long curTime = Stopwatch.GetTimestamp();
                _stopTime = curTime;
                _stopped = true;
            }
        }

        public void Tick()
        {
            if (_stopped)
            {
                AbsoluteDeltaTimeS = AbsoluteDeltaTimeMS = DeltaTimeS = DeltaTimeMS = 0;
                return;
            }
            _currTime = Stopwatch.GetTimestamp();
            DeltaTimeMS = (float)((_currTime - _prevTime) * _countsPerMSec);

            if (DeltaTimeMS < 0) { DeltaTimeMS = 0; }

            AbsoluteDeltaTimeMS = DeltaTimeMS;
            AbsoluteDeltaTimeS = DeltaTimeMS / 1000.0f;

            if (DeltaTimeMS > MAX_FRAME_TIME)
            {
                DeltaTimeMS = MAX_FRAME_TIME;
            }
            DeltaTimeS = DeltaTimeMS / 1000.0f;
            _prevTime = _currTime;
        }

        #endregion
    }
}