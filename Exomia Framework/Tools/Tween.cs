#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Reflection;
using Exomia.Framework.Game;

namespace Exomia.Framework.Tools
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class Tween : IUpdateable
    {
        #region Constructors

        #region Statics

        #endregion

        public Tween(object target, object values, float duration, float delay, EasingFunction callback)
        {
            if (values == null) { throw new ArgumentNullException(nameof(values)); }

            _target = target ?? throw new ArgumentNullException(nameof(target));
            _duration = duration;
            _delay = delay;
            _callback = callback;

            _items = new List<TweenItem>();

            Type valueType = values.GetType();
            Type targetType = target.GetType();

            foreach (PropertyInfo info in valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                PropertyInfo info2 = targetType.GetProperty(info.Name);
                if (info2 == null) { return; }
                float from = (float)Convert.ChangeType(info2.GetValue(target, null), typeof(float));
                float to = (float)Convert.ChangeType(info.GetValue(values, null), typeof(float));

                _items.Add(new TweenItem(from, to, info2));
            }
        }

        #endregion

        #region Methods

        #region Statics

        #endregion

        public void Update(GameTime gameTime)
        {
            _delayTime = Math.Min(_delayTime + gameTime.DeltaTimeMS, _delay);
            if (_delayTime < _delay) { return; }

            _time += gameTime.DeltaTimeMS;

            if (_time > _duration)
            {
                _time = _duration;
                _enabled = false;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                TweenItem item = _items[i];
                float value = _callback(_time, item.From, item.To, _duration);
                item.PropertyInfo.SetValue(_target, Convert.ChangeType(value, item.PropertyInfo.PropertyType), null);
            }
        }

        #endregion

        private class TweenItem
        {
            public TweenItem(float from, float to, PropertyInfo info)
            {
                From = from;
                To = to;
                PropertyInfo = info;
            }

            public float From { get; }
            public float To { get; }
            public PropertyInfo PropertyInfo { get; }
        }

        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> EnabledChanged;

        private readonly EasingFunction _callback;

        private bool _enabled;
        private int _updateOrder;

        private readonly object _target;
        private readonly float _duration;
        private readonly float _delay;

        private float _time;
        private float _delayTime;

        private readonly List<TweenItem> _items;

        #endregion

        #region Properties

        #region Statics

        #endregion

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

        #endregion
    }
}