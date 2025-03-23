namespace IL.Daiquiri
{
    public sealed class WrappedStruct<T>
        where T : struct
    {
        public T Value
        {
            get;
            set;
        }
    }
}
