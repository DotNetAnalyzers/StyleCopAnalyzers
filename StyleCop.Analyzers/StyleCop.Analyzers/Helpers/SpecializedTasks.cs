namespace StyleCop.Analyzers.Helpers
{
    using System.Threading.Tasks;

    internal static class SpecializedTasks
    {
        internal static Task CompletedTask { get; } = Task.FromResult(default(VoidResult));

        private struct VoidResult
        {
        }
    }
}
