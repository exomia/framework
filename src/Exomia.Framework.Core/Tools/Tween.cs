#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Reflection;
using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Core.Tools;

/// <summary>
///     A tween. This class cannot be inherited.
/// </summary>
public sealed class Tween : IUpdateable
{
    /// <summary>
    ///     Occurs when Enabled Changed.
    /// </summary>
    /// <inheritdoc />
    public event EventHandler? EnabledChanged;

    /// <summary>
    ///     Occurs when Update Order Changed.
    /// </summary>
    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

    private readonly EasingFunction  _callback;
    private readonly float           _delay, _duration;
    private readonly List<TweenItem> _items;
    private readonly object          _target;
    private          float           _delayTime, _time;
    private          bool            _enabled;
    private          int             _updateOrder;

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
    ///     Initializes a new instance of the <see cref="Tween" /> class.
    /// </summary>
    /// <param name="target">   Target for the. </param>
    /// <param name="values">   The values. </param>
    /// <param name="duration"> The duration. </param>
    /// <param name="delay">    The delay. </param>
    /// <param name="callback"> The callback. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    public Tween(object target, object values, float duration, float delay, EasingFunction callback)
    {
        if (values == null) { throw new ArgumentNullException(nameof(values)); }

        _target   = target ?? throw new ArgumentNullException(nameof(target));
        _duration = duration;
        _delay    = delay;
        _callback = callback;

        _items = new List<TweenItem>();

        Type valueType  = values.GetType();
        Type targetType = target.GetType();

        foreach (PropertyInfo info in valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            PropertyInfo? info2 = targetType.GetProperty(info.Name);
            if (info2 == null) { return; }
            float from = (float)Convert.ChangeType(info2.GetValue(target, null), typeof(float))!;
            float to   = (float)Convert.ChangeType(info.GetValue(values, null),  typeof(float))!;

            _items.Add(new TweenItem(from, to, info2));
        }
    }

    /// <inheritdoc />
    public void Update(Time time)
    {
        _delayTime = Math.Min(_delayTime + time.DeltaTimeMs, _delay);
        if (_delayTime < _delay) { return; }

        _time += time.DeltaTimeMs;

        if (_time > _duration)
        {
            _time    = _duration;
            _enabled = false;
        }

        for (int i = 0; i < _items.Count; i++)
        {
            TweenItem item  = _items[i];
            float     value = _callback(_time, item.From, item.To, _duration);
            item.PropertyInfo.SetValue(_target, Convert.ChangeType(value, item.PropertyInfo.PropertyType), null);
        }
    }

    /// <summary>
    ///     A tween item.
    /// </summary>
    private class TweenItem
    {
        /// <summary>
        ///     Gets the source for the.
        /// </summary>
        /// <value>
        ///     from.
        /// </value>
        public float From { get; }

        /// <summary>
        ///     Gets information describing the property.
        /// </summary>
        /// <value>
        ///     Information describing the property.
        /// </value>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        ///     Gets to.
        /// </summary>
        /// <value>
        ///     to.
        /// </value>
        public float To { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TweenItem" /> class.
        /// </summary>
        /// <param name="from"> Source for the. </param>
        /// <param name="to">   to. </param>
        /// <param name="info"> The information. </param>
        public TweenItem(float from, float to, PropertyInfo info)
        {
            From         = from;
            To           = to;
            PropertyInfo = info;
        }
    }
}