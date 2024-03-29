﻿// <auto-generated/>

#nullable enable
using System.Reflection;


namespace StyleCop.Analyzers.ReadabilityRules
{
    internal static partial class ReadabilityResources
    {
        private static global::System.Resources.ResourceManager? s_resourceManager;
        public static global::System.Resources.ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new global::System.Resources.ResourceManager(typeof(ReadabilityResources)));
        public static global::System.Globalization.CultureInfo? Culture { get; set; }
        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("defaultValue")]
        internal static string? GetResourceString(string resourceKey, string? defaultValue = null) =>  ResourceManager.GetString(resourceKey, Culture) ?? defaultValue;
        /// <summary>Fix indentation</summary>
        public static string @IndentationCodeFix => GetResourceString("IndentationCodeFix")!;
        /// <summary>Remove region</summary>
        public static string @RemoveRegionCodeFix => GetResourceString("RemoveRegionCodeFix")!;
        /// <summary>Replace 'base.' with 'this.'</summary>
        public static string @SA1100CodeFix => GetResourceString("SA1100CodeFix")!;
        /// <summary>A call to a member from an inherited class begins with 'base.', and the local class does not contain an override or implementation of the member.</summary>
        public static string @SA1100Description => GetResourceString("SA1100Description")!;
        /// <summary>Do not prefix calls with base unless local implementation exists</summary>
        public static string @SA1100MessageFormat => GetResourceString("SA1100MessageFormat")!;
        /// <summary>Do not prefix calls with base unless local implementation exists</summary>
        public static string @SA1100Title => GetResourceString("SA1100Title")!;
        /// <summary>Prefix reference with 'this.'</summary>
        public static string @SA1101CodeFix => GetResourceString("SA1101CodeFix")!;
        /// <summary>A call to an instance member of the local class or a base class is not prefixed with 'this.', within a C# code file.</summary>
        public static string @SA1101Description => GetResourceString("SA1101Description")!;
        /// <summary>Prefix local calls with this</summary>
        public static string @SA1101MessageFormat => GetResourceString("SA1101MessageFormat")!;
        /// <summary>Prefix local calls with this</summary>
        public static string @SA1101Title => GetResourceString("SA1101Title")!;
        /// <summary>Remove separating lines</summary>
        public static string @SA1102CodeFix => GetResourceString("SA1102CodeFix")!;
        /// <summary>A C# query clause does not begin on the same line as the previous clause, or on the next line.</summary>
        public static string @SA1102Description => GetResourceString("SA1102Description")!;
        /// <summary>Query clause should follow previous clause.</summary>
        public static string @SA1102MessageFormat => GetResourceString("SA1102MessageFormat")!;
        /// <summary>Query clause should follow previous clause</summary>
        public static string @SA1102Title => GetResourceString("SA1102Title")!;
        /// <summary>Place on multiple lines</summary>
        public static string @SA1103CodeFixMultipleLines => GetResourceString("SA1103CodeFixMultipleLines")!;
        /// <summary>Place on single line</summary>
        public static string @SA1103CodeFixSingleLine => GetResourceString("SA1103CodeFixSingleLine")!;
        /// <summary>The clauses within a C# query expression are not all placed on the same line, and each clause is not placed on its own line.</summary>
        public static string @SA1103Description => GetResourceString("SA1103Description")!;
        /// <summary>Query clauses should be on separate lines or all on one line</summary>
        public static string @SA1103MessageFormat => GetResourceString("SA1103MessageFormat")!;
        /// <summary>Query clauses should be on separate lines or all on one line</summary>
        public static string @SA1103Title => GetResourceString("SA1103Title")!;
        /// <summary>A clause within a C# query expression begins on the same line as the previous clause, when the previous clause spans across multiple lines.</summary>
        public static string @SA1104Description => GetResourceString("SA1104Description")!;
        /// <summary>Query clause should begin on new line when previous clause spans multiple lines</summary>
        public static string @SA1104MessageFormat => GetResourceString("SA1104MessageFormat")!;
        /// <summary>Insert new line</summary>
        public static string @SA1104SA1105CodeFix => GetResourceString("SA1104SA1105CodeFix")!;
        /// <summary>Query clause should begin on new line when previous clause spans multiple lines</summary>
        public static string @SA1104Title => GetResourceString("SA1104Title")!;
        /// <summary>A clause within a C# query expression spans across multiple lines, and does not begin on its own line.</summary>
        public static string @SA1105Description => GetResourceString("SA1105Description")!;
        /// <summary>Query clauses spanning multiple lines should begin on own line</summary>
        public static string @SA1105MessageFormat => GetResourceString("SA1105MessageFormat")!;
        /// <summary>Query clauses spanning multiple lines should begin on own line</summary>
        public static string @SA1105Title => GetResourceString("SA1105Title")!;
        /// <summary>Remove empty statement</summary>
        public static string @SA1106CodeFix => GetResourceString("SA1106CodeFix")!;
        /// <summary>The C# code contains an extra semicolon.</summary>
        public static string @SA1106Description => GetResourceString("SA1106Description")!;
        /// <summary>Code should not contain empty statements</summary>
        public static string @SA1106MessageFormat => GetResourceString("SA1106MessageFormat")!;
        /// <summary>Code should not contain empty statements</summary>
        public static string @SA1106Title => GetResourceString("SA1106Title")!;
        /// <summary>Enter new line</summary>
        public static string @SA1107CodeFix => GetResourceString("SA1107CodeFix")!;
        /// <summary>The C# code contains more than one statement on a single line.</summary>
        public static string @SA1107Description => GetResourceString("SA1107Description")!;
        /// <summary>Code should not contain multiple statements on one line</summary>
        public static string @SA1107MessageFormat => GetResourceString("SA1107MessageFormat")!;
        /// <summary>Code should not contain multiple statements on one line</summary>
        public static string @SA1107Title => GetResourceString("SA1107Title")!;
        /// <summary>A C# statement contains a comment between the declaration of the statement and the opening brace of the statement.</summary>
        public static string @SA1108Description => GetResourceString("SA1108Description")!;
        /// <summary>Block statements should not contain embedded comments</summary>
        public static string @SA1108MessageFormat => GetResourceString("SA1108MessageFormat")!;
        /// <summary>Block statements should not contain embedded comments</summary>
        public static string @SA1108Title => GetResourceString("SA1108Title")!;
        /// <summary>A C# statement contains a region tag between the declaration of the statement and the opening brace of the statement.</summary>
        public static string @SA1109Description => GetResourceString("SA1109Description")!;
        /// <summary></summary>
        public static string @SA1109MessageFormat => GetResourceString("SA1109MessageFormat")!;
        /// <summary>Block statements should not contain embedded regions</summary>
        public static string @SA1109Title => GetResourceString("SA1109Title")!;
        /// <summary>The opening parenthesis or bracket is not placed on the same line as the method/indexer/attribute/array name.</summary>
        public static string @SA1110Description => GetResourceString("SA1110Description")!;
        /// <summary>Opening parenthesis or bracket should be on declaration line</summary>
        public static string @SA1110MessageFormat => GetResourceString("SA1110MessageFormat")!;
        /// <summary>Opening parenthesis or bracket should be on declaration line</summary>
        public static string @SA1110Title => GetResourceString("SA1110Title")!;
        /// <summary>The closing parenthesis or bracket in a call to or declaration of a C# method/indexer/attribute/array/constructor/delegate is not placed on the same line as the last parameter.</summary>
        public static string @SA1111Description => GetResourceString("SA1111Description")!;
        /// <summary>Closing parenthesis should be on line of last parameter</summary>
        public static string @SA1111MessageFormat => GetResourceString("SA1111MessageFormat")!;
        /// <summary>Closing parenthesis should be on line of last parameter</summary>
        public static string @SA1111Title => GetResourceString("SA1111Title")!;
        /// <summary>The closing parenthesis or bracket in a call to a C# method or indexer, or the declaration of a method or indexer, is not placed on the same line as the opening bracket when the element does not take any parameters.</summary>
        public static string @SA1112Description => GetResourceString("SA1112Description")!;
        /// <summary>Closing parenthesis should be on line of opening parenthesis</summary>
        public static string @SA1112MessageFormat => GetResourceString("SA1112MessageFormat")!;
        /// <summary>Closing parenthesis should be on line of opening parenthesis</summary>
        public static string @SA1112Title => GetResourceString("SA1112Title")!;
        /// <summary>A comma between two parameters in a call to a C# method or indexer, or in the declaration of a method or indexer, is not placed on the same line as the previous parameter.</summary>
        public static string @SA1113Description => GetResourceString("SA1113Description")!;
        /// <summary>Comma should be on the same line as previous parameter</summary>
        public static string @SA1113MessageFormat => GetResourceString("SA1113MessageFormat")!;
        /// <summary>Comma should be on the same line as previous parameter</summary>
        public static string @SA1113Title => GetResourceString("SA1113Title")!;
        /// <summary>The start of the parameter list for a method/constructor/indexer/array/operator call or declaration does not begin on the same line as the opening bracket, or on the line after the opening bracket.</summary>
        public static string @SA1114Description => GetResourceString("SA1114Description")!;
        /// <summary>Parameter list should follow declaration</summary>
        public static string @SA1114MessageFormat => GetResourceString("SA1114MessageFormat")!;
        /// <summary>Parameter list should follow declaration</summary>
        public static string @SA1114Title => GetResourceString("SA1114Title")!;
        /// <summary>A parameter within a C# method or indexer call or declaration does not begin on the same line as the previous parameter, or on the next line.</summary>
        public static string @SA1115Description => GetResourceString("SA1115Description")!;
        /// <summary>The parameter should begin on the line after the previous parameter</summary>
        public static string @SA1115MessageFormat => GetResourceString("SA1115MessageFormat")!;
        /// <summary>Parameter should follow comma</summary>
        public static string @SA1115Title => GetResourceString("SA1115Title")!;
        /// <summary>Move first argument to next line</summary>
        public static string @SA1116CodeFix => GetResourceString("SA1116CodeFix")!;
        /// <summary>The parameters to a C# method or indexer call or declaration span across multiple lines, but the first parameter does not start on the line after the opening bracket.</summary>
        public static string @SA1116Description => GetResourceString("SA1116Description")!;
        /// <summary>The parameters should begin on the line after the declaration, whenever the parameter span across multiple lines</summary>
        public static string @SA1116MessageFormat => GetResourceString("SA1116MessageFormat")!;
        /// <summary>Split parameters should start on line after declaration</summary>
        public static string @SA1116Title => GetResourceString("SA1116Title")!;
        /// <summary>The parameters to a C# method or indexer call or declaration are not all on the same line or each on a separate line.</summary>
        public static string @SA1117Description => GetResourceString("SA1117Description")!;
        /// <summary>The parameters should all be placed on the same line or each parameter should be placed on its own line</summary>
        public static string @SA1117MessageFormat => GetResourceString("SA1117MessageFormat")!;
        /// <summary>Parameters should be on same line or separate lines</summary>
        public static string @SA1117Title => GetResourceString("SA1117Title")!;
        /// <summary>A parameter to a C# method/indexer/attribute/array, other than the first parameter, spans across multiple lines. If the parameter is short, place the entire parameter on a single line. Otherwise, save the contents of the parameter in a temporary variable a ...</summary>
        public static string @SA1118Description => GetResourceString("SA1118Description")!;
        /// <summary>The parameter spans multiple lines</summary>
        public static string @SA1118MessageFormat => GetResourceString("SA1118MessageFormat")!;
        /// <summary>Parameter should not span multiple lines</summary>
        public static string @SA1118Title => GetResourceString("SA1118Title")!;
        /// <summary>Remove empty comment</summary>
        public static string @SA1120CodeFix => GetResourceString("SA1120CodeFix")!;
        /// <summary>The C# comment does not contain any comment text.</summary>
        public static string @SA1120Description => GetResourceString("SA1120Description")!;
        /// <summary>Comments should contain text</summary>
        public static string @SA1120MessageFormat => GetResourceString("SA1120MessageFormat")!;
        /// <summary>Comments should contain text</summary>
        public static string @SA1120Title => GetResourceString("SA1120Title")!;
        /// <summary>Replace with built-in type</summary>
        public static string @SA1121CodeFix => GetResourceString("SA1121CodeFix")!;
        /// <summary>The code uses one of the basic C# types, but does not use the built-in alias for the type.</summary>
        public static string @SA1121Description => GetResourceString("SA1121Description")!;
        /// <summary>Use built-in type alias</summary>
        public static string @SA1121MessageFormat => GetResourceString("SA1121MessageFormat")!;
        /// <summary>Use built-in type alias</summary>
        public static string @SA1121Title => GetResourceString("SA1121Title")!;
        /// <summary>Replace with string.Empty</summary>
        public static string @SA1122CodeFix => GetResourceString("SA1122CodeFix")!;
        /// <summary>The C# code includes an empty string, written as "".</summary>
        public static string @SA1122Description => GetResourceString("SA1122Description")!;
        /// <summary>Use string.Empty for empty strings</summary>
        public static string @SA1122MessageFormat => GetResourceString("SA1122MessageFormat")!;
        /// <summary>Use string.Empty for empty strings</summary>
        public static string @SA1122Title => GetResourceString("SA1122Title")!;
        /// <summary>The C# code contains a region within the body of a code element.</summary>
        public static string @SA1123Description => GetResourceString("SA1123Description")!;
        /// <summary>Region should not be located within a code element</summary>
        public static string @SA1123MessageFormat => GetResourceString("SA1123MessageFormat")!;
        /// <summary>Do not place regions within elements</summary>
        public static string @SA1123Title => GetResourceString("SA1123Title")!;
        /// <summary>The C# code contains a region.</summary>
        public static string @SA1124Description => GetResourceString("SA1124Description")!;
        /// <summary>Do not use regions</summary>
        public static string @SA1124MessageFormat => GetResourceString("SA1124MessageFormat")!;
        /// <summary>Do not use regions</summary>
        public static string @SA1124Title => GetResourceString("SA1124Title")!;
        /// <summary>The Nullable&lt;T&gt; type has been defined not using the C# shorthand. For example, Nullable&lt;DateTime&gt; has been used instead of the preferred DateTime?</summary>
        public static string @SA1125Description => GetResourceString("SA1125Description")!;
        /// <summary>Use shorthand for nullable types</summary>
        public static string @SA1125MessageFormat => GetResourceString("SA1125MessageFormat")!;
        /// <summary>Use shorthand for nullable types</summary>
        public static string @SA1125Title => GetResourceString("SA1125Title")!;
        /// <summary>A call to a member is not prefixed with the 'this.', 'base.', 'object.' or 'typename.' prefix to indicate the intended method call, within a C# code file.</summary>
        public static string @SA1126Description => GetResourceString("SA1126Description")!;
        /// <summary></summary>
        public static string @SA1126MessageFormat => GetResourceString("SA1126MessageFormat")!;
        /// <summary>Prefix calls correctly</summary>
        public static string @SA1126Title => GetResourceString("SA1126Title")!;
        /// <summary>Place each type constraint on a new line</summary>
        public static string @SA1127CodeFix => GetResourceString("SA1127CodeFix")!;
        /// <summary>Each type constraint clause for a generic type parameter should be listed on a line of code by itself.</summary>
        public static string @SA1127Description => GetResourceString("SA1127Description")!;
        /// <summary>Generic type constraints should be on their own line</summary>
        public static string @SA1127MessageFormat => GetResourceString("SA1127MessageFormat")!;
        /// <summary>Generic type constraints should be on their own line</summary>
        public static string @SA1127Title => GetResourceString("SA1127Title")!;
        /// <summary>Place constructor initializer on own line</summary>
        public static string @SA1128CodeFix => GetResourceString("SA1128CodeFix")!;
        /// <summary>A constructor initializer, including the colon character, should be on its own line.</summary>
        public static string @SA1128Description => GetResourceString("SA1128Description")!;
        /// <summary>Put constructor initializers on their own line</summary>
        public static string @SA1128MessageFormat => GetResourceString("SA1128MessageFormat")!;
        /// <summary>Put constructor initializers on their own line</summary>
        public static string @SA1128Title => GetResourceString("SA1128Title")!;
        /// <summary>Replace with default(T)</summary>
        public static string @SA1129CodeFix => GetResourceString("SA1129CodeFix")!;
        /// <summary>When creating a new instance of a value type T, the syntax 'default(T)' is functionally equivalent to the syntax 'new T()'. To avoid confusion regarding the behavior of the resulting instance, the first form is preferred.</summary>
        public static string @SA1129Description => GetResourceString("SA1129Description")!;
        /// <summary>Do not use default value type constructor</summary>
        public static string @SA1129MessageFormat => GetResourceString("SA1129MessageFormat")!;
        /// <summary>Do not use default value type constructor</summary>
        public static string @SA1129Title => GetResourceString("SA1129Title")!;
        /// <summary>Replace with lambda.</summary>
        public static string @SA1130CodeFix => GetResourceString("SA1130CodeFix")!;
        /// <summary>Lambda expressions are more succinct and easier to read than anonymous methods, so they should are preferred whenever the two are functionally equivalent.</summary>
        public static string @SA1130Description => GetResourceString("SA1130Description")!;
        /// <summary>Use lambda syntax</summary>
        public static string @SA1130MessageFormat => GetResourceString("SA1130MessageFormat")!;
        /// <summary>Use lambda syntax</summary>
        public static string @SA1130Title => GetResourceString("SA1130Title")!;
        /// <summary>Swap operands</summary>
        public static string @SA1131CodeFix => GetResourceString("SA1131CodeFix")!;
        /// <summary>When a comparison is made between a variable and a literal, the variable should be placed on the left-hand-side to maximize readability.</summary>
        public static string @SA1131Description => GetResourceString("SA1131Description")!;
        /// <summary>Constant values should appear on the right-hand side of comparisons</summary>
        public static string @SA1131MessageFormat => GetResourceString("SA1131MessageFormat")!;
        /// <summary>Use readable conditions</summary>
        public static string @SA1131Title => GetResourceString("SA1131Title")!;
        /// <summary>Place each field on a new line</summary>
        public static string @SA1132CodeFix => GetResourceString("SA1132CodeFix")!;
        /// <summary>Each field should be declared on its own line, in order to clearly see each field of a type and allow for proper documentation of the behavior of each field.</summary>
        public static string @SA1132Description => GetResourceString("SA1132Description")!;
        /// <summary>Each field should be declared on its own line</summary>
        public static string @SA1132MessageFormat => GetResourceString("SA1132MessageFormat")!;
        /// <summary>Do not combine fields</summary>
        public static string @SA1132Title => GetResourceString("SA1132Title")!;
        /// <summary>Give each attribute its own square brackets</summary>
        public static string @SA1133CodeFix => GetResourceString("SA1133CodeFix")!;
        /// <summary>Each attribute usage should be placed in its own set of square brackets for maximum readability.</summary>
        public static string @SA1133Description => GetResourceString("SA1133Description")!;
        /// <summary>Each attribute should be placed in its own set of square brackets</summary>
        public static string @SA1133MessageFormat => GetResourceString("SA1133MessageFormat")!;
        /// <summary>Do not combine attributes</summary>
        public static string @SA1133Title => GetResourceString("SA1133Title")!;
        /// <summary>Place attribute on own line.</summary>
        public static string @SA1134CodeFix => GetResourceString("SA1134CodeFix")!;
        /// <summary>Each attribute should be placed on its own line of code.</summary>
        public static string @SA1134Description => GetResourceString("SA1134Description")!;
        /// <summary>Each attribute should be placed on its own line of code</summary>
        public static string @SA1134MessageFormat => GetResourceString("SA1134MessageFormat")!;
        /// <summary>Attributes should not share line</summary>
        public static string @SA1134Title => GetResourceString("SA1134Title")!;
        /// <summary>Qualify using directive</summary>
        public static string @SA1135CodeFix => GetResourceString("SA1135CodeFix")!;
        /// <summary>All using directives should be qualified.</summary>
        public static string @SA1135Description => GetResourceString("SA1135Description")!;
        /// <summary>Using directive for namespace '{0}' should be qualified</summary>
        public static string @SA1135MessageFormatNamespace => GetResourceString("SA1135MessageFormatNamespace")!;
        /// <summary>Using directive for type '{0}' should be qualified</summary>
        public static string @SA1135MessageFormatType => GetResourceString("SA1135MessageFormatType")!;
        /// <summary>Using directives should be qualified</summary>
        public static string @SA1135Title => GetResourceString("SA1135Title")!;
        /// <summary>Place enum values own their own lines</summary>
        public static string @SA1136CodeFix => GetResourceString("SA1136CodeFix")!;
        /// <summary>Enum values should be placed on their own lines for maximum readability.</summary>
        public static string @SA1136Description => GetResourceString("SA1136Description")!;
        /// <summary>Enum values should be on separate lines</summary>
        public static string @SA1136MessageFormat => GetResourceString("SA1136MessageFormat")!;
        /// <summary>Enum values should be on separate lines</summary>
        public static string @SA1136Title => GetResourceString("SA1136Title")!;
        /// <summary>Elements at the same level in the syntax tree should have the same indentation.</summary>
        public static string @SA1137Description => GetResourceString("SA1137Description")!;
        /// <summary>Elements should have the same indentation</summary>
        public static string @SA1137MessageFormat => GetResourceString("SA1137MessageFormat")!;
        /// <summary>Elements should have the same indentation</summary>
        public static string @SA1137Title => GetResourceString("SA1137Title")!;
        /// <summary>Use literal suffix notation instead of casting</summary>
        public static string @SA1139CodeFix => GetResourceString("SA1139CodeFix")!;
        /// <summary>Use literal suffix notation instead of casting, in order to improve readability, avoid bugs related to illegal casts and ensure that optimal IL is produced.</summary>
        public static string @SA1139Description => GetResourceString("SA1139Description")!;
        /// <summary>Use literal suffix notation instead of casting</summary>
        public static string @SA1139MessageFormat => GetResourceString("SA1139MessageFormat")!;
        /// <summary>Use literal suffix notation instead of casting</summary>
        public static string @SA1139Title => GetResourceString("SA1139Title")!;
        /// <summary>Replace with tuple syntax</summary>
        public static string @SA1141CodeFix => GetResourceString("SA1141CodeFix")!;
        /// <summary>Use tuple syntax instead of the underlying ValueTuple implementation type.</summary>
        public static string @SA1141Description => GetResourceString("SA1141Description")!;
        /// <summary>Use tuple syntax</summary>
        public static string @SA1141MessageFormat => GetResourceString("SA1141MessageFormat")!;
        /// <summary>Use tuple syntax</summary>
        public static string @SA1141Title => GetResourceString("SA1141Title")!;
        /// <summary>Use tuple field name</summary>
        public static string @SA1142CodeFix => GetResourceString("SA1142CodeFix")!;
        /// <summary>A field of a tuple was referenced by its metadata name when a field name is available.</summary>
        public static string @SA1142Description => GetResourceString("SA1142Description")!;
        /// <summary>Refer to tuple fields by name</summary>
        public static string @SA1142MessageFormat => GetResourceString("SA1142MessageFormat")!;
        /// <summary>Refer to tuple fields by name</summary>
        public static string @SA1142Title => GetResourceString("SA1142Title")!;
        /// <summary>Remove 'this.' prefix</summary>
        public static string @SX1101CodeFix => GetResourceString("SX1101CodeFix")!;
        /// <summary>A call to an instance member of the local class or a base class is prefixed with `this.`.</summary>
        public static string @SX1101Description => GetResourceString("SX1101Description")!;
        /// <summary>Do not prefix local calls with 'this.'</summary>
        public static string @SX1101MessageFormat => GetResourceString("SX1101MessageFormat")!;
        /// <summary>Do not prefix local calls with 'this.'</summary>
        public static string @SX1101Title => GetResourceString("SX1101Title")!;

    }
}
