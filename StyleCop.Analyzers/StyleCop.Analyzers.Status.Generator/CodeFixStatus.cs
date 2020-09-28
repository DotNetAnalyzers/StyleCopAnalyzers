// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Status.Generator
{
    /// <summary>
    /// This enum is used to indicate whether or not a code fix is implemented.
    /// </summary>
    public enum CodeFixStatus
    {
        /// <summary>
        /// This value indicates, that a code fix is implemented.
        /// </summary>
        Implemented,

        /// <summary>
        /// This value indicates, that a code fix is not implemented and
        /// will not be implemented because it either can't be implemented
        /// or a code fix would not be able to fix it rationally.
        /// </summary>
        NotImplemented,

        /// <summary>
        /// This value indicates, that a code fix is not implemented because
        /// no one implemented it yet, or it is not yet decided if a code fix
        /// is going to be implemented in the future.
        /// </summary>
        NotYetImplemented,
    }
}
