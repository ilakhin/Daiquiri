namespace IL.Daiquiri.Core
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
