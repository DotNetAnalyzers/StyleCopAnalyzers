# C# 8 APIs supported via light-up

## Semantics

## Syntax

## Uncategorized

See [Microsoft.CodeAnalysis release/dev16.3@c955f3c99b5698c906e0700ef691b5b1571c8136](https://raw.githubusercontent.com/dotnet/roslyn/c955f3c99b5698c906e0700ef691b5b1571c8136/src/Compilers/Core/Portable/PublicAPI.Unshipped.txt)

* [ ] `*REMOVED*Microsoft.CodeAnalysis.Operations.IEventAssignmentOperation.EventReference.get -> Microsoft.CodeAnalysis.Operations.IEventReferenceOperation`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.SpecialType.Count = 43 -> Microsoft.CodeAnalysis.SpecialType`
* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.EmitPdbFile.get -> bool`
* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.GetOutputFilePath(string outputFileName) -> string`
* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.GetPdbFilePath(string outputFileName) -> string`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.Compilation.CreateTupleTypeSymbol(Microsoft.CodeAnalysis.INamedTypeSymbol underlyingType, System.Collections.Immutable.ImmutableArray<string> elementNames = default(System.Collections.Immutable.ImmutableArray<string>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> elementLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateAnonymousTypeSymbol(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> memberTypes, System.Collections.Immutable.ImmutableArray<string> memberNames, System.Collections.Immutable.ImmutableArray<bool> memberIsReadOnly = default(System.Collections.Immutable.ImmutableArray<bool>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> memberLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> memberNullableAnnotations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.Compilation.CreateAnonymousTypeSymbol(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> memberTypes, System.Collections.Immutable.ImmutableArray<string> memberNames, System.Collections.Immutable.ImmutableArray<bool> memberIsReadOnly = default(System.Collections.Immutable.ImmutableArray<bool>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> memberLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateAnonymousTypeSymbol(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> memberTypes, System.Collections.Immutable.ImmutableArray<string> memberNames, System.Collections.Immutable.ImmutableArray<bool> memberIsReadOnly, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> memberLocations) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateArrayTypeSymbol(Microsoft.CodeAnalysis.ITypeSymbol elementType, int rank = 1, Microsoft.CodeAnalysis.NullableAnnotation elementNullableAnnotation = Microsoft.CodeAnalysis.NullableAnnotation.None) -> Microsoft.CodeAnalysis.IArrayTypeSymbol`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.Compilation.CreateArrayTypeSymbol(Microsoft.CodeAnalysis.ITypeSymbol elementType, int rank = 1) -> Microsoft.CodeAnalysis.IArrayTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateArrayTypeSymbol(Microsoft.CodeAnalysis.ITypeSymbol elementType, int rank) -> Microsoft.CodeAnalysis.IArrayTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateTupleTypeSymbol(Microsoft.CodeAnalysis.INamedTypeSymbol underlyingType, System.Collections.Immutable.ImmutableArray<string> elementNames = default(System.Collections.Immutable.ImmutableArray<string>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> elementLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> elementNullableAnnotations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateTupleTypeSymbol(Microsoft.CodeAnalysis.INamedTypeSymbol underlyingType, System.Collections.Immutable.ImmutableArray<string> elementNames, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> elementLocations) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.Compilation.CreateTupleTypeSymbol(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> elementTypes, System.Collections.Immutable.ImmutableArray<string> elementNames = default(System.Collections.Immutable.ImmutableArray<string>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> elementLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateTupleTypeSymbol(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> elementTypes, System.Collections.Immutable.ImmutableArray<string> elementNames = default(System.Collections.Immutable.ImmutableArray<string>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> elementLocations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location>), System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> elementNullableAnnotations = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation>)) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.CreateTupleTypeSymbol(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> elementTypes, System.Collections.Immutable.ImmutableArray<string> elementNames, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Location> elementLocations) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Compilation.HasImplicitConversion(Microsoft.CodeAnalysis.ITypeSymbol fromType, Microsoft.CodeAnalysis.ITypeSymbol toType) -> bool`
* [ ] `Microsoft.CodeAnalysis.Compilation.IsSymbolAccessibleWithin(Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.ISymbol within, Microsoft.CodeAnalysis.ITypeSymbol throughType = null) -> bool`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.DiagnosticSuppressor`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.DiagnosticSuppressor.DiagnosticSuppressor() -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Suppression`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Suppression.Descriptor.get -> Microsoft.CodeAnalysis.SuppressionDescriptor`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Suppression.SuppressedDiagnostic.get -> Microsoft.CodeAnalysis.Diagnostic`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SuppressionAnalysisContext`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SuppressionAnalysisContext.CancellationToken.get -> System.Threading.CancellationToken`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SuppressionAnalysisContext.Compilation.get -> Microsoft.CodeAnalysis.Compilation`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SuppressionAnalysisContext.GetSemanticModel(Microsoft.CodeAnalysis.SyntaxTree syntaxTree) -> Microsoft.CodeAnalysis.SemanticModel`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SuppressionAnalysisContext.Options.get -> Microsoft.CodeAnalysis.Diagnostics.AnalyzerOptions`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SuppressionAnalysisContext.ReportSuppression(Microsoft.CodeAnalysis.Diagnostics.Suppression suppression) -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SuppressionAnalysisContext.ReportedDiagnostics.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SuppressionActionsCount.get -> int`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SuppressionActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.IArrayTypeSymbol.ElementNullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.IDiscardSymbol.NullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.IEventSymbol.NullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.IFieldSymbol.NullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.ILocalSymbol.NullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.IMethodSymbol.Construct(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> typeArguments, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> typeArgumentNullableAnnotations) -> Microsoft.CodeAnalysis.IMethodSymbol`
* [ ] `Microsoft.CodeAnalysis.IMethodSymbol.ReceiverNullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.IMethodSymbol.ReturnNullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.IMethodSymbol.TypeArgumentNullableAnnotations.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation>`
* [ ] `Microsoft.CodeAnalysis.INamedTypeSymbol.Construct(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ITypeSymbol> typeArguments, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> typeArgumentNullableAnnotations) -> Microsoft.CodeAnalysis.INamedTypeSymbol`
* [ ] `Microsoft.CodeAnalysis.INamedTypeSymbol.TypeArgumentNullableAnnotations.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation>`
* [ ] `Microsoft.CodeAnalysis.IParameterSymbol.NullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.IPropertySymbol.NullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.ISymbol.Equals(Microsoft.CodeAnalysis.ISymbol other, Microsoft.CodeAnalysis.SymbolEqualityComparer equalityComparer) -> bool`
* [ ] `Microsoft.CodeAnalysis.ITypeParameterSymbol.ConstraintNullableAnnotations.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation>`
* [ ] `Microsoft.CodeAnalysis.ITypeParameterSymbol.HasNotNullConstraint.get -> bool`
* [ ] `Microsoft.CodeAnalysis.ITypeParameterSymbol.ReferenceTypeConstraintNullableAnnotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.ITypeSymbol.ToDisplayParts(Microsoft.CodeAnalysis.NullableFlowState topLevelNullability, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.SymbolDisplayPart>`
* [ ] `Microsoft.CodeAnalysis.ITypeSymbol.ToDisplayString(Microsoft.CodeAnalysis.NullableFlowState topLevelNullability, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> string`
* [ ] `Microsoft.CodeAnalysis.ITypeSymbol.ToMinimalDisplayParts(Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.NullableFlowState topLevelNullability, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.SymbolDisplayPart>`
* [ ] `Microsoft.CodeAnalysis.ITypeSymbol.ToMinimalDisplayString(Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.NullableFlowState topLevelNullability, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> string`
* [ ] `Microsoft.CodeAnalysis.NullabilityInfo`
* [ ] `Microsoft.CodeAnalysis.NullabilityInfo.Annotation.get -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.NullabilityInfo.Equals(Microsoft.CodeAnalysis.NullabilityInfo other) -> bool`
* [ ] `Microsoft.CodeAnalysis.NullabilityInfo.FlowState.get -> Microsoft.CodeAnalysis.NullableFlowState`
* [ ] `Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.NullableAnnotation.Annotated = 2 -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.NullableAnnotation.None = 0 -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.NullableAnnotation.NotAnnotated = 1 -> Microsoft.CodeAnalysis.NullableAnnotation`
* [ ] `Microsoft.CodeAnalysis.NullableContext`
* [ ] `Microsoft.CodeAnalysis.NullableContext.AnnotationsContextInherited = 8 -> Microsoft.CodeAnalysis.NullableContext`
* [ ] `Microsoft.CodeAnalysis.NullableContext.AnnotationsEnabled = 2 -> Microsoft.CodeAnalysis.NullableContext`
* [ ] `Microsoft.CodeAnalysis.NullableContext.ContextInherited = Microsoft.CodeAnalysis.NullableContext.WarningsContextInherited | Microsoft.CodeAnalysis.NullableContext.AnnotationsContextInherited -> Microsoft.CodeAnalysis.NullableContext`
* [ ] `Microsoft.CodeAnalysis.NullableContext.Disabled = 0 -> Microsoft.CodeAnalysis.NullableContext`
* [ ] `Microsoft.CodeAnalysis.NullableContext.Enabled = Microsoft.CodeAnalysis.NullableContext.WarningsEnabled | Microsoft.CodeAnalysis.NullableContext.AnnotationsEnabled -> Microsoft.CodeAnalysis.NullableContext`
* [ ] `Microsoft.CodeAnalysis.NullableContext.WarningsContextInherited = 4 -> Microsoft.CodeAnalysis.NullableContext`
* [ ] `Microsoft.CodeAnalysis.NullableContext.WarningsEnabled = 1 -> Microsoft.CodeAnalysis.NullableContext`
* [ ] `Microsoft.CodeAnalysis.NullableContextExtensions`
* [ ] `Microsoft.CodeAnalysis.NullableContextOptions`
* [ ] `Microsoft.CodeAnalysis.NullableContextOptions.Annotations = 2 -> Microsoft.CodeAnalysis.NullableContextOptions`
* [ ] `Microsoft.CodeAnalysis.NullableContextOptions.Disable = 0 -> Microsoft.CodeAnalysis.NullableContextOptions`
* [ ] `Microsoft.CodeAnalysis.NullableContextOptions.Enable = Microsoft.CodeAnalysis.NullableContextOptions.Warnings | Microsoft.CodeAnalysis.NullableContextOptions.Annotations -> Microsoft.CodeAnalysis.NullableContextOptions`
* [ ] `Microsoft.CodeAnalysis.NullableContextOptions.Warnings = 1 -> Microsoft.CodeAnalysis.NullableContextOptions`
* [ ] `Microsoft.CodeAnalysis.NullableContextOptionsExtensions`
* [ ] `Microsoft.CodeAnalysis.NullableFlowState`
* [ ] `Microsoft.CodeAnalysis.NullableFlowState.MaybeNull = 2 -> Microsoft.CodeAnalysis.NullableFlowState`
* [ ] `Microsoft.CodeAnalysis.NullableFlowState.None = 0 -> Microsoft.CodeAnalysis.NullableFlowState`
* [ ] `Microsoft.CodeAnalysis.NullableFlowState.NotNull = 1 -> Microsoft.CodeAnalysis.NullableFlowState`
* [ ] `Microsoft.CodeAnalysis.SuppressionDescriptor`
* [ ] `Microsoft.CodeAnalysis.SuppressionDescriptor.Equals(Microsoft.CodeAnalysis.SuppressionDescriptor other) -> bool`
* [ ] `Microsoft.CodeAnalysis.SuppressionDescriptor.Id.get -> string`
* [ ] `Microsoft.CodeAnalysis.SuppressionDescriptor.Justification.get -> Microsoft.CodeAnalysis.LocalizableString`
* [ ] `Microsoft.CodeAnalysis.SuppressionDescriptor.SuppressedDiagnosticId.get -> string`
* [ ] `Microsoft.CodeAnalysis.SuppressionDescriptor.SuppressionDescriptor(string id, string suppressedDiagnosticId, Microsoft.CodeAnalysis.LocalizableString justification) -> void`
* [ ] `Microsoft.CodeAnalysis.SuppressionDescriptor.SuppressionDescriptor(string id, string suppressedDiagnosticId, string justification) -> void`
* [ ] `Microsoft.CodeAnalysis.OperationKind.PropertySubpattern = 107 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.Operations.IPropertySubpatternOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IPropertySubpatternOperation.Member.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IPropertySubpatternOperation.Pattern.get -> Microsoft.CodeAnalysis.Operations.IPatternOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IRecursivePatternOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IRecursivePatternOperation.DeclaredSymbol.get -> Microsoft.CodeAnalysis.ISymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.IRecursivePatternOperation.DeconstructSymbol.get -> Microsoft.CodeAnalysis.ISymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.IRecursivePatternOperation.DeconstructionSubpatterns.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Operations.IPatternOperation>`
* [ ] `Microsoft.CodeAnalysis.Operations.IRecursivePatternOperation.MatchedType.get -> Microsoft.CodeAnalysis.ITypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.IRecursivePatternOperation.PropertySubpatterns.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Operations.IPropertySubpatternOperation>`
* [ ] `Microsoft.CodeAnalysis.SymbolEqualityComparer`
* [ ] `Microsoft.CodeAnalysis.SymbolEqualityComparer.Equals(Microsoft.CodeAnalysis.ISymbol x, Microsoft.CodeAnalysis.ISymbol y) -> bool`
* [ ] `Microsoft.CodeAnalysis.SymbolEqualityComparer.GetHashCode(Microsoft.CodeAnalysis.ISymbol obj) -> int`
* [ ] `Microsoft.CodeAnalysis.TypeInfo.ConvertedNullability.get -> Microsoft.CodeAnalysis.NullabilityInfo`
* [ ] `Microsoft.CodeAnalysis.TypeInfo.Nullability.get -> Microsoft.CodeAnalysis.NullabilityInfo`
* [ ] `abstract Microsoft.CodeAnalysis.Compilation.ClassifyCommonConversion(Microsoft.CodeAnalysis.ITypeSymbol source, Microsoft.CodeAnalysis.ITypeSymbol destination) -> Microsoft.CodeAnalysis.Operations.CommonConversion`
* [ ] `abstract Microsoft.CodeAnalysis.Compilation.ContainsSymbolsWithName(string name, Microsoft.CodeAnalysis.SymbolFilter filter = Microsoft.CodeAnalysis.SymbolFilter.TypeAndMember, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> bool`
* [ ] `abstract Microsoft.CodeAnalysis.Compilation.GetSymbolsWithName(string name, Microsoft.CodeAnalysis.SymbolFilter filter = Microsoft.CodeAnalysis.SymbolFilter.TypeAndMember, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.ISymbol>`
* [ ] `abstract Microsoft.CodeAnalysis.CompilationOptions.NullableContextOptions.get -> Microsoft.CodeAnalysis.NullableContextOptions`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions.TryGetValue(string key, out string value) -> bool`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider.GetOptions(Microsoft.CodeAnalysis.AdditionalText textFile) -> Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider.GetOptions(Microsoft.CodeAnalysis.SyntaxTree tree) -> Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.DiagnosticSuppressor.ReportSuppressions(Microsoft.CodeAnalysis.Diagnostics.SuppressionAnalysisContext context) -> void`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.DiagnosticSuppressor.SupportedSuppressions.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.SuppressionDescriptor>`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterCodeBlockAction(System.Action<Microsoft.CodeAnalysis.Diagnostics.CodeBlockAnalysisContext> action) -> void`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterCodeBlockStartAction<TLanguageKindEnum>(System.Action<Microsoft.CodeAnalysis.Diagnostics.CodeBlockStartAnalysisContext<TLanguageKindEnum>> action) -> void`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterOperationAction(System.Action<Microsoft.CodeAnalysis.Diagnostics.OperationAnalysisContext> action, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.OperationKind> operationKinds) -> void`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterOperationBlockAction(System.Action<Microsoft.CodeAnalysis.Diagnostics.OperationBlockAnalysisContext> action) -> void`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterOperationBlockStartAction(System.Action<Microsoft.CodeAnalysis.Diagnostics.OperationBlockStartAnalysisContext> action) -> void`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterSymbolEndAction(System.Action<Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext> action) -> void`
* [ ] `abstract Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterSyntaxNodeAction<TLanguageKindEnum>(System.Action<Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext> action, System.Collections.Immutable.ImmutableArray<TLanguageKindEnum> syntaxKinds) -> void`
* [ ] `abstract Microsoft.CodeAnalysis.SemanticModel.GetNullableContext(int position) -> Microsoft.CodeAnalysis.NullableContext`
* [ ] `const Microsoft.CodeAnalysis.WellKnownMemberNames.CountPropertyName = "Count" -> string`
* [ ] `const Microsoft.CodeAnalysis.WellKnownMemberNames.DisposeAsyncMethodName = "DisposeAsync" -> string`
* [ ] `const Microsoft.CodeAnalysis.WellKnownMemberNames.DisposeMethodName = "Dispose" -> string`
* [ ] `const Microsoft.CodeAnalysis.WellKnownMemberNames.GetAsyncEnumeratorMethodName = "GetAsyncEnumerator" -> string`
* [ ] `const Microsoft.CodeAnalysis.WellKnownMemberNames.LengthPropertyName = "Length" -> string`
* [ ] `const Microsoft.CodeAnalysis.WellKnownMemberNames.MoveNextAsyncMethodName = "MoveNextAsync" -> string`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.GlobalSection.get -> Microsoft.CodeAnalysis.AnalyzerConfig.Section`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.IsRoot.get -> bool`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.NamedSections.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.AnalyzerConfig.Section>`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.NormalizedDirectory.get -> string`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.PathToFile.get -> string`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.Section`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.Section.Name.get -> string`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.Section.Properties.get -> System.Collections.Immutable.ImmutableDictionary<string, string>`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.Section.Section(string name, System.Collections.Immutable.ImmutableDictionary<string, string> properties) -> void`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.SectionNameMatcher`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfig.SectionNameMatcher.IsMatch(string s) -> bool`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfigOptionsResult`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfigOptionsResult.AnalyzerOptions.get -> System.Collections.Immutable.ImmutableDictionary<string, string>`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfigOptionsResult.Diagnostics.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfigOptionsResult.TreeOptions.get -> System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic>`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfigSet`
* [ ] `Microsoft.CodeAnalysis.AnalyzerConfigSet.GetOptionsForSourcePath(string sourcePath) -> Microsoft.CodeAnalysis.AnalyzerConfigOptionsResult`
* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.AnalyzerConfigPaths.get -> System.Collections.Immutable.ImmutableArray<string>`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions.AnalyzerConfigOptions() -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider.AnalyzerConfigOptionsProvider() -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.AnalyzerOptions.AnalyzerConfigOptionsProvider.get -> Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.AnalyzerOptions.AnalyzerOptions(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.AdditionalText> additionalFiles, Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider optionsProvider) -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.OperationAnalysisContext.GetControlFlowGraph() -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.OperationBlockAnalysisContext.GetControlFlowGraph(Microsoft.CodeAnalysis.IOperation operationBlock) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.OperationBlockStartAnalysisContext.GetControlFlowGraph(Microsoft.CodeAnalysis.IOperation operationBlock) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.CancellationToken.get -> System.Threading.CancellationToken`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.Compilation.get -> Microsoft.CodeAnalysis.Compilation`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.Options.get -> Microsoft.CodeAnalysis.Diagnostics.AnalyzerOptions`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterOperationAction(System.Action<Microsoft.CodeAnalysis.Diagnostics.OperationAnalysisContext> action, params Microsoft.CodeAnalysis.OperationKind[] operationKinds) -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.RegisterSyntaxNodeAction<TLanguageKindEnum>(System.Action<Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext> action, params TLanguageKindEnum[] syntaxKinds) -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.Symbol.get -> Microsoft.CodeAnalysis.ISymbol`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext.SymbolStartAnalysisContext(Microsoft.CodeAnalysis.ISymbol symbol, Microsoft.CodeAnalysis.Compilation compilation, Microsoft.CodeAnalysis.Diagnostics.AnalyzerOptions options, System.Threading.CancellationToken cancellationToken) -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.Concurrent.get -> bool`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.Concurrent.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SymbolEndActionsCount.get -> int`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SymbolEndActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SymbolStartActionsCount.get -> int`
* [ ] `Microsoft.CodeAnalysis.Diagnostics.Telemetry.AnalyzerTelemetryInfo.SymbolStartActionsCount.set -> void`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.BranchValue.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.ConditionalSuccessor.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.ConditionKind.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowConditionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.EnclosingRegion.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.FallThroughSuccessor.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.IsReachable.get -> bool`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.Kind.get -> Microsoft.CodeAnalysis.FlowAnalysis.BasicBlockKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.Operations.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.IOperation>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.Ordinal.get -> int`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock.Predecessors.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlockKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlockKind.Block = 2 -> Microsoft.CodeAnalysis.FlowAnalysis.BasicBlockKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlockKind.Entry = 0 -> Microsoft.CodeAnalysis.FlowAnalysis.BasicBlockKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.BasicBlockKind.Exit = 1 -> Microsoft.CodeAnalysis.FlowAnalysis.BasicBlockKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.CaptureId`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.CaptureId.Equals(Microsoft.CodeAnalysis.FlowAnalysis.CaptureId other) -> bool`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch.Destination.get -> Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch.EnteringRegions.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch.FinallyRegions.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch.IsConditionalSuccessor.get -> bool`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch.LeavingRegions.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch.Semantics.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranch.Source.get -> Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics.Error = 7 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics.None = 0 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics.ProgramTermination = 4 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics.Regular = 1 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics.Rethrow = 6 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics.Return = 2 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics.StructuredExceptionHandling = 3 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics.Throw = 5 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowBranchSemantics`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowConditionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowConditionKind.None = 0 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowConditionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowConditionKind.WhenFalse = 1 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowConditionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowConditionKind.WhenTrue = 2 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowConditionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Blocks.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.FlowAnalysis.BasicBlock>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.GetAnonymousFunctionControlFlowGraph(Microsoft.CodeAnalysis.FlowAnalysis.IFlowAnonymousFunctionOperation anonymousFunction, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.GetLocalFunctionControlFlowGraph(Microsoft.CodeAnalysis.IMethodSymbol localFunction, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.LocalFunctions.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.IMethodSymbol>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.OriginalOperation.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Parent.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Root.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraphExtensions`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.CaptureIds.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.FlowAnalysis.CaptureId>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.EnclosingRegion.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.ExceptionType.get -> Microsoft.CodeAnalysis.ITypeSymbol`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.FirstBlockOrdinal.get -> int`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.Kind.get -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.LastBlockOrdinal.get -> int`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.LocalFunctions.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.IMethodSymbol>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.Locals.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ILocalSymbol>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion.NestedRegions.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegion>`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.Catch = 4 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.ErroneousBody = 10 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.Filter = 3 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.FilterAndHandler = 5 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.Finally = 7 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.LocalLifetime = 1 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.Root = 0 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.StaticLocalInitializer = 9 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.Try = 2 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.TryAndCatch = 6 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind.TryAndFinally = 8 -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowRegionKind`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.ICaughtExceptionOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IFlowAnonymousFunctionOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IFlowAnonymousFunctionOperation.Symbol.get -> Microsoft.CodeAnalysis.IMethodSymbol`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureOperation.Id.get -> Microsoft.CodeAnalysis.FlowAnalysis.CaptureId`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureOperation.Value.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureReferenceOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureReferenceOperation.Id.get -> Microsoft.CodeAnalysis.FlowAnalysis.CaptureId`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IIsNullOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IIsNullOperation.Operand.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IStaticLocalInitializationSemaphoreOperation`
* [ ] `Microsoft.CodeAnalysis.FlowAnalysis.IStaticLocalInitializationSemaphoreOperation.Local.get -> Microsoft.CodeAnalysis.ILocalSymbol`
* [ ] `Microsoft.CodeAnalysis.IFieldSymbol.IsFixedSizeBuffer.get -> bool`
* [ ] `Microsoft.CodeAnalysis.ILocalSymbol.IsFixed.get -> bool`
* [ ] `Microsoft.CodeAnalysis.IMethodSymbol.IsReadOnly.get -> bool`
* [ ] `Microsoft.CodeAnalysis.IOperation.SemanticModel.get -> Microsoft.CodeAnalysis.SemanticModel`
* [ ] `Microsoft.CodeAnalysis.ITypeSymbol.IsReadOnly.get -> bool`
* [ ] `Microsoft.CodeAnalysis.ITypeSymbol.IsRefLikeType.get -> bool`
* [ ] `Microsoft.CodeAnalysis.ITypeSymbol.IsUnmanagedType.get -> bool`
* [ ] `Microsoft.CodeAnalysis.OperationKind.Binary = 32 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.CaughtException = 94 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.CoalesceAssignment = 97 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.ConstructorBody = 89 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.DiscardPattern = 104 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.FlowAnonymousFunction = 96 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.FlowCapture = 91 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.FlowCaptureReference = 92 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.IsNull = 93 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.MethodBody = 88 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.Range = 99 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.RecursivePattern = 103 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.ReDim = 101 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.ReDimClause = 102 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.StaticLocalInitializationSemaphore = 95 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.SwitchExpression = 105 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.SwitchExpressionArm = 106 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.TupleBinary = 87 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.OperationKind.Unary = 31 -> Microsoft.CodeAnalysis.OperationKind`
* [ ] `Microsoft.CodeAnalysis.Operations.CommonConversion.IsImplicit.get -> bool`
* [ ] `Microsoft.CodeAnalysis.Operations.ICaseClauseOperation.Label.get -> Microsoft.CodeAnalysis.ILabelSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.ICoalesceAssignmentOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ICoalesceOperation.ValueConversion.get -> Microsoft.CodeAnalysis.Operations.CommonConversion`
* [ ] `Microsoft.CodeAnalysis.Operations.IDeclarationPatternOperation.MatchedType.get -> Microsoft.CodeAnalysis.ITypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.IDeclarationPatternOperation.MatchesNull.get -> bool`
* [ ] `Microsoft.CodeAnalysis.Operations.IDiscardPatternOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IEventAssignmentOperation.EventReference.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IForLoopOperation.ConditionLocals.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ILocalSymbol>`
* [ ] `Microsoft.CodeAnalysis.Operations.IForToLoopOperation.IsChecked.get -> bool`
* [ ] `Microsoft.CodeAnalysis.Operations.IInstanceReferenceOperation.ReferenceKind.get -> Microsoft.CodeAnalysis.Operations.InstanceReferenceKind`
* [ ] `Microsoft.CodeAnalysis.Operations.ILoopOperation.ContinueLabel.get -> Microsoft.CodeAnalysis.ILabelSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.ILoopOperation.ExitLabel.get -> Microsoft.CodeAnalysis.ILabelSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.InstanceReferenceKind`
* [ ] `Microsoft.CodeAnalysis.Operations.InstanceReferenceKind.ContainingTypeInstance = 0 -> Microsoft.CodeAnalysis.Operations.InstanceReferenceKind`
* [ ] `Microsoft.CodeAnalysis.Operations.InstanceReferenceKind.ImplicitReceiver = 1 -> Microsoft.CodeAnalysis.Operations.InstanceReferenceKind`
* [ ] `Microsoft.CodeAnalysis.Operations.InstanceReferenceKind.PatternInput = 2 -> Microsoft.CodeAnalysis.Operations.InstanceReferenceKind`
* [ ] `Microsoft.CodeAnalysis.Operations.IPatternOperation.InputType.get -> Microsoft.CodeAnalysis.ITypeSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.IRangeOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IRangeOperation.IsLifted.get -> bool`
* [ ] `Microsoft.CodeAnalysis.Operations.IRangeOperation.LeftOperand.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IRangeOperation.Method.get -> Microsoft.CodeAnalysis.IMethodSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.IRangeOperation.RightOperand.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IReDimClauseOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IReDimClauseOperation.DimensionSizes.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.IOperation>`
* [ ] `Microsoft.CodeAnalysis.Operations.IReDimClauseOperation.Operand.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IReDimOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.IReDimOperation.Clauses.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Operations.IReDimClauseOperation>`
* [ ] `Microsoft.CodeAnalysis.Operations.IReDimOperation.Preserve.get -> bool`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchCaseOperation.Locals.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ILocalSymbol>`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchExpressionArmOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchExpressionArmOperation.Guard.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchExpressionArmOperation.Locals.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ILocalSymbol>`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchExpressionArmOperation.Pattern.get -> Microsoft.CodeAnalysis.Operations.IPatternOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchExpressionArmOperation.Value.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchExpressionOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchExpressionOperation.Arms.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Operations.ISwitchExpressionArmOperation>`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchExpressionOperation.Value.get -> Microsoft.CodeAnalysis.IOperation`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchOperation.ExitLabel.get -> Microsoft.CodeAnalysis.ILabelSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.ISwitchOperation.Locals.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ILocalSymbol>`
* [ ] `Microsoft.CodeAnalysis.Operations.ITryOperation.ExitLabel.get -> Microsoft.CodeAnalysis.ILabelSymbol`
* [ ] `Microsoft.CodeAnalysis.Operations.IUsingOperation.Locals.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.ILocalSymbol>`
* [ ] `Microsoft.CodeAnalysis.Operations.IVariableDeclarationOperation.IgnoredDimensions.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.IOperation>`
* [ ] `Microsoft.CodeAnalysis.Operations.UnaryOperatorKind.Hat = 7 -> Microsoft.CodeAnalysis.Operations.UnaryOperatorKind`
* [ ] `Microsoft.CodeAnalysis.SpecialType.Count = 44 -> Microsoft.CodeAnalysis.SpecialType`
* [ ] `Microsoft.CodeAnalysis.SpecialType.System_Runtime_CompilerServices_RuntimeFeature = 44 -> Microsoft.CodeAnalysis.SpecialType`
* [ ] `Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral = 128 -> Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions`
* [ ] `Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier = 64 -> Microsoft.CodeAnalysis.SymbolDisplayMiscellaneousOptions`
* [ ] `Microsoft.CodeAnalysis.SymbolDisplayPartKind.ConstantName = 30 -> Microsoft.CodeAnalysis.SymbolDisplayPartKind`
* [ ] `Microsoft.CodeAnalysis.SymbolDisplayPartKind.EnumMemberName = 28 -> Microsoft.CodeAnalysis.SymbolDisplayPartKind`
* [ ] `Microsoft.CodeAnalysis.SymbolDisplayPartKind.ExtensionMethodName = 29 -> Microsoft.CodeAnalysis.SymbolDisplayPartKind`
* [ ] `const Microsoft.CodeAnalysis.WellKnownMemberNames.SliceMethodName = "Slice" -> string`
* [ ] `override Microsoft.CodeAnalysis.FlowAnalysis.CaptureId.Equals(object obj) -> bool`
* [ ] `override Microsoft.CodeAnalysis.FlowAnalysis.CaptureId.GetHashCode() -> int`
* [ ] `override Microsoft.CodeAnalysis.SuppressionDescriptor.Equals(object obj) -> bool`
* [ ] `override Microsoft.CodeAnalysis.SuppressionDescriptor.GetHashCode() -> int`
* [ ] `override sealed Microsoft.CodeAnalysis.Diagnostics.DiagnosticSuppressor.Initialize(Microsoft.CodeAnalysis.Diagnostics.AnalysisContext context) -> void`
* [ ] `override sealed Microsoft.CodeAnalysis.Diagnostics.DiagnosticSuppressor.SupportedDiagnostics.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.DiagnosticDescriptor>`
* [ ] `static Microsoft.CodeAnalysis.AnalyzerConfig.Parse(Microsoft.CodeAnalysis.Text.SourceText text, string pathToFile) -> Microsoft.CodeAnalysis.AnalyzerConfig`
* [ ] `static Microsoft.CodeAnalysis.AnalyzerConfig.Parse(string text, string pathToFile) -> Microsoft.CodeAnalysis.AnalyzerConfig`
* [ ] `static Microsoft.CodeAnalysis.AnalyzerConfig.ReservedKeys.get -> System.Collections.Immutable.ImmutableHashSet<string>`
* [ ] `static Microsoft.CodeAnalysis.AnalyzerConfig.ReservedValues.get -> System.Collections.Immutable.ImmutableHashSet<string>`
* [ ] `static Microsoft.CodeAnalysis.AnalyzerConfig.Section.NameComparer.get -> System.StringComparison`
* [ ] `static Microsoft.CodeAnalysis.AnalyzerConfig.Section.PropertiesKeyComparer.get -> System.StringComparer`
* [ ] `static Microsoft.CodeAnalysis.AnalyzerConfig.TryCreateSectionNameMatcher(string sectionName) -> Microsoft.CodeAnalysis.AnalyzerConfig.SectionNameMatcher?`
* [ ] `static Microsoft.CodeAnalysis.AnalyzerConfigSet.Create<TList>(TList analyzerConfigs) -> Microsoft.CodeAnalysis.AnalyzerConfigSet`
* [ ] `static Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions.KeyComparer.get -> System.StringComparer`
* [ ] `override Microsoft.CodeAnalysis.NullabilityInfo.Equals(object other) -> bool`
* [ ] `override Microsoft.CodeAnalysis.NullabilityInfo.GetHashCode() -> int`
* [ ] `static Microsoft.CodeAnalysis.Diagnostics.Suppression.Create(Microsoft.CodeAnalysis.SuppressionDescriptor descriptor, Microsoft.CodeAnalysis.Diagnostic suppressedDiagnostic) -> Microsoft.CodeAnalysis.Diagnostics.Suppression`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Create(Microsoft.CodeAnalysis.Operations.IBlockOperation body, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Create(Microsoft.CodeAnalysis.Operations.IConstructorBodyOperation constructorBody, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Create(Microsoft.CodeAnalysis.Operations.IFieldInitializerOperation initializer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Create(Microsoft.CodeAnalysis.Operations.IMethodBodyOperation methodBody, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Create(Microsoft.CodeAnalysis.Operations.IParameterInitializerOperation initializer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Create(Microsoft.CodeAnalysis.Operations.IPropertyInitializerOperation initializer, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph.Create(Microsoft.CodeAnalysis.SyntaxNode node, Microsoft.CodeAnalysis.SemanticModel semanticModel, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraphExtensions.GetAnonymousFunctionControlFlowGraphInScope(this Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph controlFlowGraph, Microsoft.CodeAnalysis.FlowAnalysis.IFlowAnonymousFunctionOperation anonymousFunction, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraphExtensions.GetLocalFunctionControlFlowGraphInScope(this Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph controlFlowGraph, Microsoft.CodeAnalysis.IMethodSymbol localFunction, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.FlowAnalysis.ControlFlowGraph`
* [ ] `static Microsoft.CodeAnalysis.NullableContextExtensions.AnnotationsEnabled(this Microsoft.CodeAnalysis.NullableContext context) -> bool`
* [ ] `static Microsoft.CodeAnalysis.NullableContextExtensions.AnnotationsInherited(this Microsoft.CodeAnalysis.NullableContext context) -> bool`
* [ ] `static Microsoft.CodeAnalysis.NullableContextExtensions.WarningsEnabled(this Microsoft.CodeAnalysis.NullableContext context) -> bool`
* [ ] `static Microsoft.CodeAnalysis.NullableContextExtensions.WarningsInherited(this Microsoft.CodeAnalysis.NullableContext context) -> bool`
* [ ] `static Microsoft.CodeAnalysis.NullableContextOptionsExtensions.AnnotationsEnabled(this Microsoft.CodeAnalysis.NullableContextOptions context) -> bool`
* [ ] `static Microsoft.CodeAnalysis.NullableContextOptionsExtensions.WarningsEnabled(this Microsoft.CodeAnalysis.NullableContextOptions context) -> bool`
* [ ] `static Microsoft.CodeAnalysis.Operations.OperationExtensions.GetCorrespondingOperation(this Microsoft.CodeAnalysis.Operations.IBranchOperation operation) -> Microsoft.CodeAnalysis.IOperation`
* [ ] `static readonly Microsoft.CodeAnalysis.SymbolEqualityComparer.Default -> Microsoft.CodeAnalysis.SymbolEqualityComparer`
* [ ] `static readonly Microsoft.CodeAnalysis.SymbolEqualityComparer.IncludeNullability -> Microsoft.CodeAnalysis.SymbolEqualityComparer`
* [ ] `static readonly Microsoft.CodeAnalysis.SyntaxTree.EmptyDiagnosticOptions -> System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic>`
* [ ] `virtual Microsoft.CodeAnalysis.Diagnostics.AnalysisContext.RegisterSymbolStartAction(System.Action<Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext> action, Microsoft.CodeAnalysis.SymbolKind symbolKind) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Diagnostics.CompilationStartAnalysisContext.RegisterSymbolStartAction(System.Action<Microsoft.CodeAnalysis.Diagnostics.SymbolStartAnalysisContext> action, Microsoft.CodeAnalysis.SymbolKind symbolKind) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitCaughtException(Microsoft.CodeAnalysis.FlowAnalysis.ICaughtExceptionOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitCoalesceAssignment(Microsoft.CodeAnalysis.Operations.ICoalesceAssignmentOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitDiscardPattern(Microsoft.CodeAnalysis.Operations.IDiscardPatternOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitFlowAnonymousFunction(Microsoft.CodeAnalysis.FlowAnalysis.IFlowAnonymousFunctionOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitFlowCapture(Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitFlowCaptureReference(Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureReferenceOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitIsNull(Microsoft.CodeAnalysis.FlowAnalysis.IIsNullOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitRangeOperation(Microsoft.CodeAnalysis.Operations.IRangeOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitReDim(Microsoft.CodeAnalysis.Operations.IReDimOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitReDimClause(Microsoft.CodeAnalysis.Operations.IReDimClauseOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitStaticLocalInitializationSemaphore(Microsoft.CodeAnalysis.FlowAnalysis.IStaticLocalInitializationSemaphoreOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitSwitchExpression(Microsoft.CodeAnalysis.Operations.ISwitchExpressionOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor.VisitSwitchExpressionArm(Microsoft.CodeAnalysis.Operations.ISwitchExpressionArmOperation operation) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitCaughtException(Microsoft.CodeAnalysis.FlowAnalysis.ICaughtExceptionOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitCoalesceAssignment(Microsoft.CodeAnalysis.Operations.ICoalesceAssignmentOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitDiscardPattern(Microsoft.CodeAnalysis.Operations.IDiscardPatternOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitFlowAnonymousFunction(Microsoft.CodeAnalysis.FlowAnalysis.IFlowAnonymousFunctionOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitFlowCapture(Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitFlowCaptureReference(Microsoft.CodeAnalysis.FlowAnalysis.IFlowCaptureReferenceOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitIsNull(Microsoft.CodeAnalysis.FlowAnalysis.IIsNullOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitRangeOperation(Microsoft.CodeAnalysis.Operations.IRangeOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitReDim(Microsoft.CodeAnalysis.Operations.IReDimOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitReDimClause(Microsoft.CodeAnalysis.Operations.IReDimClauseOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitStaticLocalInitializationSemaphore(Microsoft.CodeAnalysis.FlowAnalysis.IStaticLocalInitializationSemaphoreOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitSwitchExpression(Microsoft.CodeAnalysis.Operations.ISwitchExpressionOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.Operations.OperationVisitor<TArgument, TResult>.VisitSwitchExpressionArm(Microsoft.CodeAnalysis.Operations.ISwitchExpressionArmOperation operation, TArgument argument) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.SyntaxTree.DiagnosticOptions.get -> System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic>`
* [ ] `virtual Microsoft.CodeAnalysis.SyntaxTree.WithDiagnosticOptions(System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic> options) -> Microsoft.CodeAnalysis.SyntaxTree`

See [Microsoft.CodeAnalysis.CSharp release/dev16.3@c955f3c99b5698c906e0700ef691b5b1571c8136](https://raw.githubusercontent.com/dotnet/roslyn/c955f3c99b5698c906e0700ef691b5b1571c8136/src/Compilers/CSharp/Portable/PublicAPI.Unshipped.txt)

* [ ] `*REMOVED*static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.Create(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode root, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options = null, string path = "", System.Text.Encoding encoding = null) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `*REMOVED*static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(Microsoft.CodeAnalysis.Text.SourceText text, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options = null, string path = "", System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `*REMOVED*static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(string text, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options = null, string path = "", System.Text.Encoding encoding = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `*REMOVED*static Microsoft.CodeAnalysis.CSharp.LanguageVersionFacts.TryParse(this string version, out Microsoft.CodeAnalysis.CSharp.LanguageVersion result) -> bool`
* [ ] `*REMOVED*static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseSyntaxTree(Microsoft.CodeAnalysis.Text.SourceText text, Microsoft.CodeAnalysis.ParseOptions options = null, string path = "", System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `*REMOVED*static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseSyntaxTree(string text, Microsoft.CodeAnalysis.ParseOptions options = null, string path = "", System.Text.Encoding encoding = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `*REMOVED*abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `*REMOVED*abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `*REMOVED*abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `*REMOVED*abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `*REMOVED*abstract Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `*REMOVED*abstract Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `*REMOVED*abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `*REMOVED*abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `*REMOVED*Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind outputKind, bool reportSuppressedDiagnostics = false, string moduleName = null, string mainTypeName = null, string scriptClassName = null, System.Collections.Generic.IEnumerable<string> usings = null, Microsoft.CodeAnalysis.OptimizationLevel optimizationLevel = Microsoft.CodeAnalysis.OptimizationLevel.Debug, bool checkOverflow = false, bool allowUnsafe = false, string cryptoKeyContainer = null, string cryptoKeyFile = null, System.Collections.Immutable.ImmutableArray<byte> cryptoPublicKey = default(System.Collections.Immutable.ImmutableArray<byte>), bool? delaySign = null, Microsoft.CodeAnalysis.Platform platform = Microsoft.CodeAnalysis.Platform.AnyCpu, Microsoft.CodeAnalysis.ReportDiagnostic generalDiagnosticOption = Microsoft.CodeAnalysis.ReportDiagnostic.Default, int warningLevel = 4, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, Microsoft.CodeAnalysis.ReportDiagnostic>> specificDiagnosticOptions = null, bool concurrentBuild = true, bool deterministic = false, Microsoft.CodeAnalysis.XmlReferenceResolver xmlReferenceResolver = null, Microsoft.CodeAnalysis.SourceReferenceResolver sourceReferenceResolver = null, Microsoft.CodeAnalysis.MetadataReferenceResolver metadataReferenceResolver = null, Microsoft.CodeAnalysis.AssemblyIdentityComparer assemblyIdentityComparer = null, Microsoft.CodeAnalysis.StrongNameProvider strongNameProvider = null, bool publicSign = false, Microsoft.CodeAnalysis.MetadataImportOptions metadataImportOptions = Microsoft.CodeAnalysis.MetadataImportOptions.Public, Microsoft.CodeAnalysis.NullableContextOptions nullableContextOptions = Microsoft.CodeAnalysis.NullableContextOptions.Disable) -> void`
* [ ] `Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions.WithNullableContextOptions(Microsoft.CodeAnalysis.NullableContextOptions options) -> Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax.SemicolonToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax.Update(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken eventKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax.Update(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken eventKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax.WithSemicolonToken(Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.TargetToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken hashToken, Microsoft.CodeAnalysis.SyntaxToken nullableKeyword, Microsoft.CodeAnalysis.SyntaxToken settingToken, Microsoft.CodeAnalysis.SyntaxToken targetToken, Microsoft.CodeAnalysis.SyntaxToken endOfDirectiveToken, bool isActive) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.WithTargetToken(Microsoft.CodeAnalysis.SyntaxToken targetToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax.IsNotNull.get -> bool`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.AnnotationsKeyword = 8489 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.WarningsKeyword = 8488 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.AwaitKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `Microsoft.CodeAnalysis.CSharp.Conversion.IsSwitchExpression.get -> bool`
* [ ] `Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind outputKind, bool reportSuppressedDiagnostics, string moduleName, string mainTypeName, string scriptClassName, System.Collections.Generic.IEnumerable<string> usings, Microsoft.CodeAnalysis.OptimizationLevel optimizationLevel, bool checkOverflow, bool allowUnsafe, string cryptoKeyContainer, string cryptoKeyFile, System.Collections.Immutable.ImmutableArray<byte> cryptoPublicKey, bool? delaySign, Microsoft.CodeAnalysis.Platform platform, Microsoft.CodeAnalysis.ReportDiagnostic generalDiagnosticOption, int warningLevel, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, Microsoft.CodeAnalysis.ReportDiagnostic>> specificDiagnosticOptions, bool concurrentBuild, bool deterministic, Microsoft.CodeAnalysis.XmlReferenceResolver xmlReferenceResolver, Microsoft.CodeAnalysis.SourceReferenceResolver sourceReferenceResolver, Microsoft.CodeAnalysis.MetadataReferenceResolver metadataReferenceResolver, Microsoft.CodeAnalysis.AssemblyIdentityComparer assemblyIdentityComparer, Microsoft.CodeAnalysis.StrongNameProvider strongNameProvider, bool publicSign, Microsoft.CodeAnalysis.MetadataImportOptions metadataImportOptions) -> void`
* [x] `Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp8 = 800 -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [x] `Microsoft.CodeAnalysis.CSharp.LanguageVersion.LatestMajor = 2147483645 -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [x] `Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview = 2147483646 -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousFunctionExpressionSyntax.WithAsyncKeyword(Microsoft.CodeAnalysis.SyntaxToken asyncKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousFunctionExpressionSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousFunctionExpressionSyntax.WithBody(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode body) -> Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousFunctionExpressionSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseArgumentListSyntax.AddArguments(params Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseArgumentListSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseArgumentListSyntax.WithArguments(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax> arguments) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseArgumentListSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseCrefParameterListSyntax.AddParameters(params Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseCrefParameterListSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseCrefParameterListSyntax.WithParameters(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterSyntax> parameters) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseCrefParameterListSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.AddAttributeLists(params Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.AddDeclarationVariables(params Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.WithAttributeLists(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.WithDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.WithSemicolonToken(Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.AddAttributeLists(params Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.AddBodyStatements(params Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.AddParameterListParameters(params Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.WithAttributeLists(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.WithBody(Microsoft.CodeAnalysis.CSharp.Syntax.BlockSyntax body) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.WithExpressionBody(Microsoft.CodeAnalysis.CSharp.Syntax.ArrowExpressionClauseSyntax expressionBody) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.WithParameterList(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax parameterList) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.WithSemicolonToken(Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseParameterListSyntax.AddParameters(params Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseParameterListSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseParameterListSyntax.WithParameters(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax> parameters) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseParameterListSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.AddAccessorListAccessors(params Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.AddAttributeLists(params Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.WithAccessorList(Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList) -> Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.WithAttributeLists(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists) -> Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.WithExplicitInterfaceSpecifier(Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.WithType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.AddAttributeLists(params Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.AddBaseListTypes(params Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.WithAttributeLists(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.WithBaseList(Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax baseList) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.WithCloseBraceToken(Microsoft.CodeAnalysis.SyntaxToken closeBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.WithIdentifier(Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.WithOpenBraceToken(Microsoft.CodeAnalysis.SyntaxToken openBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax.WithSemicolonToken(Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax.WithType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BranchingDirectiveTriviaSyntax.WithEndOfDirectiveToken(Microsoft.CodeAnalysis.SyntaxToken endOfDirectiveToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.BranchingDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.BranchingDirectiveTriviaSyntax.WithHashToken(Microsoft.CodeAnalysis.SyntaxToken hashToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.BranchingDirectiveTriviaSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax.QuestionToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken classOrStructKeyword, Microsoft.CodeAnalysis.SyntaxToken questionToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax.WithQuestionToken(Microsoft.CodeAnalysis.SyntaxToken questionToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.WithAwaitKeyword(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.WithCloseParenToken(Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.WithForEachKeyword(Microsoft.CodeAnalysis.SyntaxToken forEachKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.WithInKeyword(Microsoft.CodeAnalysis.SyntaxToken inKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.WithOpenParenToken(Microsoft.CodeAnalysis.SyntaxToken openParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax.WithStatement(Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.CommonForEachStatementSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalDirectiveTriviaSyntax.WithCondition(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax condition) -> Microsoft.CodeAnalysis.CSharp.Syntax.ConditionalDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.DirectiveTriviaSyntax.WithEndOfDirectiveToken(Microsoft.CodeAnalysis.SyntaxToken endOfDirectiveToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.DirectiveTriviaSyntax.WithHashToken(Microsoft.CodeAnalysis.SyntaxToken hashToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DirectiveTriviaSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax.UnderscoreToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken underscoreToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax.WithUnderscoreToken(Microsoft.CodeAnalysis.SyntaxToken underscoreToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.SyntaxToken forEachKeyword, Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.WithAwaitKeyword(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.SyntaxToken forEachKeyword, Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, Microsoft.CodeAnalysis.SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.WithAwaitKeyword(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax.WithArrowToken(Microsoft.CodeAnalysis.SyntaxToken arrowToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax.WithAsyncKeyword(Microsoft.CodeAnalysis.SyntaxToken asyncKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax.WithBody(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode body) -> Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax.AwaitKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.SyntaxToken usingKeyword, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax.UsingKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax.WithAwaitKeyword(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax.WithUsingKeyword(Microsoft.CodeAnalysis.SyntaxToken usingKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.NullableKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.SettingToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.WithEndOfDirectiveToken(Microsoft.CodeAnalysis.SyntaxToken endOfDirectiveToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.WithHashToken(Microsoft.CodeAnalysis.SyntaxToken hashToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.WithIsActive(bool isActive) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.WithNullableKeyword(Microsoft.CodeAnalysis.SyntaxToken nullableKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.WithSettingToken(Microsoft.CodeAnalysis.SyntaxToken settingToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.AddSubpatterns(params Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.CloseParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.OpenParenToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.Subpatterns.get -> Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax>`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns, Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.WithCloseParenToken(Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.WithOpenParenToken(Microsoft.CodeAnalysis.SyntaxToken openParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.WithSubpatterns(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns) -> Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.AddSubpatterns(params Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.CloseBraceToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.OpenBraceToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.Subpatterns.get -> Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax>`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.WithCloseBraceToken(Microsoft.CodeAnalysis.SyntaxToken closeBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.WithOpenBraceToken(Microsoft.CodeAnalysis.SyntaxToken openBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.WithSubpatterns(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns) -> Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.LeftOperand.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.OperatorToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.RightOperand.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax leftOperand, Microsoft.CodeAnalysis.SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax rightOperand) -> Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.WithLeftOperand(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax leftOperand) -> Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.WithOperatorToken(Microsoft.CodeAnalysis.SyntaxToken operatorToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.WithRightOperand(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax rightOperand) -> Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.AddPositionalPatternClauseSubpatterns(params Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.AddPropertyPatternClauseSubpatterns(params Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.Designation.get -> Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.PositionalPatternClause.get -> Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.PropertyPatternClause.get -> Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.Type.get -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax positionalPatternClause, Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax propertyPatternClause, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.WithDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.WithPositionalPatternClause(Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax positionalPatternClause) -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.WithPropertyPatternClause(Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax propertyPatternClause) -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.WithType(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type) -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax.WithIdentifier(Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.SimpleNameSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax.NameColon.get -> Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax.Pattern.get -> Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax nameColon, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax.WithNameColon(Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax nameColon) -> Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax.WithPattern(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.EqualsGreaterThanToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.Expression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.Pattern.get -> Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax whenClause, Microsoft.CodeAnalysis.SyntaxToken equalsGreaterThanToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.WhenClause.get -> Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.WithEqualsGreaterThanToken(Microsoft.CodeAnalysis.SyntaxToken equalsGreaterThanToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.WithExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.WithPattern(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.WithWhenClause(Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax whenClause) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.AddArms(params Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.Arms.get -> Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax>`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.CloseBraceToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.GoverningExpression.get -> Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.OpenBraceToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.SwitchKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.Update(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax governingExpression, Microsoft.CodeAnalysis.SyntaxToken switchKeyword, Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax> arms, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.WithArms(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax> arms) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.WithCloseBraceToken(Microsoft.CodeAnalysis.SyntaxToken closeBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.WithGoverningExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax governingExpression) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.WithOpenBraceToken(Microsoft.CodeAnalysis.SyntaxToken openBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.WithSwitchKeyword(Microsoft.CodeAnalysis.SyntaxToken switchKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax.WithColonToken(Microsoft.CodeAnalysis.SyntaxToken colonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax.WithKeyword(Microsoft.CodeAnalysis.SyntaxToken keyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchLabelSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.AddAttributeLists(params Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.AddBaseListTypes(params Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.BaseTypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.AddConstraintClauses(params Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.AddMembers(params Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.AddTypeParameterListParameters(params Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithAttributeLists(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithBaseList(Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax baseList) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithCloseBraceToken(Microsoft.CodeAnalysis.SyntaxToken closeBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithConstraintClauses(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax> constraintClauses) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithIdentifier(Microsoft.CodeAnalysis.SyntaxToken identifier) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithKeyword(Microsoft.CodeAnalysis.SyntaxToken keyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithMembers(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithOpenBraceToken(Microsoft.CodeAnalysis.SyntaxToken openBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithSemicolonToken(Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax.WithTypeParameterList(Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax typeParameterList) -> Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax.AwaitKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.SyntaxToken usingKeyword, Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax.WithAwaitKeyword(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax.Designation.get -> Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax`
* [x] ~~`Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax.Update(Microsoft.CodeAnalysis.SyntaxToken varKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax`~~
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax.VarKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax.WithDesignation(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax.WithVarKeyword(Microsoft.CodeAnalysis.SyntaxToken varKeyword) -> Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax.WithEndQuoteToken(Microsoft.CodeAnalysis.SyntaxToken endQuoteToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax.WithEqualsToken(Microsoft.CodeAnalysis.SyntaxToken equalsToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax.WithName(Microsoft.CodeAnalysis.CSharp.Syntax.XmlNameSyntax name) -> Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax.WithStartQuoteToken(Microsoft.CodeAnalysis.SyntaxToken startQuoteToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.XmlAttributeSyntax`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.CoalesceAssignmentExpression = 8725 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.DiscardPattern = 9024 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.DotDotToken = 8222 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.EnableKeyword = 8487 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.IndexExpression = 8741 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.NullableDirectiveTrivia = 9055 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.NullableKeyword = 8486 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.PositionalPatternClause = 9023 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.PropertyPatternClause = 9021 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.QuestionQuestionEqualsToken = 8284 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.RangeExpression = 8658 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.RecursivePattern = 9020 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.Subpattern = 9022 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.SuppressNullableWarningExpression = 9054 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.SwitchExpression = 9025 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.SwitchExpressionArm = 9026 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.VarKeyword = 8490 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.VarPattern = 9027 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpCompilation.ClassifyCommonConversion(Microsoft.CodeAnalysis.ITypeSymbol source, Microsoft.CodeAnalysis.ITypeSymbol destination) -> Microsoft.CodeAnalysis.Operations.CommonConversion`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpCompilation.ContainsSymbolsWithName(string name, Microsoft.CodeAnalysis.SymbolFilter filter = Microsoft.CodeAnalysis.SymbolFilter.TypeAndMember, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> bool`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpCompilation.GetSymbolsWithName(string name, Microsoft.CodeAnalysis.SymbolFilter filter = Microsoft.CodeAnalysis.SymbolFilter.TypeAndMember, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.ISymbol>`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions.NullableContextOptions.get -> Microsoft.CodeAnalysis.NullableContextOptions`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitDiscardPattern(Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitNullableDirectiveTrivia(Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitPositionalPatternClause(Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitPropertyPatternClause(Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitRangeExpression(Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitRecursivePattern(Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitSubpattern(Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitSwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitSwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.CSharpSyntaxRewriter.VisitVarPattern(Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax node) -> Microsoft.CodeAnalysis.SyntaxNode`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax.AwaitKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [x] `override Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax.AwaitKeyword.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.EndOfDirectiveToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.HashToken.get -> Microsoft.CodeAnalysis.SyntaxToken`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax.IsActive.get -> bool`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax.Accept(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor visitor) -> void`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax.Accept<TResult>(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult> visitor) -> TResult`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.Create(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode root, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options = null, string path = "", System.Text.Encoding encoding = null, System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic> diagnosticOptions = null) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.Create(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode root, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options, string path, System.Text.Encoding encoding) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(Microsoft.CodeAnalysis.Text.SourceText text, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options = null, string path = "", System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic> diagnosticOptions = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(Microsoft.CodeAnalysis.Text.SourceText text, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options, string path, System.Threading.CancellationToken cancellationToken) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(string text, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options, string path, System.Text.Encoding encoding, System.Threading.CancellationToken cancellationToken) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(string text, Microsoft.CodeAnalysis.CSharp.CSharpParseOptions options = null, string path = "", System.Text.Encoding encoding = null, System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic> diagnosticOptions = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.LanguageVersionFacts.TryParse(string version, out Microsoft.CodeAnalysis.CSharp.LanguageVersion result) -> bool`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SymbolDisplay.ToDisplayParts(Microsoft.CodeAnalysis.ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.SymbolDisplayPart>`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SymbolDisplay.ToDisplayParts(Microsoft.CodeAnalysis.ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.SymbolDisplayPart>`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SymbolDisplay.ToDisplayString(Microsoft.CodeAnalysis.ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> string`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SymbolDisplay.ToDisplayString(Microsoft.CodeAnalysis.ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> string`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SymbolDisplay.ToMinimalDisplayParts(Microsoft.CodeAnalysis.ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.SymbolDisplayPart>`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SymbolDisplay.ToMinimalDisplayParts(Microsoft.CodeAnalysis.ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.SymbolDisplayPart>`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SymbolDisplay.ToMinimalDisplayString(Microsoft.CodeAnalysis.ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> string`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SymbolDisplay.ToMinimalDisplayString(Microsoft.CodeAnalysis.ITypeSymbol symbol, Microsoft.CodeAnalysis.NullableFlowState nullableFlowState, Microsoft.CodeAnalysis.SemanticModel semanticModel, int position, Microsoft.CodeAnalysis.SymbolDisplayFormat format = null) -> string`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ClassOrStructConstraint(Microsoft.CodeAnalysis.CSharp.SyntaxKind kind, Microsoft.CodeAnalysis.SyntaxToken classOrStructKeyword, Microsoft.CodeAnalysis.SyntaxToken questionToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.ClassOrStructConstraintSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DiscardPattern() -> Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.DiscardPattern(Microsoft.CodeAnalysis.SyntaxToken underscoreToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.EventDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken eventKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.AccessorListSyntax accessorList, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.EventDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken eventKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.EventDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ForEachStatement(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.SyntaxToken forEachKeyword, Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ForEachVariableStatement(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.SyntaxToken forEachKeyword, Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax variable, Microsoft.CodeAnalysis.SyntaxToken inKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.ForEachVariableStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.LocalDeclarationStatement(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.SyntaxToken usingKeyword, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.NullableDirectiveTrivia(Microsoft.CodeAnalysis.SyntaxToken hashToken, Microsoft.CodeAnalysis.SyntaxToken nullableKeyword, Microsoft.CodeAnalysis.SyntaxToken settingToken, Microsoft.CodeAnalysis.SyntaxToken targetToken, Microsoft.CodeAnalysis.SyntaxToken endOfDirectiveToken, bool isActive) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.NullableDirectiveTrivia(Microsoft.CodeAnalysis.SyntaxToken settingToken, Microsoft.CodeAnalysis.SyntaxToken targetToken, bool isActive) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.NullableDirectiveTrivia(Microsoft.CodeAnalysis.SyntaxToken settingToken, bool isActive) -> Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseMemberDeclaration(string text, int offset = 0, Microsoft.CodeAnalysis.ParseOptions options = null, bool consumeFullText = true) -> Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseSyntaxTree(Microsoft.CodeAnalysis.Text.SourceText text, Microsoft.CodeAnalysis.ParseOptions options = null, string path = "", System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic> diagnosticOptions = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseSyntaxTree(Microsoft.CodeAnalysis.Text.SourceText text, Microsoft.CodeAnalysis.ParseOptions options, string path, System.Threading.CancellationToken cancellationToken) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseSyntaxTree(string text, Microsoft.CodeAnalysis.ParseOptions options = null, string path = "", System.Text.Encoding encoding = null, System.Collections.Immutable.ImmutableDictionary<string, Microsoft.CodeAnalysis.ReportDiagnostic> diagnosticOptions = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.SyntaxTree`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseSyntaxTree(string text, Microsoft.CodeAnalysis.ParseOptions options, string path, System.Text.Encoding encoding, System.Threading.CancellationToken cancellationToken) -> Microsoft.CodeAnalysis.SyntaxTree`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.PositionalPatternClause(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns = default(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax>)) -> Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.PositionalPatternClause(Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns, Microsoft.CodeAnalysis.SyntaxToken closeParenToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.PropertyPatternClause(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns = default(Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax>)) -> Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.PropertyPatternClause(Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax> subpatterns, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RangeExpression() -> Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RangeExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax leftOperand, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax rightOperand) -> Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RangeExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax leftOperand, Microsoft.CodeAnalysis.SyntaxToken operatorToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax rightOperand) -> Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RecursivePattern() -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.RecursivePattern(Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax type, Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax positionalPatternClause, Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax propertyPatternClause, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Subpattern(Microsoft.CodeAnalysis.CSharp.Syntax.NameColonSyntax nameColon, Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Subpattern(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern) -> Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax governingExpression) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax governingExpression, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax> arms) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax governingExpression, Microsoft.CodeAnalysis.SyntaxToken switchKeyword, Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax> arms, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax whenClause, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.PatternSyntax pattern, Microsoft.CodeAnalysis.CSharp.Syntax.WhenClauseSyntax whenClause, Microsoft.CodeAnalysis.SyntaxToken equalsGreaterThanToken, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression) -> Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.UsingStatement(Microsoft.CodeAnalysis.SyntaxToken awaitKeyword, Microsoft.CodeAnalysis.SyntaxToken usingKeyword, Microsoft.CodeAnalysis.SyntaxToken openParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax declaration, Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax expression, Microsoft.CodeAnalysis.SyntaxToken closeParenToken, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.UsingStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.VarPattern(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.VarPattern(Microsoft.CodeAnalysis.SyntaxToken varKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.VariableDesignationSyntax designation) -> Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitDiscardPattern(Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitNullableDirectiveTrivia(Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitPositionalPatternClause(Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitPropertyPatternClause(Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitRangeExpression(Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitRecursivePattern(Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitSubpattern(Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitSwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitSwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor.VisitVarPattern(Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax node) -> void`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitDiscardPattern(Microsoft.CodeAnalysis.CSharp.Syntax.DiscardPatternSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitNullableDirectiveTrivia(Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitPositionalPatternClause(Microsoft.CodeAnalysis.CSharp.Syntax.PositionalPatternClauseSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitPropertyPatternClause(Microsoft.CodeAnalysis.CSharp.Syntax.PropertyPatternClauseSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitRangeExpression(Microsoft.CodeAnalysis.CSharp.Syntax.RangeExpressionSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitRecursivePattern(Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitSubpattern(Microsoft.CodeAnalysis.CSharp.Syntax.SubpatternSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitSwitchExpression(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitSwitchExpressionArm(Microsoft.CodeAnalysis.CSharp.Syntax.SwitchExpressionArmSyntax node) -> TResult`
* [ ] `virtual Microsoft.CodeAnalysis.CSharp.CSharpSyntaxVisitor<TResult>.VisitVarPattern(Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax node) -> TResult`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax.Update(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax equalsValue) -> Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax.AddAttributeLists(params Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax.Update(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax.WithAttributeLists(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists) -> Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax.AddAttributeLists(params Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax.WithAttributeLists(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists) -> Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax.AddAttributeLists(params Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax.AddModifiers(params Microsoft.CodeAnalysis.SyntaxToken[] items) -> Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax.Update(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken namespaceKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax.WithAttributeLists(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists) -> Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax`
* [ ] `Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax.WithModifiers(Microsoft.CodeAnalysis.SyntaxTokenList modifiers) -> Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `abstract Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.IncompleteMemberSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `override abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `override abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseFieldDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `override abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `override abstract Microsoft.CodeAnalysis.CSharp.Syntax.BaseMethodDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `override abstract Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `override abstract Microsoft.CodeAnalysis.CSharp.Syntax.BasePropertyDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.EnumMemberDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken identifier, Microsoft.CodeAnalysis.CSharp.Syntax.EqualsValueClauseSyntax equalsValue) -> Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.GlobalStatement(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement) -> Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.NamespaceDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members) -> Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax`
* [ ] `static Microsoft.CodeAnalysis.CSharp.SyntaxFactory.NamespaceDeclaration(Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.SyntaxTokenList modifiers, Microsoft.CodeAnalysis.SyntaxToken namespaceKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, Microsoft.CodeAnalysis.SyntaxToken openBraceToken, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ExternAliasDirectiveSyntax> externs, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax> usings, Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, Microsoft.CodeAnalysis.SyntaxToken closeBraceToken, Microsoft.CodeAnalysis.SyntaxToken semicolonToken) -> Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax.AttributeLists.get -> Microsoft.CodeAnalysis.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeListSyntax>`
* [ ] `override Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax.Modifiers.get -> Microsoft.CodeAnalysis.SyntaxTokenList`
