using System;
using System.Collections.Generic;
using System.Text;

namespace SecretNest.Hardware
{
    public class SignalStabilizer<T> : SignalStabilizerBase
    {
        public T Value => _currentValue;
        public override object ValueGeneric => _currentValue;

        private T _nextValue, _comparedValue, _currentValue;

        private object _lock = new object();

        public SignalStabilizer(TimeSpan waitingTime, T initial = default(T)) : base(waitingTime)
        {
            _currentValue = initial;
        }

        public SignalStabilizer(int waitingTimeMilliseconds, T initial = default(T)) : base(waitingTimeMilliseconds)
        {
            _currentValue = initial;
        }

        public override void SetValueGeneric(object value) => SetValue((T) value);

        private protected override bool IsNextChanged()
        {
            var working = _nextValue;
            if (working.Equals(_comparedValue))
            {
                return false;
            }
            else
            {
                _comparedValue = working;
                return true;
            }
        }

        private protected override void AnnounceValueIfChanged()
        {
            var working = _nextValue;
            if (_currentValue.Equals(working)) return;
            _currentValue = working;
            OnValueChanged(working);
        }

        public void SetValue(T value)
        {
            BeforeValueUpdating();
            _nextValue = value;
            AfterValueUpdating();
        }

        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

        private protected void OnValueChanged(T value)
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(value));
            OnValueChangedGeneric(value);
        }
    }
}
