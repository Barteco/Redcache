namespace Redcache
{
    public sealed class Optional<T>
    {
        public T Value { get; }
        public bool IsSome { get; }
        public bool IsNone { get; }

        private Optional(T value, bool hasValue)
        {
            Value = value;
            IsSome = hasValue;
            IsNone = !hasValue;
        }

        public static Optional<T> Some(T value)
        {
            return new Optional<T>(value, true);
        }

        public static Optional<T> None()
        {
            return new Optional<T>(default, false);
        }

        public static implicit operator Optional<T>(T value)
        {
            return Some(value);
        }

        public static implicit operator T(Optional<T> value)
        {
            return value.Value;
        }
    }
}