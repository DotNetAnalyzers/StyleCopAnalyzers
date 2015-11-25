// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class OrderingSettings
    {
        private static readonly ImmutableArray<OrderingTrait> DefaultElementOrder =
            ImmutableArray.Create(
                OrderingTrait.Kind,
                OrderingTrait.Accessibility,
                OrderingTrait.Constant,
                OrderingTrait.Static,
                OrderingTrait.Readonly);

        /// <summary>
        /// This is the backing field for the <see cref="ElementOrder"/> property.
        /// </summary>
        [JsonProperty("elementOrder", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<OrderingTrait>.Builder elementOrder;

        /// <summary>
        /// This is the backing field for the <see cref="SystemUsingDirectivesFirst"/> property.
        /// </summary>
        [JsonProperty("systemUsingDirectivesFirst", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool systemUsingDirectivesFirst;

        /// <summary>
        /// This is the backing field for the <see cref="UsingDirectivesPlacement"/> property.
        /// </summary>
        [JsonProperty("usingDirectivesPlacement", DefaultValueHandling = DefaultValueHandling.Include)]
        private UsingDirectivesPlacement usingDirectivesPlacement;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderingSettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal OrderingSettings()
        {
            this.elementOrder = ImmutableArray.CreateBuilder<OrderingTrait>();
            this.systemUsingDirectivesFirst = true;
            this.usingDirectivesPlacement = UsingDirectivesPlacement.InsideNamespace;
        }

        public ImmutableArray<OrderingTrait> ElementOrder
        {
            get
            {
                return this.elementOrder.Count > 0 ? this.elementOrder.ToImmutable() : DefaultElementOrder;
            }
        }

        public bool SystemUsingDirectivesFirst =>
            this.systemUsingDirectivesFirst;

        public UsingDirectivesPlacement UsingDirectivesPlacement =>
            this.usingDirectivesPlacement;
    }
}
