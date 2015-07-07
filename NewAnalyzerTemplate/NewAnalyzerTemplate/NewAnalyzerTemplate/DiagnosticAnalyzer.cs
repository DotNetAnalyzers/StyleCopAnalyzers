// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/*This tutorial is going to guide you to write a diagnostic analyzer that enforces the placement of one space between the if keyword of an if statement and the 
open parenthesis of the condition.

For more information, please reference the ReadMe.

Before you begin, go to Tools->Extensions and Updates->Online, and install:
    - .NET Compiler SDK
    - Roslyn Syntax Visualizer
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
}
