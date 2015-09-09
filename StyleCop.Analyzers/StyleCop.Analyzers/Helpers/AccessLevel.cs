// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    /// <summary>
    /// Describes an element's access level
    /// </summary>
    internal enum AccessLevel
    {
        /// <summary>No access level specified.</summary>
        NotSpecified,

        /// <summary>Public access.</summary>
        Public,

        /// <summary>Internal access.</summary>
        Internal,

        /// <summary>Protected internal access.</summary>
        ProtectedInternal,

        /// <summary>Protected access.</summary>
        Protected,

        /// <summary>Private access.</summary>
        Private
    }
}
