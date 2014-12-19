using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StyleCop.Analyzers.ReadabilityRules;

namespace StyleCop.Analyzers.Helpers
{
    internal class ClassHelper
    {
        /// <summary>
        /// Checks if class contains overriding or hiding method of method provided.
        /// </summary>
        /// <param name="classDeclarationSyntax">Class to check.</param>
        /// <param name="semanticModel">Semantic model of analyzed code.</param>
        /// <param name="methodCalledSymbol">Base method.</param>
        /// <returns></returns>
        public bool ContainsOverrideOrHidingMethod(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel, IMethodSymbol methodCalledSymbol)
        {
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
            if (classSymbol != null)
            {
                var methodCandidatesSymbols = classSymbol.GetMembers()
                    .OfType<IMethodSymbol>()
                    .Where(m => m.Name == methodCalledSymbol.Name)
                    .ToList();

                var parametersComparer = new ParameterSymbolEqualityComparer();
                foreach (var methodCandidateSymbol in methodCandidatesSymbols)
                {
                    var methodCalledParametersCount = methodCalledSymbol.Parameters.Count();
                    if (methodCandidateSymbol.Parameters.Count() != methodCalledParametersCount)
                    {
                        continue;
                    }

                    var match = true;
                    for (var i = 0; i < methodCalledParametersCount; i++)
                    {
                        if (!parametersComparer.Equals(methodCandidateSymbol.Parameters[i], methodCalledSymbol.Parameters[i], true, true))
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if class contains member with the same name.
        /// </summary>
        /// <param name="classDeclarationSyntax">Class to check.</param>
        /// <param name="semanticModel">Semantic model of analyzed code.</param>
        /// <param name="memberName">Name of the member we are looking for.</param>
        /// <returns></returns>
        public bool ContainsBaseMemberByName(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel, string memberName)
        {
            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax) as INamedTypeSymbol;
            if (classSymbol != null)
            {
                return classSymbol.MemberNames.Any(m => m == memberName);
            }

            return false;
        }
    }
}