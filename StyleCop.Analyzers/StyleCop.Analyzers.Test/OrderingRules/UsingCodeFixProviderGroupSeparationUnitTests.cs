// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using StyleCop.Analyzers.Test.Verifiers;
    using TestHelper;
    using Xunit;
    using static CombinedUsingDirectivesVerifier;

    /// <summary>
    /// Unit tests for the <see cref="UsingCodeFixProvider"/> for the special case where
    /// <see cref="OrderingSettings.BlankLinesBetweenUsingGroups"/> is <see cref="OptionSetting.Require"/>.
    /// </summary>
    public class UsingCodeFixProviderGroupSeparationUnitTests
    {
        private bool usingsInsideNamespace = true;
        private bool? systemUsingDirectivesFirst;

        /// <summary>
        /// Verifies that the code fix will properly reorder using statements.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyUsingReorderingAsync()
        {
            this.systemUsingDirectivesFirst = true;

            var testCode = @"using Microsoft.CodeAnalysis;
using SystemAction = System.Action;
using static System.Math;
using System;
using static System.String;
using MyFunc = System.Func<int,bool>;
using System.Collections.Generic;
using System.Collections;

namespace Foo
{
    public class Bar
    {
    }
}
";

            var fixedTestCode = @"namespace Foo
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Microsoft.CodeAnalysis;

    using static System.Math;
    using static System.String;

    using MyFunc = System.Func<int,bool>;
    using SystemAction = System.Action;

    public class Bar
    {
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(1, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(2, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(3, 1),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(3, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(4, 1),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(4, 1).WithArguments("System", "System.Math"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(5, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(6, 1),
                StyleCopDiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(6, 1),
                StyleCopDiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(6, 1).WithArguments("MyFunc", "SystemAction"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(7, 1),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(7, 1).WithArguments("System.Collections.Generic", "System.Math"),
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(7, 1),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorInside).WithLocation(8, 1),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(8, 1).WithArguments("System.Collections", "System.Math"),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyUsingReorderingOutsideNamespaceAsync()
        {
            this.systemUsingDirectivesFirst = true;

            var testCode = @"namespace Foo
{
    using Microsoft.CodeAnalysis;
    using SystemAction = System.Action;
    using static System.Math;
    using System;
    using static System.String;
    using MyFunc = System.Func<int,bool>;
    using System.Collections.Generic;
    using System.Collections;

    public class Bar
    {
    }
}
";

            var fixedTestCode = @"using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

using static System.Math;
using static System.String;

using MyFunc = System.Func<int, bool>;
using SystemAction = System.Action;

namespace Foo
{
    public class Bar
    {
    }
}
";

            this.usingsInsideNamespace = false;
            DiagnosticResult[] expected =
            {
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(3, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(4, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(5, 5),
                StyleCopDiagnosticVerifier<SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation>.Diagnostic().WithLocation(5, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(6, 5),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(6, 5).WithArguments("System", "System.Math"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(7, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(8, 5),
                StyleCopDiagnosticVerifier<SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives>.Diagnostic().WithLocation(8, 5),
                StyleCopDiagnosticVerifier<SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName>.Diagnostic().WithLocation(8, 5).WithArguments("MyFunc", "SystemAction"),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(9, 5),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(9, 5).WithArguments("System.Collections.Generic", "System.Math"),
                StyleCopDiagnosticVerifier<SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace>.Diagnostic().WithLocation(9, 5),
                Diagnostic(SA1200UsingDirectivesMustBePlacedCorrectly.DescriptorOutside).WithLocation(10, 5),
                StyleCopDiagnosticVerifier<SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives>.Diagnostic().WithLocation(10, 5).WithArguments("System.Collections", "System.Math"),
            };
            await this.VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        private Task VerifyCSharpFixAsync(string source, DiagnosticResult[] expected, string fixedSource, CancellationToken cancellationToken)
        {
            var systemUsingDirectivesFirst = this.systemUsingDirectivesFirst ?? true;

            var testSettings = $@"
{{
    ""settings"": {{
        ""orderingRules"": {{
            ""usingDirectivesPlacement"": ""{(this.usingsInsideNamespace ? "insideNamespace" : "outsideNamespace")}"",
            ""systemUsingDirectivesFirst"" : {systemUsingDirectivesFirst.ToString().ToLowerInvariant()},
            ""blankLinesBetweenUsingGroups"": ""require""
        }}
    }}
}}
";

            var test = new CSharpTest
            {
                TestCode = source,
                FixedCode = fixedSource,
                Settings = testSettings,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(cancellationToken);
        }
    }
}
