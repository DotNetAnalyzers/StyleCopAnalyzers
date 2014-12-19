using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace StyleCop.Analyzers.Helpers
{
    /// <summary>
    /// Compares parameters.
    /// </summary>
    internal class ParameterSymbolEqualityComparer : IEqualityComparer<IParameterSymbol>
    {
        public bool Equals(
            IParameterSymbol x,
            IParameterSymbol y,
            bool compareParameterName,
            bool isCaseSensitive)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var nameComparisonCheck = true;
            if (compareParameterName)
            {
                nameComparisonCheck = isCaseSensitive ?
                    x.Name == y.Name
                    : string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            }

            return
                x.RefKind == y.RefKind &&
                nameComparisonCheck;
        }

        public bool Equals(IParameterSymbol x, IParameterSymbol y)
        {
            return this.Equals(x, y, false, false);
        }

        public int GetHashCode(IParameterSymbol x)
        {
            if (x == null)
            {
                return 0;
            }

            return
                x.GetHashCode();
        }
    }
}