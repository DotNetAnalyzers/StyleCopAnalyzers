// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    [Generator]
    internal sealed class OperationLightupGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var operationInterfacesFile = context.AdditionalFiles.Single(x => Path.GetFileName(x.Path) == "OperationInterfaces.xml");
            var operationInterfacesText = operationInterfacesFile.GetText(context.CancellationToken);
            if (operationInterfacesText is null)
            {
                throw new InvalidOperationException("Failed to read OperationInterfaces.xml");
            }

            var operationInterfaces = XDocument.Parse(operationInterfacesText.ToString());
            this.GenerateOperationInterfaces(in context, operationInterfaces);
        }

        private void GenerateOperationInterfaces(in GeneratorExecutionContext context, XDocument operationInterfaces)
        {
            var tree = operationInterfaces.XPathSelectElement("/Tree");
            if (tree is null)
            {
                throw new InvalidOperationException("Failed to find the IOperation root.");
            }

            var documentData = new DocumentData(operationInterfaces);
            foreach (var pair in documentData.Interfaces)
            {
                this.GenerateOperationInterface(in context, pair.Value);
            }

            this.GenerateOperationWrapperHelper(in context, documentData.Interfaces.Values.ToImmutableArray());
        }

        private void GenerateOperationInterface(in GeneratorExecutionContext context, InterfaceData node)
        {
            var members = SyntaxFactory.List<MemberDeclarationSyntax>();

            // internal const string WrappedTypeName = "Microsoft.CodeAnalysis.Operations.IArgumentOperation";
            members = members.Add(SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.ConstKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(
                        identifier: SyntaxFactory.Identifier("WrappedTypeName"),
                        argumentList: null,
                        initializer: SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("Microsoft.CodeAnalysis.Operations." + node.InterfaceName))))))));

            // private static readonly Type WrappedType;
            members = members.Add(SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName("Type"),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("WrappedType")))));

            foreach (var property in node.Properties)
            {
                if (property.IsSkipped)
                {
                    continue;
                }

                // private static readonly Func<IOperation, IMethodSymbol> ConstructorAccessor;
                members = members.Add(SyntaxFactory.FieldDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                    declaration: SyntaxFactory.VariableDeclaration(
                        type: SyntaxFactory.GenericName(
                            identifier: SyntaxFactory.Identifier("Func"),
                            typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.IdentifierName("IOperation"),
                                    property.AccessorResultType,
                                }))),
                        variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(property.AccessorName)))));
            }

            // private readonly IOperation operation;
            members = members.Add(SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName("IOperation"),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator("operation")))));

            var staticCtorStatements = SyntaxFactory.SingletonList<StatementSyntax>(
                SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    left: SyntaxFactory.IdentifierName("WrappedType"),
                    right: SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("OperationWrapperHelper"),
                            name: SyntaxFactory.IdentifierName("GetWrappedType")),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                            SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(node.WrapperName)))))))));

            foreach (var property in node.Properties)
            {
                if (property.IsSkipped)
                {
                    continue;
                }

                SimpleNameSyntax helperName;
                if (property.IsDerivedOperationArray)
                {
                    // CreateOperationListPropertyAccessor<IOperation>
                    helperName = SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("CreateOperationListPropertyAccessor"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.IdentifierName("IOperation"))));
                }
                else
                {
                    // CreateOperationPropertyAccessor<IOperation, IMethodSymbol>
                    helperName = SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("CreateOperationPropertyAccessor"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(
                            new[]
                            {
                                SyntaxFactory.IdentifierName("IOperation"),
                                property.AccessorResultType,
                            })));
                }

                // ConstructorAccessor = LightupHelpers.CreateOperationPropertyAccessor<IOperation, IMethodSymbol>(WrappedType, nameof(Constructor));
                staticCtorStatements = staticCtorStatements.Add(SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    left: SyntaxFactory.IdentifierName(property.AccessorName),
                    right: SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("LightupHelpers"),
                            name: helperName),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                            new[]
                            {
                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("WrappedType")),
                                SyntaxFactory.Argument(SyntaxFactory.InvocationExpression(
                                    expression: SyntaxFactory.IdentifierName("nameof"),
                                    argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName(property.Name)))))),
                            }))))));
            }

            // static IArgumentOperationWrapper()
            // {
            //     WrappedType = OperationWrapperHelper.GetWrappedType(typeof(IObjectCreationOperationWrapper));
            // }
            members = members.Add(SyntaxFactory.ConstructorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                identifier: SyntaxFactory.Identifier(node.WrapperName),
                parameterList: SyntaxFactory.ParameterList(),
                initializer: null,
                body: SyntaxFactory.Block(staticCtorStatements)));

            // private IArgumentOperationWrapper(IOperation operation)
            // {
            //     this.operation = operation;
            // }
            members = members.Add(SyntaxFactory.ConstructorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)),
                identifier: SyntaxFactory.Identifier(node.WrapperName),
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName("IOperation"),
                    identifier: SyntaxFactory.Identifier("operation"),
                    @default: null))),
                initializer: null,
                body: SyntaxFactory.Block(
                    SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        left: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.ThisExpression(),
                            name: SyntaxFactory.IdentifierName("operation")),
                        right: SyntaxFactory.IdentifierName("operation"))))));

            // public IOperation WrappedOperation => this.operation;
            members = members.Add(SyntaxFactory.PropertyDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                type: SyntaxFactory.IdentifierName("IOperation"),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier("WrappedOperation"),
                accessorList: null,
                expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    expression: SyntaxFactory.ThisExpression(),
                    name: SyntaxFactory.IdentifierName("operation"))),
                initializer: null,
                semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            // public ITypeSymbol Type => this.WrappedOperation.Type;
            members = members.Add(SyntaxFactory.PropertyDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                type: SyntaxFactory.IdentifierName("ITypeSymbol"),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier("Type"),
                accessorList: null,
                expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    expression: SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        expression: SyntaxFactory.ThisExpression(),
                        name: SyntaxFactory.IdentifierName("WrappedOperation")),
                    name: SyntaxFactory.IdentifierName("Type"))),
                initializer: null,
                semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            foreach (var property in node.Properties)
            {
                if (property.IsSkipped)
                {
                    // Generate a NotImplementedException for public properties that do not have a supported type
                    if (property.IsPublicProperty)
                    {
                        // public object Constructor => throw new NotImplementedException("Property 'Type.Property' has unsupported type 'Type'");
                        members = members.Add(SyntaxFactory.PropertyDeclaration(
                            attributeLists: default,
                            modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                            type: SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword)),
                            explicitInterfaceSpecifier: null,
                            identifier: SyntaxFactory.Identifier(property.Name),
                            accessorList: null,
                            expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ThrowExpression(SyntaxFactory.ObjectCreationExpression(
                                type: SyntaxFactory.IdentifierName(nameof(NotImplementedException)),
                                argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal($"Property '{node.InterfaceName}.{property.Name}' has unsupported type '{property.Type}'"))))),
                                initializer: null))),
                            initializer: null,
                            semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
                    }

                    continue;
                }

                var propertyType = property.NeedsWrapper ? SyntaxFactory.IdentifierName(property.Type + "Wrapper") : property.AccessorResultType;

                // ConstructorAccessor(this.WrappedOperation)
                var evaluatedAccessor = SyntaxFactory.InvocationExpression(
                    expression: SyntaxFactory.IdentifierName(property.AccessorName),
                    argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.ThisExpression(),
                            name: SyntaxFactory.IdentifierName("WrappedOperation"))))));

                ExpressionSyntax convertedResult;
                if (property.NeedsWrapper)
                {
                    // IObjectOrCollectionInitializerOperationWrapper.FromOperation(...)
                    convertedResult = SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: propertyType,
                            name: SyntaxFactory.IdentifierName("FromOperation")),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(evaluatedAccessor))));
                }
                else
                {
                    convertedResult = evaluatedAccessor;
                }

                // public IMethodSymbol Constructor => ConstructorAccessor(this.WrappedOperation);
                members = members.Add(SyntaxFactory.PropertyDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    type: propertyType,
                    explicitInterfaceSpecifier: null,
                    identifier: SyntaxFactory.Identifier(property.Name),
                    accessorList: null,
                    expressionBody: SyntaxFactory.ArrowExpressionClause(convertedResult),
                    initializer: null,
                    semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            if (node.BaseInterface is { } baseDefinition)
            {
                var inheritedProperties = baseDefinition.Properties;
                foreach (var property in inheritedProperties)
                {
                    if (node.Properties.Any(derivedProperty => derivedProperty.Name == property.Name && derivedProperty.IsNew))
                    {
                        continue;
                    }

                    if (!property.IsPublicProperty)
                    {
                        continue;
                    }

                    var propertyType = property.IsSkipped
                        ? SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.ObjectKeyword))
                        : property.NeedsWrapper ? SyntaxFactory.IdentifierName(property.Type + "Wrapper") : property.AccessorResultType;

                    // public IOperation Instance => ((IMemberReferenceOperationWrapper)this).Instance;
                    members = members.Add(SyntaxFactory.PropertyDeclaration(
                        attributeLists: default,
                        modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                        type: propertyType,
                        explicitInterfaceSpecifier: null,
                        identifier: SyntaxFactory.Identifier(property.Name),
                        accessorList: null,
                        expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.ParenthesizedExpression(SyntaxFactory.CastExpression(
                                type: SyntaxFactory.IdentifierName(baseDefinition.WrapperName),
                                expression: SyntaxFactory.ThisExpression())),
                            name: SyntaxFactory.IdentifierName(property.Name))),
                        initializer: null,
                        semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
                }

                // public static explicit operator IFieldReferenceOperationWrapper(IMemberReferenceOperationWrapper wrapper)
                //     => FromOperation(wrapper.WrappedOperation);
                members = members.Add(SyntaxFactory.ConversionOperatorDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                    implicitOrExplicitKeyword: SyntaxFactory.Token(SyntaxKind.ExplicitKeyword),
                    operatorKeyword: SyntaxFactory.Token(SyntaxKind.OperatorKeyword),
                    type: SyntaxFactory.IdentifierName(node.WrapperName),
                    parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                        attributeLists: default,
                        modifiers: default,
                        type: SyntaxFactory.IdentifierName(baseDefinition.WrapperName),
                        identifier: SyntaxFactory.Identifier("wrapper"),
                        @default: null))),
                    body: null,
                    expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.IdentifierName("FromOperation"),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName("wrapper"),
                                name: SyntaxFactory.IdentifierName("WrappedOperation"))))))),
                    semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                // public static implicit operator IMemberReferenceOperationWrapper(IFieldReferenceOperationWrapper wrapper)
                //     => IMemberReferenceOperationWrapper.FromUpcast(wrapper.WrappedOperation);
                members = members.Add(SyntaxFactory.ConversionOperatorDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                    implicitOrExplicitKeyword: SyntaxFactory.Token(SyntaxKind.ImplicitKeyword),
                    operatorKeyword: SyntaxFactory.Token(SyntaxKind.OperatorKeyword),
                    type: SyntaxFactory.IdentifierName(baseDefinition.WrapperName),
                    parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                        attributeLists: default,
                        modifiers: default,
                        type: SyntaxFactory.IdentifierName(node.WrapperName),
                        identifier: SyntaxFactory.Identifier("wrapper"),
                        @default: null))),
                    body: null,
                    expressionBody: SyntaxFactory.ArrowExpressionClause(SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName(baseDefinition.WrapperName),
                            name: SyntaxFactory.IdentifierName("FromUpcast")),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName("wrapper"),
                                name: SyntaxFactory.IdentifierName("WrappedOperation"))))))),
                    semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
            }

            // public static IArgumentOperationWrapper FromOperation(IOperation operation)
            // {
            //     if (operation == null)
            //     {
            //         return default;
            //     }
            //
            //     if (!IsInstance(operation))
            //     {
            //         throw new InvalidCastException($"Cannot cast '{operation.GetType().FullName}' to '{WrappedTypeName}'");
            //     }
            //
            //     return new IArgumentOperationWrapper(operation);
            // }
            members = members.Add(SyntaxFactory.MethodDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                returnType: SyntaxFactory.IdentifierName(node.WrapperName),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier("FromOperation"),
                typeParameterList: null,
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName("IOperation"),
                    identifier: SyntaxFactory.Identifier("operation"),
                    @default: null))),
                constraintClauses: default,
                body: SyntaxFactory.Block(
                    SyntaxFactory.IfStatement(
                        condition: SyntaxFactory.BinaryExpression(
                            SyntaxKind.EqualsExpression,
                            left: SyntaxFactory.IdentifierName("operation"),
                            right: SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                        statement: SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)))),
                    SyntaxFactory.IfStatement(
                        condition: SyntaxFactory.PrefixUnaryExpression(
                            SyntaxKind.LogicalNotExpression,
                            operand: SyntaxFactory.InvocationExpression(
                                expression: SyntaxFactory.IdentifierName("IsInstance"),
                                argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("operation")))))),
                        statement: SyntaxFactory.Block(
                            SyntaxFactory.ThrowStatement(SyntaxFactory.ObjectCreationExpression(
                                type: SyntaxFactory.IdentifierName("InvalidCastException"),
                                argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                    SyntaxFactory.InterpolatedStringExpression(
                                        SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken),
                                        SyntaxFactory.List(new InterpolatedStringContentSyntax[]
                                        {
                                            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                                                leading: default,
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "Cannot cast '",
                                                "Cannot cast '",
                                                trailing: default)),
                                            SyntaxFactory.Interpolation(SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                expression: SyntaxFactory.InvocationExpression(
                                                    expression: SyntaxFactory.MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        expression: SyntaxFactory.IdentifierName("operation"),
                                                        name: SyntaxFactory.IdentifierName("GetType")),
                                                    argumentList: SyntaxFactory.ArgumentList()),
                                                name: SyntaxFactory.IdentifierName("FullName"))),
                                            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                                                leading: default,
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "' to '",
                                                "' to '",
                                                trailing: default)),
                                            SyntaxFactory.Interpolation(SyntaxFactory.IdentifierName("WrappedTypeName")),
                                            SyntaxFactory.InterpolatedStringText(SyntaxFactory.Token(
                                                leading: default,
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "'",
                                                "'",
                                                trailing: default)),
                                        }))))),
                                initializer: null)))),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.ObjectCreationExpression(
                        type: SyntaxFactory.IdentifierName(node.WrapperName),
                        argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("operation")))),
                        initializer: null))),
                expressionBody: null));

            // public static bool IsInstance(IOperation operation)
            // {
            //     return operation != null && LightupHelpers.CanWrapOperation(operation, WrappedType);
            // }
            members = members.Add(SyntaxFactory.MethodDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                returnType: SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier("IsInstance"),
                typeParameterList: null,
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName("IOperation"),
                    identifier: SyntaxFactory.Identifier("operation"),
                    @default: null))),
                constraintClauses: default,
                body: SyntaxFactory.Block(
                    SyntaxFactory.ReturnStatement(SyntaxFactory.BinaryExpression(
                        SyntaxKind.LogicalAndExpression,
                        left: SyntaxFactory.BinaryExpression(
                            SyntaxKind.NotEqualsExpression,
                            left: SyntaxFactory.IdentifierName("operation"),
                            right: SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
                        right: SyntaxFactory.InvocationExpression(
                            expression: SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName("LightupHelpers"),
                                name: SyntaxFactory.IdentifierName("CanWrapOperation")),
                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("operation")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("WrappedType")),
                                })))))),
                expressionBody: null));

            if (node.IsAbstract)
            {
                // internal static IMemberReferenceOperationWrapper FromUpcast(IOperation operation)
                // {
                //     return new IMemberReferenceOperationWrapper(operation);
                // }
                members = members.Add(SyntaxFactory.MethodDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                    returnType: SyntaxFactory.IdentifierName(node.WrapperName),
                    explicitInterfaceSpecifier: null,
                    identifier: SyntaxFactory.Identifier("FromUpcast"),
                    typeParameterList: null,
                    parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                        attributeLists: default,
                        modifiers: default,
                        type: SyntaxFactory.IdentifierName("IOperation"),
                        identifier: SyntaxFactory.Identifier("operation"),
                        @default: null))),
                    constraintClauses: default,
                    body: SyntaxFactory.Block(
                        SyntaxFactory.ReturnStatement(SyntaxFactory.ObjectCreationExpression(
                            type: SyntaxFactory.IdentifierName(node.WrapperName),
                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(SyntaxFactory.IdentifierName("operation")))),
                            initializer: null))),
                    expressionBody: null));
            }

            var wrapperStruct = SyntaxFactory.StructDeclaration(
                attributeLists: default,
                modifiers: SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.InternalKeyword)).Add(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                identifier: SyntaxFactory.Identifier(node.WrapperName),
                typeParameterList: null,
                baseList: SyntaxFactory.BaseList(SyntaxFactory.SingletonSeparatedList<BaseTypeSyntax>(SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("IOperationWrapper")))),
                constraintClauses: default,
                members: members);
            var wrapperNamespace = SyntaxFactory.NamespaceDeclaration(
                name: SyntaxFactory.ParseName("StyleCop.Analyzers.Lightup"),
                externs: default,
                usings: SyntaxFactory.List<UsingDirectiveSyntax>()
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Immutable")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.CodeAnalysis"))),
                members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(wrapperStruct));

            wrapperNamespace = wrapperNamespace
                .NormalizeWhitespace()
                .WithLeadingTrivia(
                    SyntaxFactory.Comment("// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved."),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment("// Licensed under the MIT License. See LICENSE in the project root for license information."),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.CarriageReturnLineFeed)
                .WithTrailingTrivia(
                    SyntaxFactory.CarriageReturnLineFeed);

            context.AddSource(node.WrapperName + ".g.cs", SourceText.From(wrapperNamespace.ToFullString(), Encoding.UTF8));
        }

        private void GenerateOperationWrapperHelper(in GeneratorExecutionContext context, ImmutableArray<InterfaceData> wrapperTypes)
        {
            // private static readonly ImmutableDictionary<Type, Type> WrappedTypes;
            var wrappedTypes = SyntaxFactory.FieldDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                declaration: SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("ImmutableDictionary"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                new[]
                                {
                                    SyntaxFactory.IdentifierName("Type"),
                                    SyntaxFactory.IdentifierName("Type"),
                                }))),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("WrappedTypes")))));

            // var codeAnalysisAssembly = typeof(SyntaxNode).GetTypeInfo().Assembly;
            // var builder = ImmutableDictionary.CreateBuilder<Type, Type>();
            var staticCtorStatements = SyntaxFactory.List<StatementSyntax>()
                .Add(SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName("var"),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(
                        identifier: SyntaxFactory.Identifier("codeAnalysisAssembly"),
                        argumentList: null,
                        initializer: SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        expression: SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName("SyntaxNode")),
                                        name: SyntaxFactory.IdentifierName("GetTypeInfo"))),
                                name: SyntaxFactory.IdentifierName("Assembly"))))))))
                .Add(SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(
                    type: SyntaxFactory.IdentifierName("var"),
                    variables: SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(
                        identifier: SyntaxFactory.Identifier("builder"),
                        argumentList: null,
                        initializer: SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    expression: SyntaxFactory.IdentifierName("ImmutableDictionary"),
                                    name: SyntaxFactory.GenericName(
                                        identifier: SyntaxFactory.Identifier("CreateBuilder"),
                                        typeArgumentList: SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                                new[]
                                                {
                                                    SyntaxFactory.IdentifierName("Type"),
                                                    SyntaxFactory.IdentifierName("Type"),
                                                })))))))))));

            foreach (var node in wrapperTypes)
            {
                // builder.Add(typeof(IArgumentOperationWrapper), codeAnalysisAssembly.GetType(IArgumentOperationWrapper.WrappedTypeName));
                staticCtorStatements = staticCtorStatements.Add(SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        expression: SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("builder"),
                            name: SyntaxFactory.IdentifierName("Add")),
                        argumentList: SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.Argument(SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(node.WrapperName))),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                            expression: SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                expression: SyntaxFactory.IdentifierName("codeAnalysisAssembly"),
                                                name: SyntaxFactory.IdentifierName("GetType")),
                                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    expression: SyntaxFactory.IdentifierName(node.WrapperName),
                                                    name: SyntaxFactory.IdentifierName("WrappedTypeName"))))))),
                                })))));
            }

            // WrappedTypes = builder.ToImmutable();
            staticCtorStatements = staticCtorStatements.Add(SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    left: SyntaxFactory.IdentifierName("WrappedTypes"),
                    right: SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            expression: SyntaxFactory.IdentifierName("builder"),
                            name: SyntaxFactory.IdentifierName("ToImmutable"))))));

            var staticCtor = SyntaxFactory.ConstructorDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                identifier: SyntaxFactory.Identifier("OperationWrapperHelper"),
                parameterList: SyntaxFactory.ParameterList(),
                initializer: null,
                body: SyntaxFactory.Block(staticCtorStatements));

            // internal static Type GetWrappedType(Type wrapperType)
            // {
            //     if (WrappedTypes.TryGetValue(wrapperType, out Type wrappedType))
            //     {
            //         return wrappedType;
            //     }
            //
            //     return null;
            // }
            var getWrappedType = SyntaxFactory.MethodDeclaration(
                attributeLists: default,
                modifiers: SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                returnType: SyntaxFactory.IdentifierName("Type"),
                explicitInterfaceSpecifier: null,
                identifier: SyntaxFactory.Identifier("GetWrappedType"),
                typeParameterList: null,
                parameterList: SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Parameter(
                    attributeLists: default,
                    modifiers: default,
                    type: SyntaxFactory.IdentifierName("Type"),
                    identifier: SyntaxFactory.Identifier("wrapperType"),
                    @default: null))),
                constraintClauses: default,
                body: SyntaxFactory.Block(
                    SyntaxFactory.IfStatement(
                        condition: SyntaxFactory.InvocationExpression(
                            expression: SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                expression: SyntaxFactory.IdentifierName("WrappedTypes"),
                                name: SyntaxFactory.IdentifierName("TryGetValue")),
                            argumentList: SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("wrapperType")),
                                    SyntaxFactory.Argument(
                                        nameColon: null,
                                        refKindKeyword: SyntaxFactory.Token(SyntaxKind.OutKeyword),
                                        expression: SyntaxFactory.DeclarationExpression(
                                            type: SyntaxFactory.IdentifierName("Type"),
                                            designation: SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("wrappedType")))),
                                }))),
                        statement: SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName("wrappedType")))),
                    SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))),
                expressionBody: null);

            var wrapperHelperClass = SyntaxFactory.ClassDeclaration(
                attributeLists: default,
                modifiers: SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.InternalKeyword)).Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                identifier: SyntaxFactory.Identifier("OperationWrapperHelper"),
                typeParameterList: null,
                baseList: null,
                constraintClauses: default,
                members: SyntaxFactory.List<MemberDeclarationSyntax>()
                    .Add(wrappedTypes)
                    .Add(staticCtor)
                    .Add(getWrappedType));
            var wrapperNamespace = SyntaxFactory.NamespaceDeclaration(
                name: SyntaxFactory.ParseName("StyleCop.Analyzers.Lightup"),
                externs: default,
                usings: SyntaxFactory.List<UsingDirectiveSyntax>()
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Collections.Immutable")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reflection")))
                    .Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Microsoft.CodeAnalysis"))),
                members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(wrapperHelperClass));

            wrapperNamespace = wrapperNamespace
                .NormalizeWhitespace()
                .WithLeadingTrivia(
                    SyntaxFactory.Comment("// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved."),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.Comment("// Licensed under the MIT License. See LICENSE in the project root for license information."),
                    SyntaxFactory.CarriageReturnLineFeed,
                    SyntaxFactory.CarriageReturnLineFeed)
                .WithTrailingTrivia(
                    SyntaxFactory.CarriageReturnLineFeed);

            context.AddSource("OperationWrapperHelper.g.cs", SourceText.From(wrapperNamespace.ToFullString(), Encoding.UTF8));
        }

        private sealed class DocumentData
        {
            public DocumentData(XDocument document)
            {
                var interfaces = new Dictionary<string, InterfaceData>();
                foreach (var node in document.XPathSelectElements("/Tree/AbstractNode"))
                {
                    var interfaceData = new InterfaceData(this, node);
                    interfaces.Add(interfaceData.InterfaceName, interfaceData);
                }

                foreach (var node in document.XPathSelectElements("/Tree/Node"))
                {
                    var interfaceData = new InterfaceData(this, node);
                    interfaces.Add(interfaceData.InterfaceName, interfaceData);
                }

                this.Interfaces = new ReadOnlyDictionary<string, InterfaceData>(interfaces);
            }

            public ReadOnlyDictionary<string, InterfaceData> Interfaces { get; }
        }

        private sealed class InterfaceData
        {
            private readonly DocumentData documentData;

            public InterfaceData(DocumentData documentData, XElement node)
            {
                this.documentData = documentData;

                this.InterfaceName = node.Attribute("Name").Value;
                this.WrapperName = this.InterfaceName + "Wrapper";
                this.BaseInterfaceName = node.Attribute("Base").Value;
                this.IsAbstract = node.Name == "AbstractNode";
                this.Properties = node.XPathSelectElements("Property").Select(property => new PropertyData(property)).ToImmutableArray();
            }

            public string InterfaceName { get; }

            public string WrapperName { get; }

            public string BaseInterfaceName { get; }

            public bool IsAbstract { get; }

            public ImmutableArray<PropertyData> Properties { get; }

            public InterfaceData BaseInterface
            {
                get
                {
                    if (this.documentData.Interfaces.TryGetValue(this.BaseInterfaceName, out var baseInterface))
                    {
                        return baseInterface;
                    }

                    return null;
                }
            }
        }

        private sealed class PropertyData
        {
            public PropertyData(XElement node)
            {
                this.Name = node.Attribute("Name").Value;
                this.AccessorName = this.Name + "Accessor";
                this.Type = node.Attribute("Type").Value;

                this.IsNew = node.Attribute("New")?.Value == "true";
                this.IsPublicProperty = node.Attribute("Internal")?.Value != "true";

                this.IsSkipped = this.Type switch
                {
                    "ArgumentKind" => true,
                    "BinaryOperatorKind" => true,
                    "BranchKind" => true,
                    "CaptureId" => true,
                    "CaseKind" => true,
                    "CommonConversion" => true,
                    "ForEachLoopOperationInfo" => true,
                    "IDiscardSymbol" => true,
                    "InstanceReferenceKind" => true,
                    "LoopKind" => true,
                    "PlaceholderKind" => true,
                    "UnaryOperatorKind" => true,
                    _ => !this.IsPublicProperty,
                };

                this.NeedsWrapper = IsAnyOperation(this.Type) && this.Type != "IOperation";
                this.IsDerivedOperationArray = IsAnyOperationArray(this.Type) && this.Type != "ImmutableArray<IOperation>";

                if (this.IsDerivedOperationArray)
                {
                    this.AccessorResultType = SyntaxFactory.GenericName(
                        identifier: SyntaxFactory.Identifier("ImmutableArray"),
                        typeArgumentList: SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList<TypeSyntax>(SyntaxFactory.IdentifierName("IOperation"))));
                }
                else if (IsAnyOperation(this.Type))
                {
                    this.AccessorResultType = SyntaxFactory.IdentifierName("IOperation");
                }
                else
                {
                    this.AccessorResultType = SyntaxFactory.ParseTypeName(this.Type);
                }
            }

            public bool IsNew { get; }

            public bool IsPublicProperty { get; }

            public bool IsSkipped { get; }

            public string Name { get; }

            public string AccessorName { get; }

            public string Type { get; }

            public bool NeedsWrapper { get; }

            public bool IsDerivedOperationArray { get; }

            public TypeSyntax AccessorResultType { get; }

            private static bool IsAnyOperation(string type)
            {
                return type.StartsWith("I") && type.EndsWith("Operation");
            }

            private static bool IsAnyOperationArray(string type)
            {
                return type.StartsWith("ImmutableArray<I") && type.EndsWith("Operation>");
            }
        }
    }
}
