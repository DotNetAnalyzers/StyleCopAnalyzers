// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.LayoutRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.LayoutRules;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for <see cref="SA1500CurlyBracketsForMultiLineStatementsMustNotShareLine"/>.
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

            await this.VerifyCSharpDiagnosticAsync(testCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
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
                this.CSharpDiagnostic().WithLocation(14, 32),
                this.CSharpDiagnostic().WithLocation(14, 34),
                this.CSharpDiagnostic().WithLocation(14, 36),

                // Invalid object initializer #2
                this.CSharpDiagnostic().WithLocation(18, 5),
                this.CSharpDiagnostic().WithLocation(19, 9),
                this.CSharpDiagnostic().WithLocation(21, 13),

                // Invalid object initializer #3
                this.CSharpDiagnostic().WithLocation(28, 41),
                this.CSharpDiagnostic().WithLocation(29, 21),
                this.CSharpDiagnostic().WithLocation(31, 28),

                // Invalid object initializer #4
                this.CSharpDiagnostic().WithLocation(49, 36),
                this.CSharpDiagnostic().WithLocation(49, 38),
                this.CSharpDiagnostic().WithLocation(49, 40),

                // Invalid object initializer #5
                this.CSharpDiagnostic().WithLocation(53, 9),
                this.CSharpDiagnostic().WithLocation(54, 13),
                this.CSharpDiagnostic().WithLocation(56, 17),

                // Invalid object initializer #6
                this.CSharpDiagnostic().WithLocation(63, 31),
                this.CSharpDiagnostic().WithLocation(64, 25),
                this.CSharpDiagnostic().WithLocation(66, 32),

                // Invalid object initializer #7
                this.CSharpDiagnostic().WithLocation(79, 32),
                this.CSharpDiagnostic().WithLocation(79, 34),

                // Invalid object initializer #8
                this.CSharpDiagnostic().WithLocation(83, 9),
                this.CSharpDiagnostic().WithLocation(85, 13),

                // Invalid object initializer #9
                this.CSharpDiagnostic().WithLocation(90, 29),
                this.CSharpDiagnostic().WithLocation(92, 34)
            };

            await this.VerifyCSharpDiagnosticAsync(testCode, expectedDiagnostics, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }
    }
}
