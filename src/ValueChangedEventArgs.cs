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
