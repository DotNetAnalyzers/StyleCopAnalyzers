﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Analyzers.MaintainabilityRules;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1413UseTrailingCommasInMultiLineInitializers"/>.
    /// </summary>
    public class SA1413UnitTests : CodeFixVerifier
    {
        /// <summary>
        /// Verifies that code without an initializer will not fire the diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyNormalObjectCreationProducesEmptyResultsAsync()
        {
            var testCode = @"using System.Text;
public class TestClass
{
    void Test()
    {
        var foo = new StringBuilder();
    }
}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a single-line initializer will not fire the diagnostic.
        /// </summary>
        /// <param name="testStatement">The statement to test.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Theory]
        [InlineData("var x = new Barnacle { Age = 100, Height = 0.2M, Weight = 0.88M };")]
        [InlineData("var x = new { Min = 0, Max = 0 };")]
        [InlineData("var x = new [] { Tuple.Create(\"A\", 1), Tuple.Create(\"B\", 2), Tuple.Create(\"C\", 3) };")]
        [InlineData("var x = new List<int>() { 1, 2, 3 };")]
        [InlineData("var x = new Dictionary<int, int>() { [0] = 1, [1] = 2, [2] = 3 };")]
        public async Task VerifySingleLineInitiaizerProducesEmptyResultsAsync(string testStatement)
        {
            var testCode = $@"
namespace TestNamespace
{{
    using System;
    using System.Collections.Generic;

    class Barnacle
    {{
        public int Age {{ get; set; }}
        public decimal Height {{ get; set; }}
        public decimal Weight {{ get; set; }}
    }}

    class TestClass
    {{
        void Foo()
        {{
            {testStatement}
        }}
    }}
}}
";
            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an object initializer without a trailing comma produces a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyObjectInitializerAsync()
        {
            var testCode = @"
namespace TestNamespace
{
    class Barnacle
    {
        public int Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
    }

    class TestClass
    {
        void Foo()
        {
            var x = new Barnacle
            {
                Age = 100,
                Height = 0.2M,
                Weight = 0.88M
            };
        }
    }
}
";

            var fixedTestCode = @"
namespace TestNamespace
{
    class Barnacle
    {
        public int Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
    }

    class TestClass
    {
        void Foo()
        {
            var x = new Barnacle
            {
                Age = 100,
                Height = 0.2M,
                Weight = 0.88M,
            };
        }
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(19, 17),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an anonymous object initializer without a trailing comma produces a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyAnonymousObjectInitializerAsync()
        {
            var testCode = @"
class TestClass
{
    void Foo()
    {
        var x = new
        {
            Min = 0,
            Max = 0
        };
    }
}
";

            var fixedTestCode = @"
class TestClass
{
    void Foo()
    {
        var x = new
        {
            Min = 0,
            Max = 0,
        };
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(9, 13),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an anonymous object initializer without a trailing comma produces a diagnostic,
        /// and the code fix preserves trailing trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyAnonymousObjectInitializerWithTrailingTriviaAsync()
        {
            var testCode = @"
class TestClass
{
    void Foo()
    {
        var x = new
        {
            Min = 0,
            Max = 0 // trivia?
        };
    }
}
";

            var fixedTestCode = @"
class TestClass
{
    void Foo()
    {
        var x = new
        {
            Min = 0,
            Max = 0, // trivia?
        };
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(9, 13),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an array initializer without a trailing comma produces a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyArrayInitializerAsync()
        {
            var testCode = @"
class TestClass
{
    void Foo()
    {
        var x = new int[]
        {
            0,
            1,
            2
        };
    }
}
";

            var fixedTestCode = @"
class TestClass
{
    void Foo()
    {
        var x = new int[]
        {
            0,
            1,
            2,
        };
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(10, 13),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an array initializer without a trailing comma produces a diagnostic,
        /// and the code fix preserves existing trivia.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyArrayInitializerWithTrailingTriviaAsync()
        {
            var testCode = @"
class TestClass
{
    void Foo()
    {
        var x = new[]
        {
            1,
            2,
            3 /* last item */
        };
    }
}
";

            var fixedTestCode = @"
class TestClass
{
    void Foo()
    {
        var x = new[]
        {
            1,
            2,
            3, /* last item */
        };
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(10, 13),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that a collection initializer without a trailing comma produces a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCollectionInitializerAsync()
        {
            var testCode = @"
using System.Collections.Generic;
class TestClass
{
    void Foo()
    {
        var x = new List<int>()
        {
            1,
            2,
            3
        };
    }
}
";

            var fixedTestCode = @"
using System.Collections.Generic;
class TestClass
{
    void Foo()
    {
        var x = new List<int>()
        {
            1,
            2,
            3,
        };
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(11, 13),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that an array initializer without a trailing comma produces a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyIndexedCollectionInitializerAsync()
        {
            var testCode = @"
using System.Collections.Generic;
class TestClass
{
    void Foo()
    {
        var x = new Dictionary<int, int>()
        {
            [0] = 1,
            [1] = 2,
            [2] = 3
        };
    }
}
";

            var fixedTestCode = @"
using System.Collections.Generic;
class TestClass
{
    void Foo()
    {
        var x = new Dictionary<int, int>()
        {
            [0] = 1,
            [1] = 2,
            [2] = 3,
        };
    }
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(11, 13),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the last value of an empty enum does not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyEmptyEnumAsync()
        {
            var testCode = @"enum EmptyEnum
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the last value of an enum with a trailing comma does not produce a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyEnumWithTrailingCommaAsync()
        {
            var testCode = @"enum TestEnum
{
    One,
    Two,
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the last value of an enum without a trailing comma produces a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyEnumWithoutTrailingCommaAsync()
        {
            var testCode = @"enum TestEnum
{
    One,
    Two
}
";

            var fixedTestCode = @"enum TestEnum
{
    One,
    Two,
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(4, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that the last value of an enum without a trailing comma produces a diagnostic.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyEnumWithValueWithoutTrailingCommaAsync()
        {
            var testCode = @"enum TestEnum
{
    One = 2 /* test comment */
}
";

            var fixedTestCode = @"enum TestEnum
{
    One = 2, /* test comment */
}
";

            DiagnosticResult[] expected =
            {
                this.CSharpDiagnostic().WithLocation(3, 5),
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1413CodeFixProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1413UseTrailingCommasInMultiLineInitializers();
        }
    }
}
