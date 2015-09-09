// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
