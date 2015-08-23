namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class StyleCopSettings
    {
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
            this.documentationRules = new DocumentationSettings();
        }

        public DocumentationSettings DocumentationRules
        {
            get
            {
                return this.documentationRules;
            }
        }
    }
}
