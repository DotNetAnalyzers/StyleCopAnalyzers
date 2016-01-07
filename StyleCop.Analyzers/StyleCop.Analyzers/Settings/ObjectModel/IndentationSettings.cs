// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class IndentationSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="IndentationSize"/> property.
        /// </summary>
        [JsonProperty("indentationSize", DefaultValueHandling = DefaultValueHandling.Include)]
        private int indentationSize;

        /// <summary>
        /// This is the backing field for the <see cref="TabSize"/> property.
        /// </summary>
        [JsonProperty("tabSize", DefaultValueHandling = DefaultValueHandling.Include)]
        private int tabSize;

        /// <summary>
        /// This is the backing field for the <see cref="UseTabs"/> property.
        /// </summary>
        [JsonProperty("useTabs", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool useTabs;

        /// <summary>
        /// This is the backing field for the <see cref="IndentBlock"/> property.
        /// </summary>
        [JsonProperty("indentBlock", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool? indentBlock;

        /// <summary>
        /// This is the backing field for the <see cref="IndentSwitchSection"/> property.
        /// </summary>
        [JsonProperty("indentSwitchSection", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool? indentSwitchSection;

        /// <summary>
        /// This is the backing field for the <see cref="IndentSwitchCaseSection"/> property.
        /// </summary>
        [JsonProperty("indentSwitchCaseSection", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool? indentSwitchCaseSection;

        /// <summary>
        /// This is the backing field for the <see cref="LabelPositioning"/> property.
        /// </summary>
        [JsonProperty("labelPositioning", DefaultValueHandling = DefaultValueHandling.Include)]
        private LabelPositioning? labelPositioning;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndentationSettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal IndentationSettings()
        {
            this.indentationSize = 4;
            this.tabSize = 4;
            this.useTabs = false;

            this.indentBlock = true;
            this.indentSwitchSection = true;
            this.indentSwitchCaseSection = true;
            this.labelPositioning = ObjectModel.LabelPositioning.OneLess;
        }

        public int IndentationSize =>
            this.indentationSize;

        public int TabSize =>
            this.tabSize;

        public bool UseTabs =>
            this.useTabs;

        public bool? IndentBlock =>
            this.indentBlock;

        public bool? IndentSwitchSection =>
            this.indentSwitchSection;

        public bool? IndentSwitchCaseSection =>
            this.indentSwitchCaseSection;

        public LabelPositioning? LabelPositioning =>
            this.labelPositioning;
    }
}
