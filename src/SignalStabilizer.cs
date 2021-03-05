using System;
using System.Collections.Generic;
using System.Text;

namespace SecretNest.Hardware
{
    public class SignalStabilizer<T> : SignalStabilizerBase
    {
        public T Value => _currentValue;
        public override object ValueGeneric => _currentValue;

        private T _nextValue, _currentValue;

        public SignalStabilizer(TimeSpan waitingTime) : base(waitingTime)
        {
        }

        public override void SetValueGeneric(object value) => SetValue((T) value);

        private protected override void AnnounceValue()
        {
            var working = _nextValue;
            if (_currentValue.Equals(working)) return;
            _currentValue = working;
            OnValueChanged(working);
        }

        public void SetValue(T value)
        {
            _nextValue = value;
            AfterValueUpdated();
        }

        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

        private protected void OnValueChanged(T value)
        {
            ValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(value));
            OnValueChangedGeneric(value);
        }
    }
}
