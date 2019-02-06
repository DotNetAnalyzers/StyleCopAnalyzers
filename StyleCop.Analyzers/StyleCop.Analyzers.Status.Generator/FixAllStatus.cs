// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Generator
{
    /// <summary>
    /// This enum captures the status of the implementation of a fix all provider.
    /// </summary>
    public enum FixAllStatus
    {
        /// <summary>
        /// No fix all provider is implemented for the given code fix.
        /// </summary>
        None,

        /// <summary>
        /// The fix all provider for this code fix uses a custom optimized implementation.
        /// </summary>
        CustomImplementation,

        /// <summary>
        /// The fix all capability is provided by the default batch fixer.
        /// This implementation might have various problems e.g. bad performance or it might not fix all problems at once.
        /// </summary>
        BatchFixer,
    }
}
