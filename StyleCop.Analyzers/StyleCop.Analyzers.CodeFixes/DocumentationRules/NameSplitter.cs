// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The name splitter.
    /// </summary>
    internal class NameSplitter
    {
        /// <summary>
        /// Splits name by upper character.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A list of words.</returns>
        public static IEnumerable<string> Split(string name)
        {
            var sb = new StringBuilder();

            foreach (char c in name)
            {
                if (char.IsUpper(c) && sb.Length > 0)
                {
                    yield return sb.ToString();
                    sb.Clear();
                    sb.Append(c);
                }
                else
                {
                    sb.Append(c);
                }
            }

            yield return sb.ToString();
        }
    }
}
