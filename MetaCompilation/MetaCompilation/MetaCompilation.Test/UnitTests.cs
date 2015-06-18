// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using System.Runtime;
using MetaCompilation;

namespace MetaCompilation.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        //no diagnostics
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //no diagnostics
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace SyntaxNodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string spacingRuleId = ""IfSpacing"";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, //make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", //allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", //allow any message
            category: ""Syntax"", //make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, //possible options
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(Rule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;
            var ifKeyword = ifStatement.IfKeyword;

            if (ifKeyword.HasTrailingTrivia)
            {
                var trailingTrivia = ifKeyword.TrailingTrivia.Last();

                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                    if (trailingTrivia.ToString() == "" "")
                    {
                        return;
                    }
                }
            }

            var openParen = ifStatement.OpenParenToken;
            var startDiagnosticSpan = ifKeyword.Span.Start;
            var endDiagnosticSpan = openParen.Span.Start;
            var diagnosticSpan = TextSpan.FromBounds(startDiagnosticSpan, endDiagnosticSpan);
            var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, diagnosticSpan);
            var diagnostic = Diagnostic.Create(Rule, diagnosticLocation, Rule.MessageFormat);
            context.ReportDiagnostic(diagnostic);
        }
    }
}";
            VerifyCSharpDiagnostic(test);
        }

        #region MissingId
        //no id, nothing else after
        [TestMethod]
        public void TestMethod3()
        {
            var test = @"using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    namespace SyntaxNodeAnalyzer
    {
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        public class SyntaxNodeAnalyzer : DiagnosticAnalyzer
        {
            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override void Initialize(AnalysisContext context)
            {
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingId,
                Message = "The analyzer 'SyntaxNodeAnalyzer' is missing a diagnostic id",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 22) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    namespace SyntaxNodeAnalyzer
    {
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        public class SyntaxNodeAnalyzer : DiagnosticAnalyzer
        {
        public const string spacingRuleId = ""IfSpacing"";

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override void Initialize(AnalysisContext context)
            {
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // no id, rules exists
        [TestMethod]
        public void TestMethod6()
        {
            var test = @"using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    namespace SyntaxNodeAnalyzer
    {
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        public class SyntaxNodeAnalyzer : DiagnosticAnalyzer
        {
            internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
                id: SpacingRuleId, //make the id specific
                title: ""If statement must have a space between the 'if' keyword and the boolean expression"", 
                messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
                category: ""Syntax"",
                defaultSeverity.Warning,
                isEnabledByDefault: true);

            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override void Initialize(AnalysisContext context)
            {
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingId,
                Message = "The analyzer 'SyntaxNodeAnalyzer' is missing a diagnostic id",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 22) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    namespace SyntaxNodeAnalyzer
    {
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        public class SyntaxNodeAnalyzer : DiagnosticAnalyzer
        {
        public const string spacingRuleId = ""IfSpacing"";
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
                id: SpacingRuleId, //make the id specific
                title: ""If statement must have a space between the 'if' keyword and the boolean expression"", 
                messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
                category: ""Syntax"",
                defaultSeverity.Warning,
                isEnabledByDefault: true);

            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override void Initialize(AnalysisContext context)
            {
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingInit
        [TestMethod]
        public void TestMethod4()
        {
            var test = @"using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    namespace SyntaxNodeAnalyzer
    {
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        public class SyntaxNodeAnalyzer : DiagnosticAnalyzer
        {
            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingInit,
                Message = "The analyzer 'SyntaxNodeAnalyzer' is missing the required Initialize method",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 16, 22) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    namespace SyntaxNodeAnalyzer
    {
        [DiagnosticAnalyzer(LanguageNames.CSharp)]
        public class SyntaxNodeAnalyzer : DiagnosticAnalyzer
        {
            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

        public override void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingRegisterStatement
        [TestMethod]
        public void TestMethod5()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace SyntaxNodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Initialize(AnalysisContext context)
        {
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingRegisterStatement,
                Message = "An action must be registered within the 'Initialize' method",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 30) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixTest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace SyntaxNodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";
            VerifyCSharpFix(test, fixTest);
        }
        #endregion

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MetaCompilationCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MetaCompilationAnalyzer();
        }
    }
}