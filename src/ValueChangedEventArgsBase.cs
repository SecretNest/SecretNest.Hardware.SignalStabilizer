using System;

namespace SecretNest.Hardware
{
    public abstract class ValueChangedEventArgsBase : EventArgs
    {
        public abstract object ValueGeneric { get; }
    }
}
