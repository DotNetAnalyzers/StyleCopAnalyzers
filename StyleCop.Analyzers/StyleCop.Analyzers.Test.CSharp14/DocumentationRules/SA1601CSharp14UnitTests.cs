// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp14.DocumentationRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp13.DocumentationRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.DocumentationRules.SA1601PartialElementsMustBeDocumented>;

    public partial class SA1601CSharp14UnitTests : SA1601CSharp13UnitTests
    {
        [Fact]
        [WorkItem(4029, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4029")]
        public async Task TestPartialConstructorAndEventWithoutDocumentationAsync()
        {
            // NOTE: The Roslyn analyzer driver (verified with 5.0 and 5.9) does not invoke syntax node actions for the
            // defining declaration of a partial constructor, so no diagnostic is reported for it.
            var testCode = @"
using System;

/// <summary>
/// A class.
/// </summary>
public partial class TestClass
{
    public partial TestClass(int x);

    public partial event EventHandler {|#1:TestEvent|};
}

/// <content>
/// More.
/// </content>
public partial class TestClass
{
    public partial {|#2:TestClass|}(int x)
    {
    }

    public partial event EventHandler {|#3:TestEvent|}
    {
        add { }
        remove { }
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4029, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4029")]
        public async Task TestPartialConstructorAndEventImplementationWithoutDocumentationAsync()
        {
            var testCode = @"
using System;

/// <summary>
/// A class.
/// </summary>
public partial class TestClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass""/> class.
    /// </summary>
    /// <param name=""x"">The value.</param>
    public partial TestClass(int x);

    /// <summary>
    /// An event.
    /// </summary>
    public partial event EventHandler TestEvent;
}

/// <content>
/// More.
/// </content>
public partial class TestClass
{
    public partial TestClass(int x)
    {
    }

    public partial event EventHandler TestEvent
    {
        add { }
        remove { }
    }
}
";

            // The defining declarations are documented, which is enough for the partial members
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4029, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4029")]
        public async Task TestPartialConstructorAndEventDefinitionWithoutDocumentationAsync()
        {
            var testCode = @"
using System;

/// <summary>
/// A class.
/// </summary>
public partial class TestClass
{
    public partial TestClass(int x);

    public partial event EventHandler TestEvent;
}

/// <content>
/// More.
/// </content>
public partial class TestClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass""/> class.
    /// </summary>
    /// <param name=""x"">The value.</param>
    public partial TestClass(int x)
    {
    }

    /// <summary>
    /// An event.
    /// </summary>
    public partial event EventHandler TestEvent
    {
        add { }
        remove { }
    }
}
";

            // The implementing declarations are documented, which is enough for the partial members
            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        [WorkItem(4029, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/4029")]
        public async Task TestPartialConstructorAndEventWithDocumentationAsync()
        {
            var testCode = @"
using System;

/// <summary>
/// A class.
/// </summary>
public partial class TestClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref=""TestClass""/> class.
    /// </summary>
    /// <param name=""x"">The value.</param>
    public partial TestClass(int x);

    /// <summary>
    /// An event.
    /// </summary>
    public partial event EventHandler TestEvent;
}

/// <content>
/// More.
/// </content>
public partial class TestClass
{
    /// <content>
    /// The implementation.
    /// </content>
    public partial TestClass(int x)
    {
    }

    /// <content>
    /// The implementation.
    /// </content>
    public partial event EventHandler TestEvent
    {
        add { }
        remove { }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
