// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.MaintainabilityRules;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using StyleCop.Analyzers.MaintainabilityRules;
using StyleCop.Analyzers.Test.CSharp7.MaintainabilityRules;
using Xunit;
using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
    StyleCop.Analyzers.MaintainabilityRules.SA1415DoNotUsePublicOnMembersOfInternalTypes,
    StyleCop.Analyzers.MaintainabilityRules.SA1415CodeFixProvider>;

public partial class SA1415CSharp8UnitTests : SA1415CSharp7UnitTests
{
    [Fact]
    public async Task TestExplicitPublicMembersOfInternalInterfaceAreNotReportedAsync()
    {
        var testCode = @"
internal interface IName
{
    void ImplicitPublicMethod();
    public void ExplicitPublicMethod();
}
";

        var test = new CSharpTest
        {
            TestCode = testCode,
            FixedCode = testCode,
        };
        test.ExplicitlyEnabledDiagnostics.Add(SA1415DoNotUsePublicOnMembersOfInternalTypes.DiagnosticId);
        await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestAsyncEnumerationPatternMembersAreNotReportedAsync()
    {
        var testCode = @"
internal class Collection
{
    public Enumerator GetAsyncEnumerator()
    {
        return new Enumerator();
    }

    {|#0:public|} void Unused()
    {
    }
}

internal struct Enumerator
{
    public int Current
    {
        get { return 0; }
    }

    public BoolTask MoveNextAsync()
    {
        return new BoolTask();
    }
}

internal struct BoolTask
{
    internal BoolAwaiter GetAwaiter()
    {
        return new BoolAwaiter();
    }
}

internal struct BoolAwaiter : System.Runtime.CompilerServices.INotifyCompletion
{
    internal bool IsCompleted
    {
        get { return true; }
    }

    internal bool GetResult()
    {
        return false;
    }

    public void OnCompleted(System.Action continuation)
    {
    }
}

internal class Consumer
{
    internal async System.Threading.Tasks.Task M(Collection values)
    {
        await foreach (var value in values)
        {
        }
    }
}
";
        var fixedCode = @"
internal class Collection
{
    public Enumerator GetAsyncEnumerator()
    {
        return new Enumerator();
    }

    internal void Unused()
    {
    }
}

internal struct Enumerator
{
    public int Current
    {
        get { return 0; }
    }

    public BoolTask MoveNextAsync()
    {
        return new BoolTask();
    }
}

internal struct BoolTask
{
    internal BoolAwaiter GetAwaiter()
    {
        return new BoolAwaiter();
    }
}

internal struct BoolAwaiter : System.Runtime.CompilerServices.INotifyCompletion
{
    internal bool IsCompleted
    {
        get { return true; }
    }

    internal bool GetResult()
    {
        return false;
    }

    public void OnCompleted(System.Action continuation)
    {
    }
}

internal class Consumer
{
    internal async System.Threading.Tasks.Task M(Collection values)
    {
        await foreach (var value in values)
        {
        }
    }
}
";

        await VerifyFixAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    [Fact]
    public async Task TestAsyncEnumerationWithExtensionAwaiterIsNotReportedAsync()
    {
        var testCode = @"
internal class Collection
{
    public Enumerator GetAsyncEnumerator()
    {
        return new Enumerator();
    }

    {|#0:public|} void Unused()
    {
    }
}

internal struct Enumerator
{
    public int Current
    {
        get { return 0; }
    }

    public BoolTask MoveNextAsync()
    {
        return new BoolTask();
    }
}

internal struct BoolTask
{
}

internal static class BoolTaskExtensions
{
    internal static BoolAwaiter GetAwaiter(this BoolTask task)
    {
        return new BoolAwaiter();
    }
}

internal struct BoolAwaiter : System.Runtime.CompilerServices.INotifyCompletion
{
    internal bool IsCompleted
    {
        get { return true; }
    }

    internal bool GetResult()
    {
        return false;
    }

    public void OnCompleted(System.Action continuation)
    {
    }
}

internal class Consumer
{
    internal async System.Threading.Tasks.Task M(Collection values)
    {
        await foreach (var value in values)
        {
        }
    }
}
";
        var fixedCode = @"
internal class Collection
{
    public Enumerator GetAsyncEnumerator()
    {
        return new Enumerator();
    }

    internal void Unused()
    {
    }
}

internal struct Enumerator
{
    public int Current
    {
        get { return 0; }
    }

    public BoolTask MoveNextAsync()
    {
        return new BoolTask();
    }
}

internal struct BoolTask
{
}

internal static class BoolTaskExtensions
{
    internal static BoolAwaiter GetAwaiter(this BoolTask task)
    {
        return new BoolAwaiter();
    }
}

internal struct BoolAwaiter : System.Runtime.CompilerServices.INotifyCompletion
{
    internal bool IsCompleted
    {
        get { return true; }
    }

    internal bool GetResult()
    {
        return false;
    }

    public void OnCompleted(System.Action continuation)
    {
    }
}

internal class Consumer
{
    internal async System.Threading.Tasks.Task M(Collection values)
    {
        await foreach (var value in values)
        {
        }
    }
}
";

        await VerifyFixAsync(testCode, fixedCode, Diagnostic().WithLocation(0)).ConfigureAwait(false);
    }

    private static Task VerifyFixAsync(string testCode, string fixedCode, params DiagnosticResult[] expected)
    {
        var test = new CSharpTest
        {
            TestCode = testCode,
            FixedCode = fixedCode,
        };
        test.ExplicitlyEnabledDiagnostics.Add(SA1415DoNotUsePublicOnMembersOfInternalTypes.DiagnosticId);
        test.ExpectedDiagnostics.AddRange(expected);
        return test.RunAsync(CancellationToken.None);
    }
}
