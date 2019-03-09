# C# 7.1 APIs supported via light-up

See [dotnet/roslyn@5520eac](https://github.com/dotnet/roslyn/commit/5520eaccd5d22ae98a39a5f88120277f02097dbf).

## Semantics

* [ ] `Microsoft.CodeAnalysis.CSharp.LanguageVersionFacts`
* [ ] `const Microsoft.CodeAnalysis.LanguageNames.FSharp = "F#" -> string`
* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.OutputRefFilePath.get -> string`
* [ ] `Microsoft.CodeAnalysis.CommandLineArguments.RuleSetPath.get -> string`
* [ ] `Microsoft.CodeAnalysis.CommandLineReference.CommandLineReference(string reference, Microsoft.CodeAnalysis.MetadataReferenceProperties properties) -> void`
* [ ] `Microsoft.CodeAnalysis.CommandLineSourceFile.CommandLineSourceFile(string path, bool isScript) -> void`
* [ ] `Microsoft.CodeAnalysis.Compilation.Emit(System.IO.Stream peStream, System.IO.Stream pdbStream = null, System.IO.Stream xmlDocumentationStream = null, System.IO.Stream win32Resources = null, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.ResourceDescription> manifestResources = null, Microsoft.CodeAnalysis.Emit.EmitOptions options = null, Microsoft.CodeAnalysis.IMethodSymbol debugEntryPoint = null, System.IO.Stream sourceLinkStream = null, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.EmbeddedText> embeddedTexts = null, System.IO.Stream metadataPEStream = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken)) -> Microsoft.CodeAnalysis.Emit.EmitResult`
* [ ] `Microsoft.CodeAnalysis.Compilation.Emit(System.IO.Stream peStream, System.IO.Stream pdbStream, System.IO.Stream xmlDocumentationStream, System.IO.Stream win32Resources, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.ResourceDescription> manifestResources, Microsoft.CodeAnalysis.Emit.EmitOptions options, Microsoft.CodeAnalysis.IMethodSymbol debugEntryPoint, System.IO.Stream sourceLinkStream, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.EmbeddedText> embeddedTexts, System.Threading.CancellationToken cancellationToken) -> Microsoft.CodeAnalysis.Emit.EmitResult`
* [ ] `Microsoft.CodeAnalysis.Emit.EmitOptions.EmitOptions(bool metadataOnly = false, Microsoft.CodeAnalysis.Emit.DebugInformationFormat debugInformationFormat = (Microsoft.CodeAnalysis.Emit.DebugInformationFormat)0, string pdbFilePath = null, string outputNameOverride = null, int fileAlignment = 0, ulong baseAddress = 0, bool highEntropyVirtualAddressSpace = false, Microsoft.CodeAnalysis.SubsystemVersion subsystemVersion = default(Microsoft.CodeAnalysis.SubsystemVersion), string runtimeMetadataVersion = null, bool tolerateErrors = false, bool includePrivateMembers = true, System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind> instrumentationKinds = default(System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Emit.InstrumentationKind>)) -> void`
* [ ] `Microsoft.CodeAnalysis.ParseOptions.Errors.get -> System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>`
* [ ] `Microsoft.CodeAnalysis.ParseOptions.SpecifiedKind.get -> Microsoft.CodeAnalysis.SourceCodeKind`
* [ ] `static Microsoft.CodeAnalysis.Compilation.GetRequiredLanguageVersion(Microsoft.CodeAnalysis.Diagnostic diagnostic) -> string`
* [ ] `static Microsoft.CodeAnalysis.CSharp.LanguageVersionFacts.MapSpecifiedToEffectiveVersion(this Microsoft.CodeAnalysis.CSharp.LanguageVersion version) -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [ ] `static Microsoft.CodeAnalysis.CSharp.LanguageVersionFacts.ToDisplayString(this Microsoft.CodeAnalysis.CSharp.LanguageVersion version) -> string`
* [ ] `static Microsoft.CodeAnalysis.CSharp.LanguageVersionFacts.TryParse(this string version, out Microsoft.CodeAnalysis.CSharp.LanguageVersion result) -> bool`

## Syntax

* [x] `Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp7_1 = 701 -> Microsoft.CodeAnalysis.CSharp.LanguageVersion`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.ConflictMarkerTrivia = 8564 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `Microsoft.CodeAnalysis.CSharp.SyntaxKind.DefaultLiteralExpression = 8755 -> Microsoft.CodeAnalysis.CSharp.SyntaxKind`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFacts.IsReservedTupleElementName(string elementName) -> bool`
* [x] `static Microsoft.CodeAnalysis.CSharp.SyntaxFacts.TryGetInferredMemberName(this Microsoft.CodeAnalysis.SyntaxNode syntax) -> string`
