# C# 7.3 APIs supported via light-up

See [dotnet/roslyn@e60c9fe](https://github.com/dotnet/roslyn/commit/e60c9fe8accc442218b7858ca79f07b115e493b2).

## Semantics

* [ ] `abstract Microsoft.CodeAnalysis.DataFlowAnalysis.CapturedInside.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ISymbol>`
* [ ] `abstract Microsoft.CodeAnalysis.DataFlowAnalysis.CapturedOutside.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ISymbol>`
* [ ] `const Microsoft.CodeAnalysis.WellKnownMemberNames.DeconstructMethodName = "Deconstruct" -> string`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.MetadataImportOptions.get -> Microsoft.CodeAnalysis.MetadataImportOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithMetadataImportOptions(Microsoft.CodeAnalysis.MetadataImportOptions value) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.EmitOptions(bool metadataOnly = false, Microsoft.CodeAnalysis.Emit.DebugInformationFormat debugInformationFormat = (Microsoft.CodeAnalysis.Emit.DebugInformationFormat)0, string pdbFilePath = null, string outputNameOverride = null, int fileAlignment = 0, ulong baseAddress = 0, bool highEntropyVirtualAddressSpace = false, Microsoft.CodeAnalysis.SubsystemVersion subsystemVersion = default(Microsoft.CodeAnalysis.SubsystemVersion), string runtimeMetadataVersion = null, bool tolerateErrors = false, bool includePrivateMembers = true, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind> instrumentationKinds = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind>), System.Security.Cryptography.HashAlgorithmName? pdbChecksumAlgorithm = null) -> void`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.EmitOptions(bool metadataOnly, Microsoft.CodeAnalysis.Emit.DebugInformationFormat debugInformationFormat, string pdbFilePath, string outputNameOverride, int fileAlignment, ulong baseAddress, bool highEntropyVirtualAddressSpace, Microsoft.CodeAnalysis.SubsystemVersion subsystemVersion, string runtimeMetadataVersion, bool tolerateErrors, bool includePrivateMembers, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind> instrumentationKinds) -> void`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.PdbChecksumAlgorithm.get -> System.Security.Cryptography.HashAlgorithmName`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.WithPdbChecksumAlgorithm(System.Security.Cryptography.HashAlgorithmName name) -> Microsoft.CodeAnalysis.Emit.EmitOptions`
* [x] `Microsoft.CodeAnalysis.INamedTypeSymbol.IsSerializable.get -> bool`
* [x] `Microsoft.CodeAnalysis.ITypeParameterSymbol.HasUnmanagedTypeConstraint.get -> bool`
* [ ] `Microsoft.CodeAnalysis.MetadataImportOptions`
* [ ] `Microsoft.CodeAnalysis.MetadataImportOptions.All = 2 -> Microsoft.CodeAnalysis.MetadataImportOptions`
* [ ] `Microsoft.CodeAnalysis.MetadataImportOptions.Internal = 1 -> Microsoft.CodeAnalysis.MetadataImportOptions`
* [ ] `Microsoft.CodeAnalysis.MetadataImportOptions.Public = 0 -> Microsoft.CodeAnalysis.MetadataImportOptions`
* [ ] `Microsoft.CodeAnalysis.OperationKind.ConstructorBodyOperation = 89 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.Discard = 90 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.MethodBodyOperation = 88 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.TupleBinaryOperator = 87 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.Operations.IConstructorBodyOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IConstructorBodyOperation.Initializer.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IConstructorBodyOperation.Locals.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ILocalSymbol>`
* [ ] `Microsoft.CodeAnalysis.Operations.IDiscardOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IDiscardOperation.DiscardSymbol.get -> Microsoft.CodeAnalysis.IDiscardSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.ILocalFunctionOperation.IgnoredBody.get -> Microsoft.CodeAnalysis.Operations.IBlockOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IMethodBodyBaseOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IMethodBodyBaseOperation.BlockBody.get -> Microsoft.CodeAnalysis.Operations.IBlockOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IMethodBodyBaseOperation.ExpressionBody.get -> Microsoft.CodeAnalysis.Operations.IBlockOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IMethodBodyOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ISymbolInitializerOperation.Locals.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ILocalSymbol>`
* [ ] `Microsoft.CodeAnalysis.Operations.ITupleBinaryOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ITupleBinaryOperation.LeftOperand.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ITupleBinaryOperation.OperatorKind.get -> Microsoft.CodeAnalysis.Operations.BinaryOperatorKind`
* [ ] `Microsoft.CodeAnalysis.Operations.ITupleBinaryOperation.RightOperand.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ITupleOperation.NaturalType.get -> Microsoft.CodeAnalysis.ITypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Platform.Arm64 = 6 -> Microsoft.CodeAnalysis.Platform`
* [ ] `static Microsoft.CodeAnalysis.Diagnostic.Create(Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, Microsoft.CodeAnalysis.DiagnosticSeverity effectiveSeverity, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs) -> Microsoft.CodeAnalysis.Diagnostic`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitConstructorBodyOperation(Microsoft.CodeAnalysis.Operations.IConstructorBodyOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitDiscardOperation(Microsoft.CodeAnalysis.Operations.IDiscardOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitMethodBodyOperation(Microsoft.CodeAnalysis.Operations.IMethodBodyOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitTupleBinaryOperator(Microsoft.CodeAnalysis.Operations.ITupleBinaryOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitConstructorBodyOperation(Microsoft.CodeAnalysis.Operations.IConstructorBodyOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitDiscardOperation(Microsoft.CodeAnalysis.Operations.IDiscardOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitMethodBodyOperation(Microsoft.CodeAnalysis.Operations.IMethodBodyOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitTupleBinaryOperator(Microsoft.CodeAnalysis.Operations.ITupleBinaryOperation operation, TArgument argument) -> TResult`
* [ ] `Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind outputKind, bool reportSuppressedDiagnostics = false, string moduleName = null, string mainTypeName = null, string scriptClassName = null, System.Collections.Generic.IEnumerable<string> usings = null, Microsoft.CodeAnalysis.OptimizationLevel optimizationLevel = Microsoft.CodeAnalysis.OptimizationLevel.Debug, bool checkOverflow = false, bool allowUnsafe = false, string cryptoKeyContainer = null, string cryptoKeyFile = null, System.Collections.Immutable.ImmutableArray<byte> cryptoPublicKey = default(System.Collections.Immutable.ImmutableArray<byte>), bool? delaySign = null, Microsoft.CodeAnalysis.Platform platform = Microsoft.CodeAnalysis.Platform.AnyCpu, Microsoft.CodeAnalysis.ReportDiagnostic generalDiagnosticOption = Microsoft.CodeAnalysis.ReportDiagnostic.Default, int warningLevel = 4, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, Microsoft.CodeAnalysis.ReportDiagnostic>> specificDiagnosticOptions = null, bool concurrentBuild = true, bool deterministic = false, Microsoft.CodeAnalysis.XmlReferenceResolver xmlReferenceResolver = null, Microsoft.CodeAnalysis.SourceReferenceResolver sourceReferenceResolver = null, Microsoft.CodeAnalysis.MetadataReferenceResolver metadataReferenceResolver = null, Microsoft.CodeAnalysis.AssemblyIdentityComparer assemblyIdentityComparer = null, Microsoft.CodeAnalysis.StrongNameProvider strongNameProvider = null, bool publicSign = false, Microsoft.CodeAnalysis.MetadataImportOptions metadataImportOptions = Microsoft.CodeAnalysis.MetadataImportOptions.Public) -> void`
* [ ] `Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind outputKind, bool reportSuppressedDiagnostics, string moduleName, string mainTypeName, string scriptClassName, System.Collections.Generic.IEnumerable<string> usings, Microsoft.CodeAnalysis.OptimizationLevel optimizationLevel, bool checkOverflow, bool allowUnsafe, string cryptoKeyContainer, string cryptoKeyFile, System.Collections.Immutable.ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Microsoft.CodeAnalysis.Platform platform, Microsoft.CodeAnalysis.ReportDiagnostic generalDiagnosticOption, int warningLevel, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, Microsoft.CodeAnalysis.ReportDiagnostic>> specificDiagnosticOptions, bool concurrentBuild, bool deterministic, Microsoft.CodeAnalysis.XmlReferenceResolver xmlReferenceResolver, Microsoft.CodeAnalysis.SourceReferenceResolver sourceReferenceResolver, Microsoft.CodeAnalysis.MetadataReferenceResolver metadataReferenceResolver, Microsoft.CodeAnalysis.AssemblyIdentityComparer assemblyIdentityComparer, Microsoft.CodeAnalysis.StrongNameProvider strongNameProvider, bool publicSign) -> void`
* [ ] `Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions.WithMetadataImportOptions(Microsoft.CodeAnalysis.MetadataImportOptions value) -> Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CSharp.DeconstructionInfo`
* [ ] `Microsoft.CodeAnalysis.CSharp.DeconstructionInfo.Conversion.get -> Microsoft.CodeAnalysis.CSharp.Conversion?`
* [ ] `Microsoft.CodeAnalysis.CSharp.DeconstructionInfo.Method.get -> Microsoft.CodeAnalysis.IMethodSymbol`
* [ ] `Microsoft.CodeAnalysis.CSharp.DeconstructionInfo.Nested.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.CSharp.DeconstructionInfo>`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpCommandLineParser.Script.get -> Microsoft.CodeAnalysis.CSharp.CSharpCommandLineParser`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetDeconstructionInfo(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.AssignmentExpressionSyntax assignment) -> Microsoft.CodeAnalysis.CSharp.DeconstructionInfo`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetDeconstructionInfo(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax foreach) -> Microsoft.CodeAnalysis.CSharp.DeconstructionInfo`

## Syntax

* [x] `Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_3 = 703 -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax.RefKindKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax.WithRefKindKeyword(Microsoft.CodeAnalysis.SyntaxToken refKindKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax.RefKindKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax.WithRefKindKeyword(Microsoft.CodeAnalysis.SyntaxToken refKindKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.AddInitializerExpressions(params Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.CloseBracketToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.Initializer.get -> Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.OpenBracketToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.StackAllocKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken stackAllocKeyword, Microsoft.CodeAnalysis.SyntaxToken openBracketToken, Microsoft.CodeAnalysis.SyntaxToken closeBracketToken, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer) -> Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.WithCloseBracketToken(Microsoft.CodeAnalysis.SyntaxToken closeBracketToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.WithInitializer(Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer) -> Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.WithOpenBracketToken(Microsoft.CodeAnalysis.SyntaxToken openBracketToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.WithStackAllocKeyword(Microsoft.CodeAnalysis.SyntaxToken stackAllocKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax.Initializer.get -> Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken stackAllocKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer) -> Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax.WithInitializer(Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer) -> Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax.IsUnmanaged.get -> bool`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.ImplicitStackAllocArrayCreationExpression = 9053 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitImplicitStackAllocArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ImplicitStackAllocArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer) -> Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ImplicitStackAllocArrayCreationExpression(Microsoft.CodeAnalysis.SyntaxToken stackAllocKeyword, Microsoft.CodeAnalysis.SyntaxToken openBracketToken, Microsoft.CodeAnalysis.SyntaxToken closeBracketToken, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer) -> Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.StackAllocArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer) -> Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.StackAllocArrayCreationExpression(Microsoft.CodeAnalysis.SyntaxToken stackAllocKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.InitializerExpressionSyntax initializer) -> Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitImplicitStackAllocArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitImplicitStackAllocArrayCreationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax node) -> TResult`
