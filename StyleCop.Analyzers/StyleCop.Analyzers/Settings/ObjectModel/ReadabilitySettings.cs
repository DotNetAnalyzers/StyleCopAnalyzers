// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class ReadabilitySettings
    {
        [JsonProperty("attributeParameterSplitting", DefaultValueHandling = DefaultValueHandling.Include)]
        private AttributeParameterSplitting attributeParameterSplitting;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadabilitySettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal ReadabilitySettings()
        {
            this.attributeParameterSplitting = AttributeParameterSplitting.Default;
        }

        public AttributeParameterSplitting AttributeParameterSplitting =>
            this.attributeParameterSplitting;
    }
}
