using System;
using System.Collections.Generic;
using System.Text;

namespace SecretNest.Hardware
{
    public class ValueChangedEventArgs<T> : ValueChangedEventArgsBase
    {
        public ValueChangedEventArgs(T value)
        {
            Value = value;
        }

        public override object ValueGeneric => Value;

        public T Value { get; }
    }
}
