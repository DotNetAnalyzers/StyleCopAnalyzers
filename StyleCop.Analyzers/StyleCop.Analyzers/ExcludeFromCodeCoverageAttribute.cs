// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#if NETSTANDARD1_1

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    internal sealed class ExcludeFromCodeCoverageAttribute : Attribute
    {
    }
}

#endif
