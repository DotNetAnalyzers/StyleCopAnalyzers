// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test
{
    using Xunit;

    /// <summary>
    /// Defines a collection for tests which cannot run in parallel with other tests.
    /// </summary>
    [CollectionDefinition(nameof(SequentialTestCollection), DisableParallelization = true)]
    public sealed class SequentialTestCollection
    {
    }
}
