// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1213EventAccessorsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1212SA1213CodeFixProvider>;

    public class SA1213UnitTests
    {
        [Theory]
        [MemberData(nameof(CommonData.EndOfLineSequences), MemberType = typeof(CommonData))]
        public async Task TestAddAccessorAfterRemoveAccessorAsync(string lineEnding)
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        {|#0:remove|}
        {
            this.nameChanged -= value;
        }
        add
        {
            this.nameChanged += value;
        }
    }
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic().WithLocation(0);

            var fixedCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add
        {
            this.nameChanged += value;
        }
        remove
        {
            this.nameChanged -= value;
        }
    }
}".ReplaceLineEndings(lineEnding);

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessorWithLineCommentAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        // This is the remove accessor.
        remove
        {
            this.nameChanged -= value;
        }

        // This is the add accessor.
        add
        {
            this.nameChanged += value;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(10, 9);

            var fixedCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        // This is the add accessor.
        add
        {
            this.nameChanged += value;
        }

        // This is the remove accessor.
        remove
        {
            this.nameChanged -= value;
        }
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessorWithBlockCommentAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        /*
         * This is the remove accessor.
         */
        remove
        {
            this.nameChanged -= value;
        }

        /*
         * This is the add accessor.
         */
        add
        {
            this.nameChanged += value;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(12, 9);

            var fixedCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        /*
         * This is the add accessor.
         */
        add
        {
            this.nameChanged += value;
        }

        /*
         * This is the remove accessor.
         */
        remove
        {
            this.nameChanged -= value;
        }
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorAfterRemoveAccessorSameLineAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        remove { this.nameChanged -= value; } add { this.nameChanged += value; }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 9);

            var fixedCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; } 
    }
}";

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorBeforeRemoveAccessorAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add { this.nameChanged += value; }
        remove { this.nameChanged -= value; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAddAccessorBeforeRemoveAccessorSameLineAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler NameChanged
    {
        add { this.nameChanged += value; } remove { this.nameChanged -= value; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOnlyAddAccessorAsync()
        {
            var testCode = @"
using System;
public class Foo
{
    private EventHandler nameChanged;

    public event EventHandler {|CS0065:NameChanged|}
    {
        add { this.nameChanged += value; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEventWithMissingAccessorListAsync()
        {
            var testCode = @"
using System;

public interface ITest
{
    event EventHandler TestEvent;
}

public class TestClass : ITest
{
    event EventHandler ITest.TestEvent;
}
";

            var expected = this.GetExpectedResultTestEventWithMissingAccessorList();

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        protected virtual DiagnosticResult[] GetExpectedResultTestEventWithMissingAccessorList()
        {
            return new[]
            {
                DiagnosticResult.CompilerError("CS0535").WithSpan(9, 26, 9, 31).WithArguments("TestClass", "ITest.TestEvent"),
                DiagnosticResult.CompilerError("CS0071").WithSpan(11, 29, 11, 30),
                DiagnosticResult.CompilerError("CS0065").WithSpan(11, 30, 11, 30).WithArguments("TestClass."),
                DiagnosticResult.CompilerError("CS0539").WithSpan(11, 30, 11, 30).WithArguments("TestClass."),
                DiagnosticResult.CompilerError("CS1519").WithSpan(11, 39, 11, 40).WithArguments(";"),
                DiagnosticResult.CompilerError("CS1519").WithSpan(11, 39, 11, 40).WithArguments(";"),
            };
        }
    }
}
