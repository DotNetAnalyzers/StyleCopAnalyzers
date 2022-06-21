// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    /// <summary>
    /// Describes an element's access level.
    /// </summary>
    internal enum AccessLevel
    {
        /// <summary>No access level specified.</summary>
        NotSpecified,

        /// <summary>Private access.</summary>
        Private,

        /// <summary>Private protected access.</summary>
        PrivateProtected,

        /// <summary>Protected access.</summary>
        Protected,

        /// <summary>Protected internal access.</summary>
        ProtectedInternal,

        /// <summary>Internal access.</summary>
        Internal,

        /// <summary>Public access.</summary>
        Public,
    }
}
