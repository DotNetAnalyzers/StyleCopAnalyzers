namespace StyleCop.Analyzers.OrderingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// An element within a C# code file is out of order within regard to access level, in relation to other elements in
    /// the code.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when the code elements within a file do not follow a standard ordering
    /// scheme based on access level.</para>
    ///
    /// <para>To comply with this rule, adjacent elements of the same type must be positioned in the following order by
    /// access level:</para>
    ///
    /// <list type="bullet">
    /// <item>public</item>
    /// <item>internal</item>
    /// <item>protected internal</item>
    /// <item>protected</item>
    /// <item>private</item>
    /// </list>
    ///
    /// <para>Complying with a standard ordering scheme based on access level can increase the readability and
    /// maintainability of the file and make it easier to identify the public interface that is being exposed from a
    /// class.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SA1202ElementsMustBeOrderedByAccess : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1202ElementsMustBeOrderedByAccess"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1202";
        private const string Title = "Elements must be ordered by access";
        private const string MessageFormat = "{0} {1} must come before {2}.";
        private const string Description = "An element within a C# code file is out of order within regard to access level, in relation to other elements in the code.";
        private const string HelpLink = "http://www.stylecop.com/docs/SA1202.html";

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.OrderingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly ImmutableArray<DiagnosticDescriptor> SupportedDiagnosticsValue =
            ImmutableArray.Create(Descriptor);

        private static readonly Dictionary<AccessLevel, string> UpperAccessLevelNames = new Dictionary<AccessLevel, string>
        {
            [AccessLevel.Public] = "Public",
            [AccessLevel.Internal] = "Internal",
            [AccessLevel.ProtectedInternal] = "Protected internal",
            [AccessLevel.Protected] = "Protected",
            [AccessLevel.Private] = "Private"
        };

        private static readonly Dictionary<AccessLevel, string> LowerAccessLevelNames = new Dictionary<AccessLevel, string>
        {
            [AccessLevel.Public] = "public",
            [AccessLevel.Internal] = "internal",
            [AccessLevel.ProtectedInternal] = "protected internal",
            [AccessLevel.Protected] = "protected",
            [AccessLevel.Private] = "private"
        };

        private static readonly Dictionary<SyntaxKind, string> MemberNames = new Dictionary<SyntaxKind, string>
        {
            [SyntaxKind.DelegateDeclaration] = "delegates",
            [SyntaxKind.EnumDeclaration] = "enums",
            [SyntaxKind.InterfaceDeclaration] = "interfaces",
            [SyntaxKind.StructDeclaration] = "structs",
            [SyntaxKind.ClassDeclaration] = "classes",
            [SyntaxKind.FieldDeclaration] = "fields",
            [SyntaxKind.ConstructorDeclaration] = "constructors",
            [SyntaxKind.EventDeclaration] = "events",
            [SyntaxKind.PropertyDeclaration] = "properties",
            [SyntaxKind.IndexerDeclaration] = "indexers",
            [SyntaxKind.MethodDeclaration] = "methods",
            [SyntaxKind.ConversionOperatorDeclaration] = "conversions",
            [SyntaxKind.OperatorDeclaration] = "operators"
        };

        private enum AccessLevel
        {
            /// <summary>No access level specified.</summary>
            NotSpecified,

            /// <summary>Public access.</summary>
            Public,

            /// <summary>Internal access.</summary>
            Internal,

            /// <summary>Protected internal access.</summary>
            ProtectedInternal,

            /// <summary>Protected access.</summary>
            Protected,

            /// <summary>Private access.</summary>
            Private
        }

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return SupportedDiagnosticsValue;
            }
        }

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(HandleCompilationUnit, SyntaxKind.CompilationUnit);
            context.RegisterSyntaxNodeActionHonorExclusions(HandleTypeDeclaration, SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration);
        }

        private static void HandleCompilationUnit(SyntaxNodeAnalysisContext context)
        {
            var compilationUnit = (CompilationUnitSyntax)context.Node;

            HandleMemberList(context, compilationUnit.Members.Where(x => x.IsKind(SyntaxKind.EnumDeclaration)).ToList(), MemberNames[SyntaxKind.EnumDeclaration]);
            HandleMemberList(context, compilationUnit.Members.Where(x => x.IsKind(SyntaxKind.InterfaceDeclaration)).ToList(), MemberNames[SyntaxKind.InterfaceDeclaration]);
            HandleMemberList(context, compilationUnit.Members.Where(x => x.IsKind(SyntaxKind.StructDeclaration)).ToList(), MemberNames[SyntaxKind.StructDeclaration]);
            HandleMemberList(context, compilationUnit.Members.Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).ToList(), MemberNames[SyntaxKind.ClassDeclaration]);
        }

        private static void HandleTypeDeclaration(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;

            HandleMemberList(
                context,
                typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.FieldDeclaration) && x.GetModifiers().Any(SyntaxKind.ConstKeyword)).ToList(),
                MemberNames[SyntaxKind.FieldDeclaration]);
            HandleMemberList(
                context,
                typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.FieldDeclaration) && x.GetModifiers().Any(SyntaxKind.ReadOnlyKeyword)).ToList(),
                MemberNames[SyntaxKind.FieldDeclaration]);
            HandleMemberList(
                context,
                typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.FieldDeclaration) && x.GetModifiers().All(field => !field.IsKind(SyntaxKind.ConstKeyword) && !field.IsKind(SyntaxKind.ReadOnlyKeyword))).ToList(),
                MemberNames[SyntaxKind.FieldDeclaration]);

            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.ConstructorDeclaration)).ToList(), MemberNames[SyntaxKind.ConstructorDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.DelegateDeclaration)).ToList(), MemberNames[SyntaxKind.DelegateDeclaration]);
            HandleMemberList(
                context,
                typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.EventDeclaration) || x.IsKind(SyntaxKind.EventFieldDeclaration)).ToList(),
                MemberNames[SyntaxKind.EventDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.EnumDeclaration)).ToList(), MemberNames[SyntaxKind.EnumDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.InterfaceDeclaration)).ToList(), MemberNames[SyntaxKind.InterfaceDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.PropertyDeclaration)).ToList(), MemberNames[SyntaxKind.PropertyDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.IndexerDeclaration)).ToList(), MemberNames[SyntaxKind.IndexerDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.ConversionOperatorDeclaration)).ToList(), MemberNames[SyntaxKind.ConversionOperatorDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.OperatorDeclaration)).ToList(), MemberNames[SyntaxKind.OperatorDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.MethodDeclaration)).ToList(), MemberNames[SyntaxKind.MethodDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.StructDeclaration)).ToList(), MemberNames[SyntaxKind.StructDeclaration]);
            HandleMemberList(context, typeDeclaration.Members.Where(x => x.IsKind(SyntaxKind.ClassDeclaration)).ToList(), MemberNames[SyntaxKind.ClassDeclaration]);
        }

        private static void HandleMemberList(SyntaxNodeAnalysisContext context, IList<MemberDeclarationSyntax> members, string memberType)
        {
            for (int i = 0; i < members.Count - 1; ++i)
            {
                var currentAccessLevel = GetAccessLevel(members[i]);
                if (currentAccessLevel == AccessLevel.NotSpecified)
                {
                    continue;
                }

                var nextAccessLevel = GetAccessLevel(members[i + 1]);
                if (nextAccessLevel == AccessLevel.NotSpecified)
                {
                    continue;
                }

                if (currentAccessLevel > nextAccessLevel)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(Descriptor,
                        NamedTypeHelpers.GetNameOrIdentifierLocation(members[i + 1]),
                        UpperAccessLevelNames[nextAccessLevel],
                        memberType,
                        LowerAccessLevelNames[currentAccessLevel]));
                }
            }
        }

        private static AccessLevel GetAccessLevel(MemberDeclarationSyntax member)
        {
            bool isProtected = false;
            bool isInternal = false;
            foreach (var modifier in member.GetModifiers())
            {
                switch (modifier.Kind())
                {
                case SyntaxKind.PublicKeyword:
                    return AccessLevel.Public;
                case SyntaxKind.PrivateKeyword:
                    return AccessLevel.Private;
                case SyntaxKind.InternalKeyword:
                    if (isProtected)
                    {
                        return AccessLevel.ProtectedInternal;
                    }
                    else
                    {
                        isInternal = true;
                    }

                    break;
                case SyntaxKind.ProtectedKeyword:
                    if (isInternal)
                    {
                        return AccessLevel.ProtectedInternal;
                    }
                    else
                    {
                        isProtected = true;
                    }

                    break;
                }
            }

            if (isProtected)
            {
                return AccessLevel.Protected;
            }
            else if (isInternal)
            {
                return AccessLevel.Internal;
            }

            return AccessLevel.NotSpecified;
        }
    }
}
