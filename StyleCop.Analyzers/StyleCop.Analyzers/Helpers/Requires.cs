// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;

    /// <summary>
    /// This class contains helper methods for argument validation.
    /// </summary>
    internal static class Requires
    {
        /// <summary>
        /// Validates that an argument is not null.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="argument">The argument value.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="argument"/> is <see langword="null"/>.
        /// </exception>
        public static void NotNull<T>(T argument, string parameterName)
            where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
