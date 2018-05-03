// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Test.SpacingRules;
    using TestHelper;
    using Xunit;

    using static StyleCop.Analyzers.SpacingRules.SA1003SymbolsMustBeSpacedCorrectly;

    public class SA1003CSharp7UnitTests : SA1003UnitTests
    {
        /// <summary>
        /// Verifies that the additional expression-bodied members supported in C# 7 trigger diagnostics as expected.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCSharp7ExpressionBodiedMembersAsync()
        {
            var testCode = @"using System;
namespace N1
{
    public class C1
    {
        private int x;
        private EventHandler e;

        public C1()=>x = 1; // Constructors
        ~C1()=>x = 0; // Finalizers
        public event EventHandler E { add=>e += value; remove=>e -= value; } // Event accessors
        public int Answer { get=>42; set=>x = 2; } // Property accessors
        public int this[int index] { get=>42; set=>x = value; } // Indexer accessors
        public void Method()
        {
            int LocalFunction()=>42; // Local functions
        }
    }
}
";
            var fixedTestCode = @"using System;
namespace N1
{
    public class C1
    {
        private int x;
        private EventHandler e;

        public C1() => x = 1; // Constructors
        ~C1() => x = 0; // Finalizers
        public event EventHandler E { add => e += value; remove => e -= value; } // Event accessors
        public int Answer { get => 42; set => x = 2; } // Property accessors
        public int this[int index] { get => 42; set => x = value; } // Indexer accessors
        public void Method()
        {
            int LocalFunction() => 42; // Local functions
        }
    }
}
";
            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(9, 20).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(9, 20).WithArguments("=>"),

                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(10, 14).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(10, 14).WithArguments("=>"),

                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(11, 42).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(11, 42).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(11, 62).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(11, 62).WithArguments("=>"),

                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(12, 32).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(12, 32).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(12, 41).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(12, 41).WithArguments("=>"),

                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(13, 41).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(13, 41).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(13, 50).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(13, 50).WithArguments("=>"),

                this.CSharpDiagnostic(DescriptorPrecededByWhitespace).WithLocation(16, 32).WithArguments("=>"),
                this.CSharpDiagnostic(DescriptorFollowedByWhitespace).WithLocation(16, 32).WithArguments("=>"),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
