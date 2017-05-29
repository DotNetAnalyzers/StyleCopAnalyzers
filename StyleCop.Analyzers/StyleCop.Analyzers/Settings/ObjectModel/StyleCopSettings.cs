// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;

    internal class StyleCopSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="SpacingRules"/> property.
        /// </summary>
        private SpacingSettings spacingRules;

        /// <summary>
        /// This is the backing field for the <see cref="ReadabilityRules"/> property.
        /// </summary>
        private ReadabilitySettings readabilityRules;

        /// <summary>
        /// This is the backing field for the <see cref="OrderingRules"/> property.
        /// </summary>
        private OrderingSettings orderingRules;

        /// <summary>
        /// This is the backing field for the <see cref="NamingRules"/> property.
        /// </summary>
        private NamingSettings namingRules;

        /// <summary>
        /// This is the backing field for the <see cref="MaintainabilityRules"/> property.
        /// </summary>
        private MaintainabilitySettings maintainabilityRules;

        /// <summary>
        /// This is the backing field for the <see cref="LayoutRules"/> property.
        /// </summary>
        private LayoutSettings layoutRules;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentationRules"/> property.
        /// </summary>
        private DocumentationSettings documentationRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCopSettings"/> class during JSON deserialization.
        /// </summary>
        protected internal StyleCopSettings()
        {
            this.spacingRules = new SpacingSettings();
            this.readabilityRules = new ReadabilitySettings();
            this.orderingRules = new OrderingSettings();
            this.namingRules = new NamingSettings();
            this.maintainabilityRules = new MaintainabilitySettings();
            this.layoutRules = new LayoutSettings();
            this.documentationRules = new DocumentationSettings();
        }

        protected internal StyleCopSettings(JsonObject settingsObject)
            : this()
        {
            foreach (var kvp in settingsObject)
            {
                var childSettingsObject = kvp.Value.AsJsonObject;
                switch (kvp.Key)
                {
                case "spacingRules":
                    kvp.AssertIsObject();
                    this.spacingRules = new SpacingSettings(childSettingsObject);
                    break;

                case "readabilityRules":
                    kvp.AssertIsObject();
                    this.readabilityRules = new ReadabilitySettings(childSettingsObject);
                    break;

                case "orderingRules":
                    kvp.AssertIsObject();
                    this.orderingRules = new OrderingSettings(childSettingsObject);
                    break;

                case "namingRules":
                    kvp.AssertIsObject();
                    this.namingRules = new NamingSettings(childSettingsObject);
                    break;

                case "maintainabilityRules":
                    kvp.AssertIsObject();
                    this.maintainabilityRules = new MaintainabilitySettings(childSettingsObject);
                    break;

                case "layoutRules":
                    kvp.AssertIsObject();
                    this.layoutRules = new LayoutSettings(childSettingsObject);
                    break;

                case "documentationRules":
                    kvp.AssertIsObject();
                    this.documentationRules = new DocumentationSettings(childSettingsObject);
                    break;

                default:
                    break;
                }
            }
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

        public LayoutSettings LayoutRules =>
            this.layoutRules;

        public DocumentationSettings DocumentationRules =>
            this.documentationRules;
    }
}
