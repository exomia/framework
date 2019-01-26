#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
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

using Exomia.Framework.Game;

namespace Exomia.Framework.Tools
{
    /// <inheritdoc />
    public sealed class Tween : IUpdateable
    {
        /// <inheritdoc />
        public event EventHandler EnabledChanged;

        /// <inheritdoc />
        public event EventHandler UpdateOrderChanged;

        private readonly EasingFunction _callback;
        private readonly float _delay;
        private readonly float _duration;

        private readonly List<TweenItem> _items;

        private readonly object _target;
        private float _delayTime;

        private bool _enabled;

        private float _time;
        private int _updateOrder;

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

        /// <inheritdoc />
        public Tween(object target, object values, float duration, float delay, EasingFunction callback)
        {
            if (values == null) { throw new ArgumentNullException(nameof(values)); }

            _target   = target ?? throw new ArgumentNullException(nameof(target));
            _duration = duration;
            _delay    = delay;
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

        /// <inheritdoc />
        public void Update(GameTime gameTime)
        {
            _delayTime = Math.Min(_delayTime + gameTime.DeltaTimeMS, _delay);
            if (_delayTime < _delay) { return; }

            _time += gameTime.DeltaTimeMS;

            if (_time > _duration)
            {
                _time    = _duration;
                _enabled = false;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                TweenItem item = _items[i];
                float value = _callback(_time, item.From, item.To, _duration);
                item.PropertyInfo.SetValue(_target, Convert.ChangeType(value, item.PropertyInfo.PropertyType), null);
            }
        }

        private class TweenItem
        {
            public float From { get; }
            public PropertyInfo PropertyInfo { get; }
            public float To { get; }

            public TweenItem(float from, float to, PropertyInfo info)
            {
                From         = from;
                To           = to;
                PropertyInfo = info;
            }
        }
    }
}