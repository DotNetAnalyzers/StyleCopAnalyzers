// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.OrderingRules
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.OrderingRules;
    using StyleCop.Analyzers.Settings.ObjectModel;
    using TestHelper;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="UsingCodeFixProvider"/> for the special case where
    /// <see cref="OrderingSettings.BlankLinesBetweenUsingGroups"/> is <see cref="OptionSetting.Require"/>.
    /// </summary>
    public class UsingCodeFixProviderGroupSeparationUnitTests : CodeFixVerifier
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

            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
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
            await this.VerifyCSharpDiagnosticAsync(fixedTestCode, EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await this.VerifyCSharpFixAsync(testCode, fixedTestCode).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override string GetSettings()
        {
            var systemUsingDirectivesFirst = this.systemUsingDirectivesFirst ?? true;

            return $@"
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
        }

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetCSharpDiagnosticAnalyzers()
        {
            yield return new SA1200UsingDirectivesMustBePlacedCorrectly();
            yield return new SA1208SystemUsingDirectivesMustBePlacedBeforeOtherUsingDirectives();
            yield return new SA1209UsingAliasDirectivesMustBePlacedAfterOtherUsingDirectives();
            yield return new SA1210UsingDirectivesMustBeOrderedAlphabeticallyByNamespace();
            yield return new SA1211UsingAliasDirectivesMustBeOrderedAlphabeticallyByAliasName();
            yield return new SA1216UsingStaticDirectivesMustBePlacedAtTheCorrectLocation();
            yield return new SA1217UsingStaticDirectivesMustBeOrderedAlphabetically();
        }

        /// <inheritdoc/>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new UsingCodeFixProvider();
        }
    }
}
