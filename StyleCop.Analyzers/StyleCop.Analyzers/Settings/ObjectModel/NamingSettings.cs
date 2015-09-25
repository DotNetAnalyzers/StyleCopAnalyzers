// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Immutable;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class NamingSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="AllowCommonHungarianPrefixes"/> property.
        /// </summary>
        [JsonProperty("allowCommonHungarianPrefixes", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool allowCommonHungarianPrefixes;

        /// <summary>
        /// This is the backing field for the <see cref="AllowedHungarianPrefixes"/> property.
        /// </summary>
        [JsonProperty("allowedHungarianPrefixes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableArray<string>.Builder allowedHungarianPrefixes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamingSettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal NamingSettings()
        {
            this.allowCommonHungarianPrefixes = true;
            this.allowedHungarianPrefixes = ImmutableArray<string>.Empty.ToBuilder();
        }

        public bool AllowCommonHungarianPrefixes =>
            this.allowCommonHungarianPrefixes;

        public ImmutableArray<string> AllowedHungarianPrefixes
        {
            get
            {
                return this.allowedHungarianPrefixes.ToImmutable();
            }
        }
    }
}
