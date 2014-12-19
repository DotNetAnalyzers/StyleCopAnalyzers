using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StyleCop.Analyzers.ReadabilityRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestHelper;

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    [TestClass]
    public class SA1100UnitTests : CodeFixVerifier
    {
        private const string DiagnosticId = SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists.DiagnosticId;
        protected static readonly DiagnosticResult[] EmptyDiagnosticResults = { };

        [TestMethod]
        public async Task TestChildClassUsesBaseButNoOverride()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected override void Baz()
    {
        base.Bar();
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "A call to a member from an inherited class begins with ‘base.’, and the local class does not contain an override or implementation of the member",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 14, 9)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestChildClassUsesBaseButMethodIsNotVirtual()
        {
            var testCode = @"
public class Foo
{
    protected void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected override void Baz()
    {
        base.Bar();
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "A call to a member from an inherited class begins with ‘base.’, and the local class does not contain an override or implementation of the member",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 14, 9)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestChildClassUsesBaseBaseAndChildHaveMethodWithSameName()
        {
            var testCode = @"
public class Foo
{
    protected  void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected  void Baz()
    {
        base.Bar();
    }
    protected  void Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestChildClassUsesBaseBaseIsVirtualChildHidesBase()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected  void Baz()
    {
        base.Bar();
    }
    protected  void Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestChildClassUsesBasePropertyButNoOverride()
        {
            var testCode = @"
public class Foo
{
    protected virtual string Bar
    {
        get;set;
    }
}

public class FooChild : Foo
{
    protected override void Baz()
    {
        var s = base.Bar;
    }
}";

            var expected = new[]
            {
                new DiagnosticResult
                {
                    Id = DiagnosticId,
                    Message = "A call to a member from an inherited class begins with ‘base.’, and the local class does not contain an override or implementation of the member",
                    Severity =  DiagnosticSeverity.Warning,
                    Locations =
                        new[]
                        {
                            new DiagnosticResultLocation("Test0.cs", 14, 17)
                        }
                }
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestChildClassUsesBaseOverrideExists()
        {
            var testCode = @"
public class Foo
{
    protected virtual void Bar()
    {

    }
}

public class FooChild : Foo
{
    protected override void Baz()
    {
        base.Bar();
    }
    protected override void Bar()
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        [TestMethod]
        public async Task TestChildClassUsesBasePropertyOverrideExists()
        {
            var testCode = @"
public class Foo
{
    protected virtual string Bar
    {
        get;set;
    }
}

public class FooChild : Foo
{
    protected override void Baz()
    {
        var s = base.Bar;
    }

    protected override string Bar
    {
        get;set;
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1100DoNotPrefixCallsWithBaseUnlessLocalImplementationExists();
        }
    }
}
