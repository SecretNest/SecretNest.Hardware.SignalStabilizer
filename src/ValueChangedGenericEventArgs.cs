using System;
using System.Collections.Generic;
using System.Text;

namespace SecretNest.Hardware
{
    public class ValueChangedGenericEventArgs : ValueChangedEventArgs<object>
    {
        public ValueChangedGenericEventArgs(object value) : base(value)
        { }
    }
}
