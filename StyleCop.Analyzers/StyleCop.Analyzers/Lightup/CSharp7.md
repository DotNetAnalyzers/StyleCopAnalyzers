# C# 7 APIs supported via light-up

See [dotnet/roslyn@c2af711](https://github.com/dotnet/roslyn/commit/c2af71127234e2b6df231fad3b9f7db6cc7cb444).

## Semantics

* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.DisplayVersion.get -> bool`
* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.EmbeddedFiles.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.CommandLineSourceFile>`
* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.SourceLink.get -> string`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateAnonymousTypeSymbol(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> memberTypes, System.Collections.Immutable.ImmutableArray<string> memberNames, System.Collections.Immutable.ImmutableArray<bool> memberIsReadOnly = default(System.Collections.Immutable.ImmutableArray<bool>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> memberLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateErrorNamespaceSymbol(Microsoft.CodeAnalysis.INamespaceSymbol container, string name) -> Microsoft.CodeAnalysis.INamespaceSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateTupleTypeSymbol(Microsoft.CodeAnalysis.INamedTypeSymbol underlyingType, System.Collections.Immutable.ImmutableArray<string> elementNames = default(System.Collections.Immutable.ImmutableArray<string>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> elementLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateTupleTypeSymbol(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> elementTypes, System.Collections.Immutable.ImmutableArray<string> elementNames = default(System.Collections.Immutable.ImmutableArray<string>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> elementLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.Emit(System.IO.Stream peStream, System.IO.Stream pdbStream = null, System.IO.Stream xmlDocumentationStream = null, System.IO.Stream win32Resources = null, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.ResourceDescription> manifestResources = null, Microsoft.CodeAnalysis.Emit.EmitOptions options = null, Microsoft.CodeAnalysis.IMethodSymbol debugEntryPoint = null, System.IO.Stream sourceLinkStream = null, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.EmbeddedText> embeddedTexts = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.Emit.EmitResult`
* [ ] `Microsoft.CodeAnalysis.Compilation.Emit(System.IO.Stream peStream, System.IO.Stream pdbStream, System.IO.Stream xmlDocumentationStream, System.IO.Stream win32Resources, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.ResourceDescription> manifestResources, Microsoft.CodeAnalysis.Emit.EmitOptions options, Microsoft.CodeAnalysis.IMethodSymbol debugEntryPoint, System.Threading.CancellationToken cancellationToken) -> Microsoft.CodeAnalysis.Emit.EmitResult`
* [ ] `Microsoft.CodeAnalysis.Compilation.GetUnreferencedAssemblyIdentities(Microsoft.CodeAnalysis.Diagnostic diagnostic) -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.AssemblyIdentity>`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithConcurrentBuild(bool concurrent) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithCryptoKeyContainer(string cryptoKeyContainer) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithCryptoKeyFile(string cryptoKeyFile) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithCryptoPublicKey(System.Collections.Immutable.ImmutableArray<byte> cryptoPublicKey) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithDelaySign(bool? delaySign) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithMainTypeName(string mainTypeName) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithModuleName(string moduleName) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithOverflowChecks(bool checkOverflow) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CompilationOptions.WithScriptClassName(string scriptClassName) -> Microsoft.CodeAnalysis.CompilationOptions`
* [ ] `Microsoft.CodeAnalysis.DesktopStrongNameProvider.DesktopStrongNameProvider(System.Collections.Immutable.ImmutableArray<string> keyFileSearchPaths = default(System.Collections.Immutable.ImmutableArray<string>), string tempPath = null) -> void`
* [ ] `Microsoft.CodeAnalysis.DesktopStrongNameProvider.DesktopStrongNameProvider(System.Collections.Immutable.ImmutableArray<string> keyFileSearchPaths) -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.AnalyzerTelemetryInfo() -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.CodeBlockActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.CodeBlockEndActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.CodeBlockStartActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.CompilationActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.CompilationEndActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.CompilationStartActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.ExecutionTime.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.OperationActionsCount.get -> int`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.OperationActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.OperationBlockActionsCount.get -> int`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.OperationBlockActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.OperationBlockEndActionsCount.get -> int`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.OperationBlockEndActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.OperationBlockStartActionsCount.get -> int`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.OperationBlockStartActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SemanticModelActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SymbolActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SyntaxNodeActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SyntaxTreeActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.EmbeddedText`
* [ ] `Microsoft.CodeAnalysis.EmbeddedText.Checksum.get -> System.Collections.Immutable.ImmutableArray<byte>`
* [ ] `Microsoft.CodeAnalysis.EmbeddedText.ChecksumAlgorithm.get -> Microsoft.CodeAnalysis.Text.SourceHashAlgorithm`
* [ ] `Microsoft.CodeAnalysis.EmbeddedText.FilePath.get -> string`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.EmitOptions(bool metadataOnly = false, Microsoft.CodeAnalysis.Emit.DebugInformationFormat debugInformationFormat = (Microsoft.CodeAnalysis.Emit.DebugInformationFormat)0, string pdbFilePath = null, string outputNameOverride = null, int fileAlignment = 0, ulong baseAddress = 0, bool highEntropyVirtualAddressSpace = false, Microsoft.CodeAnalysis.SubsystemVersion subsystemVersion = default(Microsoft.CodeAnalysis.SubsystemVersion), string runtimeMetadataVersion = null, bool tolerateErrors = false, bool includePrivateMembers = false, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind> instrumentationKinds = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind>)) -> void`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.EmitOptions(bool metadataOnly, Microsoft.CodeAnalysis.Emit.DebugInformationFormat debugInformationFormat, string pdbFilePath, string outputNameOverride, int fileAlignment, ulong baseAddress, bool highEntropyVirtualAddressSpace, Microsoft.CodeAnalysis.SubsystemVersion subsystemVersion, string runtimeMetadataVersion, bool tolerateErrors, bool includePrivateMembers) -> void`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.InstrumentationKinds.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind>`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.WithInstrumentationKinds(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind> instrumentationKinds) -> Microsoft.CodeAnalysis.Emit.EmitOptions`
* [ ] `Microsoft.CodeAnalysis.Emit.InstrumentationKind`
* [ ] `Microsoft.CodeAnalysis.Emit.InstrumentationKind.None = 0 -> Microsoft.CodeAnalysis.Emit.InstrumentationKind`
* [ ] `Microsoft.CodeAnalysis.Emit.InstrumentationKind.TestCoverage = 1 -> Microsoft.CodeAnalysis.Emit.InstrumentationKind`
* [ ] `Microsoft.CodeAnalysis.IArrayTypeSymbol.IsSZArray.get -> bool`
* [ ] `Microsoft.CodeAnalysis.IArrayTypeSymbol.LowerBounds.get -> System.Collections.Immutable.ImmutableArray<int>`
* [ ] `Microsoft.CodeAnalysis.IArrayTypeSymbol.Sizes.get -> System.Collections.Immutable.ImmutableArray<int>`
* [ ] `Microsoft.CodeAnalysis.IDiscardSymbol`
* [ ] `Microsoft.CodeAnalysis.IDiscardSymbol.Type.get -> Microsoft.CodeAnalysis.ITypeSymbol`
* [x] `Microsoft.CodeAnalysis.IFieldSymbol.CorrespondingTupleField.get -> Microsoft.CodeAnalysis.IFieldSymbol`
* [ ] `Microsoft.CodeAnalysis.ILocalSymbol.IsRef.get -> bool`
* [ ] `Microsoft.CodeAnalysis.IMethodSymbol.RefCustomModifiers.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.CustomModifier>`
* [ ] `Microsoft.CodeAnalysis.IMethodSymbol.ReturnsByRef.get -> bool`
* [ ] `Microsoft.CodeAnalysis.INamedTypeSymbol.GetTypeArgumentCustomModifiers(int ordinal) -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.CustomModifier>`
* [ ] `Microsoft.CodeAnalysis.INamedTypeSymbol.IsComImport.get -> bool`
* [x] `Microsoft.CodeAnalysis.INamedTypeSymbol.TupleElements.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.IFieldSymbol>`
* [x] `Microsoft.CodeAnalysis.INamedTypeSymbol.TupleUnderlyingType.get -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.IParameterSymbol.RefCustomModifiers.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.CustomModifier>`
* [ ] `Microsoft.CodeAnalysis.IPropertySymbol.RefCustomModifiers.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.CustomModifier>`
* [ ] `Microsoft.CodeAnalysis.IPropertySymbol.ReturnsByRef.get -> bool`
* [x] `Microsoft.CodeAnalysis.ITypeSymbol.IsTupleType.get -> bool`
* [x] `Microsoft.CodeAnalysis.MethodKind.LocalFunction = 17 -> Microsoft.CodeAnalysis.MethodKind`
* [ ] `Microsoft.CodeAnalysis.PortableExecutableReference.GetMetadataId() -> Microsoft.CodeAnalysis.MetadataId`
* [ ] `Microsoft.CodeAnalysis.SymbolDisplayFormat.RemoveGenericsOptions(Microsoft.CodeAnalysis.SymbolDisplayGenericsOptions options) -> Microsoft.CodeAnalysis.SymbolDisplayFormat`
* [ ] `Microsoft.CodeAnalysis.SymbolDisplayFormat.RemoveLocalOptions(Microsoft.CodeAnalysis.SymbolDisplayLocalOptions options) -> Microsoft.CodeAnalysis.SymbolDisplayFormat`
* [ ] `Microsoft.CodeAnalysis.SymbolDisplayFormat.RemoveMiscellaneousOptions(Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions options) -> Microsoft.CodeAnalysis.SymbolDisplayFormat`
* [x] `Microsoft.CodeAnalysis.SymbolDisplayLocalOptions.IncludeRef = 4 -> Microsoft.CodeAnalysis.SymbolDisplayLocalOptions`
* [x] `Microsoft.CodeAnalysis.SymbolDisplayMemberOptions.IncludeRef = 128 -> Microsoft.CodeAnalysis.SymbolDisplayMemberOptions`
* [x] `Microsoft.CodeAnalysis.SymbolKind.Discard = 19 -> Microsoft.CodeAnalysis.SymbolKind`
* [ ] `Microsoft.CodeAnalysis.Text.SourceText.CanBeEmbedded.get -> bool`
* [ ] `Microsoft.CodeAnalysis.Text.SourceText.GetChecksum() -> System.Collections.Immutable.ImmutableArray<byte>`
* [ ] `abstract Microsoft.CodeAnalysis.CompilationOptions.Language.get -> string`
* [ ] `abstract Microsoft.CodeAnalysis.ParseOptions.Language.get -> string`
* [x] ~~`override Microsoft.CodeAnalysis.SyntaxNode.ToString() -> string`~~
* [ ] `static Microsoft.CodeAnalysis.CaseInsensitiveComparison.StartsWith(string value, string possibleStart) -> bool`
* [ ] `static Microsoft.CodeAnalysis.EmbeddedText.FromBytes(string filePath, System.ArraySegment<byte> bytes, Microsoft.CodeAnalysis.Text.SourceHashAlgorithm checksumAlgorithm = Microsoft.CodeAnalysis.Text.SourceHashAlgorithm.Sha1) -> Microsoft.CodeAnalysis.EmbeddedText`
* [ ] `static Microsoft.CodeAnalysis.EmbeddedText.FromSource(string filePath, Microsoft.CodeAnalysis.Text.SourceText text) -> Microsoft.CodeAnalysis.EmbeddedText`
* [ ] `static Microsoft.CodeAnalysis.EmbeddedText.FromStream(string filePath, System.IO.Stream stream, Microsoft.CodeAnalysis.Text.SourceHashAlgorithm checksumAlgorithm = Microsoft.CodeAnalysis.Text.SourceHashAlgorithm.Sha1) -> Microsoft.CodeAnalysis.EmbeddedText`
* [ ] `static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode>.implicit operator Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.SyntaxNode>(Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode> nodes) -> Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.SyntaxNode>`
* [ ] `static Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode>.implicit operator Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode>(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.SyntaxNode> nodes) -> Microsoft.CodeAnalysis.SeparatedSyntaxList<TNode>`
* [ ] `static Microsoft.CodeAnalysis.SyntaxNodeExtensions.WithoutTrivia(this Microsoft.CodeAnalysis.SyntaxToken token) -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `static Microsoft.CodeAnalysis.Text.SourceText.From(System.IO.Stream stream, System.Text.Encoding encoding = null, Microsoft.CodeAnalysis.Text.SourceHashAlgorithm checksumAlgorithm = Microsoft.CodeAnalysis.Text.SourceHashAlgorithm.Sha1, bool throwIfBinaryDetected = false, bool canBeEmbedded = false) -> Microsoft.CodeAnalysis.Text.SourceText`
* [ ] `static Microsoft.CodeAnalysis.Text.SourceText.From(System.IO.Stream stream, System.Text.Encoding encoding, Microsoft.CodeAnalysis.Text.SourceHashAlgorithm checksumAlgorithm, bool throwIfBinaryDetected) -> Microsoft.CodeAnalysis.Text.SourceText`
* [ ] `static Microsoft.CodeAnalysis.Text.SourceText.From(System.IO.TextReader reader, int length, System.Text.Encoding encoding = null, Microsoft.CodeAnalysis.Text.SourceHashAlgorithm checksumAlgorithm = Microsoft.CodeAnalysis.Text.SourceHashAlgorithm.Sha1) -> Microsoft.CodeAnalysis.Text.SourceText`
* [ ] `static Microsoft.CodeAnalysis.Text.SourceText.From(byte[] buffer, int length, System.Text.Encoding encoding = null, Microsoft.CodeAnalysis.Text.SourceHashAlgorithm checksumAlgorithm = Microsoft.CodeAnalysis.Text.SourceHashAlgorithm.Sha1, bool throwIfBinaryDetected = false, bool canBeEmbedded = false) -> Microsoft.CodeAnalysis.Text.SourceText`
* [ ] `static Microsoft.CodeAnalysis.Text.SourceText.From(byte[] buffer, int length, System.Text.Encoding encoding, Microsoft.CodeAnalysis.Text.SourceHashAlgorithm checksumAlgorithm, bool throwIfBinaryDetected) -> Microsoft.CodeAnalysis.Text.SourceText`
* [ ] `virtual Microsoft.CodeAnalysis.SymbolVisitor.VisitDiscard(Microsoft.CodeAnalysis.IDiscardSymbol symbol) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.SymbolVisitor<TResult>.VisitDiscard(Microsoft.CodeAnalysis.IDiscardSymbol symbol) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.SyntaxNode.ChildThatContainsPosition(int position) -> Microsoft.CodeAnalysis.SyntaxNodeOrToken`
* [ ] `virtual Microsoft.CodeAnalysis.SyntaxNode.SerializeTo(System.IO.Stream stream, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.SyntaxNode.ToFullString() -> string`
* [ ] `virtual Microsoft.CodeAnalysis.SyntaxNode.WriteTo(System.IO.TextWriter writer) -> void`

## Syntax

* [ ] `Microsoft.CodeAnalysis.CSharp.CSharpParseOptions.CSharpParseOptions(Microsoft.CodeAnalysis.CSharp.LanguageVersion languageVersion = Microsoft.CodeAnalysis.CSharp.LanguageVersion.Default, Microsoft.CodeAnalysis.DocumentationMode documentationMode = Microsoft.CodeAnalysis.DocumentationMode.Parse, Microsoft.CodeAnalysis.SourceCodeKind kind = Microsoft.CodeAnalysis.SourceCodeKind.Regular, System.Collections.Generic.IEnumerable<string> preprocessorSymbols = null) -> void`
* [ ] `Microsoft.CodeAnalysis.CSharp.CSharpParseOptions.SpecifiedLanguageVersion.get -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [ ] `Microsoft.CodeAnalysis.CSharp.Conversion.IsThrow.get -> bool`
* [ ] `Microsoft.CodeAnalysis.CSharp.Conversion.IsTupleConversion.get -> bool`
* [ ] `Microsoft.CodeAnalysis.CSharp.Conversion.IsTupleLiteralConversion.get -> bool`
* [x] `Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7 = 7 -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [x] `Microsoft.CodeAnalysis.CSharp.LanguageVersion.Default = 0 -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [x] `Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest = 2147483647 -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax.ExpressionBody.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax.Update(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax.WithExpressionBody(Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.Pattern.get -> Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax whenClause, Microsoft.CodeAnalysis.SyntaxToken colonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.WhenClause.get -> Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.WithColonToken(Microsoft.CodeAnalysis.SyntaxToken colonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.WithKeyword(Microsoft.CodeAnalysis.SyntaxToken keyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.WithPattern(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.WithWhenClause(Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax whenClause) -> Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax.Expression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax.WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax.Update(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax.WithExpressionBody(Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax.Designation.get -> Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax.Type.get -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax.WithDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax.WithType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax.Designation.get -> Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax.Type.get -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax.WithDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax.WithType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax.Update(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken tildeToken, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax.WithExpressionBody(Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax.UnderscoreToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken underscoreToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax.WithUnderscoreToken(Microsoft.CodeAnalysis.SyntaxToken underscoreToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken forEachKeyword, Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, Microsoft.CodeAnalysis.SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.Variable.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.WithCloseParenToken(Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.WithForEachKeyword(Microsoft.CodeAnalysis.SyntaxToken forEachKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.WithInKeyword(Microsoft.CodeAnalysis.SyntaxToken inKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.WithOpenParenToken(Microsoft.CodeAnalysis.SyntaxToken openParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.WithStatement(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.WithVariable(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.Expression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.IsKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.Pattern.get -> Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken isKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.WithIsKeyword(Microsoft.CodeAnalysis.SyntaxToken isKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.WithPattern(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.AddBodyStatements(params Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.AddConstraintClauses(params Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.AddParameterListParameters(params Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.AddTypeParameterListParameters(params Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.Body.get -> Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.ConstraintClauses.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax>`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.ExpressionBody.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.Identifier.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.ParameterList.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.ReturnType.get -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.SemicolonToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.TypeParameterList.get -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.Update(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithBody(Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithConstraintClauses(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithExpressionBody(Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithIdentifier(Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithParameterList(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithReturnType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithSemicolonToken(Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.WithTypeParameterList(Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.AddVariables(params Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.CloseParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.OpenParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax> variables, Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.Variables.get -> Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax>`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.WithCloseParenToken(Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.WithOpenParenToken(Microsoft.CodeAnalysis.SyntaxToken openParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.WithVariables(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax> variables) -> Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax.Expression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax.RefKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken refKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax.WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax.WithRefKeyword(Microsoft.CodeAnalysis.SyntaxToken refKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax.RefKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax.Type.get -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken refKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax.WithRefKeyword(Microsoft.CodeAnalysis.SyntaxToken refKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax.WithType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax.Identifier.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax.WithIdentifier(Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax.Expression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax.ThrowKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken throwKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax.WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax.WithThrowKeyword(Microsoft.CodeAnalysis.SyntaxToken throwKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax.Identifier.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax.Type.get -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax.WithIdentifier(Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax.WithType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.AddArguments(params Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.Arguments.get -> Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax>`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.CloseParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.OpenParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments, Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.WithArguments(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.WithCloseParenToken(Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.WithOpenParenToken(Microsoft.CodeAnalysis.SyntaxToken openParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.AddElements(params Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.CloseParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.Elements.get -> Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax>`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.OpenParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax> elements, Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.WithCloseParenToken(Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.WithElements(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax> elements) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.WithOpenParenToken(Microsoft.CodeAnalysis.SyntaxToken openParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax.Condition.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken whenKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition) -> Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax.WhenKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax.WithCondition(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition) -> Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax.WithWhenKeyword(Microsoft.CodeAnalysis.SyntaxToken whenKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.CasePatternSwitchLabel = 9009 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.ConstantPattern = 9002 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.DeclarationExpression = 9040 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.DeclarationPattern = 9000 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.DiscardDesignation = 9014 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.ForEachVariableStatement = 8929 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.IsPatternExpression = 8657 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.LocalFunctionStatement = 8830 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.ParenthesizedVariableDesignation = 8928 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.RefExpression = 9050 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.RefType = 9051 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleVariableDesignation = 8927 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.ThrowExpression = 9052 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.TupleElement = 8925 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.TupleExpression = 8926 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.TupleType = 8924 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.UnderscoreToken = 8491 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.WhenClause = 9013 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.ExpressionBody.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax`
* [x] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.CloseParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.Expression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.ForEachKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.InKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.OpenParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.Statement.get -> Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions.Language.get -> string`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpParseOptions.Language.get -> string`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitCasePatternSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitConstantPattern(Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitDeclarationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitDeclarationPattern(Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitDiscardDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitForEachVariableStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitIsPatternExpression(Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitLocalFunctionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitParenthesizedVariableDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitRefExpression(Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitRefType(Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitSingleVariableDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitThrowExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitTupleElement(Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitTupleExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitTupleType(Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitWhenClause(Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.ColonToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax.Keyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax.ExpressionBody.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax.ExpressionBody.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax.ExpressionBody.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.CloseParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.Expression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.ForEachKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.InKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.OpenParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.Statement.get -> Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.CloseParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.Expression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.ForEachKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.InKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.OpenParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.Statement.get -> Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax.ExpressionBody.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax.ExpressionBody.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetDeclaredSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax declaratorSyntax, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.ISymbol`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetDeclaredSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax designationSyntax, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.ISymbol`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetDeclaredSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax declarationSyntax, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.ISymbol`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetDeclaredSymbol(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax declaratorSyntax, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpExtensions.GetForEachStatementInfo(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax forEachStatement) -> Microsoft.CodeAnalysis.CSharp.ForEachStatementInfo`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorDeclaration(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind) -> Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorDeclaration(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body) -> Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorDeclaration(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorDeclaration(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorDeclaration(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.AccessorDeclaration(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.CasePatternSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax whenClause, Microsoft.CodeAnalysis.SyntaxToken colonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.CasePatternSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.SyntaxToken colonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.CasePatternSwitchLabel(Microsoft.CodeAnalysis.SyntaxToken keyword, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax whenClause, Microsoft.CodeAnalysis.SyntaxToken colonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ConstantPattern(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ConstructorDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ConstructorDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ConstructorDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ConstructorDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax initializer, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DeclarationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DeclarationPattern(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DestructorDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DestructorDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DestructorDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken tildeToken, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DestructorDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken tildeToken, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DiscardDesignation() -> Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DiscardDesignation(Microsoft.CodeAnalysis.SyntaxToken underscoreToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ForEachVariableStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ForEachVariableStatement(Microsoft.CodeAnalysis.SyntaxToken forEachKeyword, Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, Microsoft.CodeAnalysis.SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.IsPatternExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.IsPatternExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken isKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.LocalFunctionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.LocalFunctionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, string identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.LocalFunctionStatement(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.LocalFunctionStatement(Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax returnType, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList, Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses, Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body, Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParenthesizedVariableDesignation(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax> variables = default(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax>)) -> Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParenthesizedVariableDesignation(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax> variables, Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RefExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RefExpression(Microsoft.CodeAnalysis.SyntaxToken refKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RefType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RefType(Microsoft.CodeAnalysis.SyntaxToken refKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SingleVariableDesignation(Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ThrowExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ThrowExpression(Microsoft.CodeAnalysis.SyntaxToken throwKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.TupleElement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.TupleElement(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.TupleExpression(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments = default(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax>)) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.TupleExpression(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments, Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.TupleType(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax> elements = default(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax>)) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.TupleType(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax> elements, Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.WhenClause(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition) -> Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.WhenClause(Microsoft.CodeAnalysis.SyntaxToken whenKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition) -> Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitCasePatternSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitConstantPattern(Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitDeclarationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitDeclarationPattern(Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitDiscardDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitForEachVariableStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitIsPatternExpression(Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitLocalFunctionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitParenthesizedVariableDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitRefExpression(Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitRefType(Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitSingleVariableDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitThrowExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitTupleElement(Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitTupleExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitTupleType(Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitWhenClause(Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitCasePatternSwitchLabel(Microsoft.CodeAnalysis.CSharp.Syntax.CasePatternSwitchLabelSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitConstantPattern(Microsoft.CodeAnalysis.CSharp.Syntax.ConstantPatternSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitDeclarationExpression(Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitDeclarationPattern(Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitDiscardDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.DiscardDesignationSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitForEachVariableStatement(Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitIsPatternExpression(Microsoft.CodeAnalysis.CSharp.Syntax.IsPatternExpressionSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitLocalFunctionStatement(Microsoft.CodeAnalysis.CSharp.Syntax.LocalFunctionStatementSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitParenthesizedVariableDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitRefExpression(Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitRefType(Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitSingleVariableDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitThrowExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitTupleElement(Microsoft.CodeAnalysis.CSharp.Syntax.TupleElementSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitTupleExpression(Microsoft.CodeAnalysis.CSharp.Syntax.TupleExpressionSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitTupleType(Microsoft.CodeAnalysis.CSharp.Syntax.TupleTypeSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitWhenClause(Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax node) -> TResult`
