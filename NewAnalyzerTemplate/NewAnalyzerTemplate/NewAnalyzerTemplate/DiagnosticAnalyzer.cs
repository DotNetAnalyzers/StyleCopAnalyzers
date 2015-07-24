/*This tutorial is going to guide you to write a diagnostic analyzer that enforces the placement of a single space between the if keyword of an if statement and the 
open parenthesis of the condition.

For more information, please reference the ReadMe.

Before you begin, go to Tools->Extensions and Updates->Online, and install .NET compiler SDK
*/

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace NewAnalyzerTemplate
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NewAnalyzerTemplateAnalyzer : DiagnosticAnalyzer
    {
        // The SupportedDiagnostics property stores an ImmutableArray containing any diagnostics that can be reported by this analyzer
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // The Initialize method is used to register methods to perform analysis of the Syntax Tree when there is a change to the Syntax Tree
        // The AnalysisContext parameter has members, such as RegisterSyntaxNodeAction, that perform the registering mentioned above
        public override void Initialize(AnalysisContext context)
        {
            
        }
    }
}
