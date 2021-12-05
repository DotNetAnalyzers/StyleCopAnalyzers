// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides helper methods related to ResourceManager.
    /// </summary>
    internal static class ResourceManagerHelper
    {
        /// <summary>
        /// Returns a CultureInfo instance specified by the given name. Only to be passed to ResourceManager.
        /// </summary>
        /// <param name="name">The culture name.</param>
        /// <returns>The created CultureInfo instance.</returns>
        public static CultureInfo GetCultureInfo(string name)
        {
            return new ResourceManagerCultureInfo(name);
        }

        /// <summary>
        /// Sub class of CultureInfo, used only to get strings from ResourceManager.
        /// Makes it possible to create a CultureInfo with an arbitrary name even in globalization invariant mode in .NET 6,
        /// unlike when using the normal CultureInfo class.
        ///
        /// The only thing necessary for the ResourceManager seeems to be the Name property.
        /// Fortunately, almost all of CultureInfo's members are virtual, so this implementation creates
        /// an invariant culture instance and overrides the Name property.
        ///
        /// Overrides all other members as well in debug builds, just to make sure that they are not used.
        /// </summary>
        private class ResourceManagerCultureInfo : CultureInfo
        {
            public ResourceManagerCultureInfo(string name)
                : base(string.Empty) // Creates an instance of the invariant culture, which is always ok.
            {
                this.Name = name; // Sets the correct name, so that resource string lookups will work.
            }

            public override string Name { get; }

#if DEBUG
            public override string EnglishName => throw new NotImplementedException();

            public override CultureInfo Parent => throw new NotImplementedException();

            public override Calendar[] OptionalCalendars => throw new NotImplementedException();

            public override NumberFormatInfo NumberFormat => throw new NotImplementedException();

            public override string NativeName => throw new NotImplementedException();

            public override bool IsNeutralCulture => throw new NotImplementedException();

            public override string TwoLetterISOLanguageName => throw new NotImplementedException();

            public override TextInfo TextInfo => throw new NotImplementedException();

            public override DateTimeFormatInfo DateTimeFormat => throw new NotImplementedException();

            public override CompareInfo CompareInfo => throw new NotImplementedException();

            public override Calendar Calendar => throw new NotImplementedException();

            public override string DisplayName => throw new NotImplementedException();

            public override object Clone() => throw new NotImplementedException();

            public override bool Equals(object value) => throw new NotImplementedException();

            public override object GetFormat(Type formatType) => throw new NotImplementedException();

            public override int GetHashCode() => throw new NotImplementedException();

            public override string ToString() => throw new NotImplementedException();
#endif
        }
    }
}
