// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.NamingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.NamingRules;
    using TestHelper;
    using Xunit;
    using Microsoft.CodeAnalysis;
    public class SA1315UnitTests : CodeFixVerifier
    {
        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForMethodWithoutArgumentsAsync()
        {
            var testCode = @"public class TypeName
{
    public void AMethod() { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForMethodWithoutInheritNamesAsync()
        {
            var testCode = @"public class TypeName
{
    public void AMethod(string arg1, string arg2) { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForMethodWithoutInheritNamesAndArglistAsync()
        {
            var testCode = @"public class TypeName
{
    public void AMethod(string arg1, string arg2, __arglist) { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForMethodWithoutInheritNamesAnParamsAsync()
        {
            var testCode = @"public class TypeName
{
    public void AMethod(string arg1, string arg2, params string[] arg3) { }
}";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedForMethodWithInheritNamesAsync()
        {
            var testCode = @"public class TypeName : BaseClass
{
    public override void AMethod(string arg1, string arg2) { }
}

public abstract class BaseClass
{
    public abstract void AMethod(string baseArg1, string baseArg2);
}";

            var fixedCode = @"public class TypeName : BaseClass
{
    public override void AMethod(string baseArg1, string baseArg2) { }
}

public abstract class BaseClass
{
    public abstract void AMethod(string baseArg1, string baseArg2);
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 41),
                this.CSharpDiagnostic().WithLocation(3, 54)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 2).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedForMethodWithInheritNamesAndArglistAsync()
        {
            var testCode = @"public class TypeName : BaseClass
{
    public override void AMethod(string arg1, string arg2, __arglist) { }
}

public abstract class BaseClass
{
    public abstract void AMethod(string baseArg1, string baseArg2, __arglist);
}";

            var fixedCode = @"public class TypeName : BaseClass
{
    public override void AMethod(string baseArg1, string baseArg2, __arglist) { }
}

public abstract class BaseClass
{
    public abstract void AMethod(string baseArg1, string baseArg2, __arglist);
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 41),
                this.CSharpDiagnostic().WithLocation(3, 54)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 2).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedForMethodWithInheritNamesAnParamsAsync()
        {
            var testCode = @"public class TypeName : BaseClass
{
    public override void AMethod(string arg1, string arg2, params string[] arg3) { }
}

public abstract class BaseClass
{
    public abstract void AMethod(string baseArg1, string baseArg2, params string[] baseArg3);
}";

            var fixedCode = @"public class TypeName : BaseClass
{
    public override void AMethod(string baseArg1, string baseArg2, params string[] baseArg3) { }
}

public abstract class BaseClass
{
    public abstract void AMethod(string baseArg1, string baseArg2, params string[] baseArg3);
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 41),
                this.CSharpDiagnostic().WithLocation(3, 54),
                this.CSharpDiagnostic().WithLocation(3, 76)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 3).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedForMethodWithInheritNamesWithInterfaceAsync()
        {
            var testCode = @"public class TypeName : IBase
{
    public void AMethod(string arg1, string arg2) { }
}

public interface IBase
{
    void AMethod(string baseArg1, string baseArg2);
}";

            var fixedCode = @"public class TypeName : IBase
{
    public void AMethod(string baseArg1, string baseArg2) { }
}

public interface IBase
{
    void AMethod(string baseArg1, string baseArg2);
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 32),
                this.CSharpDiagnostic().WithLocation(3, 45)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 2).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedForMethodWithInheritNamesAndArglistWithInterfaceAsync()
        {
            var testCode = @"public class TypeName : IBase
{
    public void AMethod(string arg1, string arg2, __arglist) { }
}

public interface IBase
{
    void AMethod(string baseArg1, string baseArg2, __arglist);
}";

            var fixedCode = @"public class TypeName : IBase
{
    public void AMethod(string baseArg1, string baseArg2, __arglist) { }
}

public interface IBase
{
    void AMethod(string baseArg1, string baseArg2, __arglist);
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 32),
                this.CSharpDiagnostic().WithLocation(3, 45)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 2).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedForMethodWithInheritNamesAnParamsWithInterfaceAsync()
        {
            var testCode = @"public class TypeName : IBase
{
    public void AMethod(string arg1, string arg2, params string[] arg3) { }
}

public interface IBase
{
    void AMethod(string baseArg1, string baseArg2, params string[] baseArg3);
}";

            var fixedCode = @"public class TypeName : IBase
{
    public void AMethod(string baseArg1, string baseArg2, params string[] baseArg3) { }
}

public interface IBase
{
    void AMethod(string baseArg1, string baseArg2, params string[] baseArg3);
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(3, 32),
                this.CSharpDiagnostic().WithLocation(3, 45),
                this.CSharpDiagnostic().WithLocation(3, 67)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 3).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsNotReportedForInvalidOverrideAsync()
        {
            var testCode = @"public class TypeName
{
    public override void AMethod(string arg1, string arg2, params string[] arg3) { }
}";

            var expected =
                new DiagnosticResult
                {
                    Id = "CS0115",
                    Severity = DiagnosticSeverity.Error,
                    Message = "'TypeName.AMethod(string, string, params string[])': no suitable method found to override",
                    Locations = new[] { new DiagnosticResultLocation("Test0.cs", 3, 26) }
                };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatDiagnosticIsReportedForLongerInheritageChainAsync()
        {
            var testCode = @"public class TopLevelBaseClass
{
    public virtual void Method(int baseArg) { }
}

public class IntermediateBaseClass : TopLevelBaseClass
{
}

public class TestClass : IntermediateBaseClass
{
  public override void Method(int arg) { }
}";

            var fixedCode = @"public class TopLevelBaseClass
{
    public virtual void Method(int baseArg) { }
}

public class IntermediateBaseClass : TopLevelBaseClass
{
}

public class TestClass : IntermediateBaseClass
{
  public override void Method(int baseArg) { }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(12, 35)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 1).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestThatRenameToBaseClassTakesPriorityAsync()
        {
            var testCode = @"interface IInterface
{
  void Method(string p1);
}

abstract class BaseClass
{
  public abstract void Method(string p2);
}

class Derived : BaseClass, IInterface
{
  public override void Method(string p)
  {
  }
}";

            var fixedCode = @"interface IInterface
{
  void Method(string p1);
}

abstract class BaseClass
{
  public abstract void Method(string p2);
}

class Derived : BaseClass, IInterface
{
  public override void Method(string p2)
  {
  }
}";

            var expected = new[]
            {
                this.CSharpDiagnostic().WithLocation(13, 38)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);

            Assert.Equal(1, (await this.GetOfferedCSharpFixesAsync(testCode).ConfigureAwait(false)).Length);
            await this.VerifyCSharpFixAsync(testCode, fixedCode, numberOfFixAllIterations: 1).ConfigureAwait(false);
        }

        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1315ParametersShouldMatchInheritedNames();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new RenameToAnyCodeFixProvider();
        }
    }
}
