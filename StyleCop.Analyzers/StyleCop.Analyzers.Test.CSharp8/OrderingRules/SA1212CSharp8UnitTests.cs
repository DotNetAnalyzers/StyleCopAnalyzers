// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.OrderingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.OrderingRules.SA1212PropertyAccessorsMustFollowOrder,
        StyleCop.Analyzers.OrderingRules.SA1212SA1213CodeFixProvider>;

    public partial class SA1212CSharp8UnitTests : SA1212CSharp7UnitTests
    {
        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestPropertyWithReadonlyGetterAfterSetterAsync()
        {
            var testCode = @"
public struct S
{
    private int _value;

    public int Value
    {
        [|set
        {
            _value = value;
        }|]
        readonly get
        {
            return _value;
        }
    }
}";

            var fixedCode = @"
public struct S
{
    private int _value;

    public int Value
    {
        readonly get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestIndexerWithReadonlyGetterAfterSetterAsync()
        {
            var testCode = @"
public struct S
{
    private int[] _values;

    public int this[int index]
    {
        [|set
        {
            _values[index] = value;
        }|]
        readonly get
        {
            return _values[index];
        }
    }
}";

            var fixedCode = @"
public struct S
{
    private int[] _values;

    public int this[int index]
    {
        readonly get
        {
            return _values[index];
        }
        set
        {
            _values[index] = value;
        }
    }
}";

            await VerifyCSharpFixAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(3001, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3001")]
        public async Task TestReadonlyGetterBeforeSetterAsync()
        {
            var testCode = @"
public struct S
{
    private int _value;

    public int Value
    {
        readonly get => _value;
        set => _value = value;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
