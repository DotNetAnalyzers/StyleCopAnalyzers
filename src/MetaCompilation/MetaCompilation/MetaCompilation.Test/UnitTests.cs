//  Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
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
        // no diagnostics
        [Fact]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        // no diagnostics
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();

                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var openParen = ifStatement.OpenParenToken;
            var startDiagnosticSpan = ifKeyword.SpanStart;
            var endDiagnosticSpan = openParen.SpanStart;
            var diagnosticSpan = TextSpan.FromBounds(startDiagnosticSpan, endDiagnosticSpan);
            var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, diagnosticSpan);
            var diagnostic = Diagnostic.Create(Rule, diagnosticLocation, Rule.MessageFormat);
            context.ReportDiagnostic(diagnostic);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.GoToCodeFix,
                Message = MessagePrefix + "Congratulations! You have written an analyzer! If you would like to explore a code fix for your diagnostic, open up CodeFixProvider.cs and take a look!",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 18) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        // no diagnostics
        [Fact]
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
    public class SyntaxNodeAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string spacingRuleId = ""IfSpacing"";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            var ifStatement = context.Node as IfStatementSyntax;
            var ifKeyword = ifStatement.IfKeyword;

            if (ifKeyword.HasTrailingTrivia)
            {
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();

                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var openParen = ifStatement.OpenParenToken;
            var startDiagnosticSpan = ifKeyword.SpanStart;
            var endDiagnosticSpan = openParen.SpanStart;
            var diagnosticSpan = TextSpan.FromBounds(startDiagnosticSpan, endDiagnosticSpan);
            var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, diagnosticSpan);
            var diagnostic = Diagnostic.Create(Rule, diagnosticLocation, Rule.MessageFormat);
            context.ReportDiagnostic(diagnostic);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.GoToCodeFix,
                Message = MessagePrefix + "Congratulations! You have written an analyzer! If you would like to explore a code fix for your diagnostic, open up CodeFixProvider.cs and take a look!",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 18) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
        #endregion

        #region MissingId

        public const string MissingIdMessage = MessagePrefix + "'SyntaxNodeAnalyzer' should have a diagnostic id (a public, constant string uniquely identifying each diagnostic)";

        // no id, nothing else after
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingId,
                Message = MissingIdMessage,
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
        // Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs
        public const string spacingRuleId = ""IfSpacing001"";

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

        //  no id, rules exists
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
                id: SpacingRuleId, // make the id specific
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingId,
                Message = MissingIdMessage,
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
        // Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs
        public const string spacingRuleId = ""IfSpacing001"";
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
                id: SpacingRuleId, // make the id specific
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingId,
                Message = MissingIdMessage,
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
        // Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs
        public const string spacingRuleId = ""IfSpacing001"";
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingId,
                Message = MissingIdMessage,
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
        // Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs
        public const string spacingRuleId = ""IfSpacing001"";
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingId,
                Message = MissingIdMessage,
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
        // Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs
        public const string spacingRuleId = ""IfSpacing001"";
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingId,
                Message = MissingIdMessage,
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
        // Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs
        public const string spacingRuleId = ""IfSpacing001"";
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingInit

        public const string MissingInitMessage = MessagePrefix + "'SyntaxNodeAnalyzer' is missing the required inherited Initialize method, needed to register analysis actions";

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
        public const string spacingRuleId = ""IfSpacing"";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
            isEnabledByDefault: true);
            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    return ImmutableArray.Create(Rule);
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingInit,
                Message = MissingInitMessage,
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
        public const string spacingRuleId = ""IfSpacing"";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
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
            throw new NotImplementedException();
        }
    }
    }";



            VerifyCSharpFix(test, fixtest);
        }

        // slight misspelling
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
        public const string spacingRuleId = ""IfSpacing"";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
            isEnabledByDefault: true);
            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    return ImmutableArray.Create(Rule);
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
                Message = MissingInitMessage,
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
        public const string spacingRuleId = ""IfSpacing"";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
            isEnabledByDefault: true);
            public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            {
                get
                {
                    return ImmutableArray.Create(Rule);
                }
            }
            public override void initialize(AnalysisContext context)
            {
                throw new NotImplementedException();
            }

        public override void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        //  everything except the initialize method
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            var startDiagnosticSpan = ifKeyword.SpanStart;
            var endDiagnosticSpan = openParen.SpanStart;
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
                Message = MissingInitMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            var startDiagnosticSpan = ifKeyword.SpanStart;
            var endDiagnosticSpan = openParen.SpanStart;
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

        public const string MissingRegisterStatementMessage = MessagePrefix + "A syntax node action should be registered within the 'Initialize' method";

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
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingRegisterStatement,
                Message = MissingRegisterStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 30) }
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
            // Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        // This method, which is the method that is registered within Initialize, performs the analysis of the Syntax Tree when an IfStatementSyntax Node is found. If the analysis finds an error, a diagnostic is reported
        // In this tutorial, this method will walk through the Syntax Tree seen in IfSyntaxTree.jpg and determine if the if-statement being analyzed has the correct spacing
        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixTest, allowNewCompilerDiagnostics: true);
        }

        //  register statement in comments
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
            //  context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingRegisterStatement,
                Message = MissingRegisterStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 30) }
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
            // Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        // This method, which is the method that is registered within Initialize, performs the analysis of the Syntax Tree when an IfStatementSyntax Node is found. If the analysis finds an error, a diagnostic is reported
        // In this tutorial, this method will walk through the Syntax Tree seen in IfSyntaxTree.jpg and determine if the if-statement being analyzed has the correct spacing
        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void MissingRegister3()
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
                return ImmutableArray.Create(Rule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
        }
        private void Method2(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingRegisterStatement,
                Message = MissingRegisterStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 30) }
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
            // Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement
            context.RegisterSyntaxNodeAction(Method2, SyntaxKind.IfStatement);
        }

        private void Method2(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixTest, allowNewCompilerDiagnostics: true);
        }
        #endregion

        #region TooManyInitStatements

        public const string TooManyInitStatementsMessage = MessagePrefix + "For this tutorial, the 'Initialize' method should only register one action";

        //  statement below, incorrect method name
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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = TooManyInitStatementsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 30) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  statement below, incorrect syntax kind
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
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = TooManyInitStatementsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 30) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  incorrect statement above
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
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = TooManyInitStatementsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 30) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        // multiple incorrect statements below
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
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = TooManyInitStatementsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 30) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  multiple incorrect statements above
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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = TooManyInitStatementsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 30) }
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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  no correct statements, multiple incorrect
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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyInitStatements,
                Message = TooManyInitStatementsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 31, 30) }
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
            context.RegisterSyntaxNodeAction(Practice, SyntaxKind.IfStatement);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region InvalidStatement

        public const string InvalidStatementMessage = MessagePrefix + "The Initialize method only registers actions, therefore any other statement placed in Initialize is incorrect";

        //  invalid throw statement
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
           throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 33, 12) }
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
        }
    }
}";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        //  invalid break statement
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
           break;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 12) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        // invalid check statement
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
           checked { var num = num + 1; }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 12) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid continue statement
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
           continue;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 12) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  do while statement
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
           do { var i = 1; } while (i > 3);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 12) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid random expression statement
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
           context.GetHashCode();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 12) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid foreach statement
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
           foreach() { break; }
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 12) }
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
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid for statement
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
                for(int i = 1; i < 3; i++) { i++; }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 17) }
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
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid if statement
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
                if (i < 3) { i++; }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 17) }
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
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid labeled statement
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
                context: return context;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 17) }
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
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid local declaration statement
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
                int i;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 17) }
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
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid lock statement
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
                lock () {}
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 17) }
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
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  invalid return statement
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
                return;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 17) }
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
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  multiple invalid statements
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
                int one = 1;
                int two = 2;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 17) }
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
                int two = 2;
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  one invalid statement, no valid statements
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
                int one = 1;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 33, 17) }
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
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  multiple invalid statements, no valid statements
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
                int one = 1;
                int two = 2;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 33, 17) }
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
                int two = 2;
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  multiple valid statements, one invalid statement
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
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
                int one = 1;
                int two = 2;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.InvalidStatement,
                Message = InvalidStatementMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 35, 17) }
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
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
                int two = 2;
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IncorrectKind

        public const string IncorrectKindMessage = MessagePrefix + "This tutorial only allows registering for SyntaxKind.IfStatement";

        [Fact]
        public void IncorrectKind()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfKeyword);
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectKind,
                Message = IncorrectKindMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 70) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            // Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }
        }
    }";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }
        #endregion

        #region IncorrectArguments

        public const string IncorrectArgumentsMessage = MessagePrefix + "The method RegisterSyntaxNodeAction requires 2 arguments: a method and a SyntaxKind";

        [Fact]
        public void IncorrectArguments1()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.Diagnostics;

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
                context.RegisterSyntaxNodeAction();
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectArguments,
                Message = IncorrectArgumentsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            // Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }
        }
    }";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void IncorrectArguments2()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            context.RegisterSyntaxNodeAction();
        }

        private void Method2(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectArguments,
                Message = IncorrectArgumentsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 13) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            // Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement
            context.RegisterSyntaxNodeAction(Method2, SyntaxKind.IfStatement);
        }

        private void Method2(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void IncorrectArguments3()
        {
            var test = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            context.RegisterSyntaxNodeAction(SyntaxKind.IfStatement);
        }

        private void Method2(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectArguments,
                Message = IncorrectArgumentsMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 34, 13) }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"using System;
            using System.Collections.Generic;
            using System.Collections.Immutable;
            using System.Linq;
            using System.Threading;
            using Microsoft.CodeAnalysis;
            using Microsoft.CodeAnalysis.CSharp;
            using Microsoft.CodeAnalysis.Diagnostics;

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
            // Calls the method (first argument) to perform analysis whenever this is a change to a SyntaxNode of kind IfStatement
            context.RegisterSyntaxNodeAction(Method2, SyntaxKind.IfStatement);
        }

        private void Method2(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
        }
    }";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }
        #endregion

        #region IncorrectInitSig

        public const string IncorrectInitSigMessage = MessagePrefix + "The 'Initialize' method should return void, have the 'override' modifier, and have a single parameter of type 'AnalysisContext'";

        //  more than one parameter
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

        public override void Initialize(AnalysisContext context, int i)
        {
            throw new NotImplementedException();
        }
    }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = IncorrectInitSigMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 35, 30) }
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
            throw new NotImplementedException();
        }
    }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  Wrong type for first parameter
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

        public override void Initialize(int context)
        {
            throw new NotImplementedException();
        }
    }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = IncorrectInitSigMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 35, 30) }
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
            throw new NotImplementedException();
        }
    }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  accessibility is not public
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

        private override void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = IncorrectInitSigMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 35, 31) }
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
            throw new NotImplementedException();
        }
    }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  initialize method is not overriden
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

        public void Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = IncorrectInitSigMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 35, 21) }
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
            throw new NotImplementedException();
        }
    }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  initialize method does not return void
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

        public override int Initialize(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectInitSig,
                Message = IncorrectInitSigMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 35, 29) }
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
            throw new NotImplementedException();
        }
    }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IfStatementIncorrect

        public const string IfStatementIncorrectMessage = MessagePrefix + "This statement should extract the if-statement being analyzed by casting context.Node to IfStatementSyntax";

        //  No identifier for statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  ifStatement not initialized
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        //  no cast
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  Wrong cast type
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  not a member access expression
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  wrong object
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  doesn't access node
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  check that statements below are retained
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
            var ifKeyword = ifStatement.IfKeyword;
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IfKeywordIncorrect

        public const string IfKeywordIncorrectMessage = MessagePrefix + "This statement should extract the if-keyword SyntaxToken from 'ifStatement'";

        //  not initialized
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfKeywordIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // This statement navigates down the syntax tree one level to extract the 'if' keyword
            var ifKeyword = ifStatement.IfKeyword;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  no member access expression
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfKeywordIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // This statement navigates down the syntax tree one level to extract the 'if' keyword
            var ifKeyword = ifStatement.IfKeyword;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  wrong identifier name
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfKeywordIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // This statement navigates down the syntax tree one level to extract the 'if' keyword
            var ifKeyword = ifStatement.IfKeyword;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  doesn't access IfKeyword
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfKeywordIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // This statement navigates down the syntax tree one level to extract the 'if' keyword
            var ifKeyword = ifStatement.IfKeyword;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  no variable declaration 
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfKeywordIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // This statement navigates down the syntax tree one level to extract the 'if' keyword
            var ifKeyword = ifStatement.IfKeyword;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  statements below are retained
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfKeywordIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // This statement navigates down the syntax tree one level to extract the 'if' keyword
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

        public const string IfStatementMissingMessage = MessagePrefix + "The first step of the SyntaxNode analysis is to extract the if-statement from 'context' by casting context.Node to IfStatementSyntax";

        //  no statements in analyze method
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfStatementMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  check comments aren't counted as statements
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                // var ifStatement = (IfStatementSyntax)context.Node;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfStatementMissing,
                Message = IfStatementMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // The SyntaxNode found by the Initialize method should be cast to the expected type. Here, this type is IfStatementSyntax
            var ifStatement = (IfStatementSyntax)context.Node;
            // var ifStatement = (IfStatementSyntax)context.Node;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IfKeywordMissing

        public const string IfKeywordMissingMessage = MessagePrefix + "Next, extract the if-keyword SyntaxToken from 'ifStatement'";

        //  no 2nd statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IfKeywordMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // This statement navigates down the syntax tree one level to extract the 'if' keyword
            var ifKeyword = ifStatement.IfKeyword;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  second statement is in the comments
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                // var ifKeyword = ifStatement.IfKeyword;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IfKeywordMissing,
                Message = IfKeywordMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
            // This statement navigates down the syntax tree one level to extract the 'if' keyword
            var ifKeyword = ifStatement.IfKeyword;
            // var ifKeyword = ifStatement.IfKeyword;
        }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TrailingTriviaCheckMissing

        public const string TrailingTriviaCheckMissingMessage = MessagePrefix + "Next, begin looking for the space between 'if' and '(' by checking if 'ifKeyword' has trailing trivia";

        //  no 3rd statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        //  3rd statement commented
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        public const string TrailingTriviaCheckIncorrectMessage = MessagePrefix + "This statement should be an if-statement that checks to see if 'ifKeyword' has trailing trivia";

        //  no if statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        //  misslabeled accessor
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        //  Doesnt access HasTrailingTrivia
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        //  throw statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        //  statements below incorrect statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        //  statements within if block
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        //  incorrect statement is next if statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = TrailingTriviaCheckIncorrectMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        public const string TrailingTriviaVarMissingMessage = MessagePrefix + "Next, extract the first trailing trivia of 'ifKeyword' into a variable";

        //  no variable declaration
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                Message = TrailingTriviaVarMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  statement below if block
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    }
                }
                var trailing = ifKeyword.TrailingTrivia.First();
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                Message = TrailingTriviaVarMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
                var trailing = ifKeyword.TrailingTrivia.First();
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  declaration in comments
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        // var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarMissing,
                Message = TrailingTriviaVarMissingMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    // var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TrailingTriviaVarIncorrect

        public const string TrailingTriviaVarIncorrectMessage = MessagePrefix + "This statement should extract the first trailing trivia of 'ifKeyword' into a variable";

        //  not initialized
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia;
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  no member access expressions
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword;
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // only one member access expression
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia;
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        //  member access expression order switched
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.First().TrailingTrivia;
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  throw statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        throw new NotImplementedException();
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  wrong accessor
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifStatement.TrailingTrivia.First();
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  statements below if block
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifStatement.TrailingTrivia.First();
                    }
                }
                var openParen = ifStatement.OpenParenToken;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
                var openParen = ifStatement.OpenParenToken;
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  statements within if block
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifStatement.TrailingTrivia.First();
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
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // incorrect statement is the next statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
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
                Id = MetaCompilationAnalyzer.TrailingTriviaVarIncorrect,
                Message = TrailingTriviaVarIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region InternalAndStaticError

        public const string InternalAndStaticErrorMessage = MessagePrefix + "The 'Rule' field should be internal and static";

        [Fact]
        public void InternalAndStatic1() // missing internal modifier
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
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
                Id = MetaCompilationAnalyzer.InternalAndStaticError,
                Message = InternalAndStaticErrorMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
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
        public void InternalAndStatic2() // missing static modifier
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
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
                Id = MetaCompilationAnalyzer.InternalAndStaticError,
                Message = InternalAndStaticErrorMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
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
        public void InternalAndStatic3() // missing both modifiers
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
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
                Id = MetaCompilationAnalyzer.InternalAndStaticError,
                Message = InternalAndStaticErrorMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
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

        #endregion

        #region EnabledByDefault

        public const string EnabledByDefaultMessage = MessagePrefix + "The 'isEnabledByDefault' field should be set to true";

        [Fact]
        public void EnabledByDefault1() // isEnabledByDefault set to false
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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: false);
        // id: Identifies each rule. Same as the public constant declared above
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial

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
                Id = MetaCompilationAnalyzer.EnabledByDefaultError,
                Message = EnabledByDefaultMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 33) }
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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true
        // id: Identifies each rule. Same as the public constant declared above
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial

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
        public void EnabledByDefault2() // isEnabledByDefault set to undeclared variable
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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EnabledByDefaultError,
                Message = EnabledByDefaultMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 26, 33) }
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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true

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
        public void EnabledByDefault3() // isEnabledByDefault set to member access expression
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EnabledByDefaultError,
                Message = EnabledByDefaultMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true

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
        public void EnabledByDefault4() // isEnabledByDefault with argument missing
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EnabledByDefaultError,
                Message = EnabledByDefaultMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // isEnabledByDefault: Determines whether the analyzer is enabled by default or if the user must manually enable it. Generally set to true
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
        #endregion

        #region DefaultSeverityError

        public const string DefaultSeverityErrorMessage = MessagePrefix + "The 'defaultSeverity' should be either DiagnosticSeverity.Error or DiagnosticSeverity.Warning";

        [Fact]
        public void DefaultSeverity1() // defaultSeverity set to undeclared variable.
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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: test, // possible options
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
                Id = MetaCompilationAnalyzer.DefaultSeverityError,
                Message = DefaultSeverityErrorMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 25, 30) }
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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
            isEnabledByDefault: true);
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial

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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial

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

            VerifyCSharpFix(test, fixtestError, 0);
            VerifyCSharpFix(test, fixtestWarning, 1);
        }

        [Fact]
        public void DefaultSeverity2() // defaultSeverity.Name set to arbitrary string
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.test, // possible options
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
                Id = MetaCompilationAnalyzer.DefaultSeverityError,
                Message = DefaultSeverityErrorMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
            isEnabledByDefault: true);
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial

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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial

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

            VerifyCSharpFix(test, fixtestError, 0);
            VerifyCSharpFix(test, fixtestWarning, 1);
        }

        [Fact]
        public void DefaultSeverity3() // defaultSeverity with argument missing
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: , // possible options
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
                Id = MetaCompilationAnalyzer.DefaultSeverityError,
                Message = DefaultSeverityErrorMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Error, // possible options
            isEnabledByDefault: true);
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // defaultSeverity: Is set to DiagnosticSeverity.[severity] where severity can be Error, Warning, Hidden or Info, but can only be Error or Warning for the purposes of this tutorial
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

            VerifyCSharpFix(test, fixtestError, 0);
            VerifyCSharpFix(test, fixtestWarning, 1);
        }
        #endregion

        #region IdDeclTypeError

        public const string IdDeclTypeErrorMessage = MessagePrefix + "The diagnostic id should be the constant string declared above";
        public const string IdStringLiteralMessage = MessagePrefix + "The ID should not be a string literal, because the ID must be accessible from the code fix provider";

        [Fact]
        public void IdDeclType1() // id set to a literal string
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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: ""test"", // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Id = MetaCompilationAnalyzer.IdStringLiteral,
                Message = IdStringLiteralMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 21, 17) }
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

        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // id: Identifies each rule. Same as the public constant declared above

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
        public void IdDeclType2() // id set to a member access expression
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
            id: DiagnosticSeverity.Warning, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IdDeclTypeErrorMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // id: Identifies each rule. Same as the public constant declared above
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
        public void IdDeclType3() // id set to true
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
            id: true, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IdDeclTypeErrorMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // id: Identifies each rule. Same as the public constant declared above
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
        public void IdDeclType4() // id with argument missing
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
            id: , // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Message = IdDeclTypeErrorMessage,
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
            isEnabledByDefault: true);
        // id: Identifies each rule. Same as the public constant declared above
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

        public const string MissingIdDeclarationMessage = MessagePrefix + "This diagnostic id should be the constant string declared above";

        [Fact]
        public void MissingIdDeclaration1() // id set to undeclared variable
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
            id: test, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                Id = MetaCompilationAnalyzer.MissingIdDeclaration,
                Message = MissingIdDeclarationMessage,
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
        // Each analyzer needs a public id to identify each DiagnosticDescriptor and subsequently fix diagnostics in CodeFixProvider.cs
        public const string test = ""DescriptiveId"";
        public const string spacingRuleId = ""IfSpacing"";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: test, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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

        #region OpenParenTests

        public const string MissingOpenParenMessage = MessagePrefix + "Moving on to the creation and reporting of the diagnostic, extract the open parenthesis of 'ifState' into a variable to use as the end of the diagnostic span";
        public const string IncorrectOpenParenMessage = MessagePrefix + "This statement should extract the open parenthesis of 'ifState' to use as the end of the diagnostic span";

        [Fact]
        public void MissingOpenParen() // no DiagnosticDescriptor field
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.OpenParenMissing,
                Message = MissingOpenParenMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 45, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }
            // Extracts the opening parenthesis of the if-statement condition
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                return;
                            }
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
                Message = IncorrectOpenParenMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 60, 17) }
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                return;
                            }
                        }
                    }
                }
            // Extracts the opening parenthesis of the if-statement condition
            var openParen = ifState.OpenParenToken;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingSuppDiag

        public const string MissingSuppDiagMessage = MessagePrefix + "You are missing the required inherited SupportedDiagnostics property";

        [Fact]
        public void MissingSuppDiag1() // no SupportedDiagnostics property
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
                Message = MissingSuppDiagMessage,
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

        public const string IncorrectSigSuppDiagMessage = MessagePrefix + "The overriden SupportedDiagnostics property should return an Immutable Array of Diagnostic Descriptors";

        [Fact]
        public void IncorrectSigSuppDiag1() // no public modifier
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
                Message = IncorrectSigSuppDiagMessage,
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void IncorrectSigSuppDiag2() // no override modifier
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
                Message = IncorrectSigSuppDiagMessage,
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void IncorrectSigSuppDiag3() // no modifiers
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
                Message = IncorrectSigSuppDiagMessage,
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }
        #endregion

        #region MissingAccessor

        public const string MissingAccessorMessage = MessagePrefix + "The 'SupportedDiagnostics' property is missing a get-accessor to return a list of supported diagnostics";

        [Fact]
        public void MissingAccessor1() // empty SupportedDiagnostics property
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
                Message = MissingAccessorMessage,
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
        public void MissingAccessor2() // SupportedDiagnostics property contains only set accessor
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
                Message = MissingAccessorMessage,
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

        public const string TooManyAccessorsMessage = MessagePrefix + "The 'SupportedDiagnostics' property needs only a single get-accessor";

        [Fact]
        public void TooManyAccessors1() // SupportedDiagnostics property with get and then set accessors
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
                Message = TooManyAccessorsMessage,
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
        public void TooManyAccessors2() // SupportedDiagnostics property with set and then get accessors
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
                Message = TooManyAccessorsMessage,
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
        public void TooManyAccessors3() // SupportedDiagnostics property with two get accessors
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
                Message = TooManyAccessorsMessage,
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
        
        public const string IncorrectAccessorReturnMessage = MessagePrefix + "The get-accessor should return an ImmutableArray containing all of the DiagnosticDescriptor rules";
        public const string SuppDiagReturnValueMessage = MessagePrefix + "The 'SupportedDiagnostics' property's get-accessor should return an ImmutableArray containing all DiagnosticDescriptor rules";

        [Fact]
        public void IncorrectAccessorReturn1() // empty get accessor
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
                Message = IncorrectAccessorReturnMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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
        public void IncorrectAccessorReturn2() // get accessor throwing NotImplementedException
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
                Message = IncorrectAccessorReturnMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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
        public void IncorrectAccessorReturn3() // get accessor returning nothing
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
                Message = IncorrectAccessorReturnMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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
        public void IncorrectAccessorReturn4() // get accessor returning true
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
                Message = IncorrectAccessorReturnMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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
        public void SuppDiagReturn1() // get accessor returning incorrect invocation expression
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
                Message = SuppDiagReturnValueMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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

        public const string SupportedRulesMessage = MessagePrefix + "The ImmutableArray should contain every DiagnosticDescriptor rule that was created";

        [Fact]
        public void SupportedRules1() // invocation expression form with no arguments
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
                Message = SupportedRulesMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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
        public void SupportedRules2() // variable declaration form with no arguments
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
                Message = SupportedRulesMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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
        public void SupportedRules3() // invocation expression form with missing rules
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
                Message = SupportedRulesMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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
        public void SupportedRules4() // variable declaration form with missing rules
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
                Message = SupportedRulesMessage,
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
                // This array contains all the diagnostics that can be shown to the user
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

        [Fact]
        public void SupportedRules5() // check that "return array;" is supported. ie SupportedRules diagnostic should surface
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
                Message = SupportedRulesMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 30, 29) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
        #endregion

        #region StartSpanTests

        public const string StartSpanMissingMessage = MessagePrefix + "Next, extract the start of the span of 'ifKeyword' into a variable, to be used as the start of the diagnostic span";
        public const string StartSpanIncorrectMessage = MessagePrefix + "This statement should extract the start of the span of 'ifKeyword' into a variable, to be used as the start of the diagnostic span";

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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
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
                Message = StartSpanMissingMessage,
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            // Determines the start of the span of the diagnostic that will be reported, ie the start of the squiggle
            var startDiagnosticSpan = ifKeyword.SpanStart;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectStartSpan1()
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifState.SpanStart;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.StartSpanIncorrect,
                Message = StartSpanIncorrectMessage,
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            // Determines the start of the span of the diagnostic that will be reported, ie the start of the squiggle
            var startDiagnosticSpan = ifKeyword.SpanStart;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        // Test that Span.Start is also accepted
        [Fact]
        public void IncorrectStartSpan2()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();

                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var openParen = ifStatement.OpenParenToken;
            var startDiagnosticSpan = ifKeyword.Span.Start;
            var endDiagnosticSpan = openParen.SpanStart;
            var diagnosticSpan = TextSpan.FromBounds(startDiagnosticSpan, endDiagnosticSpan);
            var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, diagnosticSpan);
            var diagnostic = Diagnostic.Create(Rule, diagnosticLocation, Rule.MessageFormat);
            context.ReportDiagnostic(diagnostic);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.GoToCodeFix,
                Message = MessagePrefix + "Congratulations! You have written an analyzer! If you would like to explore a code fix for your diagnostic, open up CodeFixProvider.cs and take a look!",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 18) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
        #endregion

        #region EndSpanTests

        public const string EndSpanMissingMessage = MessagePrefix + "Next, determine the end of the span of the diagnostic that is going to be reported";
        public const string EndSpanIncorrectMessage = MessagePrefix + "This statement should extract the start of the span of 'open' into a variable, to be used as the end of the diagnostic span";

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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EndSpanMissing,
                Message = EndSpanMissingMessage,
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            // Determines the end of the span of the diagnostic that will be reported
            var endDiagnosticSpan = open.SpanStart;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void IncorrectEndSpan1()
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            return 1;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.EndSpanIncorrect,
                Message = EndSpanIncorrectMessage,
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            // Determines the end of the span of the diagnostic that will be reported
            var endDiagnosticSpan = open.SpanStart;
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        // Check that functionality start.span is supported
        [Fact]
        public void IncorrectEndSpan2()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();

                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var openParen = ifStatement.OpenParenToken;
            var startDiagnosticSpan = ifKeyword.SpanStart;
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
                Id = MetaCompilationAnalyzer.GoToCodeFix,
                Message = MessagePrefix + "Congratulations! You have written an analyzer! If you would like to explore a code fix for your diagnostic, open up CodeFixProvider.cs and take a look!",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 18) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
        #endregion

        #region SpanTests

        public const string SpanMissingMessage = MessagePrefix + "Next, using TextSpan.FromBounds, create a variable that is the span of the diagnostic that will be reported";
        public const string SpanIncorrectMessage = MessagePrefix + "This statement should use TextSpan.FromBounds, 'start', and 'end' to create the span of the diagnostic that will be reported";

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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SpanMissing,
                Message = SpanMissingMessage,
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            // The span is the range of integers that define the position of the characters the red squiggle will underline
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            if (true) {}
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.SpanIncorrect,
                Message = SpanIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 63, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            // The span is the range of integers that define the position of the characters the red squiggle will underline
            var diagnosticSpan = TextSpan.FromBounds(start, end);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region LocationTests

        public const string LocationMissingMessage = MessagePrefix + "Next, using Location.Create, create a location for the diagnostic";
        public const string LocationIncorrectMessage = MessagePrefix + "This statement should use Location.Create, 'ifState', and 'span' to create the location of the diagnostic";

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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.LocationMissing,
                Message = LocationMissingMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 63, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            // Uses the span created above to create a location for the diagnostic squiggle to appear within the syntax tree passed in as an argument
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var diagnosticLocation = ""Hello World"";
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.LocationIncorrect,
                Message = LocationIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 64, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            // Uses the span created above to create a location for the diagnostic squiggle to appear within the syntax tree passed in as an argument
            var diagnosticLocation = Location.Create(ifState.SyntaxTree, span);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region DiagnosticTests

        public const string DiagnosticMissingMessage = MessagePrefix + "Next, use Diagnostic.Create to create the diagnostic";
        public const string DiagnosticIncorrectMessage = MessagePrefix + "This statement should use Diagnostic.Create, 'spacingRule', and 'location' to create the diagnostic that will be reported";

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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticMissing,
                Message = DiagnosticMissingMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 64, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            // Holds the diagnostic and all necessary information to be reported
            var diagnostic = Diagnostic.Create(spacingRule, location);
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            diagnostic = Diagnostic.Create(spacingRule, location, spacingRule.MessageFormat);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticIncorrect,
                Message = DiagnosticIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 65, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            // Holds the diagnostic and all necessary information to be reported
            var diagnostic = Diagnostic.Create(spacingRule, location);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region TrailingTriviaKindCheckMissing

        public const string TrailingTriviaKindCheckMissingMessage = MessagePrefix + "Next, check if the kind of 'trailingTrivia' is whitespace trivia";

        [Fact]
        public void TrailingKindMissing1() //  no whitespace check
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckMissing,
                Message = TrailingTriviaKindCheckMissingMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 43, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
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

        #region TrailingTriviaKindCheckIncorrect

        public const string TrailingTriviaKindCheckIncorrectMessage = MessagePrefix + "This statement should check to see if the kind of 'trailingTrivia' is whitespace trivia";

        // random variable declaration
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        var ifCheck = true;
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            { 
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  wrong variable name
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (ifKeyword.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            { 
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //  Doesn't access kind method
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind == SyntaxKind.WhitespaceTrivia)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //  Accesses different method (not kind)
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.IsKind() == SyntaxKind.WhitespaceTrivia)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // one equals sign
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() = SyntaxKind.WhitespaceTrivia)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            { 
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        // wrong member accessor
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SymbolKind.WhitespaceTrivia)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //  wrong accessed
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.IfStatement)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //  first statement not member access
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia == SyntaxKind.WhitespaceTrivia)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //  second statement not member access
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind)
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //  no condition
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
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
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  incorrect statement is next statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                    }
                }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        
        //  statements within if statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia == SyntaxKind.WhitespaceTrivia)
                        {
                            var one = 1;
                            one++;
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TrailingTriviaKindCheckIncorrect,
                Message = TrailingTriviaKindCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        var one = 1;
                            one++;
                    }
                }
                }
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region WhitespaceCheckMissing

        public const string WhitespaceCheckMissingMessage = MessagePrefix + "Next, check if 'trailingTrivia' is a single whitespace, which is the desired formatting";

        //  no whitespace check
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
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
                Id = MetaCompilationAnalyzer.WhitespaceCheckMissing,
                Message = WhitespaceCheckMissingMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
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

        public const string WhitespaceCheckIncorrectMessage = MessagePrefix + "This statement should check to see if 'trailingTrivia' is a single whitespace, which is the desired formatting";

        //  random variable declaration
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            int one = 1;
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  wrong variable name
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (ifKeyword.ToString() == "" "")
                            {
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        //  wrong method accessed
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.FullSpan == "" "")
                            {
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  no member access expression
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia == "" "")
                            {
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  wrong equals sign
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() = "" "")
                            {
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  wrong condition
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == ""trailingTrivia.ToString()"")
                            {
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  empty condition
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if ()
                            {
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  previous if statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                            {
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  next statement
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
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
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                        if (trailingTrivia.ToString() == "" "")
                        {
                        }
                    }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        // statements within incorrect if
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
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
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.WhitespaceCheckIncorrect,
                Message = WhitespaceCheckIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
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
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region MissingRule

        public const string MissingRuleMessage = MessagePrefix + "The analyzer should have at least one DiagnosticDescriptor rule";

        [Fact]
        public void MissingRule1() // Rule id but no rule
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
                Message = MissingRuleMessage,
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
        // If the analyzer finds an issue, it will report the DiagnosticDescriptor rule
        internal static DiagnosticDescriptor spacingRule = new DiagnosticDescriptor(
            id: ""Change me to the name of the above constant"",
            title: ""Enter a title for this diagnostic"",
            messageFormat: ""Enter a message to be displayed with this diagnostic"",
            category: ""Enter a category for this diagnostic (e.g. Formatting)"",
            defaultSeverity: default(DiagnosticSeverity),
            isEnabledByDefault: default(bool));

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

        #region ReturnStatementMissing

        public const string ReturnStatementMissingMessage = MessagePrefix + "Next, since if the code reaches this point the formatting must be correct, return from 'AnalyzeIfStatement'";

        // no return statement
        [Fact]
        public void ReturnMissing1()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.ReturnStatementMissing,
                Message = ReturnStatementMissingMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                            // If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic
                            return;
                        }
                        }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region ReturnStatementIncorrect

        public const string ReturnStatementIncorrectMessage = MessagePrefix + "This statement should return from 'AnalyzeIfStatement', because reaching this point in the code means that the if-statement being analyzed has the correct spacing";

        //  throw new NotImplementedException statement
        [Fact]
        public void ReturnIncorrect1()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                throw new NotImplementedException();
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.ReturnStatementIncorrect,
                Message = ReturnStatementIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 48, 33) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                            // If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic
                            return;
                        }
                        }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  return statement not first statement
        [Fact]
        public void ReturnIncorrect2()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                var one = 1;
                                return;
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.ReturnStatementIncorrect,
                Message = ReturnStatementIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 48, 33) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                            // If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic
                            return;
                            return;
                            }
                        }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        //  next statement
        [Fact]
        public void ReturnIncorrect3()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                var openParen = ifStatement.OpenParenToken;
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.ReturnStatementIncorrect,
                Message = ReturnStatementIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 48, 33) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                            // If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic
                            return;
                        }
                        }
                    }
                }
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }

        //  statements below
        [Fact]
        public void ReturnIncorrect4()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                var one = 1;
                            }
                        }
                    }
                }
                var openParen = ifStatement.OpenParenToken;
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.ReturnStatementIncorrect,
                Message = ReturnStatementIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 48, 33) }
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                            // If the analyzer is satisfied that there is only a single whitespace between 'if' and '(', it will return from this method without reporting a diagnostic
                            return;
                        }
                        }
                    }
                }
                var openParen = ifStatement.OpenParenToken;
            }
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region AnalysisMethod

        #region MissingAnalysisMethod

        public const string MissingAnalysisMethodMessage = MessagePrefix + "The method 'AnalyzeIfStatement' that was registered to perform the analysis is missing";

        [Fact]
        public void MissingAnalysisMethod1() // missing the analysis method called in Initialize
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
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.MissingAnalysisMethod,
                Message = MissingAnalysisMethodMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 36, 46) }
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
        // This method, which is the method that is registered within Initialize, performs the analysis of the Syntax Tree when an IfStatementSyntax Node is found. If the analysis finds an error, a diagnostic is reported
        // In this tutorial, this method will walk through the Syntax Tree seen in IfSyntaxTree.jpg and determine if the if-statement being analyzed has the correct spacing
        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }
        #endregion

        #region IncorrectAnalysisAccessibility
        public const string IncorrectAnalysisAccessibilityMessage = MessagePrefix + "The 'AnalyzeIfStatement' method should be private";

        [Fact]
        public void IncorrectAnalysisAccessibility1() // analysis method public
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

        public void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAnalysisAccessibility,
                Message = IncorrectAnalysisAccessibilityMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 21) }
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
        public void IncorrectAnalysisAccessibility2() // analysis method w/o declared accessibility
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

         void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAnalysisAccessibility,
                Message = IncorrectAnalysisAccessibilityMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 15) }
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

        #endregion

        #region IncorrectAnalysisReturnType
        public const string IncorrectAnalysisReturnTypeMessage = MessagePrefix + "The 'AnalyzeIfStatement' method should have a void return type";

        [Fact]
        public void IncorrectAnalysisReturnType1() // analysis method returning incorrect type
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

        private bool AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAnalysisReturnType,
                Message = IncorrectAnalysisReturnTypeMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 22) }
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        [Fact]
        public void IncorrectAnalysisReturnType2() // analysis method without return type explicitly declared
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

        private AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAnalysisReturnType,
                Message = IncorrectAnalysisReturnTypeMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 17) }
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

            VerifyCSharpFix(test, fixtest, allowNewCompilerDiagnostics: true);
        }

        #endregion

        #region IncorrectAnalysisParameter
        public const string IncorrectAnalysisParameterMessage = MessagePrefix + "The 'AnalyzeIfStatement' method should take one parameter of type SyntaxNodeAnalysisContext";

        [Fact]
        public void IncorrectAnalysisParameter1() // analysis method taking incorrect parameter type
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

        private void AnalyzeIfStatement(bool context)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAnalysisParameter,
                Message = IncorrectAnalysisParameterMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 40) }
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
        public void IncorrectAnalysisParameter2() // analysis method taking too many parameters
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

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context, bool boolean)
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAnalysisParameter,
                Message = IncorrectAnalysisParameterMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 40) }
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
        public void IncorrectAnalysisParameter3() // analysis method taking no parameters
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

        private void AnalyzeIfStatement()
        {
            throw new NotImplementedException();
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.IncorrectAnalysisParameter,
                Message = IncorrectAnalysisParameterMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 40) }
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
        #endregion

        #endregion

        #region TooManyStatements
        
        // Trivia check block
        [Fact]
        public void TooManyStatements1()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                 return;
                            }
                        }
                    }
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyStatements,
                Message = MessagePrefix + "This if-block should only have 1 statement(s), which should check the number of trailing trivia on the if-keyword",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 39, 17) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        // Trivia count block
        [Fact]
        public void TooManyStatements2()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                 return;
                            }
                        }
                        var openParen = ifStatement.OpenParenToken;
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyStatements,
                Message = MessagePrefix + "This if-block should only have 2 statement(s), which should extract the first trivia of the if-keyword and check its kind",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 41, 21) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        // Trivia kind check block
        [Fact]
        public void TooManyStatements3()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                 return;
                            }
                            var openParen = ifStatement.OpenParenToken;
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyStatements,
                Message = MessagePrefix + "This if-block should only have 1 statement(s), which should check if the trivia is a single space",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 44, 25) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        // Return block
        [Fact]
        public void TooManyStatements4()
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
            id: spacingRuleId, // make the id specific
            title: ""If statement must have a space between 'if' and the boolean expression"", // allow any title
            messageFormat: ""If statements must contain a space between the 'if' keyword and the boolean expression"", // allow any message
            category: ""Syntax"", // make the category specific
            defaultSeverity: DiagnosticSeverity.Warning, // possible options
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
                    if (ifKeyword.TrailingTrivia.Count == 1)
                    {
                        var trailingTrivia = ifKeyword.TrailingTrivia.First();
                        if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                        {
                            if (trailingTrivia.ToString() == "" "")
                            {
                                return;
                                var openParen = ifStatement.OpenParenToken;
                            }
                        }
                    }
                }
            }
        }
    }";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyStatements,
                Message = MessagePrefix + "This if-block should only have 1 statement(s), which should return from the method",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 46, 29) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        // Method declaration (all statements)
        [Fact]
        public void TooManyStatements5()
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            context.ReportDiagnostic(diagnostic);
            return;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.TooManyStatements,
                Message = MessagePrefix + "This method should only have 10 statement(s), which should walk through the Syntax Tree and check the spacing of the if-statement",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 40, 22) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        // Too many get-accessor statements, using one single return line
        [Fact]
        public void TooManyStatements6()
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
                var one = 1;
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
                Id = MetaCompilationAnalyzer.TooManyStatements,
                Message = MessagePrefix + "This get accessor should only have 1 or 2 statement(s), which should create and return an ImmutableArray containing all DiagnosticDescriptors",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 28, 13) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        // Too many get-accessor statements, create array then return
        [Fact]
        public void TooManyStatements7()
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
                var array = ImmutableArray.Create(Rule);
                var one = 1;
                return array;
            }
        }

        public override void Initialize(AnalysisContext context)
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
                Id = MetaCompilationAnalyzer.TooManyStatements,
                Message = MessagePrefix + "This get accessor should only have 1 or 2 statement(s), which should create and return an ImmutableArray containing all DiagnosticDescriptors",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 28, 13) }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        #endregion

        #region DiagnosticReportMissing

        private const string s_diagnosticReportMissingMessage = MessagePrefix + "Next, use 'context'.ReportDiagnostic to report the diagnostic that has been created";

        [Fact]
        public void DiagnosticReportMissing1()
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticReportMissing,
                Message = s_diagnosticReportMissingMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 65, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            // Sends diagnostic information to the IDE to be shown to the user
            context.ReportDiagnostic(diagnostic);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        #endregion

        #region DiagnosticReportIncorrect

        private const string s_diagnosticReportIncorrectMessage = MessagePrefix + "This statement should use context.ReportDiagnostic to report 'diagnostic'";

        // Incorrect accessor
        [Fact]
        public void DiagnosticReportIncorrect1()
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            obj.ReportDiagnostic(diagnostic);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticReportIncorrect,
                Message = s_diagnosticReportIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 66, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            // Sends diagnostic information to the IDE to be shown to the user
            context.ReportDiagnostic(diagnostic);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        // Doesn't call ReportDiagnostic
        [Fact]
        public void DiagnosticReportIncorrect2()
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            context.Equals(diagnostic);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticReportIncorrect,
                Message = s_diagnosticReportIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 66, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            // Sends diagnostic information to the IDE to be shown to the user
            context.ReportDiagnostic(diagnostic);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        // Doesn't report diagnostic (does something else)
        [Fact]
        public void DiagnosticReportIncorrect3()
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            context.ReportDiagnostic(location);
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticReportIncorrect,
                Message = s_diagnosticReportIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 66, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            // Sends diagnostic information to the IDE to be shown to the user
            context.ReportDiagnostic(diagnostic);
        }
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        // Simple member access expression as opposed to Invocation expression
        [Fact]
        public void DiagnosticReportIncorrect4()
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            context.CancellationToken;
        }
    }
}";

            var expected = new DiagnosticResult
            {
                Id = MetaCompilationAnalyzer.DiagnosticReportIncorrect,
                Message = s_diagnosticReportIncorrectMessage,
                Severity = DiagnosticSeverity.Error,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 66, 13) }
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
                if (ifKeyword.TrailingTrivia.Count == 1)
                {
                    var trailingTrivia = ifKeyword.TrailingTrivia.First();
                    if (trailingTrivia.Kind() == SyntaxKind.WhitespaceTrivia)
                    {
                        if (trailingTrivia.ToString() == "" "")
                        {
                            return;
                        }
                    }
                }
            }

            var open = ifState.OpenParenToken;
            var start = ifKeyword.SpanStart;
            var end = open.SpanStart;
            var span = TextSpan.FromBounds(start, end);
            var location = Location.Create(ifState.SyntaxTree, span);
            var diagnostic = Diagnostic.Create(spacingRule, location);
            // Sends diagnostic information to the IDE to be shown to the user
            context.ReportDiagnostic(diagnostic);
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