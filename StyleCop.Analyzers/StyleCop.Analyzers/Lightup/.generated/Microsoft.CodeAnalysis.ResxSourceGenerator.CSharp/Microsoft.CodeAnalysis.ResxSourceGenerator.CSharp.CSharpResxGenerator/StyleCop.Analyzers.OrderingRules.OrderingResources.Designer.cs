﻿// <auto-generated/>

#nullable enable
using System.Reflection;


namespace StyleCop.Analyzers.OrderingRules
{
    internal static partial class OrderingResources
    {
        private static global::System.Resources.ResourceManager? s_resourceManager;
        internal static global::System.Resources.ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new global::System.Resources.ResourceManager(typeof(OrderingResources)));
        internal static global::System.Globalization.CultureInfo? Culture { get; set; }
        [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        internal static string? GetResourceString(string resourceKey, string? defaultValue = null) =>  ResourceManager.GetString(resourceKey, Culture) ?? defaultValue;
        /// <summary>Fix element order</summary>
        internal static string? @ElementOrderCodeFix => GetResourceString("ElementOrderCodeFix");
        /// <summary>Fix modifier order</summary>
        internal static string? @ModifierOrderCodeFix => GetResourceString("ModifierOrderCodeFix");
        /// <summary>A C# using directive is placed outside of a namespace element.</summary>
        internal static string? @SA1200DescriptionInside => GetResourceString("SA1200DescriptionInside");
        /// <summary>A C# using directive is placed inside of a namespace declaration.</summary>
        internal static string? @SA1200DescriptionOutside => GetResourceString("SA1200DescriptionOutside");
        /// <summary>Using directive should appear within a namespace declaration</summary>
        internal static string? @SA1200MessageFormatInside => GetResourceString("SA1200MessageFormatInside");
        /// <summary>Using directive should appear outside a namespace declaration</summary>
        internal static string? @SA1200MessageFormatOutside => GetResourceString("SA1200MessageFormatOutside");
        /// <summary>Using directives should be placed correctly</summary>
        internal static string? @SA1200Title => GetResourceString("SA1200Title");
        /// <summary>An element within a C# code file is out of order in relation to the other elements in the code.</summary>
        internal static string? @SA1201Description => GetResourceString("SA1201Description");
        /// <summary>A {0} should not follow a {1}</summary>
        internal static string? @SA1201MessageFormat => GetResourceString("SA1201MessageFormat");
        /// <summary>Elements should appear in the correct order</summary>
        internal static string? @SA1201Title => GetResourceString("SA1201Title");
        /// <summary>An element within a C# code file is out of order in relation to other elements in the code.</summary>
        internal static string? @SA1202Description => GetResourceString("SA1202Description");
        /// <summary>'{0}' members should come before '{1}' members</summary>
        internal static string? @SA1202MessageFormat => GetResourceString("SA1202MessageFormat");
        /// <summary>Elements should be ordered by access</summary>
        internal static string? @SA1202Title => GetResourceString("SA1202Title");
        /// <summary>A constant field is placed beneath a non-constant field.</summary>
        internal static string? @SA1203Description => GetResourceString("SA1203Description");
        /// <summary>Constant fields should appear before non-constant fields</summary>
        internal static string? @SA1203MessageFormat => GetResourceString("SA1203MessageFormat");
        /// <summary>Constants should appear before fields</summary>
        internal static string? @SA1203Title => GetResourceString("SA1203Title");
        /// <summary>A static element is positioned beneath an instance element.</summary>
        internal static string? @SA1204Description => GetResourceString("SA1204Description");
        /// <summary>Static members should appear before non-static members</summary>
        internal static string? @SA1204MessageFormat => GetResourceString("SA1204MessageFormat");
        /// <summary>Static elements should appear before instance elements</summary>
        internal static string? @SA1204Title => GetResourceString("SA1204Title");
        /// <summary>Add access modifier</summary>
        internal static string? @SA1205CodeFix => GetResourceString("SA1205CodeFix");
        /// <summary>The partial element does not have an access modifier defined.</summary>
        internal static string? @SA1205Description => GetResourceString("SA1205Description");
        /// <summary>Partial elements should declare an access modifier</summary>
        internal static string? @SA1205MessageFormat => GetResourceString("SA1205MessageFormat");
        /// <summary>Partial elements should declare access</summary>
        internal static string? @SA1205Title => GetResourceString("SA1205Title");
        /// <summary>The keywords within the declaration of an element do not follow a standard ordering scheme.</summary>
        internal static string? @SA1206Description => GetResourceString("SA1206Description");
        /// <summary>The '{0}' modifier should appear before '{1}'</summary>
        internal static string? @SA1206MessageFormat => GetResourceString("SA1206MessageFormat");
        /// <summary>Declaration keywords should follow order</summary>
        internal static string? @SA1206Title => GetResourceString("SA1206Title");
        /// <summary>Place keyword 'protected' before keyword 'internal'</summary>
        internal static string? @SA1207CodeFix => GetResourceString("SA1207CodeFix");
        /// <summary>The keyword '{0}' is positioned after the keyword '{1}' within the declaration of a {0} {1} C# element.</summary>
        internal static string? @SA1207Description => GetResourceString("SA1207Description");
        /// <summary>The keyword '{0}' should come before '{1}'</summary>
        internal static string? @SA1207MessageFormat => GetResourceString("SA1207MessageFormat");
        /// <summary>Protected should come before internal</summary>
        internal static string? @SA1207Title => GetResourceString("SA1207Title");
        /// <summary>A using directive which declares a member of the 'System' namespace appears after a using directive which declares a member of a different namespace, within a C# code file.</summary>
        internal static string? @SA1208Description => GetResourceString("SA1208Description");
        /// <summary>Using directive for '{0}' should appear before directive for '{1}'</summary>
        internal static string? @SA1208MessageFormat => GetResourceString("SA1208MessageFormat");
        /// <summary>System using directives should be placed before other using directives</summary>
        internal static string? @SA1208Title => GetResourceString("SA1208Title");
        /// <summary>A using-alias directive is positioned before a regular using directive.</summary>
        internal static string? @SA1209Description => GetResourceString("SA1209Description");
        /// <summary>Using alias directives should be placed after all using namespace directives</summary>
        internal static string? @SA1209MessageFormat => GetResourceString("SA1209MessageFormat");
        /// <summary>Using alias directives should be placed after other using directives</summary>
        internal static string? @SA1209Title => GetResourceString("SA1209Title");
        /// <summary>The using directives within a C# code file are not sorted alphabetically by namespace.</summary>
        internal static string? @SA1210Description => GetResourceString("SA1210Description");
        /// <summary>Using directives should be ordered alphabetically by the namespaces</summary>
        internal static string? @SA1210MessageFormat => GetResourceString("SA1210MessageFormat");
        /// <summary>Using directives should be ordered alphabetically by namespace</summary>
        internal static string? @SA1210Title => GetResourceString("SA1210Title");
        /// <summary>The using-alias directives within a C# code file are not sorted alphabetically by alias name.</summary>
        internal static string? @SA1211Description => GetResourceString("SA1211Description");
        /// <summary>Using alias directive for '{0}' should appear before using alias directive for '{1}'</summary>
        internal static string? @SA1211MessageFormat => GetResourceString("SA1211MessageFormat");
        /// <summary>Using alias directives should be ordered alphabetically by alias name</summary>
        internal static string? @SA1211Title => GetResourceString("SA1211Title");
        /// <summary>A get accessor appears after a set accessor within a property or indexer.</summary>
        internal static string? @SA1212Description => GetResourceString("SA1212Description");
        /// <summary>A get accessor appears after a set accessor within a property or indexer</summary>
        internal static string? @SA1212MessageFormat => GetResourceString("SA1212MessageFormat");
        /// <summary>Property accessors should follow order</summary>
        internal static string? @SA1212Title => GetResourceString("SA1212Title");
        /// <summary>Fix accessor order</summary>
        internal static string? @SA1213CodeFix => GetResourceString("SA1213CodeFix");
        /// <summary>An add accessor appears after a remove accessor within an event.</summary>
        internal static string? @SA1213Description => GetResourceString("SA1213Description");
        /// <summary>Event accessors should follow order</summary>
        internal static string? @SA1213MessageFormat => GetResourceString("SA1213MessageFormat");
        /// <summary>Event accessors should follow order</summary>
        internal static string? @SA1213Title => GetResourceString("SA1213Title");
        /// <summary>A readonly field is positioned beneath a non-readonly field.</summary>
        internal static string? @SA1214Description => GetResourceString("SA1214Description");
        /// <summary>Readonly fields should appear before non-readonly fields</summary>
        internal static string? @SA1214MessageFormat => GetResourceString("SA1214MessageFormat");
        /// <summary>Readonly fields should appear before non-readonly fields</summary>
        internal static string? @SA1214Title => GetResourceString("SA1214Title");
        /// <summary>A using static directive is positioned before a regular or after an alias using directive.</summary>
        internal static string? @SA1216Description => GetResourceString("SA1216Description");
        /// <summary>Using static directives should be placed at the correct location</summary>
        internal static string? @SA1216MessageFormat => GetResourceString("SA1216MessageFormat");
        /// <summary>Using static directives should be placed at the correct location</summary>
        internal static string? @SA1216Title => GetResourceString("SA1216Title");
        /// <summary>All using static directives should be ordered alphabetically.</summary>
        internal static string? @SA1217Description => GetResourceString("SA1217Description");
        /// <summary>The using static directive for '{0}' should appear after the using static directive for '{1}'</summary>
        internal static string? @SA1217MessageFormat => GetResourceString("SA1217MessageFormat");
        /// <summary>Using static directives should be ordered alphabetically</summary>
        internal static string? @SA1217Title => GetResourceString("SA1217Title");
        /// <summary>Reorder using statements</summary>
        internal static string? @UsingCodeFix => GetResourceString("UsingCodeFix");

    }
}