using System;
using System.Collections.Generic;
using System.Text;

namespace SecretNest.Hardware
{
    public abstract class ValueChangedEventArgsBase : EventArgs
    {
        public abstract object ValueGeneric { get; }
    }
}
