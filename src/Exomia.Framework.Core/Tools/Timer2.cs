#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core.Tools;

/// <summary>
///     A timer 2. This class cannot be inherited.
/// </summary>
public sealed class Timer2 : IUpdateable
{
    /// <summary>
    ///     Occurs when Enabled Changed.
    /// </summary>
    public event EventHandler? EnabledChanged;

    /// <summary>
    ///     Occurs when Update Order Changed.
    /// </summary>
    public event EventHandler? UpdateOrderChanged;

    /// <summary>
    ///     timer finished event.
    /// </summary>
    public event EventHandler<Timer2>? TimerFinished;

    /// <summary>
    ///     timer ticked event.
    /// </summary>
    public event EventHandler<Timer2>? TimerTicked;

    private readonly uint  _maxIterations;
    private          float _elapsedTime;
    private          bool  _enabled;
    private          int   _updateOrder;

    /// <summary>
    ///     Gets the current iteration or 0 if maxIteration = 0.
    /// </summary>
    /// <value>
    ///     The current iteration.
    /// </value>
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
                EnabledChanged?.Invoke();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the timer tick(time in ms after a timer Tick occurs)
    /// </summary>
    /// <value>
    ///     The timer tick.
    /// </value>
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
                UpdateOrderChanged?.Invoke();
            }
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Timer2" /> class.
    /// </summary>
    /// <param name="tick">          time in ms after a timer Tick occurs. </param>
    /// <param name="maxIterations">
    ///     (Optional) set the max iteration count for this timer or 0 for
    ///     unlimited.
    /// </param>
    public Timer2(float tick, uint maxIterations = 0)
    {
        TimerTick      = tick;
        _maxIterations = maxIterations;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="T:Exomia.Framework.Core.Tools.Timer2" /> class.
    /// </summary>
    /// <param name="tick">          time in ms after a timer Tick occurs. </param>
    /// <param name="tickCallback">  callback for each tick event. </param>
    /// <param name="maxIterations">
    ///     (Optional) set the max iteration count for this timer or 0 for
    ///     unlimited.
    /// </param>
    public Timer2(float tick, EventHandler<Timer2> tickCallback, uint maxIterations = 0)
        : this(tick, maxIterations)
    {
        TimerTicked += tickCallback;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="T:Exomia.Framework.Core.Tools.Timer2" /> class.
    /// </summary>
    /// <param name="tick">             time in ms after a timer Tick occurs. </param>
    /// <param name="tickCallback">     callback for each tick event. </param>
    /// <param name="finishedCallback"> callback for timer finished. </param>
    /// <param name="maxIterations">
    ///     set the max iteration count for this timer or 0 for
    ///     unlimited.
    /// </param>
    public Timer2(float                tick,
                  EventHandler<Timer2> tickCallback,
                  EventHandler<Timer2> finishedCallback,
                  uint                 maxIterations)
        : this(tick, tickCallback, maxIterations)
    {
        TimerFinished += finishedCallback;
    }

    /// <inheritdoc />
    public void Update(Time time)
    {
        if (!_enabled) { return; }

        _elapsedTime += time.DeltaTimeMs;
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
    ///     Resets this object.
    /// </summary>
    public void Reset()
    {
        _elapsedTime     = 0;
        CurrentIteration = 0;
        _enabled         = true;
    }
}