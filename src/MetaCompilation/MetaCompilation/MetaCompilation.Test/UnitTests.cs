// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using MetaCompilation;
using Xunit;

namespace MetaCompilation.Test
{
    public class UnitTest : CodeFixVerifier
    {
        public const string MessagePrefix = "T: ";
        
        #region default no diagnostics tests
        //no diagnostics
        [Fact]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //no diagnostics
        [Fact]
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
        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing a diagnostic id",
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
        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing a diagnostic id",
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

        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing a diagnostic id",
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

        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing a diagnostic id",
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

        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing a diagnostic id",
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

        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing a diagnostic id",
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
        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing the required Initialize method",
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

        //slight misspelling
        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing the required Initialize method",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 16, 22) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        // everything except the initialize method
        [Fact]
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
                Message = MessagePrefix + "The analyzer 'SyntaxNodeAnalyzer' is missing the required Initialize method",
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
        [Fact]
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
                Message = MessagePrefix + "An action must be registered within the 'Initialize' method",
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
        [Fact]
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
                Message = MessagePrefix + "An action must be registered within the 'Initialize' method",
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
        [Fact]
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
                Message = MessagePrefix + "The 'Initialize' method registers multiple actions",
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
        [Fact]
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
                Message = MessagePrefix + "The 'Initialize' method registers multiple actions",
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
        [Fact]
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
                Message = MessagePrefix + "The 'Initialize' method registers multiple actions",
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
        [Fact]
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
                Message = MessagePrefix + "The 'Initialize' method registers multiple actions",
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
        [Fact]
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
                Message = MessagePrefix + "The 'Initialize' method registers multiple actions",
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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        // no correct statements, multiple incorrect
        [Fact]
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
                Message = MessagePrefix + "The 'Initialize' method registers multiple actions",
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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region InvalidStatement
        // invalid throw statement
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'throw new NotImplementedException();' is invalid",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 24, 12) }
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

        // invalid break statement
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'break;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'checked { var num = num + 1; }' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'continue;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'do { var i = 1; } while (i > 3);' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'context.GetHashCode();' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'foreach() { break; }' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'for(int i = 1; i < 3; i++) { i++; }' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'if (i < 3) { i++; }' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'context: return context;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'int i;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'lock () {}' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'return;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'int one = 1;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'int one = 1;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'int one = 1;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The Initialize method only registers actions: the statement 'int one = 1;' is invalid",
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
        [Fact]
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
                Message = MessagePrefix + "The signature for the 'Initialize' method is incorrect",
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
        [Fact]
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
                Message = MessagePrefix + "The signature for the 'Initialize' method is incorrect",
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
        [Fact]
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

        private override void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = MessagePrefix + "The signature for the 'Initialize' method is incorrect",
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
        [Fact]
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

        public void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = MessagePrefix + "The signature for the 'Initialize' method is incorrect",
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
        [Fact]
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

        public override int Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = MessagePrefix + "The signature for the 'Initialize' method is incorrect",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the if statement in question by casting context.Node to IfStatementSyntax",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the 'if' keyword from ifStatement",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the 'if' keyword from ifStatement",
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
        [Fact]
        public void IfKeyword3()
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
                Message = MessagePrefix + "This statement should extract the 'if' keyword from ifStatement",
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
        [Fact]
        public void IfKeyword4()
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
                Message = MessagePrefix + "This statement should extract the 'if' keyword from ifStatement",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the 'if' keyword from ifStatement",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should extract the 'if' keyword from ifStatement",
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
        [Fact]
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
                Message = MessagePrefix + "The first step of the node analysis is to extract the if statement from context",
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
        [Fact]
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
                Message = MessagePrefix + "The first step of the node analysis is to extract the if statement from context",
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
        [Fact]
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
                Message = MessagePrefix + "The second step is to extract the 'if' keyword from ifStatement",
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
        [Fact]
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
                Message = MessagePrefix + "The second step is to extract the 'if' keyword from ifStatement",
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
        [Fact]
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
                Message = MessagePrefix + "The third step is to begin looking for the space between 'if' and '(' by checking if ifKeyword has trailing trivia",
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
        [Fact]
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
                Message = MessagePrefix + "The third step is to begin looking for the space between 'if' and '(' by checking if ifKeyword has trailing trivia",
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

        #region TrailingTriviaCheckIncorrect
        // no if statement
        [Fact]
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
                Message = MessagePrefix + "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
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
        [Fact]
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
                Message = MessagePrefix + "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
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

        // incorrect statement is next if statement
        [Fact]
        public void TrailingCheckIncorrect7()
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaCheckIncorrect,
                Message = MessagePrefix + "This statement should be an if statement that checks to see if ifKeyword has trailing trivia",
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
        #endregion

        #region TrailingTriviaVarMissing
        // no variable declaration
        [Fact]
        public void TrailingVarMissing1()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                Message = MessagePrefix + "The fourth step is to extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 40, 17) }
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

        // statement below if block
        [Fact]
        public void TrailingVarMissing2()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                }
                var trailing = ifKeyword.TrailingTrivia.Last();
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                Message = MessagePrefix + "The fourth step is to extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 40, 17) }
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
                var trailing = ifKeyword.TrailingTrivia.Last();
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // declaration in comments
        [Fact]
        public void TrailingVarMissing3()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    //var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                Message = MessagePrefix + "The fourth step is to extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 40, 17) }
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
                //var trailingTrivia = ifKeyword.TrailingTrivia.Last();
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TrailingTriviaVarIncorrect
        // not initialized
        [Fact]
        public void TrailingVarIncorrect1()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia;
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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

        // no member access expressions
        [Fact]
        public void TrailingVarIncorrect2()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword;
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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

        //only one member access expression
        [Fact]
        public void TrailingVarIncorrect3()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia;
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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
        // member access expression order switched
        [Fact]
        public void TrailingVarIncorrect4()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.Last().TrailingTrivia;
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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

        // throw statement
        [Fact]
        public void TrailingVarIncorrect5()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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

        // wrong accessor
        [Fact]
        public void TrailingVarIncorrect6()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifStatement.TrailingTrivia.Last();
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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

        // statements below if block
        [Fact]
        public void TrailingVarIncorrect7()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifStatement.TrailingTrivia.Last();
                }
                var openParen = ifStatement.OpenParenToken;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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
                var openParen = ifStatement.OpenParenToken;
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // statements within if block
        [Fact]
        public void TrailingVarIncorrect8()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifStatement.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //incorrect statement is the next statement
        [Fact]
        public void TrailingVarIncorrect9()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = MessagePrefix + "This statement should extract the last trailing trivia of ifKeyword into a variable",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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

        #region InternalAndStaticError
        [Fact]
        public void InternalAndStatic1() //missing internal modifier
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

        static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, //make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", //allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", //allow any message
            category: ""Syntax"", //make the category specific
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InternalAndStaticError,
                Message = MessagePrefix + "The Rule field should be internal and static",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 19, 37) }
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
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void InternalAndStatic2() //missing static modifier
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

        internal DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, //make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", //allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", //allow any message
            category: ""Syntax"", //make the category specific
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InternalAndStaticError,
                Message = MessagePrefix + "The Rule field should be internal and static",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 19, 39) }
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
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void InternalAndStatic3() //missing both modifiers
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

         DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, //make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", //allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", //allow any message
            category: ""Syntax"", //make the category specific
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InternalAndStaticError,
                Message = MessagePrefix + "The Rule field should be internal and static",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 19, 31) }
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
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void InternalAndStatic4() //modifiers = "static internal"
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

        static internal DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, //make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", //allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", //allow any message
            category: ""Syntax"", //make the category specific
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            VerifyCSharpDiagnostic(test);
        }
        #endregion

        #region EnabledByDefault
        [Fact]
        public void EnabledByDefault1() //isEnabledByDefault set to false
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
            isEnabledByDefault: false);

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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EnabledByDefaultError,
                Message = MessagePrefix + "isEnabledByDefault should be set to true",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 33) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void EnabledByDefault2() //isEnabledByDefault set to undeclared variable
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
            isEnabledByDefault: test);

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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EnabledByDefaultError,
                Message = MessagePrefix + "isEnabledByDefault should be set to true",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 33) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void EnabledByDefault3() //isEnabledByDefault set to member access expression
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
            isEnabledByDefault: DiagnosticSeverity.Error);

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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EnabledByDefaultError,
                Message = MessagePrefix + "isEnabledByDefault should be set to true",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 33) }
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void EnabledByDefault4() //isEnabledByDefault with argument missing
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
            isEnabledByDefault: );
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EnabledByDefaultError,
                Message = MessagePrefix + "isEnabledByDefault should be set to true",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 23, 13) }
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region DefaultSeverityError
        [Fact]
        public void DefaultSeverity1() //defaultSeverity set to undeclared variable.
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
            defaultSeverity: test, //possible options
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DefaultSeverityError,
                Message = MessagePrefix + "defaultSeverity must be of the form: DiagnosticSeverity.[severity]",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 24, 30) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtestError = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            var fixtestWarning = @"using System;
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
    }
}";

            var fixtestHidden = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Hidden, //possible options
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
    }
}";

            var fixtestInfo = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Info, //possible options
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
    }
}";

            VerifyCSharpFix(test, fixtestError, 0);
            VerifyCSharpFix(test, fixtestWarning, 1);
            VerifyCSharpFix(test, fixtestHidden, 2);
            VerifyCSharpFix(test, fixtestInfo, 3);
        }

        [Fact]
        public void DefaultSeverity2() //defaultSeverity.Name set to arbitrary string
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
            defaultSeverity: DiagnosticSeverity.test, //possible options
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DefaultSeverityError,
                Message = MessagePrefix + "defaultSeverity must be of the form: DiagnosticSeverity.[severity]",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 24, 30) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtestError = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            var fixtestWarning = @"using System;
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
    }
}";

            var fixtestHidden = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Hidden, //possible options
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
    }
}";

            var fixtestInfo = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Info, //possible options
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
    }
}";

            VerifyCSharpFix(test, fixtestError, 0);
            VerifyCSharpFix(test, fixtestWarning, 1);
            VerifyCSharpFix(test, fixtestHidden, 2);
            VerifyCSharpFix(test, fixtestInfo, 3);
        }

        [Fact]
        public void DefaultSeverity3() //defaultSeverity with argument missing
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
            defaultSeverity: , //possible options
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DefaultSeverityError,
                Message = MessagePrefix + "defaultSeverity must be of the form: DiagnosticSeverity.[severity]",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 22, 13) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtestError = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Error, //possible options
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
    }
}";

            var fixtestWarning = @"using System;
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
    }
}";

            var fixtestHidden = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Hidden, //possible options
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
    }
}";

            var fixtestInfo = @"using System;
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
            defaultSeverity: DiagnosticSeverity.Info, //possible options
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
    }
}";

            VerifyCSharpFix(test, fixtestError, 0);
            VerifyCSharpFix(test, fixtestWarning, 1);
            VerifyCSharpFix(test, fixtestHidden, 2);
            VerifyCSharpFix(test, fixtestInfo, 3);
        }
        #endregion

        #region IdDeclTypeError
        [Fact]
        public void IdDeclType1() //id set to a literal string
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
            id: ""test"", //make the id specific
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IdDeclTypeError,
                Message = MessagePrefix + "The diagnostic id should be the const declared above this",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 20, 17) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IdDeclType2() //id set to a member access expression
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
            id: DiagnosticSeverity.Warning, //make the id specific
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IdDeclTypeError,
                Message = MessagePrefix + "The diagnostic id should be the const declared above this",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 18, 17) }
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IdDeclType3() //id set to true
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
            id: true, //make the id specific
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IdDeclTypeError,
                Message = MessagePrefix + "The diagnostic id should be the const declared above this",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 18, 17) }
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IdDeclType4() //id with argument missing
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
            id: , //make the id specific
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IdDeclTypeError,
                Message = MessagePrefix + "The diagnostic id should be the const declared above this",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 18, 13) }
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingIdDeclaration
        [Fact]
        public void MissingIdDeclaration1() //id set to undeclared variable
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
            id: test, //make the id specific
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingIdDeclaration,
                Message = MessagePrefix + "This diagnostic id has not been declared",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 20, 17) }
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
        public const string test = ""DescriptiveId"";
        public const string spacingRuleId = ""IfSpacing"";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: test, //make the id specific
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region OpenParenTests
        [Fact]

        public void MissingOpenParen() //no DiagnosticDescriptor field
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.OpenParenMissing,
                Message = MessagePrefix + "The next step is to extract the open parenthesis of the if statement condition",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 45, 17) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var openParen = ifState.OpenParenToken;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectOpenParen()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
                var ifState = (IfStatementSyntax)context.Node;
                var ifKeyword = ifState.IfKeyword;

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

                var test = ifState.Equals;
            }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.OpenParenIncorrect,
                Message = MessagePrefix + "This statement should extract the open parenthesis of ifState to use as the end of the diagnostic span",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 57, 17) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
                var ifState = (IfStatementSyntax)context.Node;
                var ifKeyword = ifState.IfKeyword;

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

            var openParen = ifState.OpenParenToken;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingSuppDiag
        [Fact]
        public void MissingSuppDiag1() //no SupportedDiagnostics property
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingSuppDiag,
                Message = MessagePrefix + "You are missing the required SupportedDiagnostics property",
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
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string spacingRuleId = ""IfSpacing"";
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IncorrectSigSuppDiag
        [Fact]
        public void IncorrectSigSuppDiag1() //no public modifier
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectSigSuppDiag,
                Message = MessagePrefix + "The signature of the SupportedDiagnostics property is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 55) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectSigSuppDiag2() //no override modifier
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectSigSuppDiag,
                Message = MessagePrefix + "The signature of the SupportedDiagnostics property is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 53) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectSigSuppDiag3() //no modifiers
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectSigSuppDiag,
                Message = MessagePrefix + "The signature of the SupportedDiagnostics property is incorrect",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 46) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingAccessor
        [Fact]
        public void MissingAccessor1() //empty SupportedDiagnostics property
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {

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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingAccessor,
                Message = MessagePrefix + "The SupportedDiagnostics property is missing a get accessor",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 62) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void MissingAccessor2() //SupportedDiagnostics property contains only set accessor
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            set
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingAccessor,
                Message = MessagePrefix + "The SupportedDiagnostics property is missing a get accessor",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 62) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TooManyAccessors

        [Fact]
        public void TooManyAccessors1() //SupportedDiagnostics property with get and then set accessors
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                throw new NotImplementedException();
            }
            set
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyAccessors,
                Message = MessagePrefix + "The SupportedDiagnostics property only needs one get accessor, no additional get accessors or any set accessors are needed",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 32, 13) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }


        [Fact]
        public void TooManyAccessors2() //SupportedDiagnostics property with set and then get accessors
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            set
            {
                throw new NotImplementedException();
            }
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyAccessors,
                Message = MessagePrefix + "The SupportedDiagnostics property only needs one get accessor, no additional get accessors or any set accessors are needed",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 32, 13) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }


        [Fact]
        public void TooManyAccessors3() //SupportedDiagnostics property with two get accessors
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create();
            }
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyAccessors,
                Message = MessagePrefix + "The SupportedDiagnostics property only needs one get accessor, no additional get accessors or any set accessors are needed",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 32, 13) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create();
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

        #region AccessorReturnValue (fix for IncorrectAccessorReturn & SuppDiagReturn)

        [Fact]
        public void AccessorReturnValue1() //check that "return array;" is supported. ie SupportedRules diagnostic should surface
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var array = ImmutableArray.Create();
                return array;
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SupportedRules,
                Message = MessagePrefix + "The immutable array should contain every DiagnosticDescriptor rule that was created",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 30, 29) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [Fact]
        public void IncorrectAccessorReturn1() //empty get accessor
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {

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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAccessorReturn,
                Message = MessagePrefix + "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 62) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create();
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void IncorrectAccessorReturn2() //get accessor throwing NotImplementedException
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAccessorReturn,
                Message = MessagePrefix + "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 28, 13) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create();
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void IncorrectAccessorReturn3() //get accessor returning nothing
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return;
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAccessorReturn,
                Message = MessagePrefix + "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 30, 17) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create();
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void IncorrectAccessorReturn4() //get accessor returning true
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return true;
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAccessorReturn,
                Message = MessagePrefix + "The get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 30, 17) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create();
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void SuppDiagReturn1() //get accessor returning incorrect invocation expression
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Equals();
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SuppDiagReturnValue,
                Message = MessagePrefix + "The SupportedDiagnostics property's get accessor needs to return an ImmutableArray containing all of your DiagnosticDescriptor rules",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 30, 17) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create();
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }
        #endregion

        #region SupportedRules
        [Fact]
        public void SupportedRules1() //invocation expression form with no arguments
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create();
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SupportedRules,
                Message = MessagePrefix + "The immutable array should contain every DiagnosticDescriptor rule that was created",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 30, 24) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void SupportedRules2() //variable declaration form with no arguments
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var array = ImmutableArray.Create();
                return array;
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SupportedRules,
                Message = MessagePrefix + "The immutable array should contain every DiagnosticDescriptor rule that was created",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 30, 29) }
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
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var array = ImmutableArray.Create(Rule);
                return array;
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

        [Fact]
        public void SupportedRules3() //invocation expression form with missing rules
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
        public const string spacingRuleId2 = ""IfSpacing2"";
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
        internal static DiagnosticDescriptor Rule2 = new DiagnosticDescriptor(
            id: spacingRuleId2,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SupportedRules,
                Message = MessagePrefix + "The immutable array should contain every DiagnosticDescriptor rule that was created",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 38, 24) }
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
        public const string spacingRuleId2 = ""IfSpacing2"";
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
        internal static DiagnosticDescriptor Rule2 = new DiagnosticDescriptor(
            id: spacingRuleId2,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(Rule, Rule2);
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

        [Fact]
        public void SupportedRules4() //variable declaration form with missing rules
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
        public const string spacingRuleId2 = ""IfSpacing2"";
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
        internal static DiagnosticDescriptor Rule2 = new DiagnosticDescriptor(
            id: spacingRuleId2,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var array = ImmutableArray.Create(Rule);
                return array;
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

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SupportedRules,
                Message = MessagePrefix + "The immutable array should contain every DiagnosticDescriptor rule that was created",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 38, 29) }
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
        public const string spacingRuleId2 = ""IfSpacing2"";
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
        internal static DiagnosticDescriptor Rule2 = new DiagnosticDescriptor(
            id: spacingRuleId2,
            title: ""title"",
            messageFormat: ""message"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                var array = ImmutableArray.Create(Rule, Rule2);
                return array;
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

        #region StartSpanTests
        [Fact]
        public void MissingStartSpan()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.StartSpanMissing,
                Message = MessagePrefix + "The next step is to determine the start of the span for the diagnostic that will be reported",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 57, 13) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var startDiagnosticSpan = ifKeyword.Span.Start;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectStartSpan()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifState.Span.Start;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.StartSpanIncorrect,
                Message = MessagePrefix + "This statement should extract the start of the span of ifKeyword into a variable, to be used as the start of the diagnostic span",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 58, 13) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var startDiagnosticSpan = ifKeyword.Span.Start;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region EndSpanTests
        [Fact]
        public void MissingEndSpan()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EndSpanMissing,
                Message = MessagePrefix + "The next step is to determine the end of the span for the diagnostic that is going to be reported",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 58, 13) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var endDiagnosticSpan = open.Span.Start;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectEndSpan()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            return 1;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EndSpanIncorrect,
                Message = MessagePrefix + "This statement should extract the start of the span of open into a variable, to be used as the end of the diagnostic span",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 59, 13) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var endDiagnosticSpan = open.Span.Start;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region SpanTests
        [Fact]
        public void MissingSpan()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SpanMissing,
                Message = MessagePrefix + "The next step is to create a variable that is the span of the diagnostic that will be reported",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 59, 13) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var diagnosticSpan = TextSpan.FromBounds(start, end);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectSpan()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            if (true) {}
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SpanIncorrect,
                Message = MessagePrefix + "This statement should use TextSpan.FromBounds, start, and end to create the span of the diagnostic that will be reported",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 60, 13) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var diagnosticSpan = TextSpan.FromBounds(start, end);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region SpanTests
        [Fact]
        public void MissingLocation()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var span = TextSpan.FromBounds(start, end);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.LocationMissing,
                Message = MessagePrefix + "The next step is to create a location for the diagnostic",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 60, 13) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var span = TextSpan.FromBounds(start, end);
            var diagnosticLocation = Location.Create(ifState.SyntaxTree, span);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectLocation()
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var span = TextSpan.FromBounds(start, end);
            var diagnosticLocation = ""Hello World"";
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.LocationIncorrect,
                Message = MessagePrefix + "This statement should use Location.Create, ifState, and span to create the location of the diagnostic",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 61, 13) }
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
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
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
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var span = TextSpan.FromBounds(start, end);
            var diagnosticLocation = Location.Create(ifState.SyntaxTree, span);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region DiagnosticTests
        [Fact]
        public void MissingDiagnostic()
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

        internal static DiagnosticDescriptor spacingRule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(spacingRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticMissing,
                Message = MessagePrefix + "The next step is to create a variable to hold the diagnostic",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 61, 13) }
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

        internal static DiagnosticDescriptor spacingRule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(spacingRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location, spacingRule.MessageFormat);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectDiagnostic()
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

        internal static DiagnosticDescriptor spacingRule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(spacingRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            diagnostic = Diagnostic.Create(spacingRule, location, spacingRule.MessageFormat);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticIncorrect,
                Message = MessagePrefix + "This statement should use Diagnostic.Create, spacingRule, and location to create the diagnostic that will be reported",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 62, 13) }
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

        internal static DiagnosticDescriptor spacingRule = new DiagnosticDescriptor(
            id: spacingRuleId,
            title: ""If statement must have a space between 'if' and the boolean expression"",
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"",
            category: ""Syntax"",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(spacingRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifState = (IfStatementSyntax)context.Node;
            var ifKeyword = ifState.IfKeyword;

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

            var open = ifState.OpenParenToken;
            var start = ifKeyword.Span.Start;
            var end = open.Span.Start;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location, spacingRule.MessageFormat);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TrailingTriviaKindCheckMissing
        [Fact]
        public void TrailingKindMissing1() // no whitespace check
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckMissing,
                Message = MessagePrefix + "The fifth step is to check the kind of trailingTrivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TrailingTriviaKindCheckIncorrect
        //random variable declaration
        [Fact]
        public void TrailingKindIncorrect1()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    var ifCheck = true;
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            { 
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // wrong variable name
        [Fact]
        public void TrailingKindIncorrect2()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (ifKeyword.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            { 
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // Doesn't access kind method
        [Fact]
        public void TriviaKindIncorrect3()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // Accesses different method (not kind)
        [Fact]
        public void TrailingKindIncorrect4()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.IsKind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //one equals sign
        [Fact]
        public void TrailingKindIncorrect5()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() = SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            { 
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //wrong member accessor
        [Fact]
        public void TrailingKindIncorrect6()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SymbolKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // wrong accessed
        [Fact]
        public void TrailingKindIncorrect7()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.IfStatement)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // first statement not member access
        [Fact]
        public void TrailingKindIncorrect8()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // second statement not member access
        [Fact]
        public void TrailingKindIncorrect9()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // no condition
        [Fact]
        public void TrailingKindIncorrect10()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if ()
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // incorrect statement is next statement
        [Fact]
        public void TrailingKindIncorrect11()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                }
            }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // statements within if statement
        [Fact]
        public void TrailingKind12()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia == SyntaxKind.WhitespaceTrivia)
                    {
                        var one = 1;
                        one++;
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if the kind of trailingTrivia is whitespace trivia",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 42, 21) }
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
                if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                {
                    var one = 1;
                        one++;
                }
            }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region WhitespaceCheckMissing
        // no whitespace check
        [Fact]
        public void WhitespaceMissing1()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckMissing,
                Message = MessagePrefix + "The sixth step is to make sure trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 21) }
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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region WhitespaceCheckIncorrect
        // random variable declaration
        [Fact]
        public void WhitespaceIncorrect1()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        var one = 1;
                    }
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };
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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // wrong variable name
        [Fact]
        public void WhitespaceIncorrect2()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (ifKeyword.ToString() == "" "")
                        {
                        }
                    }
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        // wrong method accessed
        [Fact]
        public void WhitespaceIncorrect3()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.FullSpan == "" "")
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // no member access expression
        [Fact]
        public void WhitespaceIncorrect4()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia == "" "")
                        {
                        }
                    }
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        // wrong equals sign
        [Fact]
        public void WhitespaceIncorrect5()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() = "" "")
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // wrong condition
        [Fact]
        public void WhitespaceIncorrect6()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == ""trailingTrivia.ToString()"")
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };
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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // empty condition
        [Fact]
        public void WhitespaceIncorrect7()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if ()
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // previous if statement
        [Fact]
        public void WhitespaceIncorrect8()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // next statement
        [Fact]
        public void WhitespaceIncorrect9()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        return;
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                    }
                }
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        //statements within incorrect if
        [Fact]
        public void WhitespaceIncorrect10()
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
                if (ifKeyword.HasTrailingTrivia)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.Last();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia == "" "")
                        {
                            return;
                        }
                    }
                }
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = MessagePrefix + "This statement should check to see if trailingTrivia is a single whitespace",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

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
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    if (trailingTrivia.ToString() == "" "")
                    {
                        return;
                    }
                }
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingRule
        [Fact]
        public void MissingRule1() //Rule id but no rule
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

namespace SyntaxNodeAnalyzerAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingRule,
                Message = MessagePrefix + "You need to have at least one DiagnosticDescriptor rule",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 17, 29) }
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

namespace SyntaxNodeAnalyzerAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string spacingRuleId = ""IfSpacing"";
        internal static DiagnosticDescriptor spacingRule = new DiagnosticDescriptor(id: , title: ""Enter a title for this diagnostic"", messageFormat: ""Enter a message to be displayed with this diagnostic"", category: ""Enter a category for this diagnostic"", defaultSeverity: , isEnabledByDefault: );

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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext obj)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
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