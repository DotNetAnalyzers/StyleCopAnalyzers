namespace StyleCop.Analyzers.Helpers
{
    internal static class ArrayEx
    {
        internal static T[] Empty<T>()
        {
            return EmptyArrayHolder<T>.Instance;
        }

        private static class EmptyArrayHolder<T>
        {
            internal static readonly T[] Instance = new T[0];
        }
    }
}
