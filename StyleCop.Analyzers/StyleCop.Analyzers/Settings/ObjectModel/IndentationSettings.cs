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
        /// Initializes a new instance of the <see cref="IndentationSettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal IndentationSettings()
        {
            this.indentationSize = 4;
            this.tabSize = 4;
            this.useTabs = false;
        }

        public int IndentationSize =>
            this.indentationSize;

        public int TabSize =>
            this.tabSize;

        public bool UseTabs =>
            this.useTabs;
    }
}
