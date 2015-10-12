// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class StyleCopSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="NamingRules"/> property.
        /// </summary>
        [JsonProperty("namingRules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private NamingSettings namingRules;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentationRules"/> property.
        /// </summary>
        [JsonProperty("documentationRules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private DocumentationSettings documentationRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopSettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal StyleCopSettings()
        {
            this.namingRules = new NamingSettings();
            this.documentationRules = new DocumentationSettings();
        }

        public NamingSettings NamingRules =>
            this.namingRules;

        public DocumentationSettings DocumentationRules =>
            this.documentationRules;
    }
}
