// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;

namespace MetaCompilation
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MetaCompilationAnalyzer : DiagnosticAnalyzer
    {
        private const string s_messagePrefix = "T: ";
        
        //default values for the DiagnosticDescriptors
        private const string s_ruleCategory = "Tutorial";
        private const DiagnosticSeverity _ruleDefaultSeverity = DiagnosticSeverity.Error;
        private const bool _ruleEnabledByDefault = true;

        //creates a DiagnosticDescriptor with the above defaults
        public static DiagnosticDescriptor CreateRule(string id, string title, string messageFormat, string description = "")
        {
            DiagnosticDescriptor rule = new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                defaultSeverity: _ruleDefaultSeverity,
                isEnabledByDefault: _ruleEnabledByDefault,
                category: s_ruleCategory,
                description: description
                );

            return rule;
        }

        #region id rules
        public const string MissingId = "MetaAnalyzer001";
        internal static DiagnosticDescriptor MissingIdRule = CreateRule(MissingId, "Missing diagnostic id", s_messagePrefix + "'{0}' should have a diagnostic id (a public, constant string uniquely identifying each diagnostic)", "The diagnostic id identifies a particular diagnostic so that the diagnotic can be fixed in CodeFixProvider.cs");
        #endregion

        #region Initialize rules
        public const string MissingInit = "MetaAnalyzer002";
        internal static DiagnosticDescriptor MissingInitRule = CreateRule(MissingInit, "Missing Initialize method", s_messagePrefix + "'{0}' is missing the required inherited Initialize method, needed to register analysis actions", "An analyzer requires the Initialize method to register code analysis actions. Actions are registered to call an analysis method when something specific changes in the syntax tree or semantic model. For example, context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.IfStatement) will call AnalyzeMethod every time an if-statement changes in the syntax tree.");

        public const string MissingRegisterStatement = "MetaAnalyzer003";
        internal static DiagnosticDescriptor MissingRegisterRule = CreateRule(MissingRegisterStatement, "Missing register statement", s_messagePrefix + "A syntax node action should be registered within the '{0}' method", "The Initialize method must register for at least one action so that some analysis can be performed. Otherwise, analysis will not be performed and no diagnostics will be reported. Registering a syntax node action is useful for analyzing the syntax of a piece of code.");

        public const string TooManyInitStatements = "MetaAnalyzer004";
        internal static DiagnosticDescriptor TooManyInitStatementsRule = CreateRule(TooManyInitStatements, "Multiple registered actions", s_messagePrefix + "For this tutorial, the '{0}' method should only register one action", "For this tutorial only, the Initialize method should only register one action. More complicated analyzers may need to register multiple actions.");
        
        public const string IncorrectInitSig = "MetaAnalyzer005";
        internal static DiagnosticDescriptor IncorrectInitSigRule = CreateRule(IncorrectInitSig, "Incorrect method signature", s_messagePrefix + "The '{0}' method should return void, have the 'override' modifier, and have a single parameter of type 'AnalysisContext'", "The Initialize method should override the abstract Initialize class member from the DiagnosticAnalyzer class. It therefore needs to be public, overriden, and return void. It needs to have a single input parameter of type 'AnalysisContext.'");

        public const string InvalidStatement = "MetaAnalyzer006";
        internal static DiagnosticDescriptor InvalidStatementRule = CreateRule(InvalidStatement, "Incorrect statement", s_messagePrefix + "The Initialize method only registers actions, therefore any other statement placed in Initialize is incorrect", "By definition, the purpose of the Initialize method is to register actions for analysis. Therefore, all other statements placed in Initialize are incorrect.");

        public const string IncorrectKind = "MetaAnalyzer051";
        internal static DiagnosticDescriptor IncorrectKindRule = CreateRule(IncorrectKind, "Incorrect kind", s_messagePrefix + "This tutorial only allows registering for SyntaxKind.IfStatement", "For the purposes of this tutorial, the only analysis will occur on an if-statement, so it is only necessary to register for syntax of kind IfStatement");

        public const string IncorrectRegister = "MetaAnalyzer052";
        internal static DiagnosticDescriptor IncorrectRegisterRule = CreateRule(IncorrectRegister, "Incorrect register", s_messagePrefix + "This tutorial only registers SyntaxNode actions", "For the purposes of this tutorial, analysis will occur on SyntaxNodes, so only actions on SyntaxNodes should be registered");

        public const string IncorrectArguments = "MetaAnalyzer053";
        internal static DiagnosticDescriptor IncorrectArgumentsRule = CreateRule(IncorrectArguments, "Incorrect arguments", s_messagePrefix + "The method RegisterSyntaxNodeAction requires 2 arguments: a method and a SyntaxKind", "The RegisterSyntaxNodeAction method takes two arguments. The first argument is a method that will be called to perform the analysis. The second argument is a SyntaxKind, which is the kind of syntax that the method will be triggered on");
        #endregion

        #region SupportedDiagnostics rules
        public const string MissingSuppDiag = "MetaAnalyzer007";
        internal static DiagnosticDescriptor MissingSuppDiagRule = CreateRule(MissingSuppDiag, "Missing SupportedDiagnostics property", s_messagePrefix + "You are missing the required inherited SupportedDiagnostics property", "The SupportedDiagnostics property tells the analyzer which diagnostic ids the analyzer supports, in other words, which DiagnosticDescriptors might be reported by the analyzer. Generally, any DiagnosticDescriptor should be returned by SupportedDiagnostics.");

        public const string IncorrectSigSuppDiag = "MetaAnalyzer008";
        internal static DiagnosticDescriptor IncorrectSigSuppDiagRule = CreateRule(IncorrectSigSuppDiag, "Incorrect SupportedDiagnostics property", s_messagePrefix + "The overriden SupportedDiagnostics property should return an Immutable Array of Diagnostic Descriptors");

        public const string MissingAccessor = "MetaAnalyzer009";
        internal static DiagnosticDescriptor MissingAccessorRule = CreateRule(MissingAccessor, "Missing get-accessor", s_messagePrefix + "The '{0}' property is missing a get-accessor to return a list of supported diagnostics", "The SupportedDiagnostics property needs to have a get-accessor to make the ImmutableArray of DiagnosticDescriptors accessible");

        public const string TooManyAccessors = "MetaAnalyzer010";
        internal static DiagnosticDescriptor TooManyAccessorsRule = CreateRule(TooManyAccessors, "Too many accessors", s_messagePrefix + "The '{0}' property needs only a single get-accessor", "The purpose of the SupportedDiagnostics property is to return a list of all diagnostics that can be reported by a particular analyzer, so it doesn't have a need for any other accessors");

        public const string IncorrectAccessorReturn = "MetaAnalyzer011";
        internal static DiagnosticDescriptor IncorrectAccessorReturnRule = CreateRule(IncorrectAccessorReturn, "Get accessor missing return value", s_messagePrefix + "The get-accessor should return an ImmutableArray containing all of the DiagnosticDescriptor rules", "The purpose of the SupportedDiagnostics property's get-accessor is to return a list of all diagnostics that can be reported by a particular analyzer");

        public const string SuppDiagReturnValue = "MetaAnalyzer012";
        internal static DiagnosticDescriptor SuppDiagReturnValueRule = CreateRule(SuppDiagReturnValue, "SupportedDiagnostics return value incorrect", s_messagePrefix + "The '{0}' property's get-accessor should return an ImmutableArray containing all DiagnosticDescriptor rules", "The purpose of the SupportedDiagnostics property's get-accessor is to return a list of all diagnostics that can be reported by a particular analyzer");

        public const string SupportedRules = "MetaAnalyzer013";
        internal static DiagnosticDescriptor SupportedRulesRule = CreateRule(SupportedRules, "ImmutableArray incorrect", s_messagePrefix + "The ImmutableArray should contain every DiagnosticDescriptor rule that was created", "The purpose of the SupportedDiagnostics property is to return a list of all diagnostics that can be reported by a particular analyzer, so it should include every DiagnosticDescriptor rule that is created");

        #endregion

        #region rule rules
        public const string IdDeclTypeError = "MetaAnalyzer014";
        internal static DiagnosticDescriptor IdDeclTypeErrorRule = CreateRule(IdDeclTypeError, "Incorrect DiagnosticDescriptor id", s_messagePrefix + "The diagnostic id should be the constant string declared above", "The id parameter of a DiagnosticDescriptor should be a string constant previously declared. This ensures that the diagnostic id is accessible from the CodeFixProvider.cs file.");

        public const string MissingIdDeclaration = "MetaAnalyzer015";
        internal static DiagnosticDescriptor MissingIdDeclarationRule = CreateRule(MissingIdDeclaration, "Missing Diagnostic id declaration", s_messagePrefix + "This diagnostic id should be the constant string declared above", "The id parameter of a DiagnosticDescriptor should be a string constant previously declared. This ensures that the diagnostic id is accessible from the CodeFixProvider.cs file.");

        public const string DefaultSeverityError = "MetaAnalyzer016";
        internal static DiagnosticDescriptor DefaultSeverityErrorRule = CreateRule(DefaultSeverityError, "Incorrect defaultSeverity", s_messagePrefix + "The 'defaultSeverity' should be either DiagnosticSeverity.Error or DiagnosticSeverity.Warning", "There are four option for the severity of the diagnostic: error, warning, hidden, and info. An error is completely not allowed and causes build errors. A warning is something that might be a problem, but is not a build error. An info diagnostic is simply information and is not actually a problem. A hidden diagnostic is raised as an issue, but it is not accessible through normal means. In simple analyzers, the most common severities are error and warning.");

        public const string EnabledByDefaultError = "MetaAnalyzer017";
        internal static DiagnosticDescriptor EnabledByDefaultErrorRule = CreateRule(EnabledByDefaultError, "Incorrect isEnabledByDefault", s_messagePrefix + "The 'isEnabledByDefault' field should be set to true", "The 'isEnabledByDefault' field determines whether the diagnostic is enabled by default or the user of the analyzer has to manually enable the diagnostic. Generally, it will be set to true.");

        public const string InternalAndStaticError = "MetaAnalyzer018";
        internal static DiagnosticDescriptor InternalAndStaticErrorRule = CreateRule(InternalAndStaticError, "Incorrect DiagnosticDescriptor modifiers", s_messagePrefix + "The '{0}' field should be internal and static", "The DiagnosticDescriptor rules should all be internal because they only need to be accessed by pieces of this project and nothing outside. They should be static because their lifetime will extend throughout the entire running of this program");

        public const string MissingRule = "MetaAnalyzer019";
        internal static DiagnosticDescriptor MissingRuleRule = CreateRule(MissingRule, "Missing DiagnosticDescriptor", s_messagePrefix + "The analyzer should have at least one DiagnosticDescriptor rule", "The DiagnosticDescriptor rule is what is reported by the analyzer when it finds a problem, so there must be at least one. It should have an id to differentiate the diagnostic, a default severity level, a boolean describing if it's enabled by default, a title, a message, and a category.");

        public const string IdStringLiteral = "MetaAnalyzer059";
        internal static DiagnosticDescriptor IdStringLiteralRule = CreateRule(IdStringLiteral, "ID string literal", s_messagePrefix + "The ID should not be a string literal, because the ID must be accessible from the code fix provider");

        public const string Title = "MetaAnalyzer060";
        internal static DiagnosticDescriptor TitleRule = CreateRule(Title, "Change default title", s_messagePrefix + "Please change the title to a string of your choosing");

        public const string Message = "MetaAnalyzer061";
        internal static DiagnosticDescriptor MessageRule = CreateRule(Message, "Change default message", s_messagePrefix + "Please change the default message to a string of your choosing");

        public const string Category = "MetaAnalyzer062";
        internal static DiagnosticDescriptor CategoryRule = CreateRule(Category, "Change default category", s_messagePrefix + "Please change the category to a string of your choosing");
        #endregion

        #region analysis for IfStatement rules
        public const string IfStatementMissing = "MetaAnalyzer020";
        internal static DiagnosticDescriptor IfStatementMissingRule = CreateRule(IfStatementMissing, "Missing if-statement extraction", s_messagePrefix + "The first step of the SyntaxNode analysis is to extract the if-statement from '{0}' by casting {0}.Node to IfStatementSyntax", "The context parameter has a Node member. This Node is what the register statement from Initialize triggered on, and so should be cast to the expected syntax or symbol type");

        public const string IfStatementIncorrect = "MetaAnalyzer021";
        internal static DiagnosticDescriptor IfStatementIncorrectRule = CreateRule(IfStatementIncorrect, "If-statement extraction incorrect", s_messagePrefix + "This statement should extract the if-statement being analyzed by casting {0}.Node to IfStatementSyntax", "The context parameter has a Node member. This Node is what the register statement from Initialize triggered on, so it should be cast to the expected syntax or symbol type");

        public const string IfKeywordMissing = "MetaAnalyzer022";
        internal static DiagnosticDescriptor IfKeywordMissingRule = CreateRule(IfKeywordMissing, "Missing if-keyword extraction", s_messagePrefix + "Next, extract the if-keyword SyntaxToken from '{0}'", "In the syntax tree, a node of type IfStatementSyntax has an IfKeyword attached to it. On the syntax tree diagram, this is represented by the green 'if' SyntaxToken");

        public const string IfKeywordIncorrect = "MetaAnalyzer023";
        internal static DiagnosticDescriptor IfKeywordIncorrectRule = CreateRule(IfKeywordIncorrect, "Incorrect if-keyword extraction", s_messagePrefix + "This statement should extract the if-keyword SyntaxToken from '{0}'", "In the syntax tree, a node of type IfStatementSyntax has an IfKeyword attached to it. On the syntax tree diagram, this is represented by the green 'if' SyntaxToken");

        public const string TrailingTriviaCheckMissing = "MetaAnalyzer024";
        internal static DiagnosticDescriptor TrailingTriviaCheckMissingRule = CreateRule(TrailingTriviaCheckMissing, "Missing trailing trivia check", s_messagePrefix + "Next, begin looking for the space between 'if' and '(' by checking if '{0}' has trailing trivia", "Syntax trivia are all the things that aren't actually code (i.e. comments, whitespace, end of line tokens, etc). The first step in checking for a single space between the if-keyword and '(' is to check if the if-keyword SyntaxToken has any trailing trivia");

        public const string TrailingTriviaCheckIncorrect = "MetaAnalyzer025";
        internal static DiagnosticDescriptor TrailingTriviaCheckIncorrectRule = CreateRule(TrailingTriviaCheckIncorrect, "Incorrect trailing trivia check", s_messagePrefix + "This statement should be an if-statement that checks to see if '{0}' has trailing trivia", "Syntax trivia are all the things that aren't actually code (i.e. comments, whitespace, end of line tokens, etc). The first step in checking for a single space between the if-keyword and '(' is to check if the if-keyword SyntaxToken has any trailing trivia");

        public const string TrailingTriviaVarMissing = "MetaAnalyzer026";
        internal static DiagnosticDescriptor TrailingTriviaVarMissingRule = CreateRule(TrailingTriviaVarMissing, "Missing trailing trivia extraction", s_messagePrefix + "Next, extract the first trailing trivia of '{0}' into a variable", "The first trailing trivia of the if-keyword should be a single whitespace");

        public const string TrailingTriviaVarIncorrect = "MetaAnalyzer027";
        internal static DiagnosticDescriptor TrailingTriviaVarIncorrectRule = CreateRule(TrailingTriviaVarIncorrect, "Incorrect trailing trivia extraction", s_messagePrefix + "This statement should extract the first trailing trivia of '{0}' into a variable", "The first trailing trivia of the if-keyword should be a single whitespace");

        public const string TrailingTriviaKindCheckMissing = "MetaAnalyzer028";
        internal static DiagnosticDescriptor TrailingTriviaKindCheckMissingRule = CreateRule(TrailingTriviaKindCheckMissing, "Missing SyntaxKind check", s_messagePrefix + "Next, check if the kind of '{0}' is whitespace trivia");

        public const string TrailingTriviaKindCheckIncorrect = "MetaAnalyzer029";
        internal static DiagnosticDescriptor TrailingTriviaKindCheckIncorrectRule = CreateRule(TrailingTriviaKindCheckIncorrect, "Incorrect SyntaxKind check", s_messagePrefix + "This statement should check to see if the kind of '{0}' is whitespace trivia");

        public const string WhitespaceCheckMissing = "MetaAnalyzer030";
        internal static DiagnosticDescriptor WhitespaceCheckMissingRule = CreateRule(WhitespaceCheckMissing, "Missing whitespace check", s_messagePrefix + "Next, check if '{0}' is a single whitespace, which is the desired formatting");

        public const string WhitespaceCheckIncorrect = "MetaAnalyzer031";
        internal static DiagnosticDescriptor WhitespaceCheckIncorrectRule = CreateRule(WhitespaceCheckIncorrect, "Incorrect whitespace check", s_messagePrefix + "This statement should check to see if '{0}' is a single whitespace, which is the desired formatting");

        public const string ReturnStatementMissing = "MetaAnalyzer032";
        internal static DiagnosticDescriptor ReturnStatementMissingRule = CreateRule(ReturnStatementMissing, "Missing return", s_messagePrefix + "Next, since if the code reaches this point the formatting must be correct, return from '{0}'", "If the analyzer determines that there are no issues with the code it is analyzing, it can simply return from the analysis method without reporting any diagnostics");

        public const string ReturnStatementIncorrect = "MetaAnalyzer033";
        internal static DiagnosticDescriptor ReturnStatementIncorrectRule = CreateRule(ReturnStatementIncorrect, "Incorrect return", s_messagePrefix + "This statement should return from '{0}', because reaching this point in the code means that the if-statement being analyzed has the correct spacing", "If the analyzer determines that there are no issues with the code it is analyzing, it can simply return from the analysis method without reporting any diagnostics");

        public const string OpenParenMissing = "MetaAnalyzer034";
        internal static DiagnosticDescriptor OpenParenMissingRule = CreateRule(OpenParenMissing, "Missing open parenthesis variable", s_messagePrefix + "Moving on to the creation and reporting of the diagnostic, extract the open parenthesis of '{0}' into a variable to use as the end of the diagnostic span", "The open parenthesis of the condition is going to be the end point of the diagnostic squiggle that is created");

        public const string OpenParenIncorrect = "MetaAnalyzer035";
        internal static DiagnosticDescriptor OpenParenIncorrectRule = CreateRule(OpenParenIncorrect, "Open parenthesis variable incorrect", s_messagePrefix + "This statement should extract the open parenthesis of '{0}' to use as the end of the diagnostic span", "The open parenthesis of the condition is going to be the end point of the diagnostic squiggle that is created");

        public const string StartSpanMissing = "MetaAnalyzer036";
        internal static DiagnosticDescriptor StartSpanMissingRule = CreateRule(StartSpanMissing, "Start span variable missing", s_messagePrefix + "Next, extract the start of the span of '{0}' into a variable, to be used as the start of the diagnostic span", "Each node in the syntax tree has a span. This span represents the number of character spaces that the node takes up");

        public const string StartSpanIncorrect = "MetaAnalyzer037";
        internal static DiagnosticDescriptor StartSpanIncorrectRule = CreateRule(StartSpanIncorrect, "Start span variable incorrect", s_messagePrefix + "This statement should extract the start of the span of '{0}' into a variable, to be used as the start of the diagnostic span", "Each node in the syntax tree has a span. This span represents the number of character spaces that the node takes up");

        public const string EndSpanMissing = "MetaAnalyzer038";
        internal static DiagnosticDescriptor EndSpanMissingRule = CreateRule(EndSpanMissing, "End span variable missing", s_messagePrefix + "Next, determine the end of the span of the diagnostic that is going to be reported", "The open parenthesis of the condition is going to be the end point of the diagnostic squiggle that is created");

        public const string EndSpanIncorrect = "MetaAnalyzer039";
        internal static DiagnosticDescriptor EndSpanIncorrectRule = CreateRule(EndSpanIncorrect, "End span variable incorrect", s_messagePrefix + "This statement should extract the start of the span of '{0}' into a variable, to be used as the end of the diagnostic span", "Each node in the syntax tree has a span. This span represents the number of character spaces that the node takes up");

        public const string SpanMissing = "MetaAnalyzer040";
        internal static DiagnosticDescriptor SpanMissingRule = CreateRule(SpanMissing, "Diagnostic span variable missing", s_messagePrefix + "Next, using TextSpan.FromBounds, create a variable that is the span of the diagnostic that will be reported", "Each node in the syntax tree has a span. This span represents the number of character spaces that the node takes up");

        public const string SpanIncorrect = "MetaAnalyzer041";
        internal static DiagnosticDescriptor SpanIncorrectRule = CreateRule(SpanIncorrect, "Diagnostic span variable incorrect", s_messagePrefix + "This statement should use TextSpan.FromBounds, '{0}', and '{1}' to create the span of the diagnostic that will be reported", "Each node in the syntax tree has a span. This span represents the number of character spaces that the node takes up. TextSpan.FromBounds(start, end) can be used to create a span to use for a diagnostic");

        public const string LocationMissing = "MetaAnalyzer042";
        internal static DiagnosticDescriptor LocationMissingRule = CreateRule(LocationMissing, "Diagnostic location variable missing", s_messagePrefix + "Next, using Location.Create, create a location for the diagnostic", "A location can be created by combining a span with a syntax tree. The span is applied to the given syntax tree so that the location within the syntax tree is determined");

        public const string LocationIncorrect = "MetaAnalyzer043";
        internal static DiagnosticDescriptor LocationIncorrectRule = CreateRule(LocationIncorrect, "Diagnostic location variable incorrect", s_messagePrefix + "This statement should use Location.Create, '{0}', and '{1}' to create the location of the diagnostic", "A location can be created by combining a span with a syntax tree. The span is applied to the given syntax tree so that the location within the syntax tree is determined");

        public const string TrailingTriviaCountMissing = "MetaAnalyzer057";
        internal static DiagnosticDescriptor TriviaCountMissingRule = CreateRule(TrailingTriviaCountMissing, "Trailing trivia count missing", s_messagePrefix + "Next, check that '{0}' only has one trailing trivia element");

        public const string TrailingTriviaCountIncorrect = "MetaAnalyzer058";
        internal static DiagnosticDescriptor TriviaCountIncorrectRule = CreateRule(TrailingTriviaCountIncorrect, "Trailing trivia count incorrect", s_messagePrefix + "This statement should check that '{0}' only has one trailing trivia element");
        #endregion

        #region analysis rules
        public const string MissingAnalysisMethod = "MetaAnalyzer044";
        internal static DiagnosticDescriptor MissingAnalysisMethodRule = CreateRule(MissingAnalysisMethod, "Missing analysis method", s_messagePrefix + "The method '{0}' that was registered to perform the analysis is missing", "In Initialize, the register statement denotes an analysis method to be called when an action is triggered. This method needs to be created");

        public const string IncorrectAnalysisAccessibility = "MetaAnalyzer054";
        internal static DiagnosticDescriptor IncorrectAnalysisAccessibilityRule = CreateRule(IncorrectAnalysisAccessibility, "Incorrect analysis method accessibility", s_messagePrefix + "The '{0}' method should be private");

        public const string IncorrectAnalysisReturnType = "MetaAnalyzer055";
        internal static DiagnosticDescriptor IncorrectAnalysisReturnTypeRule = CreateRule(IncorrectAnalysisReturnType, "Incorrect analysis method return type", s_messagePrefix + "The '{0}' method should have a void return type");

        public const string IncorrectAnalysisParameter = "MetaAnalyzer056";
        internal static DiagnosticDescriptor IncorrectAnalysisParameterRule = CreateRule(IncorrectAnalysisParameter, "Incorrect parameter to analysis method", s_messagePrefix + "The '{0}' method should take one parameter of type SyntaxNodeAnalysisContext");

        public const string TooManyStatements = "MetaAnalyzer045";
        internal static DiagnosticDescriptor TooManyStatementsRule = CreateRule(TooManyStatements, "Too many statements", s_messagePrefix + "This {0} should only have {1} statement(s), which should {2}", "For the purpose of this tutorial there are too many statements here, use the code fixes to guide you through the creation of this section");

        public const string DiagnosticMissing = "MetaAnalyzer046";
        internal static DiagnosticDescriptor DiagnosticMissingRule = CreateRule(DiagnosticMissing, "Diagnostic variable missing", s_messagePrefix + "Next, use Diagnostic.Create to create the diagnostic", "This is the diagnostic that will be reported to the user as an error squiggle");

        public const string DiagnosticIncorrect = "MetaAnalyzer047";
        internal static DiagnosticDescriptor DiagnosticIncorrectRule = CreateRule(DiagnosticIncorrect, "Diagnostic variable incorrect", s_messagePrefix + "This statement should use Diagnostic.Create, '{0}', and '{1}' to create the diagnostic that will be reported", "The diagnostic is created with a DiagnosticDescriptor, a Location, and message arguments. The message arguments are the inputs to the DiagnosticDescriptor MessageFormat format string");

        public const string DiagnosticReportMissing = "MetaAnalyzer048";
        internal static DiagnosticDescriptor DiagnosticReportMissingRule = CreateRule(DiagnosticReportMissing, "Diagnostic report missing", s_messagePrefix + "Next, use '{0}'.ReportDiagnostic to report the diagnostic that has been created", "A diagnostic is reported to a context so that the diagnostic will appear as a squiggle and in the eroor list");

        public const string DiagnosticReportIncorrect = "MetaAnalyzer049";
        internal static DiagnosticDescriptor DiagnosticReportIncorrectRule = CreateRule(DiagnosticReportIncorrect, "Diagnostic report incorrect", s_messagePrefix + "This statement should use {0}.ReportDiagnostic to report '{1}'", "A diagnostic is reported to a context so that the diagnostic will appear as a squiggle and in the eroor list");
        #endregion

        public const string GoToCodeFix = "MetaAnalyzer050";
        internal static DiagnosticDescriptor GoToCodeFixRule = new DiagnosticDescriptor(
            id: GoToCodeFix,
            title: "Analyzer tutorial complete",
            messageFormat: s_messagePrefix + "Congratulations! You have written an analyzer! If you would like to explore a code fix for your diagnostic, open up CodeFixProvider.cs and take a look! To see your analyzer in action, press F5. A new instance of Visual Studio will open up, in which you can open a new C# console app and write test if-statements.",
            category: s_ruleCategory,
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(MissingIdRule, 
                                             MissingInitRule, 
                                             MissingRegisterRule, 
                                             TooManyInitStatementsRule, 
                                             IncorrectInitSigRule,
                                             InvalidStatementRule,
                                             MissingSuppDiagRule,
                                             IncorrectSigSuppDiagRule,
                                             MissingAccessorRule,
                                             TooManyAccessorsRule,
                                             IncorrectAccessorReturnRule,
                                             SuppDiagReturnValueRule,
                                             SupportedRulesRule,
                                             IdDeclTypeErrorRule,
                                             MissingIdDeclarationRule,
                                             DefaultSeverityErrorRule,
                                             EnabledByDefaultErrorRule, 
                                             InternalAndStaticErrorRule,
                                             MissingRuleRule,
                                             IfStatementMissingRule,
                                             IfStatementIncorrectRule,
                                             IfKeywordMissingRule,
                                             IfKeywordIncorrectRule,
                                             TrailingTriviaCheckMissingRule,
                                             TrailingTriviaCheckIncorrectRule,
                                             TrailingTriviaVarMissingRule,
                                             TrailingTriviaVarIncorrectRule,
                                             TrailingTriviaKindCheckIncorrectRule,
                                             TrailingTriviaKindCheckMissingRule,
                                             WhitespaceCheckMissingRule,
                                             WhitespaceCheckIncorrectRule,
                                             ReturnStatementMissingRule,
                                             ReturnStatementIncorrectRule,
                                             OpenParenIncorrectRule,
                                             OpenParenMissingRule,
                                             StartSpanIncorrectRule,
                                             StartSpanMissingRule,
                                             EndSpanIncorrectRule,
                                             EndSpanMissingRule,
                                             SpanIncorrectRule,
                                             SpanMissingRule,
                                             LocationIncorrectRule,
                                             LocationMissingRule,
                                             MissingAnalysisMethodRule,
                                             IncorrectAnalysisAccessibilityRule,
                                             IncorrectAnalysisReturnTypeRule,
                                             IncorrectAnalysisParameterRule,
                                             TooManyStatementsRule,
                                             DiagnosticMissingRule,
                                             DiagnosticIncorrectRule,
                                             DiagnosticReportIncorrectRule,
                                             DiagnosticReportMissingRule,
                                             GoToCodeFixRule,
                                             IncorrectKindRule,
                                             IncorrectRegisterRule,
                                             IncorrectArgumentsRule,
                                             TriviaCountMissingRule,
                                             TriviaCountIncorrectRule,
                                             IdStringLiteralRule,
                                             TitleRule,
                                             MessageRule,
                                             CategoryRule);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(SetupAnalysis);
        }

        //creates an instance of a class to perform the analysis statefully, and registers for various actions
        private void SetupAnalysis(CompilationStartAnalysisContext context)
        {
            //state collector
            CompilationAnalyzer compilationAnalyzer = new CompilationAnalyzer();

            //collects all class, method, field, and property symbols as state
            context.RegisterSymbolAction(compilationAnalyzer.AddClass, SymbolKind.NamedType);
            context.RegisterSymbolAction(compilationAnalyzer.AddMethod, SymbolKind.Method);
            context.RegisterSymbolAction(compilationAnalyzer.AddField, SymbolKind.Field);
            context.RegisterSymbolAction(compilationAnalyzer.AddProperty, SymbolKind.Property);

            //analyzes the state that has been collected
            context.RegisterCompilationEndAction(compilationAnalyzer.ReportCompilationEndDiagnostics);
        }

        // Performs stateful analysis
        private class CompilationAnalyzer
        {
            private List<IMethodSymbol> _analyzerMethodSymbols = new List<IMethodSymbol>();
            private List<IPropertySymbol> _analyzerPropertySymbols = new List<IPropertySymbol>();
            private List<IFieldSymbol> _analyzerFieldSymbols = new List<IFieldSymbol>();
            private List<INamedTypeSymbol> _otherAnalyzerClassSymbols = new List<INamedTypeSymbol>();
            private IMethodSymbol _initializeSymbol = null;
            private IPropertySymbol _propertySymbol = null;
            private INamedTypeSymbol _analyzerClassSymbol = null;
            private Dictionary<string, string> _branchesDict = new Dictionary<string, string>();
            private IPropertySymbol _codeFixFixableDiagnostics = null;
            private List<IMethodSymbol> _codeFixMethodSymbols = new List<IMethodSymbol>();
            private IMethodSymbol _registerCodeFixesAsync = null;
            private INamedTypeSymbol _codeFixClassSymbol = null;

            //"main" method, performs the analysis once state has been collected
            internal protected void ReportCompilationEndDiagnostics(CompilationAnalysisContext context)
            {
                //supported main branches for tutorial
                _branchesDict.Add("RegisterSyntaxNodeAction", "SyntaxNode");

                //supported sub-branches for tutorial
                List<string> allowedKinds = new List<string>();
                allowedKinds.Add("IfStatement");

                if (_analyzerClassSymbol == null)
                {
                    return;
                }

                //look for and interpret id fields
                List<string> idNames = CheckIds();

                if (idNames.Count > 0)
                {
                    //look for and interpret rule fields
                    List<string> ruleNames = CheckRules(idNames, context);

                    if (ruleNames.Count > 0)
                    {
                        //look for and interpret SupportedDiagnostics property
                        bool supportedDiagnosticsCorrect = CheckSupportedDiagnostics(ruleNames, context);

                        if (supportedDiagnosticsCorrect)
                        {
                            //gather initialize info
                            CheckInitializeInfo registerInfo = CheckInitialize(context);
                            if (registerInfo == null)
                            {
                                return;
                            }

                            var registerSymbol = registerInfo.RegisterMethod;
                            if (registerSymbol == null)
                            {
                                return;
                            }

                            var registerArgs = registerInfo.RegisterArgs;
                            if (registerArgs == null)
                            {
                                return;
                            }

                            if (registerArgs.Count == 0)
                            {
                                return;
                            }

                            IMethodSymbol analysisMethodSymbol = null;
                            if (registerArgs.Count > 0)
                            {
                                analysisMethodSymbol = (IMethodSymbol)registerArgs[0];
                            }

                            IFieldSymbol kind = null;
                            if (registerArgs.Count > 1)
                            {
                                kind = (IFieldSymbol)registerArgs[1];
                            }
                            else
                            {
                                return;
                            }

                            var invocationExpression = registerInfo.InvocationExpr;
                            if (invocationExpression == null)
                            {
                                return;
                            }

                            //interpret initialize info
                            if (_branchesDict.ContainsKey(registerSymbol.Name))
                            {
                                string kindName = null;
                                if (kind != null)
                                {
                                    kindName = kind.Name;
                                }

                                if (kindName == null || allowedKinds.Contains(kindName))
                                {
                                    //look for and interpret analysis methods
                                    bool analysisMethodFound = CheckMethods(invocationExpression, context);

                                    if (analysisMethodFound)
                                    {
                                        //check the SyntaxNode, Symbol, Compilation, CodeBlock, etc analysis method(s)
                                        bool analysisCorrect = CheckAnalysis(_branchesDict[registerSymbol.Name], kindName, ruleNames, context, analysisMethodSymbol);

                                        if (analysisCorrect)
                                        {
                                            ReportDiagnostic(context, GoToCodeFixRule, _analyzerClassSymbol.Locations[0]);
                                        }
                                        else
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }


                                else
                                {
                                    Location loc = null;
                                    if (kindName == null)
                                    {
                                        loc = invocationExpression.ArgumentList.GetLocation();
                                    }
                                    else
                                    {
                                        loc = invocationExpression.ArgumentList.Arguments[1].GetLocation();
                                    }

                                    ReportDiagnostic(context, IncorrectKindRule, loc);
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ReportDiagnostic(context, MissingIdRule, _analyzerClassSymbol.Locations[0], _analyzerClassSymbol.Name);
                }
            }

            // Checks the syntax tree analysis part of the user analyzer, returns a bool representing whether the check was successful or not
            private bool CheckAnalysis(string branch, string kind, List<string> ruleNames, CompilationAnalysisContext context, IMethodSymbol analysisMethodSymbol)
            {
                if (branch == "SyntaxNode")
                {
                    if (kind == "IfStatement")
                    {
                        return CheckIfStatementAnalysis(ruleNames, context, analysisMethodSymbol);
                    }
                }

                return false;
            }

            #region CheckAnalysis for IfStatement
            // Checks the AnalyzeIfStatement of the user's analyzer, returns a bool representing whether the check was successful or not
            private bool CheckIfStatementAnalysis(List<string> ruleNames, CompilationAnalysisContext context, IMethodSymbol analysisMethodSymbol)
            {
                var methodDeclaration = AnalysisGetStatements(analysisMethodSymbol) as MethodDeclarationSyntax;
                var body = methodDeclaration.Body as BlockSyntax;
                if (body == null)
                {
                    return false;
                }

                var statements = body.Statements;
                if (statements == null)
                {
                    return false;
                }

                var contextParameter = methodDeclaration.ParameterList.Parameters[0] as ParameterSyntax;
                if (contextParameter == null)
                {
                    return false;
                }

                int statementCount = statements.Count;

                if (statementCount > 0)
                {
                    SyntaxToken statementIdentifierToken = IfStatementAnalysis1(statements, contextParameter);
                    if (statementIdentifierToken.Text == "")
                    {
                        IfDiagnostic(context, statements[0], IfStatementIncorrectRule, contextParameter.Identifier.Text);
                        return false;
                    }

                    if (statementCount > 1)
                    {
                        SyntaxToken keywordIdentifierToken = IfStatementAnalysis2(statements, statementIdentifierToken);
                        if (keywordIdentifierToken.Text == "")
                        {
                            IfDiagnostic(context, statements[1], IfKeywordIncorrectRule, statementIdentifierToken.Text);
                            return false;
                        }

                        // HasTrailingTrivia if-statement in user analyzer
                        if (statementCount > 2)
                        {
                            var triviaBlock = IfStatementAnalysis3(statements, keywordIdentifierToken) as BlockSyntax;
                            if (triviaBlock == null)
                            {
                                IfDiagnostic(context, statements[2], TrailingTriviaCheckIncorrectRule, keywordIdentifierToken.Text);
                                return false;
                            }

                            SyntaxList<StatementSyntax> triviaBlockStatements = triviaBlock.Statements;
                            if (triviaBlockStatements == null)
                            {
                                IfDiagnostic(context, triviaBlock.Parent as StatementSyntax, TriviaCountMissingRule, keywordIdentifierToken.Text);
                                return false;
                            }

                            if (triviaBlockStatements.Count > 0)
                            {
                                BlockSyntax triviaCountBlock = IfStatementAnalysis8(triviaBlockStatements, keywordIdentifierToken);
                                if (triviaCountBlock == null)
                                {
                                    IfDiagnostic(context, triviaBlockStatements[0], TriviaCountIncorrectRule, keywordIdentifierToken.Text);
                                    return false;
                                }

                                SyntaxList<StatementSyntax> triviaCountBlockStatements = triviaCountBlock.Statements;
                                if (triviaCountBlockStatements.Count > 0)
                                {
                                    SyntaxToken triviaIdentifierToken = IfStatementAnalysis4(triviaCountBlockStatements, keywordIdentifierToken);
                                    if (triviaIdentifierToken.Text == "")
                                    {
                                        IfDiagnostic(context, triviaCountBlockStatements[0], TrailingTriviaVarIncorrectRule, keywordIdentifierToken.Text);
                                        return false;
                                    }

                                    // Kind if-statement in user analyzer
                                    if (triviaCountBlockStatements.Count > 1)
                                    {
                                        BlockSyntax triviaKindCheckBlock = IfStatementAnalysis5(triviaCountBlockStatements, triviaIdentifierToken);
                                        if (triviaKindCheckBlock == null)
                                        {
                                            IfDiagnostic(context, triviaCountBlockStatements[1], TrailingTriviaKindCheckIncorrectRule, triviaIdentifierToken.Text);
                                            return false;
                                        }

                                        SyntaxList<StatementSyntax> triviaKindCheckBlockStatements = triviaKindCheckBlock.Statements;
                                        if (triviaKindCheckBlockStatements == null)
                                        {
                                            IfDiagnostic(context, triviaCountBlockStatements[1], TrailingTriviaKindCheckIncorrectRule, triviaIdentifierToken.Text);
                                            return false;
                                        }

                                        // Whitespace if-statement in user analyzer
                                        if (triviaKindCheckBlockStatements.Count > 0)
                                        {
                                            BlockSyntax triviaCheckBlock = IfStatementAnalysis6(triviaKindCheckBlock.Statements, triviaIdentifierToken);
                                            if (triviaCheckBlock == null)
                                            {
                                                IfDiagnostic(context, triviaKindCheckBlockStatements[0], WhitespaceCheckIncorrectRule, triviaIdentifierToken.Text);
                                                return false;
                                            }

                                            SyntaxList<StatementSyntax> triviaCheckBlockStatements = triviaCheckBlock.Statements;
                                            if (triviaCheckBlockStatements == null)
                                            {
                                                IfDiagnostic(context, triviaKindCheckBlockStatements[0], WhitespaceCheckIncorrectRule, triviaIdentifierToken.Text);
                                                return false;
                                            }

                                            if (triviaCheckBlockStatements.Count > 0)
                                            {
                                                if (!IfStatementAnalysis7(triviaCheckBlockStatements))
                                                {
                                                    IfDiagnostic(context, triviaCheckBlockStatements[0], ReturnStatementIncorrectRule, methodDeclaration.Identifier.Text);
                                                    return false;
                                                }

                                                if (triviaCheckBlockStatements.Count > 1)
                                                {
                                                    IfDiagnostic(context, triviaCheckBlock.Parent as StatementSyntax, TooManyStatementsRule, "if-block", "1", "return from the method");
                                                    return false;
                                                }

                                                //successfully through if-statement checks
                                            }
                                            else
                                            {
                                                IfDiagnostic(context, triviaCheckBlock.Parent as StatementSyntax, ReturnStatementMissingRule, methodDeclaration.Identifier.Text);
                                                return false;
                                            }

                                            if (triviaKindCheckBlockStatements.Count > 1)
                                            {
                                                IfDiagnostic(context, triviaKindCheckBlock.Parent as StatementSyntax, TooManyStatementsRule, "if-block", "1", "check if the trivia is a single space");
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            IfDiagnostic(context, triviaKindCheckBlock.Parent as StatementSyntax, WhitespaceCheckMissingRule, triviaIdentifierToken.Text);
                                            return false;
                                        }

                                        if (triviaCountBlockStatements.Count > 2)
                                        {
                                            IfDiagnostic(context, triviaCountBlock.Parent as StatementSyntax, TooManyStatementsRule, "if-block", "2", "extract the first trivia of the if-keyword and check its kind");
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        ReportDiagnostic(context, TrailingTriviaKindCheckMissingRule, triviaCountBlockStatements[0].GetLocation(), triviaIdentifierToken.Text);
                                        return false;
                                    }
                                }
                                else
                                {
                                    IfDiagnostic(context, triviaCountBlock.Parent as StatementSyntax, TrailingTriviaVarMissingRule, keywordIdentifierToken.Text);
                                    return false;
                                }

                                if (triviaBlockStatements.Count > 1)
                                {
                                    IfDiagnostic(context, triviaBlock.Parent as StatementSyntax, TooManyStatementsRule, "if-block", "1", "check the number of trailing trivia on the if-keyword");
                                    return false;
                                }
                            }
                            else
                            {
                                IfDiagnostic(context, triviaBlock.Parent as StatementSyntax, TriviaCountMissingRule, keywordIdentifierToken.Text);
                                return false;
                            }

                            //check diagnostic reporting statements
                            if (statementCount > 3)
                            {
                                bool diagnosticReportingCorrect = CheckDiagnosticCreation(context, statementIdentifierToken, keywordIdentifierToken, ruleNames, statements, contextParameter);
                                if (!diagnosticReportingCorrect)
                                {
                                    return false;
                                }

                                if (statementCount > 10)
                                {
                                    ReportDiagnostic(context, TooManyStatementsRule, methodDeclaration.Identifier.GetLocation(), "method", "10", "walk through the Syntax Tree and check the spacing of the if-statement");
                                    return false;
                                }
                            }
                            else
                            {
                                IfDiagnostic(context, statements[2], OpenParenMissingRule, statementIdentifierToken.Text);
                                return false;
                            }
                        }
                        else
                        {
                            ReportDiagnostic(context, TrailingTriviaCheckMissingRule, statements[1].GetLocation(), keywordIdentifierToken.Text);
                            return false;
                        }
                    }
                    else
                    {
                        ReportDiagnostic(context, IfKeywordMissingRule, statements[0].GetLocation(), statementIdentifierToken.Text);
                        return false;
                    }
                }
                else
                {
                    ReportDiagnostic(context, IfStatementMissingRule, methodDeclaration.Identifier.GetLocation(), contextParameter.Identifier.Text);
                    return false;
                }

                return true;
            }

            // Checks step one of the user's AnalyzerIfStatement method, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken IfStatementAnalysis1(SyntaxList<StatementSyntax> statements, ParameterSyntax contextParameter)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var ifStatement = statements[0] as LocalDeclarationStatementSyntax;
                if (ifStatement == null)
                {
                    return emptyResult;
                }

                var statementName = GetIdentifierTokenFromLocalDecl(ifStatement);
                if (statementName.Text == "")
                {
                    return emptyResult;
                }

                var statementEqualsValueClause = GetEqualsValueClauseFromLocalDecl(ifStatement);
                if (statementEqualsValueClause == null)
                {
                    return emptyResult;
                }

                var statementCastExpression = statementEqualsValueClause.Value as CastExpressionSyntax;
                if (statementCastExpression == null)
                {
                    var statementAsExpression = statementEqualsValueClause.Value as BinaryExpressionSyntax;
                    if (statementAsExpression == null)
                    {
                        return emptyResult;
                    }

                    var left = statementAsExpression.Left as MemberAccessExpressionSyntax;
                    if (left == null)
                    {
                        return emptyResult;
                    }

                    var leftName = left.Name as IdentifierNameSyntax;
                    if (leftName == null || leftName.Identifier.Text != "Node")
                    {
                        return emptyResult;
                    }

                    var leftMember = left.Expression as IdentifierNameSyntax;
                    if (leftMember == null || leftMember.Identifier.Text != contextParameter.Identifier.Text)
                    {
                        return emptyResult;
                    }

                    var right = statementAsExpression.Right as IdentifierNameSyntax;
                    if (right == null || right.Identifier.Text != "IfStatementSyntax")
                    {
                        return emptyResult;
                    }

                    return statementName;
                }

                var statementIdentifier = statementCastExpression.Type as IdentifierNameSyntax;
                if (statementIdentifier == null || statementIdentifier.Identifier.Text != "IfStatementSyntax")
                {
                    return emptyResult;
                }

                var statementExpression = statementCastExpression.Expression as MemberAccessExpressionSyntax;
                if (statementExpression == null)
                {
                    return emptyResult;
                }

                var statementExpressionIdentifier = statementExpression.Expression as IdentifierNameSyntax;
                if (statementExpressionIdentifier == null || statementExpressionIdentifier.Identifier.Text != contextParameter.Identifier.Text)
                {
                    return emptyResult;
                }

                var statementExpressionNode = statementExpression.Name as IdentifierNameSyntax;
                if (statementExpressionNode == null || statementExpressionNode.Identifier.Text != "Node")
                {
                    return emptyResult;
                }

                return statementName;
            }

            // Checks step two of the user's AnalyzerIfStatement method, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken IfStatementAnalysis2(SyntaxList<StatementSyntax> statements, SyntaxToken statementIdentifierToken)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[1] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken keywordIdentifierToken = GetIdentifierTokenFromLocalDecl(statement);
                if (keywordIdentifierToken.Text == "")
                {
                    return emptyResult;
                }

                var equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var memberExpr = equalsValueClause.Value as MemberAccessExpressionSyntax;
                if (memberExpr == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpr.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != statementIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var name = memberExpr.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "IfKeyword")
                {
                    return emptyResult;
                }

                return keywordIdentifierToken;
            }

            // Checks step three of the user's AnalyzerIfStatement method, returns null if analysis failed
            private BlockSyntax IfStatementAnalysis3(SyntaxList<StatementSyntax> statements, SyntaxToken keywordIdentifierToken)
            {
                BlockSyntax emptyResult = null;

                var statement = statements[2] as IfStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                var booleanExpression = statement.Condition as MemberAccessExpressionSyntax;
                if (booleanExpression == null)
                {
                    return emptyResult;
                }

                var identifier = booleanExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != keywordIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var name = booleanExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "HasTrailingTrivia")
                {
                    return emptyResult;
                }

                var block = statement.Statement as BlockSyntax;
                if (block == null)
                {
                    return emptyResult;
                }

                return block;
            }

            // Checks step four of the user's AnalyzerIfStatement method, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken IfStatementAnalysis4(SyntaxList<StatementSyntax> statements, SyntaxToken keywordIdentifierToken)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[0] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken triviaIdentifierToken = GetIdentifierTokenFromLocalDecl(statement);
                if (triviaIdentifierToken.Text == "")
                {
                    return emptyResult;
                }

                var statementEqualsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (statementEqualsValueClause == null)
                {
                    return emptyResult;
                }

                var invocationExpression = statementEqualsValueClause.Value as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return emptyResult;
                }

                var memberExpr = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpr == null)
                {
                    return emptyResult;
                }

                var memberExprInner = memberExpr.Expression as MemberAccessExpressionSyntax;
                if (memberExprInner == null)
                {
                    return emptyResult;
                }

                var innerIdentifier = memberExprInner.Expression as IdentifierNameSyntax;
                if (innerIdentifier == null || innerIdentifier.Identifier.Text != keywordIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var innerName = memberExprInner.Name as IdentifierNameSyntax;
                if (innerName == null || innerName.Identifier.Text != "TrailingTrivia")
                {
                    return emptyResult;
                }

                var memberExprName = memberExpr.Name as IdentifierNameSyntax;
                if (memberExprName == null || memberExprName.Identifier.Text != "First")
                {
                    return emptyResult;
                }

                return triviaIdentifierToken;
            }

            // Checks step five of the user's AnalyzerIfStatement method, returns null if analysis failed
            private BlockSyntax IfStatementAnalysis5(SyntaxList<StatementSyntax> statements, SyntaxToken triviaIdentifierToken)
            {
                BlockSyntax emptyResult = null;

                var statement = statements[1] as IfStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                var booleanExpression = statement.Condition as BinaryExpressionSyntax;
                if (booleanExpression == null)
                {
                    var blockResult = WhitespaceKindCheckAlternate(statement, triviaIdentifierToken) as BlockSyntax;
                    if (blockResult == null)
                    {
                        return emptyResult;
                    }

                    return blockResult;
                }

                var left = booleanExpression.Left as InvocationExpressionSyntax;
                if (left == null)
                {
                    return emptyResult;
                }

                var leftMemberExpr = left.Expression as MemberAccessExpressionSyntax;
                if (leftMemberExpr == null)
                {
                    return emptyResult;
                }

                var leftIdentifier = leftMemberExpr.Expression as IdentifierNameSyntax;
                if (leftIdentifier == null || leftIdentifier.Identifier.Text != triviaIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var leftName = leftMemberExpr.Name as IdentifierNameSyntax;
                if (leftName == null || leftName.Identifier.Text != "Kind")
                {
                    return emptyResult;
                }

                var leftArgumentList = left.ArgumentList as ArgumentListSyntax;
                if (leftArgumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> leftArgs = leftArgumentList.Arguments;
                if (leftArgs == null || leftArgs.Count != 0)
                {
                    return emptyResult;
                }

                var right = booleanExpression.Right as MemberAccessExpressionSyntax;
                if (right == null)
                {
                    return emptyResult;
                }

                var rightIdentifier = right.Expression as IdentifierNameSyntax;
                if (rightIdentifier == null || rightIdentifier.Identifier.Text != "SyntaxKind")
                {
                    return emptyResult;
                }

                var rightName = right.Name as IdentifierNameSyntax;
                if (rightName == null || rightName.Identifier.Text != "WhitespaceTrivia")
                {
                    return emptyResult;
                }

                var block = statement.Statement as BlockSyntax;
                if (block == null)
                {
                    return emptyResult;
                }

                return block;
            }

            // Checks if the whitespace check is .IsKind(SyntaxKind.WhitespaceTrivia)
            private BlockSyntax WhitespaceKindCheckAlternate(IfStatementSyntax statement, SyntaxToken triviaIdentifierToken)
            {
                BlockSyntax emptyResult = null;

                var invocationExpr = statement.Condition as InvocationExpressionSyntax;
                if (invocationExpr == null)
                {
                    return emptyResult;
                }

                var memberExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
                if (memberExpr == null)
                {
                    return emptyResult;
                }

                var triviaIdentifier = memberExpr.Expression as IdentifierNameSyntax;
                if (triviaIdentifier == null || triviaIdentifier.Identifier.Text != triviaIdentifierToken.ValueText)
                {
                    return emptyResult;
                }

                var isKindIdentifier = memberExpr.Name as IdentifierNameSyntax;
                if (isKindIdentifier == null || isKindIdentifier.Identifier.Text != "IsKind")
                {
                    return emptyResult;
                }

                ArgumentListSyntax argList = invocationExpr.ArgumentList;
                if (argList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argList.Arguments;
                if (args == null || args.Count != 1)
                {
                    return emptyResult;
                }

                ArgumentSyntax whitespaceArg = args[0];
                if (whitespaceArg == null)
                {
                    return emptyResult;
                }

                var whitespaceExpr = whitespaceArg.Expression as MemberAccessExpressionSyntax;
                if (whitespaceExpr == null)
                {
                    return emptyResult;
                }

                var syntaxKindIdentifier = whitespaceExpr.Expression as IdentifierNameSyntax;
                if (syntaxKindIdentifier == null || syntaxKindIdentifier.Identifier.Text != "SyntaxKind")
                {
                    return emptyResult;
                }

                var whitespaceIdentifier = whitespaceExpr.Name as IdentifierNameSyntax;
                if (whitespaceIdentifier == null || whitespaceIdentifier.Identifier.Text != "WhitespaceTrivia")
                {
                    return emptyResult;
                }

                var blockResult = statement.Statement as BlockSyntax;
                if (blockResult == null)
                {
                    return emptyResult;
                }

                return blockResult;
            }

            // Checks step six of the user's AnalyzerIfStatement method, returns null if analysis failed
            private BlockSyntax IfStatementAnalysis6(SyntaxList<StatementSyntax> statements, SyntaxToken triviaIdentifierToken)
            {
                BlockSyntax emptyResult = null;

                var statement = statements[0] as IfStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                var booleanExpression = statement.Condition as BinaryExpressionSyntax;
                if (booleanExpression == null)
                {
                    return emptyResult;
                }

                var left = booleanExpression.Left as InvocationExpressionSyntax;
                if (left == null)
                {
                    return emptyResult;
                }

                var leftMemberExpr = left.Expression as MemberAccessExpressionSyntax;
                if (leftMemberExpr == null)
                {
                    return emptyResult;
                }

                var leftIdentifier = leftMemberExpr.Expression as IdentifierNameSyntax;
                if (leftIdentifier == null || leftIdentifier.Identifier.Text != triviaIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var leftName = leftMemberExpr.Name as IdentifierNameSyntax;
                if (leftName == null || leftName.Identifier.Text != "ToString")
                {
                    return emptyResult;
                }

                var leftArgumentList = left.ArgumentList as ArgumentListSyntax;
                if (leftArgumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> leftArgs = leftArgumentList.Arguments;
                if (leftArgs == null || leftArgs.Count != 0)
                {
                    return emptyResult;
                }

                var right = booleanExpression.Right as LiteralExpressionSyntax;
                if (right == null)
                {
                    return emptyResult;
                }

                SyntaxToken rightToken = right.Token;
                if (rightToken == null || rightToken.Text != "\" \"")
                {
                    return emptyResult;
                }

                var block = statement.Statement as BlockSyntax;
                if (block == null)
                {
                    return emptyResult;
                }

                return block;
            }

            // Checks step seven of the user's AnalyzerIfStatement method, returns a bool representing whether or not analysis failed
            private bool IfStatementAnalysis7(SyntaxList<StatementSyntax> statements)
            {
                var statement = statements[0] as ReturnStatementSyntax;
                if (statement == null)
                {
                    return false;
                }

                return true;
            }

            // Checks the count if-statement of user's AnalyzeIfStatement method, returns the statements within that if-statement if correct
            private BlockSyntax IfStatementAnalysis8(SyntaxList<StatementSyntax> statements, SyntaxToken triviaIdentifierToken)
            {
                BlockSyntax emptyResult = null;

                var statement = statements[0] as IfStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                var booleanExpression = statement.Condition as BinaryExpressionSyntax;
                if (booleanExpression == null)
                {
                    return emptyResult;
                }

                var left = booleanExpression.Left as MemberAccessExpressionSyntax;
                if (left == null)
                {
                    return null;
                }

                var leftExpression = left.Expression as MemberAccessExpressionSyntax;
                if (leftExpression == null)
                {
                    return emptyResult;
                }

                var leftExpressionIdentifier = leftExpression.Expression as IdentifierNameSyntax;
                if (leftExpressionIdentifier == null || leftExpressionIdentifier.Identifier.ValueText != triviaIdentifierToken.ValueText)
                {
                    return emptyResult;
                }

                var leftExpressionName = leftExpression.Name as IdentifierNameSyntax;
                if (leftExpressionName == null || leftExpressionName.Identifier.ValueText != "TrailingTrivia")
                {
                    return emptyResult;
                }

                var leftName = left.Name as IdentifierNameSyntax;
                if (leftName == null || leftName.Identifier.ValueText != "Count")
                {
                    return emptyResult;
                }

                var right = booleanExpression.Right as LiteralExpressionSyntax;
                if (right == null)
                {
                    return emptyResult;
                }

                if (!right.IsKind(SyntaxKind.NumericLiteralExpression))
                {
                    return emptyResult;
                }

                SyntaxToken rightToken = right.Token;
                if (rightToken == null || rightToken.ValueText != "1")
                {
                    return emptyResult;
                }

                var block = statement.Statement as BlockSyntax;
                if (block == null)
                {
                    return emptyResult;
                }

                return block;
            }

            // Checks if the incorrect statement is an if-statement, reports the diagnostic
            private void IfDiagnostic(CompilationAnalysisContext context, StatementSyntax statement, DiagnosticDescriptor diagnostic, params string[] messageArgs)
            {
                if (statement.Kind() == SyntaxKind.IfStatement)
                {
                    var ifStatement = statement as IfStatementSyntax;
                    var startDiagnosticSpan = ifStatement.SpanStart;
                    var endDiagnosticSpan = ifStatement.CloseParenToken.SpanStart;
                    var diagnosticSpan = TextSpan.FromBounds(startDiagnosticSpan, endDiagnosticSpan);
                    var diagnosticLocation = Location.Create(ifStatement.SyntaxTree, diagnosticSpan);
                    ReportDiagnostic(context, diagnostic, diagnosticLocation, messageArgs);
                }
                else
                {
                    ReportDiagnostic(context, diagnostic, statement.GetLocation(), messageArgs);
                }
            }

            // Checks the buildup steps of creating a diagnostic, returns a bool representing whether or not analysis failed
            private bool CheckDiagnosticCreation(CompilationAnalysisContext context, SyntaxToken statementIdentifierToken, SyntaxToken keywordIdentifierToken, List<string> ruleNames, SyntaxList<StatementSyntax> statements, ParameterSyntax contextParameter)
            {
                int statementCount = statements.Count;

                SyntaxToken openParenToken = OpenParenAnalysis(statementIdentifierToken, statements);
                if (openParenToken.Text == "")
                {
                    IfDiagnostic(context, statements[3], OpenParenIncorrectRule, statementIdentifierToken.Text);
                    return false;
                }

                if (statementCount > 4)
                {
                    SyntaxToken startToken = StartAnalysis(keywordIdentifierToken, statements);
                    if (startToken.Text == "")
                    {
                        IfDiagnostic(context, statements[4], StartSpanIncorrectRule, keywordIdentifierToken.Text);
                        return false;
                    }

                    if (statementCount > 5)
                    {
                        SyntaxToken endToken = EndAnalysis(openParenToken, statements);
                        if (endToken.Text == "")
                        {
                            IfDiagnostic(context, statements[5], EndSpanIncorrectRule, openParenToken.Text);
                            return false;
                        }

                        if (statementCount > 6)
                        {
                            SyntaxToken spanToken = SpanAnalysis(startToken, endToken, statements);
                            if (spanToken.Text == "")
                            {
                                IfDiagnostic(context, statements[6], SpanIncorrectRule, startToken.Text, endToken.Text);
                                return false;
                            }

                            if (statementCount > 7)
                            {
                                SyntaxToken locationToken = LocationAnalysis(statementIdentifierToken, spanToken, statements);
                                if (locationToken.Text == "")
                                {
                                    IfDiagnostic(context, statements[7], LocationIncorrectRule, statementIdentifierToken.Text, spanToken.Text);
                                    return false;
                                }

                                if (statementCount > 8)
                                {
                                    SyntaxToken diagnosticToken = DiagnosticCreationCheck(ruleNames, locationToken, statements);
                                    if (diagnosticToken == null || diagnosticToken.Text == "")
                                    {
                                        IfDiagnostic(context, statements[8], DiagnosticIncorrectRule, ruleNames[0], locationToken.Text);
                                        return false;
                                    }

                                    if (statementCount > 9)
                                    {
                                        bool reportCorrect = DiagnosticReportCheck(context, diagnosticToken, contextParameter, statements);
                                        if (!reportCorrect)
                                        {
                                            IfDiagnostic(context, statements[9], DiagnosticReportIncorrectRule, contextParameter.Identifier.Text, diagnosticToken.Text);
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        ReportDiagnostic(context, DiagnosticReportMissingRule, statements[8].GetLocation(), contextParameter.Identifier.Text);
                                        return false;
                                    }
                                }
                                else
                                {
                                    ReportDiagnostic(context, DiagnosticMissingRule, statements[7].GetLocation());
                                    return false;
                                }
                            }
                            else
                            {
                                ReportDiagnostic(context, LocationMissingRule, statements[6].GetLocation(), statementIdentifierToken.Text, spanToken.Text);
                                return false;
                            }
                        }
                        else
                        {
                            ReportDiagnostic(context, SpanMissingRule, statements[5].GetLocation(), startToken.Text, endToken.Text);
                            return false;
                        }
                    }
                    else
                    {
                        ReportDiagnostic(context, EndSpanMissingRule, statements[4].GetLocation(), openParenToken.Text);
                        return false;
                    }
                }
                else
                {
                    ReportDiagnostic(context, StartSpanMissingRule, statements[3].GetLocation(), keywordIdentifierToken.Text);
                    return false;
                }

                return true;
            }

            // Checks the open parenthesis variable, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken OpenParenAnalysis(SyntaxToken statementIdentifierToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[3] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken openParenIdentifier = GetIdentifierTokenFromLocalDecl(statement);
                if (openParenIdentifier == null || openParenIdentifier.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var memberExpression = equalsValueClause.Value as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != statementIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "OpenParenToken")
                {
                    return emptyResult;
                }

                return openParenIdentifier;
            }

            // Checks the start of the diagnostic span, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken StartAnalysis(SyntaxToken keywordIdentifierToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[4] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken startToken = GetIdentifierTokenFromLocalDecl(statement);
                if (startToken == null || startToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var memberExpression = equalsValueClause.Value as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifierExpressionName = memberExpression.Expression as IdentifierNameSyntax;
                var memberExpressionName = memberExpression.Expression as MemberAccessExpressionSyntax;
                if (identifierExpressionName == null && memberExpressionName == null)
                {
                    return emptyResult;
                }

                if (identifierExpressionName != null)
                {
                    if (identifierExpressionName.Identifier.Text != keywordIdentifierToken.Text)
                    {
                        return emptyResult;
                    }

                    var expressionMember = memberExpression.Name as IdentifierNameSyntax;
                    if (expressionMember == null || expressionMember.Identifier.Text != "SpanStart")
                    {
                        return emptyResult;
                    }
                }

                if (memberExpressionName != null)
                {
                    var identifierExpression = memberExpressionName.Expression as IdentifierNameSyntax;
                    if (identifierExpression == null || identifierExpression.Identifier.Text != keywordIdentifierToken.Text)
                    {
                        return emptyResult;
                    }

                    var memberName = memberExpressionName.Name as IdentifierNameSyntax;
                    if (memberName == null || memberName.Identifier.Text != "Span")
                    {
                        return emptyResult;
                    }

                    var finalName = memberExpression.Name as IdentifierNameSyntax;
                    if (finalName == null || finalName.Identifier.Text != "Start")
                    {
                        return emptyResult;
                    }
                }

                return startToken;
            }

            // Checks the end of the diagnostic span, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken EndAnalysis(SyntaxToken openParenToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[5] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken endToken = GetIdentifierTokenFromLocalDecl(statement);
                if (endToken == null || endToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var memberExpression = equalsValueClause.Value as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifierExpressionName = memberExpression.Expression as IdentifierNameSyntax;
                var memberExpressionName = memberExpression.Expression as MemberAccessExpressionSyntax;
                if (identifierExpressionName == null && memberExpressionName == null)
                {
                    return emptyResult;
                }

                if (identifierExpressionName != null)
                {
                    if (identifierExpressionName.Identifier.Text != openParenToken.Text)
                    {
                        return emptyResult;
                    }

                    var expressionMember = memberExpression.Name as IdentifierNameSyntax;
                    if (expressionMember == null || expressionMember.Identifier.Text != "SpanStart")
                    {
                        return emptyResult;
                    }
                }

                if (memberExpressionName != null)
                {
                    var identifierExpression = memberExpressionName.Expression as IdentifierNameSyntax;
                    if (identifierExpression == null || identifierExpression.Identifier.Text != openParenToken.Text)
                    {
                        return emptyResult;
                    }

                    var memberName = memberExpressionName.Name as IdentifierNameSyntax;
                    if (memberName == null || memberName.Identifier.Text != "Span")
                    {
                        return emptyResult;
                    }

                    var finalName = memberExpression.Name as IdentifierNameSyntax;
                    if (finalName == null || finalName.Identifier.Text != "Start")
                    {
                        return emptyResult;
                    }
                }

                return endToken;
            }

            // Checks the creation of the diagnostic span, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken SpanAnalysis(SyntaxToken startToken, SyntaxToken endToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[6] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken spanToken = GetIdentifierTokenFromLocalDecl(statement);
                if (spanToken == null || spanToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var invocationExpression = equalsValueClause.Value as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return emptyResult;
                }

                var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != "TextSpan")
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "FromBounds")
                {
                    return emptyResult;
                }

                var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
                if (argumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
                if (args == null || args.Count != 2)
                {
                    return emptyResult;
                }

                var startArg = args[0] as ArgumentSyntax;
                if (startArg == null)
                {
                    return emptyResult;
                }

                var startArgIdentifier = startArg.Expression as IdentifierNameSyntax;
                if (startArgIdentifier == null || startArgIdentifier.Identifier.Text != startToken.Text)
                {
                    return emptyResult;
                }

                var endArg = args[1] as ArgumentSyntax;
                if (endArg == null)
                {
                    return emptyResult;
                }

                var endArgIdentifier = endArg.Expression as IdentifierNameSyntax;
                if (endArgIdentifier == null || endArgIdentifier.Identifier.Text != endToken.Text)
                {
                    return emptyResult;
                }

                return spanToken;
            }

            // Checks the creation of the diagnostics location, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken LocationAnalysis(SyntaxToken statementIdentifierToken, SyntaxToken spanToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[7] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken locationToken = GetIdentifierTokenFromLocalDecl(statement);
                if (locationToken == null || locationToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var invocationExpression = equalsValueClause.Value as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return emptyResult;
                }

                var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != "Location")
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "Create")
                {
                    return emptyResult;
                }

                var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
                if (argumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
                if (args == null || args.Count != 2)
                {
                    return emptyResult;
                }

                var treeArg = args[0] as ArgumentSyntax;
                if (treeArg == null)
                {
                    return emptyResult;
                }

                var treeArgExpression = treeArg.Expression as MemberAccessExpressionSyntax;
                if (treeArgExpression == null)
                {
                    return emptyResult;
                }

                var treeIdentifier = treeArgExpression.Expression as IdentifierNameSyntax;
                if (treeIdentifier == null || treeIdentifier.Identifier.Text != statementIdentifierToken.Text)
                {
                    return emptyResult;
                }

                var treeName = treeArgExpression.Name as IdentifierNameSyntax;
                if (treeName == null || treeName.Identifier.Text != "SyntaxTree")
                {
                    return emptyResult;
                }

                var spanArg = args[1] as ArgumentSyntax;
                if (spanArg == null)
                {
                    return emptyResult;
                }

                var spanArgIdentifier = spanArg.Expression as IdentifierNameSyntax;
                if (spanArgIdentifier == null || spanArgIdentifier.Identifier.Text != spanToken.Text)
                {
                    return emptyResult;
                }

                return locationToken;
            }

            // Checks the creation of the diagnostic itself, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken DiagnosticCreationCheck(List<string> ruleNames, SyntaxToken locationToken, SyntaxList<StatementSyntax> statements)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                var statement = statements[8] as LocalDeclarationStatementSyntax;
                if (statement == null)
                {
                    return emptyResult;
                }

                SyntaxToken diagnosticToken = GetIdentifierTokenFromLocalDecl(statement);
                if (locationToken == null || locationToken.Text == "")
                {
                    return emptyResult;
                }

                EqualsValueClauseSyntax equalsValueClause = GetEqualsValueClauseFromLocalDecl(statement);
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                var invocationExpression = equalsValueClause.Value as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return emptyResult;
                }

                var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return emptyResult;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != "Diagnostic")
                {
                    return emptyResult;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "Create")
                {
                    return emptyResult;
                }

                var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
                if (argumentList == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
                if (args == null || args.Count < 2)
                {
                    return emptyResult;
                }

                var ruleArg = args[0] as ArgumentSyntax;
                if (ruleArg == null)
                {
                    return emptyResult;
                }

                var ruleArgIdentifier = ruleArg.Expression as IdentifierNameSyntax;
                if (ruleArgIdentifier == null || !ruleNames.Contains(ruleArgIdentifier.Identifier.Text))
                {
                    return emptyResult;
                }

                var locationArg = args[1] as ArgumentSyntax;
                if (locationArg == null)
                {
                    return emptyResult;
                }

                var locationArgIdentifier = locationArg.Expression as IdentifierNameSyntax;
                if (locationArgIdentifier == null || locationArgIdentifier.Identifier.Text != locationToken.Text)
                {
                    return emptyResult;
                }

                return diagnosticToken;
            }

            // Checks the reporting of the diagnostic, returns a bool representing whether or not analysis failed
            private bool DiagnosticReportCheck(CompilationAnalysisContext context, SyntaxToken diagnosticToken, ParameterSyntax contextParameter, SyntaxList<StatementSyntax> statements)
            {
                var statement = statements[9] as ExpressionStatementSyntax;
                if (statement == null)
                {
                    return false;
                }

                var invocationExpression = statement.Expression as InvocationExpressionSyntax;
                if (invocationExpression == null)
                {
                    return false;
                }

                var memberExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberExpression == null)
                {
                    return false;
                }

                var identifier = memberExpression.Expression as IdentifierNameSyntax;
                if (identifier == null || identifier.Identifier.Text != contextParameter.Identifier.Text)
                {
                    return false;
                }

                var name = memberExpression.Name as IdentifierNameSyntax;
                if (name == null || name.Identifier.Text != "ReportDiagnostic")
                {
                    return false;
                }

                var argumentList = invocationExpression.ArgumentList as ArgumentListSyntax;
                if (argumentList == null)
                {
                    return false;
                }

                SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;
                if (args == null || args.Count != 1)
                {
                    return false;
                }

                var diagnosticArg = args[0] as ArgumentSyntax;
                if (diagnosticArg == null)
                {
                    return false;
                }

                var diagnosticArgIdentifier = diagnosticArg.Expression as IdentifierNameSyntax;
                if (diagnosticArgIdentifier == null || diagnosticArgIdentifier.Identifier.Text != diagnosticToken.Text)
                {
                    return false;
                }

                return true;
            }
            #endregion

            // Extracts the equals value clause from a local declaration statement, returns null if failed
            private EqualsValueClauseSyntax GetEqualsValueClauseFromLocalDecl(LocalDeclarationStatementSyntax statement)
            {
                EqualsValueClauseSyntax emptyResult = null;

                if (statement == null)
                {
                    return emptyResult;
                }

                var variableDeclaration = statement.Declaration as VariableDeclarationSyntax;
                if (variableDeclaration == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;
                if (variables == null || variables.Count != 1)
                {
                    return emptyResult;
                }

                var variableDeclarator = variables[0] as VariableDeclaratorSyntax;
                if (variableDeclarator == null)
                {
                    return emptyResult;
                }

                SyntaxToken identifier = variableDeclarator.Identifier;
                if (identifier == null)
                {
                    return emptyResult;
                }

                var equalsValueClause = variableDeclarator.Initializer as EqualsValueClauseSyntax;
                if (equalsValueClause == null)
                {
                    return emptyResult;
                }

                return equalsValueClause;
            }

            // Extracts the name of the variable from a local declaration statement, returns a SyntaxToken of "" if analysis failed
            private SyntaxToken GetIdentifierTokenFromLocalDecl(LocalDeclarationStatementSyntax statement)
            {
                var emptyResult = SyntaxFactory.Identifier("");

                if (statement == null)
                {
                    return emptyResult;
                }

                var variableDeclaration = statement.Declaration as VariableDeclarationSyntax;
                if (variableDeclaration == null)
                {
                    return emptyResult;
                }

                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;
                if (variables == null || variables.Count != 1)
                {
                    return emptyResult;
                }

                var variableDeclarator = variables[0] as VariableDeclaratorSyntax;
                if (variableDeclarator == null)
                {
                    return emptyResult;
                }

                SyntaxToken identifier = variableDeclarator.Identifier;
                if (identifier == null)
                {
                    return emptyResult;
                }

                return identifier;
            }

            // Returns a list containing the method declaration, and the statements within the method, returns an empty list if failed
            private MethodDeclarationSyntax AnalysisGetStatements(IMethodSymbol analysisMethodSymbol)
            {
                MethodDeclarationSyntax result = null;

                if (analysisMethodSymbol == null)
                {
                    return result;
                }

                var methodDeclaration = analysisMethodSymbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                if (methodDeclaration == null)
                {
                    return result;
                }

                return methodDeclaration;
            }

            // Returns a boolean based on whether or not the SupportedDiagnostics property is correct
            private bool CheckSupportedDiagnostics(List<string> ruleNames, CompilationAnalysisContext context)
            {
                var propertyDeclaration = SuppDiagPropertySymbol(context);
                if (propertyDeclaration == null)
                {
                    return false;
                }

                BlockSyntax body = SuppDiagAccessor(context, propertyDeclaration);
                if (body == null)
                {
                    return false;
                }

                SyntaxList<StatementSyntax> statements = body.Statements;
                if (statements == null || statements.Count == 0)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, propertyDeclaration.Identifier.GetLocation());
                    return false;
                }

                if (statements.Count > 2)
                {
                    AccessorListSyntax propertyAccessorList = propertyDeclaration.AccessorList as AccessorListSyntax;
                    ReportDiagnostic(context, TooManyStatementsRule, propertyAccessorList.Accessors[0].Keyword.GetLocation(), "get accessor", "1 or 2", "create and return an ImmutableArray containing all DiagnosticDescriptors");
                    return false;
                }

                var getAccessorKeywordLocation = propertyDeclaration.AccessorList.Accessors.First().Keyword.GetLocation();

                var throwStatement = statements.First() as ThrowStatementSyntax;

                if (throwStatement != null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation);
                    return false;
                }

                IEnumerable<ReturnStatementSyntax> returnStatements = statements.OfType<ReturnStatementSyntax>();
                if (returnStatements.Count() == 0)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation);
                    return false;
                }

                ReturnStatementSyntax returnStatement = returnStatements.First();
                if (returnStatement == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation);
                    return false;
                }

                var returnExpression = returnStatement.Expression;
                if (returnExpression == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnStatement.GetLocation());
                    return false;
                }

                if (returnExpression is InvocationExpressionSyntax)
                {
                    var valueClause = returnExpression as InvocationExpressionSyntax;
                    var returnDeclaration = returnStatement as ReturnStatementSyntax;
                    var suppDiagReturnCheck = SuppDiagReturnCheck(context, valueClause, returnDeclaration, ruleNames, propertyDeclaration);
                    if (!suppDiagReturnCheck)
                    {
                        return false;
                    }

                    if (statements.Count > 1)
                    {
                        AccessorListSyntax propertyAccessorList = propertyDeclaration.AccessorList as AccessorListSyntax;
                        ReportDiagnostic(context, TooManyStatementsRule, propertyAccessorList.Accessors[0].Keyword.GetLocation(), "get accessor", "1 or 2", "create and return an ImmutableArray containing all DiagnosticDescriptors");
                        return false;
                    }
                }
                else if (returnExpression is IdentifierNameSyntax)
                {
                    SymbolInfo returnSymbolInfo = context.Compilation.GetSemanticModel(returnStatement.SyntaxTree).GetSymbolInfo(returnExpression as IdentifierNameSyntax);
                    SuppDiagReturnSymbolInfo symbolResult = SuppDiagReturnSymbol(context, returnSymbolInfo, getAccessorKeywordLocation, propertyDeclaration);

                    if (symbolResult == null)
                    {
                        return false;
                    }

                    if (symbolResult.ValueClause == null && symbolResult.ReturnDeclaration == null)
                    {
                        return false;
                    }

                    InvocationExpressionSyntax valueClause = symbolResult.ValueClause as InvocationExpressionSyntax;
                    ReturnStatementSyntax returnDeclaration = symbolResult.ReturnDeclaration as ReturnStatementSyntax;
                    var suppDiagReturnCheck = SuppDiagReturnCheck(context, valueClause, returnDeclaration, ruleNames, propertyDeclaration);
                    if (!suppDiagReturnCheck)
                    {
                        return false;
                    }
                }
                else
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnStatement.GetLocation());
                    return false;
                }

                return true;
            }

            #region CheckSupportedDiagnostics helpers
            // Returns the property declaration, null if the property symbol is incorrect
            private PropertyDeclarationSyntax SuppDiagPropertySymbol(CompilationAnalysisContext context)
            {
                if (_propertySymbol == null)
                {
                    ReportDiagnostic(context, MissingSuppDiagRule, _analyzerClassSymbol.Locations[0]);
                    return null;
                }

                if (_propertySymbol.Name != "SupportedDiagnostics" || _propertySymbol.DeclaredAccessibility != Accessibility.Public || !_propertySymbol.IsOverride)
                {
                    ReportDiagnostic(context, IncorrectSigSuppDiagRule, _propertySymbol.Locations[0]);
                    return null;
                }

                return _propertySymbol.DeclaringSyntaxReferences[0].GetSyntax() as PropertyDeclarationSyntax;
            }

            // Returns the statements of the get accessor, empty list if get accessor not found/incorrect
            private BlockSyntax SuppDiagAccessor(CompilationAnalysisContext context, PropertyDeclarationSyntax propertyDeclaration)
            {
                AccessorListSyntax accessorList = propertyDeclaration.AccessorList;
                if (accessorList == null)
                {
                    return null;
                }

                SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;
                if (accessors == null || accessors.Count == 0)
                {
                    ReportDiagnostic(context, MissingAccessorRule, propertyDeclaration.Identifier.GetLocation(), propertyDeclaration.Identifier.Text);
                    return null;
                }

                if (accessors.Count > 1)
                {
                    ReportDiagnostic(context, TooManyAccessorsRule, accessorList.Accessors[1].Keyword.GetLocation(), propertyDeclaration.Identifier.Text);
                    return null;
                }

                AccessorDeclarationSyntax getAccessor = null;
                foreach (AccessorDeclarationSyntax accessor in accessors)
                {
                    if (accessor.Keyword.IsKind(SyntaxKind.GetKeyword))
                    {
                        getAccessor = accessor;
                        break;
                    }
                }

                if (getAccessor == null || getAccessor.Keyword.Kind() != SyntaxKind.GetKeyword)
                {
                    ReportDiagnostic(context, MissingAccessorRule, propertyDeclaration.Identifier.GetLocation(), propertyDeclaration.Identifier.Text);
                    return null;
                }

                var accessorBody = getAccessor.Body as BlockSyntax;
                if (accessorBody == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessor.Keyword.GetLocation());
                    return null;
                }

                return accessorBody;
            }

            // Checks the return value of the get accessor within SupportedDiagnostics
            private bool SuppDiagReturnCheck(CompilationAnalysisContext context, InvocationExpressionSyntax valueClause, ReturnStatementSyntax returnDeclarationLocation, List<string> ruleNames, PropertyDeclarationSyntax propertyDeclaration)
            {
                if (valueClause == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnDeclarationLocation.ReturnKeyword.GetLocation());
                    return false;
                }

                var valueExpression = valueClause.Expression as MemberAccessExpressionSyntax;
                if (valueExpression == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnDeclarationLocation.ReturnKeyword.GetLocation());
                    return false;
                }
                
                var valueExprExpr = valueExpression.Expression as IdentifierNameSyntax;
                var valueExprName = valueExpression.Name as IdentifierNameSyntax;

                if (valueExprExpr == null || valueExprExpr.Identifier.Text != "ImmutableArray")
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, valueExpression.GetLocation(), propertyDeclaration.Identifier.Text);
                    return false;
                }

                if (valueExprName == null || valueExprName.Identifier.Text != "Create")
                {
                    ReportDiagnostic(context, SuppDiagReturnValueRule, valueExpression.GetLocation(), propertyDeclaration.Identifier.Text);
                    return false;
                }
                
                var valueArguments = valueClause.ArgumentList as ArgumentListSyntax;
                if (valueArguments == null)
                {
                    ReportDiagnostic(context, SupportedRulesRule, valueExpression.GetLocation(), propertyDeclaration.Identifier.Text);
                    return false;
                }

                SeparatedSyntaxList<ArgumentSyntax> valueArgs = valueArguments.Arguments;
                if (valueArgs.Count == 0)
                {
                    ReportDiagnostic(context, SupportedRulesRule, valueExpression.GetLocation());
                    return false;
                }

                if (ruleNames.Count != valueArgs.Count)
                {
                    ReportDiagnostic(context, SupportedRulesRule, valueExpression.GetLocation());
                    return false;
                }

                List<string> newRuleNames = new List<string>();
                foreach (string rule in ruleNames)
                {
                    newRuleNames.Add(rule);
                }

                foreach (ArgumentSyntax arg in valueArgs)
                {

                    bool foundRule = false;
                    foreach (string ruleName in ruleNames)
                    {
                        var argExpression = arg.Expression as IdentifierNameSyntax;
                        if (argExpression != null)
                        {
                            if (argExpression.Identifier.Text == ruleName)
                            {
                                foundRule = true;
                            }
                        }
                    }
                    if (!foundRule)
                    {
                        ReportDiagnostic(context, SupportedRulesRule, valueExpression.GetLocation());
                        return false;
                    }
                }
                return true;
            }

            //Returns the valueClause of the return statement from SupportedDiagnostics and the return declaration, empty list if failed
            private SuppDiagReturnSymbolInfo SuppDiagReturnSymbol(CompilationAnalysisContext context, SymbolInfo returnSymbolInfo, Location getAccessorKeywordLocation, PropertyDeclarationSyntax propertyDeclaration)
            {
                SuppDiagReturnSymbolInfo result = new SuppDiagReturnSymbolInfo();

                ILocalSymbol returnSymbol = null;
                if (returnSymbolInfo.CandidateSymbols.Count() == 0)
                {
                    returnSymbol = returnSymbolInfo.Symbol as ILocalSymbol;
                }
                else
                {
                    returnSymbol = returnSymbolInfo.CandidateSymbols[0] as ILocalSymbol;
                }

                if (returnSymbol == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, getAccessorKeywordLocation);
                    return result;
                }

                var variableDeclaration = returnSymbol.DeclaringSyntaxReferences[0].GetSyntax() as VariableDeclaratorSyntax;
                ReturnStatementSyntax returnDeclaration = returnSymbol.DeclaringSyntaxReferences[0].GetSyntax() as ReturnStatementSyntax;
                if (variableDeclaration == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, returnSymbol.Locations[0]);
                    return result;
                }

                var equalsValueClause = variableDeclaration.Initializer as EqualsValueClauseSyntax;
                if (equalsValueClause == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, variableDeclaration.GetLocation());
                    return result;
                }

                var valueClause = equalsValueClause.Value as InvocationExpressionSyntax;
                if (valueClause == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, variableDeclaration.GetLocation());
                    return result;
                }

                var valueClauseMemberAccess = valueClause.Expression as MemberAccessExpressionSyntax;
                if (valueClauseMemberAccess == null)
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, valueClause.GetLocation());
                    return result;
                }

                var valueClauseExpression = valueClauseMemberAccess.Expression as IdentifierNameSyntax;
                if (valueClauseExpression == null || valueClauseExpression.Identifier.Text != "ImmutableArray")
                {
                    ReportDiagnostic(context, IncorrectAccessorReturnRule, valueClause.GetLocation());
                    return result;
                }

                var valueClauseName = valueClauseMemberAccess.Name as IdentifierNameSyntax;
                if (valueClauseName == null || valueClauseName.Identifier.Text != "Create")
                {
                    ReportDiagnostic(context, SuppDiagReturnValueRule, valueClauseName.GetLocation(), propertyDeclaration.Identifier.Text);
                    return result;
                }

                result.ValueClause = valueClause;
                result.ReturnDeclaration = returnDeclaration;

                return result;
            }
            #endregion

            // Returns a list of rule names
            private List<string> CheckRules(List<string> idNames, CompilationAnalysisContext context)
            {
                List<string> ruleNames = new List<string>();
                List<string> emptyRuleNames = new List<string>();
                bool foundARule = false;

                foreach (IFieldSymbol fieldSymbol in _analyzerFieldSymbols)
                {
                    if (fieldSymbol.Type != null && fieldSymbol.Type.MetadataName == "DiagnosticDescriptor")
                    {
                        foundARule = true;

                        if (fieldSymbol.DeclaredAccessibility != Accessibility.Internal || !fieldSymbol.IsStatic)
                        {
                            ReportDiagnostic(context, InternalAndStaticErrorRule, fieldSymbol.Locations[0], fieldSymbol.Name);
                            return emptyRuleNames;
                        }

                        var declaratorSyntax = fieldSymbol.DeclaringSyntaxReferences[0].GetSyntax() as VariableDeclaratorSyntax;
                        if (declaratorSyntax == null)
                        {
                            return emptyRuleNames;
                        }

                        var initializer = declaratorSyntax.Initializer as EqualsValueClauseSyntax;
                        if (initializer == null)
                        {
                            return emptyRuleNames;
                        }

                        var objectCreationSyntax = initializer.Value as ObjectCreationExpressionSyntax;
                        if (objectCreationSyntax == null)
                        {
                            return emptyRuleNames;
                        }

                        var ruleArgumentList = objectCreationSyntax.ArgumentList;

                        for (int i = 0; i < ruleArgumentList.Arguments.Count; i++)
                        {
                            var currentArg = ruleArgumentList.Arguments[i];
                            if (currentArg == null)
                            {
                                return emptyRuleNames;
                            }

                            if (currentArg.NameColon != null)
                            {
                                string currentArgName = currentArg.NameColon.Name.Identifier.Text;
                                var currentArgExpr = currentArg.Expression;
                                var currentArgExprIdentifier = currentArgExpr as IdentifierNameSyntax;

                                if (currentArgName == "isEnabledByDefault")
                                {
                                    if (currentArgExprIdentifier != null)
                                    {
                                        if (currentArgExprIdentifier.Identifier.Text == "")
                                        {
                                            ReportDiagnostic(context, EnabledByDefaultErrorRule, currentArg.GetLocation());
                                            return emptyRuleNames;
                                        }
                                    }

                                    if (!currentArgExpr.IsKind(SyntaxKind.TrueLiteralExpression))
                                    {
                                        ReportDiagnostic(context, EnabledByDefaultErrorRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }
                                }
                                else if (currentArgName == "defaultSeverity")
                                {
                                    if (currentArgExprIdentifier != null)
                                    {
                                        if (currentArgExprIdentifier.Identifier.Text == "")
                                        {
                                            ReportDiagnostic(context, DefaultSeverityErrorRule, currentArg.GetLocation());
                                            return emptyRuleNames;
                                        }
                                    }

                                    var memberAccessExpr = currentArgExpr as MemberAccessExpressionSyntax;
                                    if (memberAccessExpr == null)
                                    {
                                        ReportDiagnostic(context, DefaultSeverityErrorRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }

                                    var expressionIdentifier = memberAccessExpr.Expression as IdentifierNameSyntax;
                                    if (expressionIdentifier == null)
                                    {
                                        ReportDiagnostic(context, DefaultSeverityErrorRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }

                                    string expressionText = expressionIdentifier.Identifier.Text;

                                    var expressionName = memberAccessExpr.Name as IdentifierNameSyntax;
                                    if (expressionName == null)
                                    {
                                        ReportDiagnostic(context, DefaultSeverityErrorRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }

                                    string identifierName = memberAccessExpr.Name.Identifier.Text;

                                    List<string> severities = new List<string> { "Warning", "Error" };

                                    if (expressionText != "DiagnosticSeverity")
                                    {
                                        ReportDiagnostic(context, DefaultSeverityErrorRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }
                                    else if (!severities.Contains(identifierName))
                                    {
                                        ReportDiagnostic(context, DefaultSeverityErrorRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }
                                }
                                else if (currentArgName == "id")
                                {
                                    if (currentArgExprIdentifier != null)
                                    {
                                        if (currentArgExprIdentifier.Identifier.Text == "")
                                        {
                                            ReportDiagnostic(context, IdDeclTypeErrorRule, Location.Create(currentArg.SyntaxTree, currentArg.NameColon.ColonToken.TrailingTrivia.FullSpan));
                                            return emptyRuleNames;
                                        }
                                    }


                                    if (currentArgExpr.IsKind(SyntaxKind.StringLiteralExpression))
                                    {
                                        ReportDiagnostic(context, IdStringLiteralRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }

                                    if (!currentArgExpr.IsKind(SyntaxKind.IdentifierName))
                                    {
                                        ReportDiagnostic(context, IdDeclTypeErrorRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }

                                    var ruleName = fieldSymbol.Name as string;
                                    if (ruleName == null)
                                    {
                                        return emptyRuleNames;
                                    }

                                    bool ruleIdFound = false;

                                    foreach (string idName in idNames)
                                    {
                                        if (idName == currentArgExprIdentifier.Identifier.Text)
                                        {
                                            ruleNames.Add(ruleName);
                                            ruleIdFound = true;
                                        }
                                    }

                                    if (!ruleIdFound)
                                    {
                                        ReportDiagnostic(context, MissingIdDeclarationRule, currentArgExpr.GetLocation());
                                        return emptyRuleNames;
                                    }
                                }
                                else if (currentArgName == "title" || currentArgName == "messageFormat" || currentArgName == "category")
                                {
                                    Dictionary<string, string> argDefaults = new Dictionary<string, string>();
                                    argDefaults.Add("title", "Enter a title for this diagnostic");
                                    argDefaults.Add("messageFormat", "Enter a message to be displayed with this diagnostic");
                                    argDefaults.Add("category", "Enter a category for this diagnostic (e.g. Formatting)");

                                    if (currentArgExpr.IsKind(SyntaxKind.StringLiteralExpression))
                                    {
                                        if ((currentArgExpr as LiteralExpressionSyntax).Token.ValueText == argDefaults[currentArgName])
                                        {
                                            if (currentArgName == "title")
                                            {
                                                ReportDiagnostic(context, TitleRule, currentArgExpr.GetLocation());
                                                return emptyRuleNames;
                                            }
                                            else if (currentArgName == "messageFormat")
                                            {
                                                ReportDiagnostic(context, MessageRule, currentArgExpr.GetLocation());
                                                return emptyRuleNames;
                                            }
                                            else if (currentArgName == "category")
                                            {
                                                ReportDiagnostic(context, CategoryRule, currentArgExpr.GetLocation());
                                                return emptyRuleNames;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (ruleArgumentList.Arguments.Count != 6)
                        {
                            return emptyRuleNames;
                        }
                    }
                }

                if (foundARule)
                {
                    return ruleNames;
                }
                else
                {
                    var analyzerClass = _analyzerClassSymbol.DeclaringSyntaxReferences[0].GetSyntax() as ClassDeclarationSyntax;
                    Location idLocation = null;
                    foreach (IFieldSymbol field in _analyzerFieldSymbols)
                    {
                        if (idNames.Contains(field.Name))
                        {
                            var idField = field.DeclaringSyntaxReferences[0].GetSyntax() as VariableDeclaratorSyntax;
                            idLocation = idField.Identifier.GetLocation();
                            break;
                        }
                    }

                    ReportDiagnostic(context, MissingRuleRule, idLocation);
                    return emptyRuleNames;
                }
            }

            // Returns a list of id names, empty if none found
            private List<string> CheckIds()
            {
                List<string> idNames = new List<string>();
                foreach (IFieldSymbol field in _analyzerFieldSymbols)
                {
                    if (field.IsStatic && field.DeclaredAccessibility == Accessibility.Public && field.Type.SpecialType == SpecialType.System_String)
                    {
                        if (field.Name != null)
                        {
                            idNames.Add(field.Name);
                        }

                    }
                }

                return idNames;
            }

            // Returns true if the method called upon registering an action exists and is correct
            private bool CheckMethods(InvocationExpressionSyntax invocationExpression, CompilationAnalysisContext context)
            {
                IMethodSymbol analysisMethod = null;
                bool analysisMethodFound = false;

                var argList = invocationExpression.ArgumentList;
                var calledMethodName = argList.Arguments.First().Expression as IdentifierNameSyntax;

                foreach (IMethodSymbol currentMethod in _analyzerMethodSymbols)
                {

                    if (calledMethodName.Identifier.Text == currentMethod.Name)
                    {
                        analysisMethod = currentMethod;
                        analysisMethodFound = true;
                        break;
                    }
                }

                if (analysisMethodFound)
                {
                    MethodDeclarationSyntax analysisMethodSyntax = analysisMethod.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                    if (analysisMethodSyntax.Modifiers.Count == 0 || analysisMethodSyntax.Modifiers.First().Text != "private" || analysisMethod.DeclaredAccessibility != Accessibility.Private)
                    {
                        ReportDiagnostic(context, IncorrectAnalysisAccessibilityRule, analysisMethodSyntax.Identifier.GetLocation(), analysisMethodSyntax.Identifier.ValueText);
                        return false;
                    }
                    else if (analysisMethodSyntax.ReturnType.IsMissing || !analysisMethod.ReturnsVoid)
                    {
                        ReportDiagnostic(context, IncorrectAnalysisReturnTypeRule, analysisMethodSyntax.Identifier.GetLocation(), analysisMethodSyntax.Identifier.ValueText);
                        return false;
                    }
                    else if (analysisMethod.Parameters.Count() != 1 || analysisMethod.Parameters.First().Type != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext"))
                    {
                        ReportDiagnostic(context, IncorrectAnalysisParameterRule, analysisMethodSyntax.ParameterList.GetLocation(), analysisMethodSyntax.Identifier.ValueText);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    ReportDiagnostic(context, MissingAnalysisMethodRule, calledMethodName.GetLocation(), calledMethodName.Identifier.Text);
                    return false;
                }

            }

            // Returns a symbol for the register call, and a list of the arguments
            private CheckInitializeInfo CheckInitialize(CompilationAnalysisContext context)
            {
                //default values for returning
                IMethodSymbol registerCall = null;
                List<ISymbol> registerArgs = new List<ISymbol>();
                InvocationExpressionSyntax invocExpr = null;

                if (_initializeSymbol == null)
                {
                    //the initialize method was not found
                    ReportDiagnostic(context, MissingInitRule, _analyzerClassSymbol.Locations[0], _analyzerClassSymbol.Name);
                    return new CheckInitializeInfo();
                }
                else
                {
                    //checking method signature
                    var codeBlock = InitializeOverview(context) as BlockSyntax;
                    if (codeBlock == null)
                    {
                        return new CheckInitializeInfo();
                    }

                    SyntaxList<StatementSyntax> statements = codeBlock.Statements;
                    if (statements.Count == 0)
                    {
                        //no statements inside initiailize
                        ReportDiagnostic(context, MissingRegisterRule, _initializeSymbol.Locations[0], _initializeSymbol.Name);
                        return new CheckInitializeInfo();
                    }
                    else if (statements.Count > 1)
                    {
                        foreach (var statement in statements)
                        {
                            if (statement.Kind() != SyntaxKind.ExpressionStatement)
                            {
                                ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation());
                                return new CheckInitializeInfo();
                            }
                        }
                        foreach (ExpressionStatementSyntax statement in statements)
                        {
                            var expression = statement.Expression as InvocationExpressionSyntax;
                            if (expression == null)
                            {
                                ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation());
                                return new CheckInitializeInfo();
                            }

                            var expressionStart = expression.Expression as MemberAccessExpressionSyntax;
                            if (expressionStart == null || expressionStart.Name == null)
                            {
                                ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation());
                                return new CheckInitializeInfo();
                            }

                            var preExpressionStart = expressionStart.Expression as IdentifierNameSyntax;
                            if (preExpressionStart == null || preExpressionStart.Identifier == null || preExpressionStart.Identifier.ValueText != _initializeSymbol.Parameters.First().Name)
                            {
                                ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation());
                                return new CheckInitializeInfo();
                            }

                            var name = expressionStart.Name.Identifier.Text;
                            if (!_branchesDict.ContainsKey(name))
                            {
                                ReportDiagnostic(context, InvalidStatementRule, statement.GetLocation());
                                return new CheckInitializeInfo();
                            }
                        }

                        //too many statements inside initialize
                        ReportDiagnostic(context, TooManyInitStatementsRule, _initializeSymbol.Locations[0], _initializeSymbol.Name);
                        return new CheckInitializeInfo();
                    }
                    //only one statement inside initialize
                    else
                    {
                        InitializeBodyInfo bodyResults = InitializeBody(context, statements);
                        if (bodyResults == null)
                        {
                            return new CheckInitializeInfo();
                        }

                        var invocationExpr = bodyResults.InvocationExpr as InvocationExpressionSyntax;
                        var memberExpr = bodyResults.MemberExpr as MemberAccessExpressionSyntax;
                        invocExpr = invocationExpr;

                        if (context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(memberExpr).CandidateSymbols.Count() == 0)
                        {
                            registerCall = context.Compilation.GetSemanticModel(memberExpr.SyntaxTree).GetSymbolInfo(memberExpr).Symbol as IMethodSymbol;
                        }
                        else
                        {
                            registerCall = context.Compilation.GetSemanticModel(memberExpr.SyntaxTree).GetSymbolInfo(memberExpr).CandidateSymbols[0] as IMethodSymbol;
                        }

                        if (registerCall == null)
                        {
                            return new CheckInitializeInfo(registerCall, invocationExpr: invocExpr);
                        }

                        SeparatedSyntaxList<ArgumentSyntax> arguments = invocationExpr.ArgumentList.Arguments;
                        if (arguments == null || arguments.Count == 0)
                        {
                            ReportDiagnostic(context, IncorrectArgumentsRule, invocationExpr.Expression.GetLocation());
                            return new CheckInitializeInfo(registerCall, invocationExpr: invocExpr);
                        }

                        if (arguments.Count > 0)
                        {
                            IMethodSymbol actionSymbol = context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(arguments[0].Expression).Symbol as IMethodSymbol;
                            registerArgs.Add(actionSymbol);

                            if (arguments.Count > 1)
                            {
                                IFieldSymbol kindSymbol = context.Compilation.GetSemanticModel(invocationExpr.SyntaxTree).GetSymbolInfo(arguments[1].Expression).Symbol as IFieldSymbol;
                                if (kindSymbol == null)
                                {
                                    return new CheckInitializeInfo(registerCall, registerArgs, invocExpr);
                                }
                                else
                                {
                                    registerArgs.Add(kindSymbol);
                                }
                            }
                            else
                            {
                                ReportDiagnostic(context, IncorrectArgumentsRule, invocationExpr.Expression.GetLocation());
                            }
                        }
                    }
                }

                return new CheckInitializeInfo(registerCall, registerArgs, invocExpr);
            }

            #region CheckInitialize helpers
            // Checks the signature of initialize and returns the block of the method, null if failed
            private BlockSyntax InitializeOverview(CompilationAnalysisContext context)
            {
                ImmutableArray<IParameterSymbol> parameters = _initializeSymbol.Parameters;
                if (parameters.Count() != 1 || parameters[0].Type != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.AnalysisContext")
                    || _initializeSymbol.DeclaredAccessibility != Accessibility.Public || !_initializeSymbol.IsOverride || !_initializeSymbol.ReturnsVoid)
                {
                    ReportDiagnostic(context, IncorrectInitSigRule, _initializeSymbol.Locations[0], _initializeSymbol.Name);
                    return null;
                }

                //looking at the contents of the initialize method
                var initializeMethod = _initializeSymbol.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
                if (initializeMethod == null)
                {
                    return null;
                }

                var codeBlock = initializeMethod.Body as BlockSyntax;
                if (codeBlock == null)
                {
                    return null;
                }

                return codeBlock;
            }

            // Checks the body of initializer, returns the invocation expression and member expression of the register statements, null if failed
            private InitializeBodyInfo InitializeBody(CompilationAnalysisContext context, SyntaxList<StatementSyntax> statements)
            {
                var statement = statements[0] as ExpressionStatementSyntax;
                if (statement == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation());
                    return null;
                }

                var invocationExpr = statement.Expression as InvocationExpressionSyntax;
                if (invocationExpr == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation());
                    return null;
                }

                var memberExpr = invocationExpr.Expression as MemberAccessExpressionSyntax;
                if (memberExpr == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation());
                    return null;
                }

                var memberExprContext = memberExpr.Expression as IdentifierNameSyntax;
                if (memberExprContext == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation());
                    return null;
                }

                MethodDeclarationSyntax methodDeclaration = statement.Parent.Parent as MethodDeclarationSyntax;
                ParameterSyntax parameter = methodDeclaration.ParameterList.Parameters[0] as ParameterSyntax;
                if (memberExprContext.Identifier.Text != parameter.Identifier.ValueText)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation());
                    return null;
                }

                var memberExprRegister = memberExpr.Name as IdentifierNameSyntax;
                if (memberExprRegister == null)
                {
                    ReportDiagnostic(context, InvalidStatementRule, statements[0].GetLocation());
                    return null;
                }

                if (!_branchesDict.ContainsKey(memberExprRegister.Identifier.Text))
                {
                    ReportDiagnostic(context, IncorrectRegisterRule, memberExprRegister.GetLocation());
                    return null;
                }

                return new InitializeBodyInfo(invocationExpr, memberExpr);
            }
            #endregion

            #region symbol collectors
            // Stores a method in state
            internal protected void AddMethod(SymbolAnalysisContext context)
            {
                var sym = (IMethodSymbol)context.Symbol;

                if (sym == null)
                {
                    return;
                }

                if (sym.ContainingType == null)
                {
                    return;
                }

                if (sym.ContainingType.BaseType == null)
                {
                    return;
                }

                if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider"))
                    {
                        return;
                    }

                    if (sym.Name == "RegisterCodeFixesAsync")
                    {
                        _registerCodeFixesAsync = sym;
                        return;
                    }
                    else
                    {
                        _codeFixMethodSymbols.Add(sym);
                        return;
                    }
                }

                if (_analyzerMethodSymbols.Contains(sym))
                {
                    return;
                }

                if (sym.Name == "Initialize")
                {
                    _initializeSymbol = sym;
                    return;
                }

                _analyzerMethodSymbols.Add(sym);
            }

            // Stores a property in state
            internal protected void AddProperty(SymbolAnalysisContext context)
            {
                var sym = (IPropertySymbol)context.Symbol;

                if (sym == null)
                {
                    return;
                }

                if (sym.ContainingType == null)
                {
                    return;
                }

                if (sym.ContainingType.BaseType == null)
                {
                    return;
                }

                if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider"))
                    {
                        return;
                    }

                    if (sym.Name == "FixableDiagnosticIds")
                    {
                        _codeFixFixableDiagnostics = sym;
                        return;
                    }

                    return;
                }

                if (_analyzerPropertySymbols.Contains(sym))
                {
                    return;
                }

                if (sym.Name == "SupportedDiagnostics")
                {
                    _propertySymbol = sym;
                    return;
                }

                _analyzerPropertySymbols.Add(sym);
            }

            // Stores a field in state
            internal protected void AddField(SymbolAnalysisContext context)
            {
                var sym = (IFieldSymbol)context.Symbol;

                if (sym == null)
                {
                    return;
                }

                if (sym.ContainingType == null)
                {
                    return;
                }

                if (sym.ContainingType.BaseType == null)
                {
                    return;
                }

                if (sym.ContainingType.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    return;
                }

                if (_analyzerFieldSymbols.Contains(sym))
                {
                    return;
                }

                _analyzerFieldSymbols.Add(sym);
            }

            // Stores a class in state
            internal protected void AddClass(SymbolAnalysisContext context)
            {
                var sym = (INamedTypeSymbol)context.Symbol;

                if (sym == null)
                {
                    return;
                }

                if (sym.BaseType == null)
                {
                    return;
                }

                if (sym.BaseType != context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    if (sym.ContainingType == null)
                    {
                        return;
                    }

                    if (sym.ContainingType.BaseType == null)
                    {
                        return;
                    }

                    if (sym.ContainingType.BaseType == context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                    {
                        if (_otherAnalyzerClassSymbols.Contains(sym))
                        {
                            return;
                        }
                        else
                        {
                            _otherAnalyzerClassSymbols.Add(sym);
                            return;
                        }
                    }
                }

                if (sym.BaseType == context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.Diagnostics.DiagnosticAnalyzer"))
                {
                    _analyzerClassSymbol = sym;
                }
                else if (sym.BaseType == context.Compilation.GetTypeByMetadataName("Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider"))
                {
                    _codeFixClassSymbol = sym;
                }
                else
                {
                    return;
                }

            }
            #endregion

            // Clears all state
            internal protected void ClearState()
            {
                _analyzerClassSymbol = null;
                _analyzerFieldSymbols = new List<IFieldSymbol>();
                _analyzerMethodSymbols = new List<IMethodSymbol>();
                _analyzerPropertySymbols = new List<IPropertySymbol>();
                _otherAnalyzerClassSymbols = new List<INamedTypeSymbol>();
                _initializeSymbol = null;
                _propertySymbol = null;
                _branchesDict = new Dictionary<string, string>();
            }

            //reports a diagnostics
            private static void ReportDiagnostic(CompilationAnalysisContext context, DiagnosticDescriptor rule, Location location, params string[] messageArgs)
            {
                Diagnostic diagnostic = Diagnostic.Create(rule, location, messageArgs);
                context.ReportDiagnostic(diagnostic);
            }

            // Provides information to SuppDiagReturnSymbol method
            internal protected class SuppDiagReturnSymbolInfo
            {
                public InvocationExpressionSyntax ValueClause
                {
                    get;
                    set;
                }
                public ReturnStatementSyntax ReturnDeclaration
                {
                    get;
                    set;
                }

                public SuppDiagReturnSymbolInfo(InvocationExpressionSyntax valueClause = null, ReturnStatementSyntax returnDeclaration = null)
                {
                    ValueClause = valueClause;
                    ReturnDeclaration = returnDeclaration;
                }
            }

            // Provides information to InitializeBody method
            internal protected class InitializeBodyInfo
            {
                public InvocationExpressionSyntax InvocationExpr
                {
                    get;
                    set;
                }
                public MemberAccessExpressionSyntax MemberExpr
                {
                    get;
                    set;
                }

                public InitializeBodyInfo(InvocationExpressionSyntax invocationExpr = null, MemberAccessExpressionSyntax memberExpr = null)
                {
                    InvocationExpr = invocationExpr;
                    MemberExpr = memberExpr;
                }
            }

            // Provides information to CheckInitialize method
            internal protected class CheckInitializeInfo
            {
                public IMethodSymbol RegisterMethod
                {
                    get;
                    set;
                }
                public List<ISymbol> RegisterArgs
                {
                    get;
                    set;
                }
                public InvocationExpressionSyntax InvocationExpr
                {
                    get;
                    set;
                }

                public CheckInitializeInfo(IMethodSymbol registerMethod = null, List<ISymbol> registerArgs = null, InvocationExpressionSyntax invocationExpr = null)
                {
                    RegisterMethod = registerMethod;
                    RegisterArgs = registerArgs;
                    InvocationExpr = invocationExpr;
                }
            }
        }
    }
}
