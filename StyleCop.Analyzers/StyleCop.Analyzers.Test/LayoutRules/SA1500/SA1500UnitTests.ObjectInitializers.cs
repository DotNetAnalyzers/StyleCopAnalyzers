// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.LayoutRules.SA1500BracesForMultiLineStatementsMustNotShareLine,
        StyleCop.Analyzers.LayoutRules.SA1500CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1500BracesForMultiLineStatementsMustNotShareLine"/>.
    /// </summary>
    public partial class SA1500UnitTests
    {
        /// <summary>
        /// Verifies that no diagnostics are reported for the valid object initializers defined in this test.
        /// </summary>
        /// <remarks>
        /// These are valid for SA1500 only, some will report other diagnostics.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestObjectInitializersValidAsync()
        {
            var testCode = @"using System.Collections.Generic;
using System.Linq;

public class Foo
{
    // Valid object initializer #1
    private Contact[] contacts1 = new[]
    {
        new Contact
        {
            Name = ""Baz Foo"",
            PhoneNumbers = 
            { 
                ""000-000-0000"", 
                ""000-000-0000"" 
            }
        }
    };

    // Valid object initializer #2
    private Contact[] contacts2 = new[]
    {
        new Contact
        {
            Name = ""Baz Foo"",
            PhoneNumbers = { ""000-000-0000"", ""000-000-0000"" }
        }
    };

    // Valid object initializer #3
    private Contact[] contacts3 = new[]
    {
        new Contact { Name = ""Baz Foo"", PhoneNumbers = { ""000-000-0000"", ""000-000-0000"" } }
    };

    // Valid object initializer #4
    private Contact[] contacts4 = new[] { new Contact { Name = ""Baz Foo"", PhoneNumbers = { ""000-000-0000"", ""000-000-0000"" } } };

    public void Bar()
    {
        // Valid object initializer #5
        var contacts5 = new[]
        {
            new Contact
            {
                Name = ""Baz Foo"",
                PhoneNumbers = 
                { 
                    ""000-000-0000"", 
                    ""000-000-0000"" 
                }
            }
        };

        // Valid object initializer #6
        var contacts6 = new[]
        {
            new Contact
            {
                Name = ""Baz Foo"",
                PhoneNumbers = { ""000-000-0000"", ""000-000-0000"" }
            }
        };

        // Valid object initializer #7
        var contacts7 = new[]
        {
            new Contact { Name = ""Baz Foo"", PhoneNumbers = { ""000-000-0000"", ""000-000-0000"" } }
        };

        // Valid object initializer #8
        var contacts8 = new[] { new Contact { Name = ""Baz Foo"", PhoneNumbers = { ""000-000-0000"", ""000-000-0000"" } } };

        // Valid object initializer #9
        var contacts9 = new
        { 
            Name = ""Foo Bar"", 
            PhoneNumbers = new[]
            {
                ""000-000-0000""
            }
        };

        // Valid object initializer #10
        var contacts10 = new
        { 
            Name = ""Foo Bar"", 
            PhoneNumbers = new[] { ""000-000-0000"" }
        };

        // Valid object initializer #11
        var contacts11 = new { Name = ""Foo Bar"", PhoneNumbers = new[] { ""000-000-0000"" } };    
    }

    // Valid object initializer #12
    public void Fish()
    {
        var someList = new List<int>
                        {
                            1,
                            2,
                            3
                        }.Select(x => x.ToString());
    }

    private class Contact
    {
        public string Name { get; set; }

        public List<string> PhoneNumbers { get; set; }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that diagnostics will be reported for all invalid object initializer definitions.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestObjectInitializersInvalidAsync()
        {
            var testCode = @"using System.Collections.Generic;

public class Foo
{
    // Invalid object initializer #1
    private Contact[] contact1 = new[]
    {
        new Contact
        {
            Name = ""Baz Foo"",
            PhoneNumbers =
            {
                ""000-000-0000"",
                ""000-000-0000"" } } };

    // Invalid object initializer #2
    private Contact[] contacts2 = new[]
    {   new Contact
        {   Name = ""Baz Foo"",
            PhoneNumbers =
            {   ""000-000-0000"",
                ""000-000-0000""
            }
        }
    };

    // Invalid object initializer #3
    private Contact[] contacts3 = new[] {
        new Contact {
            Name = ""Baz Foo"",
            PhoneNumbers = {
                ""000-000-0000"",
                ""000-000-0000""
            }
        }
    };

    public void Bar()
    {
        // Invalid object initializer #4
        var contact4 = new[]
        {
            new Contact
            {
                Name = ""Baz Foo"",
                PhoneNumbers =
                {
                    ""000-000-0000"",
                    ""000-000-0000"" } } };

        // Invalid object initializer #5
        var contacts5 = new[]
        {   new Contact
            {   Name = ""Baz Foo"",
                PhoneNumbers =
                {   ""000-000-0000"",
                    ""000-000-0000""
                }
            }
        };

        // Invalid object initializer #6
        var contacts6 = new[] {
            new Contact {
                Name = ""Baz Foo"",
                PhoneNumbers = {
                    ""000-000-0000"",
                    ""000-000-0000""
                }
            }
        };

        // Invalid object initializer #7
        var contact7 = new
        {
            Name = ""Baz Foo"",
            PhoneNumbers = new[]
            {
                ""000-000-0000"" } };

        // Invalid object initializer #8
        var contacts8 = new
        {   Name = ""Baz Foo"",
            PhoneNumbers = new[]
            {   ""000-000-0000""
            }
        };

        // Invalid object initializer #9
        var contacts9 = new {
            Name = ""Baz Foo"",
            PhoneNumbers = new[] {
                ""000-000-0000""
            }
        };
    }

    private class Contact
    {
        public string Name { get; set; }

        public List<string> PhoneNumbers { get; set; }
    }
}";

            var fixedTestCode = @"using System.Collections.Generic;

public class Foo
{
    // Invalid object initializer #1
    private Contact[] contact1 = new[]
    {
        new Contact
        {
            Name = ""Baz Foo"",
            PhoneNumbers =
            {
                ""000-000-0000"",
                ""000-000-0000""
            }
        }
    };

    // Invalid object initializer #2
    private Contact[] contacts2 = new[]
    {
        new Contact
        {
            Name = ""Baz Foo"",
            PhoneNumbers =
            {
                ""000-000-0000"",
                ""000-000-0000""
            }
        }
    };

    // Invalid object initializer #3
    private Contact[] contacts3 = new[]
    {
        new Contact
        {
            Name = ""Baz Foo"",
            PhoneNumbers =
            {
                ""000-000-0000"",
                ""000-000-0000""
            }
        }
    };

    public void Bar()
    {
        // Invalid object initializer #4
        var contact4 = new[]
        {
            new Contact
            {
                Name = ""Baz Foo"",
                PhoneNumbers =
                {
                    ""000-000-0000"",
                    ""000-000-0000""
                }
            }
        };

        // Invalid object initializer #5
        var contacts5 = new[]
        {
            new Contact
            {
                Name = ""Baz Foo"",
                PhoneNumbers =
                {
                    ""000-000-0000"",
                    ""000-000-0000""
                }
            }
        };

        // Invalid object initializer #6
        var contacts6 = new[]
        {
            new Contact
            {
                Name = ""Baz Foo"",
                PhoneNumbers =
                {
                    ""000-000-0000"",
                    ""000-000-0000""
                }
            }
        };

        // Invalid object initializer #7
        var contact7 = new
        {
            Name = ""Baz Foo"",
            PhoneNumbers = new[]
            {
                ""000-000-0000""
            }
        };

        // Invalid object initializer #8
        var contacts8 = new
        {
            Name = ""Baz Foo"",
            PhoneNumbers = new[]
            {
                ""000-000-0000""
            }
        };

        // Invalid object initializer #9
        var contacts9 = new
        {
            Name = ""Baz Foo"",
            PhoneNumbers = new[]
            {
                ""000-000-0000""
            }
        };
    }

    private class Contact
    {
        public string Name { get; set; }

        public List<string> PhoneNumbers { get; set; }
    }
}";

            DiagnosticResult[] expectedDiagnostics =
            {
                // Invalid object initializer #1
                Diagnostic().WithLocation(14, 32),
                Diagnostic().WithLocation(14, 34),
                Diagnostic().WithLocation(14, 36),

                // Invalid object initializer #2
                Diagnostic().WithLocation(18, 5),
                Diagnostic().WithLocation(19, 9),
                Diagnostic().WithLocation(21, 13),

                // Invalid object initializer #3
                Diagnostic().WithLocation(28, 41),
                Diagnostic().WithLocation(29, 21),
                Diagnostic().WithLocation(31, 28),

                // Invalid object initializer #4
                Diagnostic().WithLocation(49, 36),
                Diagnostic().WithLocation(49, 38),
                Diagnostic().WithLocation(49, 40),

                // Invalid object initializer #5
                Diagnostic().WithLocation(53, 9),
                Diagnostic().WithLocation(54, 13),
                Diagnostic().WithLocation(56, 17),

                // Invalid object initializer #6
                Diagnostic().WithLocation(63, 31),
                Diagnostic().WithLocation(64, 25),
                Diagnostic().WithLocation(66, 32),

                // Invalid object initializer #7
                Diagnostic().WithLocation(79, 32),
                Diagnostic().WithLocation(79, 34),

                // Invalid object initializer #8
                Diagnostic().WithLocation(83, 9),
                Diagnostic().WithLocation(85, 13),

                // Invalid object initializer #9
                Diagnostic().WithLocation(90, 29),
                Diagnostic().WithLocation(92, 34),
            };

            await VerifyCSharpFixAsync(testCode, expectedDiagnostics, fixedTestCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that complex element initializers are handled properly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1679, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1679")]
        public async Task TestComplexElementInitializerAsync()
        {
            var testCode = @"using System.Collections.Generic;

public class TestClass
{
    // Invalid object initializer #1
    private Dictionary<int, int> test1 = new Dictionary<int, int> {
        { 1, 1 }
    };

    // Invalid object initializer #2
    private Dictionary<int, int> test2 = new Dictionary<int, int>
    {
        { 1, 1 } };

    // Invalid object initializer #3
    private Dictionary<int, int> test3 = new Dictionary<int, int> {
        { 1, 1 } };
}
";

            var fixedCode = @"using System.Collections.Generic;

public class TestClass
{
    // Invalid object initializer #1
    private Dictionary<int, int> test1 = new Dictionary<int, int>
    {
        { 1, 1 }
    };

    // Invalid object initializer #2
    private Dictionary<int, int> test2 = new Dictionary<int, int>
    {
        { 1, 1 }
    };

    // Invalid object initializer #3
    private Dictionary<int, int> test3 = new Dictionary<int, int>
    {
        { 1, 1 }
    };
}
";

            DiagnosticResult[] expected =
            {
                // Invalid object initializer #1
                Diagnostic().WithLocation(6, 67),

                // Invalid object initializer #2
                Diagnostic().WithLocation(13, 18),

                // Invalid object initializer #3
                Diagnostic().WithLocation(16, 67),
                Diagnostic().WithLocation(17, 18),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode).ConfigureAwait(false);
        }
    }
}
