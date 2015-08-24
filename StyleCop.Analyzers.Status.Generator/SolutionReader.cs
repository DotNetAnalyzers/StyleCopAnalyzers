namespace StyleCop.Analyzers.Status.Generator
{
    using System;
    using System.Collections.Immutable;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.MSBuild;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// A class that is used to parse the StyleCop.Analyzers solution to get an overview
    /// about the implemented diagnostics.
    /// </summary>
    public class SolutionReader
    {
        private static Regex diagnosticPathRegex = new Regex(@"(?<type>[A-Za-z]+)Rules\\(?<id>S[A-Z][0-9]{4})(?<name>[A-Za-z0-9]+)\.cs$");
        private INamedTypeSymbol diagnosticAnalyzerTypeSymbol;
        private INamedTypeSymbol noCodeFixAttributeTypeSymbol;

        private Solution solution;
        private Project project;
        private MSBuildWorkspace workspace;
        private Assembly assembly;
        private Compilation compilation;
        private ITypeSymbol booleanType;

        private SolutionReader()
        {

        }

        private string SlnPath { get; set; }

        private string ProjectName { get; set; }

        private ImmutableArray<CodeFixProvider> CodeFixProviders { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="SolutionReader"/> class.
        /// </summary>
        /// <param name="pathToSln">The path to the StyleCop.Analayzers sln</param>
        /// <param name="projectName">The project name of the main project</param>
        /// <returns>A <see cref="Task{SolutionReader}"/> representing the asynchronous operation</returns>
        public static async Task<SolutionReader> CreateAsync(string pathToSln, string projectName = "StyleCop.Analyzers")
        {

            SolutionReader reader = new SolutionReader();

            reader.SlnPath = pathToSln;
            reader.ProjectName = projectName;
            reader.workspace = MSBuildWorkspace.Create(properties: new Dictionary<string, string> { { "Configuration", "Release" } });

            await reader.InitializeAsync();

            return reader;
        }

        private async Task InitializeAsync()
        {
            this.solution = await this.workspace.OpenSolutionAsync(this.SlnPath);
            this.project = this.solution.Projects.Single(x => x.Name == this.ProjectName);
            this.compilation = await this.project.GetCompilationAsync();
            this.compilation = this.compilation.WithOptions(this.compilation.Options.WithOutputKind(OutputKind.DynamicallyLinkedLibrary));
            this.booleanType = this.compilation.GetSpecialType(SpecialType.System_Boolean);
            this.Compile();

            this.noCodeFixAttributeTypeSymbol = this.compilation.GetTypeByMetadataName("StyleCop.Analyzers.NoCodeFixAttribute");
            this.diagnosticAnalyzerTypeSymbol = this.compilation.GetTypeByMetadataName(typeof(DiagnosticAnalyzer).FullName);

            this.InitializeCodeFixTypes();
        }

        private void InitializeCodeFixTypes()
        {
            var codeFixTypes = this.assembly.ExportedTypes.Where(x => x.FullName.EndsWith("CodeFixProvider"));
            this.CodeFixProviders = ImmutableArray.Create(
                codeFixTypes
                .Select(t => Activator.CreateInstance(t, true))
                .OfType<CodeFixProvider>()
                .Where(x => x != null)
                .ToArray());
        }

        private void Compile()
        {
            MemoryStream memStream = new MemoryStream();

            string path = Path.Combine(Path.GetDirectoryName(this.SlnPath), this.ProjectName);

            var emitResult = this.compilation.Emit(memStream, manifestResources: ResourceReader.GetResourcesRecursive(path));

            if (!emitResult.Success)
            {
                throw new CompilationFailedException();
            }

            this.assembly = Assembly.Load(memStream.ToArray());
        }

        /// <summary>
        /// Analyzes the project and returns information about the diagnostics in it.
        /// </summary>
        /// <returns>A <see cref="Task{ImmutableList{StyleCopDiagnostic}}"/> representing the asynchronous operation</returns>
        public async Task<ImmutableList<StyleCopDiagnostic>> GetDiagnosticsAsync()
        {
            var diagnostics = ImmutableList.CreateBuilder<StyleCopDiagnostic>();

            var syntaxTrees = this.compilation.SyntaxTrees;

            foreach (var syntaxTree in syntaxTrees)
            {
                var match = diagnosticPathRegex.Match(syntaxTree.FilePath);
                if (!match.Success)
                {
                    continue;
                }

                string id = match.Groups["id"].Value;
                string shortName = match.Groups["name"].Value;
                CodeFixStatus codeFixStatus;
                string noCodeFixReason = null;

                // Check if this syntax tree represents a diagnostic
                SyntaxNode syntaxRoot = await syntaxTree.GetRootAsync();
                SemanticModel semanticModel = this.compilation.GetSemanticModel(syntaxTree);
                SyntaxNode classSyntaxNode = syntaxRoot.DescendantNodes().First(x => x.IsKind(SyntaxKind.ClassDeclaration));

                INamedTypeSymbol classSymbol = semanticModel.GetDeclaredSymbol(classSyntaxNode) as INamedTypeSymbol;

                if (!this.InheritsFrom(classSymbol, this.diagnosticAnalyzerTypeSymbol))
                {
                    continue;
                }

                bool hasImplementation = HasImplementation(syntaxRoot);

                codeFixStatus = this.HasCodeFix(id, classSymbol, out noCodeFixReason);

                IEnumerable<DiagnosticDescriptor> descriptorInfos = this.GetDescriptor(classSymbol);

                foreach (var descriptorInfo in descriptorInfos)
                {
                    string status = this.GetStatus(classSymbol, syntaxRoot, semanticModel, descriptorInfo);

                    var diagnostic = new StyleCopDiagnostic
                    {
                        Id = descriptorInfo.Id,
                        Category = descriptorInfo.Category,
                        HasImplementation = hasImplementation,
                        Status = status,
                        Name = shortName,
                        Title = descriptorInfo.Title.ToString(),
                        HelpLink = descriptorInfo.HelpLinkUri,
                        CodeFixStatus = codeFixStatus,
                        NoCodeFixReason = noCodeFixReason
                    };
                    diagnostics.Add(diagnostic);
                }
            }

            return diagnostics.ToImmutable();
        }

        private static bool HasImplementation(SyntaxNode syntaxRoot)
        {
            bool hasImplementation = true;
            foreach (var trivia in syntaxRoot.DescendantTrivia())
            {
                if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                {
                    if (trivia.ToFullString().Contains("TODO: Implement analysis"))
                    {
                        hasImplementation = false;
                    }
                }
            }

            return hasImplementation;
        }

        private string GetStatus(INamedTypeSymbol classSymbol, SyntaxNode root, SemanticModel model, DiagnosticDescriptor descriptor)
        {
            // Some analyzers use multiple descriptors. We analyze the first one and hope that
            // thats enough.
            var members = classSymbol.GetMembers().Where(x => x.Name.Contains("Descriptor")).ToArray();

            foreach (var member in members)
            {
                VariableDeclaratorSyntax node = root.FindNode(member.Locations.FirstOrDefault().SourceSpan) as VariableDeclaratorSyntax;

                if (node == null)
                {
                    continue;
                }

                ObjectCreationExpressionSyntax initializer = node.Initializer.Value as ObjectCreationExpressionSyntax;

                var firstArgument = initializer.ArgumentList.Arguments[0];

                string constantValue = (string)model.GetConstantValue(firstArgument.Expression).Value;

                if (constantValue != descriptor.Id)
                {
                    continue;
                }

                // We use the fact that the only parameter that returns a boolean is the one we are interested in
                var enabledByDefaultParameter = from argument in initializer.ArgumentList.Arguments
                                                where model.GetTypeInfo(argument.Expression).Type == this.booleanType
                                                select argument.Expression;
                var parameter = enabledByDefaultParameter.FirstOrDefault();
                string parameterString = parameter.ToString();
                var analyzerConstantLength = "AnalyzerConstants.".Length;

                if (parameterString.Length < analyzerConstantLength)
                {
                    return parameterString;
                }

                return parameter.ToString().Substring(analyzerConstantLength);
            }

            return "Unknown";
        }

        private IEnumerable<DiagnosticDescriptor> GetDescriptor(INamedTypeSymbol classSymbol)
        {
            var analyzer = (DiagnosticAnalyzer)Activator.CreateInstance(this.assembly.GetType(classSymbol.ToString()));

            // This currently only supports one diagnostic for each analyzer.
            return analyzer.SupportedDiagnostics;
        }

        private CodeFixStatus HasCodeFix(string diagnosticId, INamedTypeSymbol classSymbol, out string noCodeFixReason)
        {
            CodeFixStatus status;

            noCodeFixReason = null;

            var noCodeFixAttribute = classSymbol.GetAttributes().SingleOrDefault(x => x.AttributeClass == this.noCodeFixAttributeTypeSymbol);

            bool hasCodeFix = noCodeFixAttribute == null;
            if (!hasCodeFix)
            {
                status = CodeFixStatus.NotImplemented;
                if (noCodeFixAttribute.ConstructorArguments.Length > 0)
                {
                    noCodeFixReason = noCodeFixAttribute.ConstructorArguments[0].Value as string;
                }
            }
            else
            {
                // Check if the code fix actually exists
                hasCodeFix = this.CodeFixProviders.Any(x => x.FixableDiagnosticIds.Contains(diagnosticId));

                status = hasCodeFix ? CodeFixStatus.Implemented : CodeFixStatus.NotYetImplemented;
            }

            return status;
        }

        private bool InheritsFrom(INamedTypeSymbol declaration, INamedTypeSymbol possibleBaseType)
        {
            while (declaration != null)
            {
                if (declaration == possibleBaseType)
                {
                    return true;
                }

                declaration = declaration.BaseType;
            }

            return false;
        }
    }
}