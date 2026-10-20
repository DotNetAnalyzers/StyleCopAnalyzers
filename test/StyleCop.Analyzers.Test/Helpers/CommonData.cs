// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers;

using Xunit;

internal static class CommonData
{
    public static TheoryData<string> EndOfLineSequences =>
        new TheoryData<string>("\n", "\r\n");
}
