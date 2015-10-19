// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The documentation text within a C# property’s &lt;summary&gt; tag takes into account all of the accessors within
    /// the property, but one of the accessors has limited access.
    /// </summary>
    /// <remarks>
    /// <para>C# syntax provides a mechanism for inserting documentation for classes and elements directly into the
    /// code, through the use of XML documentation headers. For an introduction to these headers and a description of
    /// the header syntax, see the following article:
    /// <see href="http://msdn.microsoft.com/en-us/magazine/cc302121.aspx">XML Comments Let You Build Documentation
    /// Directly From Your Visual Studio .NET Source Files</see>.</para>
    ///
    /// <para>A violation of this rule occurs when one of the accessors within the property has limited access (usually
    /// the set accessor), but the summary documentation text for the property still refers to both accessors.</para>
    ///
    /// <para>Normally, a property's summary text must begin with wording describing the types of accessors exposed
    /// within the property. If the property contains only a get accessor, the summary must begin with the word "Gets".
    /// If the property contains only a set accessor, the summary must begin with the word "Sets". If the property
    /// exposes both a get and set accessor, the summary text must begin with "Gets or sets".</para>
    ///
    /// <para>However, when an accessor within the property is given an access level which is more limited than the
    /// access level of the property, this accessor should be omitted from the summary documentation. It can sometimes
    /// be non-obvious whether the set accessor within a property is actually less accessible than the get accessor. For
    /// example, consider the case where a public property is contained within an internal class, and the set accessor
    /// is given internal accessor. In effect, both the get and set accessors have the same access level. In this case,
    /// the summary documentation should refer to both the get and set accessors, since they effectively have the same
    /// access level.</para>
    ///
    /// <para>In some situations, the set accessor for a property can have more restricted access than the get accessor.
    /// For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Gets the name of the customer.
    /// /// &lt;/summary&gt;
    /// public string Name
    /// {
    ///     get { return this.name; }
    ///     private set { this.name = value; }
    /// }
    /// </code>
    ///
    /// <para>In this example, the set accessor has been given private access, meaning that it can only be accessed by
    /// local members of the class in which it is contained. The get accessor, however, inherits its access from the
    /// parent property, thus it can be accessed by any caller, since the property has public access.</para>
    ///
    /// <para>In this case, the documentation summary text should avoid referring to the set accessor, since it is not
    /// visible to external callers.</para>
    ///
    /// <para>StyleCop applies a series of rules to determine when the set accessor should be referenced in the
    /// property's summary documentation. In general, these rules require the set accessor to be referenced whenever it
    /// is visible to the same set of callers as the get accessor, or whenever it is visible to external classes or
    /// inheriting classes.</para>
    ///
    /// <para>The specific rules for determining whether to include the set accessor in the property's summary
    /// documentation are:</para>
    ///
    /// <list type="number">
    /// <item>
    /// <para>The set accessor has the same access level as the get accessor. For example:</para>
    ///
    /// <code language="csharp">
    /// /// &lt;summary&gt;
    /// /// Gets or sets the name of the customer.
    /// /// &lt;/summary&gt;
    /// protected string Name
    /// {
    ///     get { return this.name; }
    ///     set { this.name = value; }
    /// }
    /// </code>
    /// </item>
    /// <item>
    /// <para>The property is only internally accessible within the assembly, and the set accessor also has internal
    /// access. For example:</para>
    /// <code language="csharp">
    /// internal class Class1
    /// {
    ///     /// &lt;summary&gt;
    ///     /// Gets or sets the name of the customer.
    ///     /// &lt;/summary&gt;
    ///     protected string Name
    ///     {
    ///         get { return this.name; }
    ///         internal set { this.name = value; }
    ///     }
    /// }
    /// </code>
    ///
    /// <code language="csharp">
    /// internal class Class1
    /// {
    ///     public class Class2
    ///     {
    ///         /// &lt;summary&gt;
    ///         /// Gets or sets the name of the customer.
    ///         /// &lt;/summary&gt;
    ///         public string Name
    ///         {
    ///             get { return this.name; }
    ///             internal set { this.name = value; }
    ///         }
    ///     }
    /// }
    /// </code>
    /// </item>
    /// <item>
    /// <para>The property is private or is contained beneath a private class, and the set accessor has any access
    /// modifier other than private. In the example below, the access modifier declared on the set accessor has no
    /// meaning, since the set accessor is contained within a private class and thus cannot be seen by other classes
    /// outside of <c>Class1</c>. This effectively gives the set accessor the same access level as the get
    /// accessor.</para>
    ///
    /// <code language="csharp">
    /// public class Class1
    /// {
    ///     private class Class2
    ///     {
    ///         public class Class3
    ///         {
    ///             /// &lt;summary&gt;
    ///             /// Gets or sets the name of the customer.
    ///             /// &lt;/summary&gt;
    ///             public string Name
    ///             {
    ///                 get { return this.name; }
    ///                 internal set { this.name = value; }
    ///             }
    ///         }
    ///     }
    /// }
    /// </code>
    /// </item>
    /// <item>
    /// <para>Whenever the set accessor has protected or protected internal access, it should be referenced in the
    /// documentation. A protected or protected internal set accessor can always been seen by a class inheriting from
    /// the class containing the property.</para>
    ///
    /// <code language="csharp">
    /// internal class Class1
    /// {
    ///     public class Class2
    ///     {
    ///         /// &lt;summary&gt;
    ///         /// Gets or sets the name of the customer.
    ///         /// &lt;/summary&gt;
    ///         internal string Name
    ///         {
    ///             get { return this.name; }
    ///             protected set { this.name = value; }
    ///         }
    ///     }
    ///
    ///     private class Class3 : Class2
    ///     {
    ///         public Class3(string name) { this.Name = name; }
    ///     }
    /// }
    /// </code>
    /// </item>
    /// </list>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1624PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the
        /// <see cref="SA1624PropertySummaryDocumentationMustOmitSetAccessorWithRestrictedAccess"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1624";
        private const string Title = "Property summary documentation must omit set accessor with restricted access";
        private const string MessageFormat = "TODO: Message format";
        private const string Description = "The documentation text within a C# property's <summary> tag takes into account all of the accessors within the property, but one of the accessors has limited access.";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1624.md";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.DocumentationRules, DiagnosticSeverity.Warning, AnalyzerConstants.DisabledNoTests, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            // TODO: Implement analysis
        }
    }
}
