﻿// <auto-generated/>

#nullable enable
using System.Reflection;


namespace StyleCop.Analyzers.MaintainabilityRules
{
    internal static partial class MaintainabilityResources
    {
        private static global::System.Resources.ResourceManager? s_resourceManager;
        public static global::System.Resources.ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new global::System.Resources.ResourceManager(typeof(MaintainabilityResources)));
        public static global::System.Globalization.CultureInfo? Culture { get; set; }
        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        [return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("defaultValue")]
        internal static string? GetResourceString(string resourceKey, string? defaultValue = null) =>  ResourceManager.GetString(resourceKey, Culture) ?? defaultValue;
        /// <summary>Remove parentheses</summary>
        public static string @SA1119CodeFix => GetResourceString("SA1119CodeFix")!;
        /// <summary>A C# statement contains parenthesis which are unnecessary and should be removed.</summary>
        public static string @SA1119Description => GetResourceString("SA1119Description")!;
        /// <summary>Statement should not use unnecessary parenthesis</summary>
        public static string @SA1119MessageFormat => GetResourceString("SA1119MessageFormat")!;
        /// <summary>Statement should not use unnecessary parenthesis</summary>
        public static string @SA1119Title => GetResourceString("SA1119Title")!;
        /// <summary>Declare accessibility</summary>
        public static string @SA1400CodeFix => GetResourceString("SA1400CodeFix")!;
        /// <summary>The access modifier for a C# element has not been explicitly defined.</summary>
        public static string @SA1400Description => GetResourceString("SA1400Description")!;
        /// <summary>Element '{0}' should declare an access modifier</summary>
        public static string @SA1400MessageFormat => GetResourceString("SA1400MessageFormat")!;
        /// <summary>Access modifier should be declared</summary>
        public static string @SA1400Title => GetResourceString("SA1400Title")!;
        /// <summary>A field within a C# class has an access modifier other than private.</summary>
        public static string @SA1401Description => GetResourceString("SA1401Description")!;
        /// <summary>Field should be private</summary>
        public static string @SA1401MessageFormat => GetResourceString("SA1401MessageFormat")!;
        /// <summary>Fields should be private</summary>
        public static string @SA1401Title => GetResourceString("SA1401Title")!;
        /// <summary>Move type to new file</summary>
        public static string @SA1402CodeFix => GetResourceString("SA1402CodeFix")!;
        /// <summary>A C# code file contains more than one unique type.</summary>
        public static string @SA1402Description => GetResourceString("SA1402Description")!;
        /// <summary>File may only contain a single type</summary>
        public static string @SA1402MessageFormat => GetResourceString("SA1402MessageFormat")!;
        /// <summary>File may only contain a single type</summary>
        public static string @SA1402Title => GetResourceString("SA1402Title")!;
        /// <summary>A C# code file contains more than one namespace.</summary>
        public static string @SA1403Description => GetResourceString("SA1403Description")!;
        /// <summary>File may only contain a single namespace</summary>
        public static string @SA1403MessageFormat => GetResourceString("SA1403MessageFormat")!;
        /// <summary>File may only contain a single namespace</summary>
        public static string @SA1403Title => GetResourceString("SA1403Title")!;
        /// <summary>Fix justification</summary>
        public static string @SA1404CodeFix => GetResourceString("SA1404CodeFix")!;
        /// <summary>A Code Analysis SuppressMessage attribute does not include a justification.</summary>
        public static string @SA1404Description => GetResourceString("SA1404Description")!;
        /// <summary>Code analysis suppression should have justification</summary>
        public static string @SA1404MessageFormat => GetResourceString("SA1404MessageFormat")!;
        /// <summary>Code analysis suppression should have justification</summary>
        public static string @SA1404Title => GetResourceString("SA1404Title")!;
        /// <summary>A call to Debug.Assert in C# code does not include a descriptive message.</summary>
        public static string @SA1405Description => GetResourceString("SA1405Description")!;
        /// <summary>Debug.Assert should provide message text</summary>
        public static string @SA1405MessageFormat => GetResourceString("SA1405MessageFormat")!;
        /// <summary>Debug.Assert should provide message text</summary>
        public static string @SA1405Title => GetResourceString("SA1405Title")!;
        /// <summary>A call to Debug.Fail in C# code does not include a descriptive message.</summary>
        public static string @SA1406Description => GetResourceString("SA1406Description")!;
        /// <summary>Debug.Fail should provide message text</summary>
        public static string @SA1406MessageFormat => GetResourceString("SA1406MessageFormat")!;
        /// <summary>Debug.Fail should provide message text</summary>
        public static string @SA1406Title => GetResourceString("SA1406Title")!;
        /// <summary>A C# statement contains a complex arithmetic expression which omits parenthesis around operators.</summary>
        public static string @SA1407Description => GetResourceString("SA1407Description")!;
        /// <summary>Arithmetic expressions should declare precedence</summary>
        public static string @SA1407MessageFormat => GetResourceString("SA1407MessageFormat")!;
        /// <summary>Add parentheses</summary>
        public static string @SA1407SA1408CodeFix => GetResourceString("SA1407SA1408CodeFix")!;
        /// <summary>Arithmetic expressions should declare precedence</summary>
        public static string @SA1407Title => GetResourceString("SA1407Title")!;
        /// <summary>A C# statement contains a complex conditional expression which omits parenthesis around operators.</summary>
        public static string @SA1408Description => GetResourceString("SA1408Description")!;
        /// <summary>Conditional expressions should declare precedence</summary>
        public static string @SA1408MessageFormat => GetResourceString("SA1408MessageFormat")!;
        /// <summary>Conditional expressions should declare precedence</summary>
        public static string @SA1408Title => GetResourceString("SA1408Title")!;
        /// <summary>A C# file contains code which is unnecessary and can be removed without changing the overall logic of the code.</summary>
        public static string @SA1409Description => GetResourceString("SA1409Description")!;
        /// <summary>TODO: Message format</summary>
        public static string @SA1409MessageFormat => GetResourceString("SA1409MessageFormat")!;
        /// <summary>Remove unnecessary code</summary>
        public static string @SA1409Title => GetResourceString("SA1409Title")!;
        /// <summary>A call to a C# anonymous method does not contain any method parameters, yet the statement still includes parenthesis.</summary>
        public static string @SA1410Description => GetResourceString("SA1410Description")!;
        /// <summary>Remove delegate parenthesis when possible</summary>
        public static string @SA1410MessageFormat => GetResourceString("SA1410MessageFormat")!;
        /// <summary>Remove parentheses</summary>
        public static string @SA1410SA1411CodeFix => GetResourceString("SA1410SA1411CodeFix")!;
        /// <summary>Remove delegate parenthesis when possible</summary>
        public static string @SA1410Title => GetResourceString("SA1410Title")!;
        /// <summary>TODO.</summary>
        public static string @SA1411Description => GetResourceString("SA1411Description")!;
        /// <summary>Attribute constructor should not use unnecessary parenthesis</summary>
        public static string @SA1411MessageFormat => GetResourceString("SA1411MessageFormat")!;
        /// <summary>Attribute constructor should not use unnecessary parenthesis</summary>
        public static string @SA1411Title => GetResourceString("SA1411Title")!;
        /// <summary>Change encoding from '{0}' to UTF-8 with byte order mark</summary>
        public static string @SA1412CodeFix => GetResourceString("SA1412CodeFix")!;
        /// <summary>Source files should be saved using the UTF-8 encoding with a byte order mark.</summary>
        public static string @SA1412Description => GetResourceString("SA1412Description")!;
        /// <summary>Store files as UTF-8 with byte order mark</summary>
        public static string @SA1412MessageFormat => GetResourceString("SA1412MessageFormat")!;
        /// <summary>Store files as UTF-8 with byte order mark</summary>
        public static string @SA1412Title => GetResourceString("SA1412Title")!;
        /// <summary>Add trailing comma</summary>
        public static string @SA1413CodeFix => GetResourceString("SA1413CodeFix")!;
        /// <summary>A multi-line initializer in a C# code file should use a comma on the last line.</summary>
        public static string @SA1413Description => GetResourceString("SA1413Description")!;
        /// <summary>Use trailing comma in multi-line initializers</summary>
        public static string @SA1413MessageFormat => GetResourceString("SA1413MessageFormat")!;
        /// <summary>Use trailing comma in multi-line initializers</summary>
        public static string @SA1413Title => GetResourceString("SA1413Title")!;
        /// <summary>Tuple types appearing in member declarations should have explicitly named tuple elements.</summary>
        public static string @SA1414Description => GetResourceString("SA1414Description")!;
        /// <summary>Tuple types in signatures should have element names</summary>
        public static string @SA1414MessageFormat => GetResourceString("SA1414MessageFormat")!;
        /// <summary>Tuple types in signatures should have element names</summary>
        public static string @SA1414Title => GetResourceString("SA1414Title")!;

    }
}
