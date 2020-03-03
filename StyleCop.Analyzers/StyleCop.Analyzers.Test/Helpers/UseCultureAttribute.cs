// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using Xunit.Sdk;

    /// <summary>
    /// Apply this attribute to your test method to replace the
    /// <see cref="Thread.CurrentThread" /> <see cref="CultureInfo.CurrentCulture" /> and
    /// <see cref="CultureInfo.CurrentUICulture" /> with another culture.
    /// </summary>
    /// <remarks>
    /// <para>This code was adapted from
    /// https://github.com/xunit/samples.xunit/blob/885edfc/UseCulture/UseCultureAttribute.cs.
    /// The original code is (c) 2014 Outercurve Foundation and licensed under the Apache License,
    /// Version 2.0.</para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UseCultureAttribute : BeforeAfterTestAttribute
    {
        private readonly Lazy<CultureInfo> culture;

#pragma warning disable SA1305 // Field names should not use Hungarian notation
        private readonly Lazy<CultureInfo> uiCulture;
#pragma warning restore SA1305 // Field names should not use Hungarian notation

        private CultureInfo originalCulture;

        private CultureInfo originalUiCulture;

        private CultureInfo originalDefaultCulture;

        private CultureInfo originalDefaultUiCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UseCultureAttribute"/>
        /// class with a culture.
        /// </summary>
        /// <param name="culture">The name of the culture.</param>
        /// <remarks>
        /// <para>
        /// This constructor overload uses <paramref name="culture" /> for both
        /// <see cref="Culture" /> and <see cref="UiCulture" />.
        /// </para>
        /// </remarks>
        public UseCultureAttribute(string culture)
            : this(culture, culture)
        {
        }

#pragma warning disable SA1305 // Field names should not use Hungarian notation
        /// <summary>
        /// Initializes a new instance of the <see cref="UseCultureAttribute"/>
        /// class with a culture and a UI culture.
        /// </summary>
        /// <param name="culture">The name of the culture.</param>
        /// <param name="uiCulture">The name of the UI culture.</param>
        public UseCultureAttribute(string culture, string uiCulture)
#pragma warning restore SA1305 // Field names should not use Hungarian notation
        {
            this.culture = new Lazy<CultureInfo>(() => new CultureInfo(culture));
            this.uiCulture = new Lazy<CultureInfo>(() => new CultureInfo(uiCulture));
        }

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <value>The culture.</value>
        public CultureInfo Culture => this.culture.Value;

        /// <summary>
        /// Gets the UI culture.
        /// </summary>
        /// <value>The UI culture.</value>
        public CultureInfo UiCulture => this.uiCulture.Value;

        /// <summary>
        /// Stores the current <see cref="Thread.CurrentPrincipal" />
        /// <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
        /// and replaces them with the new cultures defined in the constructor.
        /// </summary>
        /// <param name="methodUnderTest">The method under test.</param>
        public override void Before(MethodInfo methodUnderTest)
        {
            this.originalCulture = Thread.CurrentThread.CurrentCulture;
            this.originalUiCulture = Thread.CurrentThread.CurrentUICulture;
            this.originalDefaultCulture = CultureInfo.DefaultThreadCurrentCulture;
            this.originalDefaultUiCulture = CultureInfo.DefaultThreadCurrentUICulture;

            Thread.CurrentThread.CurrentCulture = this.Culture;
            Thread.CurrentThread.CurrentUICulture = this.UiCulture;

            CultureInfo.DefaultThreadCurrentCulture = this.Culture;
            CultureInfo.DefaultThreadCurrentUICulture = this.UiCulture;
        }

        /// <summary>
        /// Restores the original <see cref="CultureInfo.CurrentCulture" /> and
        /// <see cref="CultureInfo.CurrentUICulture" /> to <see cref="Thread.CurrentPrincipal" />.
        /// </summary>
        /// <param name="methodUnderTest">The method under test.</param>
        public override void After(MethodInfo methodUnderTest)
        {
            Thread.CurrentThread.CurrentCulture = this.originalCulture;
            Thread.CurrentThread.CurrentUICulture = this.originalUiCulture;

            CultureInfo.DefaultThreadCurrentCulture = this.originalDefaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = this.originalDefaultUiCulture;
        }
    }
}
