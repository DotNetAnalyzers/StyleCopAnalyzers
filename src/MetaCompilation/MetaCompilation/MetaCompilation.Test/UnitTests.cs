// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;
using MetaCompilation;

namespace MetaCompilation.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        #region default no diagnostics tests
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
        #endregion

        #region MissingId
        //no id, nothing else after
        [TestMethod]
        public void MissingId1()
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
        public void MissingId2()
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

        [TestMethod]
        public void MissingId3()
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
            public string practice = ""IfSpacing"";
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
        public string practice = ""IfSpacing"";
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

        [TestMethod]
        public void MissingId4()
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
            private const string practice = ""IfSpacing"";
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
        public class SyntaxNodeAnalyzer : DiagnosticAnalyzer
        {
        public const string spacingRuleId = ""IfSpacing"";
        private const string practice = ""IfSpacing"";
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
            VerifyCSharpFix(test, fixTest);
        }

        [TestMethod]
        public void MissingId5()
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
            string practice = ""IfSpacing"";
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
        string practice = ""IfSpacing"";
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

        [TestMethod]
        public void MissingId6()
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
            public const int practice = 7;
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
        public const int practice = 7;
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
        public void MissingInit1()
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

        // slight mis-spelling
        [TestMethod]
        public void MissingInit2()
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

        public override void initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
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
            // no test for code fix as the previous code segment produces a new diagnostic
        }

        // everything except the initialize method
        [TestMethod]
        public void MissingInit3()
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
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingInit,
                Message = "The analyzer 'SyntaxNodeAnalyzer' is missing the required Initialize method",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 18) }
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
        public void MissingRegister1()
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

        // register statement in comments
        [TestMethod]
        public void MissingRegister2()
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
            // context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
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
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TooManyInitStatements
        // statement below, incorrect method name
        [TestMethod]
        public void MultipleInit1()
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
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = "The 'Initialize' method registers multiple actions",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 30) }
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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // statement below, incorrect syntax kind
        [TestMethod]
        public void MultipleInit2()
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
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = "The 'Initialize' method registers multiple actions",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 30) }
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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // incorrect statement above
        [TestMethod]
        public void MultipleInit3()
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
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = "The 'Initialize' method registers multiple actions",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 30) }
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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        //multiple incorrect statements below
        [TestMethod]
        public void MultipleInit4()
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
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = "The 'Initialize' method registers multiple actions",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 30) }
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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // multiple incorrect statements above
        [TestMethod]
        public void MultipleInit5()
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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = "The 'Initialize' method registers multiple actions",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 30) }
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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // no correct statements, multiple incorrect
        [TestMethod]
        public void MultipleInit6()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = "The 'Initialize' method registers multiple actions",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 22, 30) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region InvalidStatement
        // invalid throw statement
        [TestMethod]
        public void InvalidStatement1()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
           throw new NotImplementedException();
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'throw new NotImplementedException();' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 24, 12) }
            };

            VerifyCSharpDiagnostic(test,expected);

            var fixtest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid break statement
        [TestMethod]
        public void InvalidStatement2()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
           break;
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'break;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 12) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        //invalid check statement
        [TestMethod]
        public void InvalidStatement3()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
           checked { var num = num + 1; }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'checked { var num = num + 1; }' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 12) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid continue statement
        [TestMethod]
        public void InvalidStatement4()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
           continue;
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'continue;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 12) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // do while statement
        [TestMethod]
        public void InvalidStatement5()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
           do { var i = 1; } while (i > 3);
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'do { var i = 1; } while (i > 3);' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 12) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid random expression statement
        [TestMethod]
        public void InvalidStatement6()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
           context.GetHashCode();
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'context.GetHashCode();' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 12) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid foreach statement
        [TestMethod]
        public void InvalidStatement7()
        {
            var test = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
           foreach() { break; }
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'foreach() { break; }' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 12) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid for statement
        [TestMethod]
        public void InvalidStatement8()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                for(int i = 1; i < 3; i++) { i++; }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'for(int i = 1; i < 3; i++) { i++; }' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid if statement
        [TestMethod]
        public void InvalidStatement9()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                if (i < 3) { i++; }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'if (i < 3) { i++; }' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid labeled statement
        [TestMethod]
        public void InvalidStatement10()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                context: return context;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'context: return context;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid local declaration statement
        [TestMethod]
        public void InvalidStatement11()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                int i;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'int i;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid lock statement
        [TestMethod]
        public void InvalidStatement12()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                lock () {}
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'lock () {}' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // invalid return statement
        [TestMethod]
        public void InvalidStatement13()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                return;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'return;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // multiple invalid statements
        [TestMethod]
        public void InvalidStatement14()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                int one = 1;
                int two = 2;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'int one = 1;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                int two = 2;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // one invalid statement, no valid statements
        [TestMethod]
        public void InvalidStatement15()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                int one = 1;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'int one = 1;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 24, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // multiple invalid statements, no valid statements
        [TestMethod]
        public void InvalidStatement16()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                int one = 1;
                int two = 2;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'int one = 1;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 24, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                int two = 2;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // multiple valid statements, one invalid statement
        [TestMethod]
        public void InvalidStatement17()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
                int one = 1;
                int two = 2;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = "The Initialize method only registers actions: the statement 'int one = 1;' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
                int two = 2;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IncorrectInitSig
        // more than one parameter
        [TestMethod]
        public void InitSig1()
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

        public override void Initialize(AnalysisContext context, int i)
        {
            throw new NotImplementedException();
        }
    }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = "The signature for the 'Initialize' method is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 30) }
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

        // Wrong type for first parameter
        [TestMethod]
        public void InitSig2()
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

        public override void Initialize(int context)
        {
            throw new NotImplementedException();
        }
    }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = "The signature for the 'Initialize' method is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 30) }
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

        // wrong first parameter name
        [TestMethod]
        public void InitSig3()
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

        public override void Initialize(AnalysisContext practice)
        {
            throw new NotImplementedException();
        }
    }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = "The signature for the 'Initialize' method is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 30) }
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

        // accessibility is not public
        [TestMethod]
        public void InitSig4()
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

        private override void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = "The signature for the 'Initialize' method is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 31) }
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

        // initialize method is not overriden
        [TestMethod]
        public void InitSig5()
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

        public void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = "The signature for the 'Initialize' method is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 21) }
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

        // initialize method does not return void
        [TestMethod]
        public void InitSig6()
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

        public override int Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = "The signature for the 'Initialize' method is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 29) }
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

        #region IfStatementIncorrect
        // No identifier for statement
        [TestMethod]
        public void IfStatementIncorrect1()
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
            var = (IfStatementSyntax)context.Node;
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementIncorrect,
                Message = "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 13) }
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
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // ifStatement not initialized
        [TestMethod]
        public void IfStatementIncorrect2()
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
            var ifStatement;
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementIncorrect,
                Message = "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 13) }
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
        }
    }
}";
            VerifyCSharpFix(test, fixtest);
        }

        // no cast
        [TestMethod]
        public void IfStatementIncorrect3()
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
                var ifStatement = context.Node;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementIncorrect,
                Message = "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // Wrong cast type
        [TestMethod]
        public void IfStatementIncorrect()
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
                var ifStatement = (MethodDeclarationSyntax)context.Node;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementIncorrect,
                Message = "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // not a member access expression
        [TestMethod]
        public void IfStatementIncorrect5()
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
                var ifStatement = (IfStatementSyntax)context;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementIncorrect,
                Message = "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // wrong object
        [TestMethod]
        public void IfStatementIncorrect6()
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
                var ifStatement = (IfStatementSyntax)obj.Node;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementIncorrect,
                Message = "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // doesn't access node
        [TestMethod]
        public void IfStatementIncorrect7()
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
                var ifStatement = (IfStatementSyntax)context.SemanticModel;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementIncorrect,
                Message = "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // check that statements below are retained
        [TestMethod]
        public void IfStatementIncorrect8()
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
                var ifStatement = (IfStatementSyntax)context.SemanticModel;
                var ifKeyword = ifStatement.IfKeyword;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementIncorrect,
                Message = "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 17) }
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
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IfKeywordIncorrect
        // not initialized
        [TestMethod]
        public void IfKeywordIncorrect1()
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
                var ifKeyword;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordIncorrect,
                Message = "This statement should extract the 'if' keyword from ifStatement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // no member access expression
        [TestMethod]
        public void IfKeywordIncorrect2()
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
                var ifKeyword = ifStatement;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordIncorrect,
                Message = "This statement should extract the 'if' keyword from ifStatement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // wrong identifier name
        [TestMethod]
        public void IfKeywordIncorrect3()
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
                var ifKeyword = ifState.IfKeyword;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordIncorrect,
                Message = "This statement should extract the 'if' keyword from ifStatement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // doesn't access IfKeyword
        [TestMethod]
        public void IfKeywordIncorrect4()
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
                var ifKeyword = ifStatement.Condition;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordIncorrect,
                Message = "This statement should extract the 'if' keyword from ifStatement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        
        // no variable declaration 
        [TestMethod]
        public void IfKeywordIncorrect5()
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
                ifStatement.IfKeyword;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordIncorrect,
                Message = "This statement should extract the 'if' keyword from ifStatement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // statements below are retained
        [TestMethod]
        public void IfKeywordIncorrect6()
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
                var ifKeyword = ifStatement;
                if (ifKeyword.HasTrailingTrivia)
                {
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordIncorrect,
                Message = "This statement should extract the 'if' keyword from ifStatement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 17) }
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
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IfStatementMissing
        // no statements in analyze method
        [TestMethod]
        public void IfStatementMissing1()
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
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementMissing,
                Message = "The first step of the node analysis is to extract the if statement from context",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 40, 26) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // check comments aren't counted as statements
        [TestMethod]
        public void IfStatementMissing2()
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
                //var ifStatement = (IfStatementSyntax)context.Node;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementMissing,
                Message = "The first step of the node analysis is to extract the if statement from context",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 40, 26) }
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
            //var ifStatement = (IfStatementSyntax)context.Node;
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IfKeywordMissing
        // no 2nd statement
        [TestMethod]
        public void IfKeywordMissing1()
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
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordMissing,
                Message = "The second step is to extract the 'if' keyword from ifStatement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 17) }
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
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // second statement is in the comments
        [TestMethod]
        public void IfKeywordMissing2()
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
                //var ifKeyword = ifStatement.IfKeyword;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordMissing,
                Message = "The second step is to extract the 'if' keyword from ifStatement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 17) }
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
            //var ifKeyword = ifStatement.IfKeyword;
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TrailingTriviaCheckMissing
        // no 3rd statement
        [TestMethod]
        public void TrailingCheckMissing1()
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
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckMissing,
                Message = "The third step is to begin looking for the space between 'if' and '(' by checking if ifKeyword has trailing trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 17) }
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
            }
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // 3rd statement commented
        [TestMethod]
        public void TrailingCheckMissing2()
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
                /* if (ifKeyword.HasTrailingTrivia)
                {
                }/*
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckMissing,
                Message = "The third step is to begin looking for the space between 'if' and '(' by checking if ifKeyword has trailing trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 17) }
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
            }                /* if (ifKeyword.HasTrailingTrivia)
                {
                }/*
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TrailingCheckIncorrect
        // no if statement
        [TestMethod]
        public void TrailingCheckIncorrect1()
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
                (ifKeyword.HasTrailingTrivia)
                {
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                Message = "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 17) }
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
            }

            {
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // misslabeled accessor
        [TestMethod]
        public void TrailingCheckIncorrect2()
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
                if (ifStatement.HasTrailingTrivia)
                {
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                Message = "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 17) }
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
            }
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // Doesnt access HasTrailingTrivia
        [TestMethod]
        public void TrailingCheckIncorrect3()
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
                if (ifKeyword.HasLeadingTrivia)
                {
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                Message = "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 17) }
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
            }
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // throw statement
        [TestMethod]
        public void TrailingCheckIncorrec4()
        {
            var test = @"using System.Collections.Immutable;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.CSharp.Syntax;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                throw new NotImplementedException();
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                Message = "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System.Collections.Immutable;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.CSharp.Syntax;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            }
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // statements below incorrect statement
        [TestMethod]
        public void TrailingCheckIncorrect5()
        {
            var test = @"using System.Collections.Immutable;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.CSharp.Syntax;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                if (ifKeyword.HasLeadingTrivia)
                {
                }
                var openParen = ifStatement.OpenParenToken;
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                Message = "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System.Collections.Immutable;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.CSharp.Syntax;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            }

            var openParen = ifStatement.OpenParenToken;
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // statements within if block
        [TestMethod]
        public void TrailingCheckIncorrect6()
        {
            var test = @"using System.Collections.Immutable;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.CSharp.Syntax;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                if (ifKeyword.HasLeadingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                Message = "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System.Collections.Immutable;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.CSharp.Syntax;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            }
        }
        }
    }";
            VerifyCSharpFix(test, fixtest);
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