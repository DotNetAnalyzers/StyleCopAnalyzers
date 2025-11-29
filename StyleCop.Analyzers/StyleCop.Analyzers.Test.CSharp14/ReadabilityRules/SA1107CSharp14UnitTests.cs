// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.ReadabilityRules
{
    using StyleCop.Analyzers.Test.CSharp13.ReadabilityRules;

    public partial class SA1107CSharp14UnitTests : SA1107CSharp13UnitTests
    {
        protected override string? GetFixedCodeTestEmptyStatementAfterBlock()
        {
            // In earlier versions of Roslyn, the fix did not change the code, but that doesn't happen anymore.
            return @"
class Program
{
    static void Main(string[] args)
    {
        {
        }

        ;
    }
}
";
        }
    }
}
