// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class StyleCopSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="SpacingRules"/> property.
        /// </summary>
        [JsonProperty("spacingRules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private SpacingSettings spacingRules;

        /// <summary>
        /// This is the backing field for the <see cref="ReadabilityRules"/> property.
        /// </summary>
        [JsonProperty("readabilityRules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ReadabilitySettings readabilityRules;

        /// <summary>
        /// This is the backing field for the <see cref="OrderingRules"/> property.
        /// </summary>
        [JsonProperty("orderingRules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private OrderingSettings orderingRules;

        /// <summary>
        /// This is the backing field for the <see cref="NamingRules"/> property.
        /// </summary>
        [JsonProperty("namingRules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private NamingSettings namingRules;

        /// <summary>
        /// This is the backing field for the <see cref="MaintainabilityRules"/> property.
        /// </summary>
        [JsonProperty("maintainabilityRules", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private MaintainabilitySettings maintainabilityRules;

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
            this.spacingRules = new SpacingSettings();
            this.readabilityRules = new ReadabilitySettings();
            this.orderingRules = new OrderingSettings();
            this.namingRules = new NamingSettings();
            this.maintainabilityRules = new MaintainabilitySettings();
            this.documentationRules = new DocumentationSettings();
        }

        public SpacingSettings SpacingRules =>
            this.spacingRules;

        public ReadabilitySettings ReadabilityRules =>
            this.readabilityRules;

        public OrderingSettings OrderingRules =>
            this.orderingRules;

        public NamingSettings NamingRules =>
            this.namingRules;

        public MaintainabilitySettings MaintainabilityRules =>
            this.maintainabilityRules;

        public DocumentationSettings DocumentationRules =>
            this.documentationRules;
    }
}
