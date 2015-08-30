namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Immutable;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    internal class DocumentationSettings
    {
        /// <summary>
        /// The default value for the <see cref="CompanyName"/> property.
        /// </summary>
        internal const string DefaultCompanyName = "PlaceholderCompany";

        /// <summary>
        /// The default value for the <see cref="CopyrightText"/> property.
        /// </summary>
        internal const string DefaultCopyrightText = "Copyright (c) {companyName}. All rights reserved.";

        /// <summary>
        /// This is the backing field for the <see cref="CompanyName"/> property.
        /// </summary>
        [JsonProperty("companyName", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string companyName;

        /// <summary>
        /// This is the backing field for the <see cref="CopyrightText"/> property.
        /// </summary>
        [JsonProperty("copyrightText", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string copyrightText;

        /// <summary>
        /// This is the backing field for the <see cref="Variables"/> property.
        /// </summary>
        [JsonProperty("variables", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string> variables;

        /// <summary>
        /// This is the backing field for the <see cref="XmlHeader"/> property.
        /// </summary>
        [JsonProperty("xmlHeader", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool xmlHeader;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentExposedMembers"/> property.
        /// </summary>
        [JsonProperty("documentExposedMembers", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool documentExposedMembers;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInternalMembers"/> property.
        /// </summary>
        [JsonProperty("documentInternalMembers", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool documentInternalMembers;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateMembers"/> property.
        /// </summary>
        [JsonProperty("documentPrivateMembers", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool documentPrivateMembers;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInterfaces"/> property.
        /// </summary>
        [JsonProperty("documentInterfaces", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool documentInterfaces;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateFields"/> property.
        /// </summary>
        [JsonProperty("documentPrivateFields", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool documentPrivateFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal DocumentationSettings()
        {
            this.companyName = DefaultCompanyName;
            this.copyrightText = DefaultCopyrightText;
            this.variables = ImmutableDictionary<string, string>.Empty;
            this.xmlHeader = true;

            this.documentExposedMembers = true;
            this.documentInternalMembers = true;
            this.documentPrivateMembers = false;
            this.documentInterfaces = true;
            this.documentPrivateFields = false;
        }

        public string CompanyName
        {
            get
            {
                return this.companyName;
            }
        }

        public string CopyrightText
        {
            get
            {
                string pattern = Regex.Escape("{") + "(?<Property>[a-zA-Z0-9]+)" + Regex.Escape("}");
                MatchEvaluator evaluator =
                    match =>
                    {
                        string key = match.Groups["Property"].Value;
                        switch (key)
                        {
                        case "companyName":
                            return this.CompanyName;

                        case "copyrightText":
                            return "[CircularReference]";

                        default:
                            string value;
                            if (this.Variables.TryGetValue(key, out value))
                            {
                                return value;
                            }

                            break;
                        }

                        return "[InvalidReference]";
                    };

                return Regex.Replace(this.copyrightText, pattern, evaluator);
            }
        }

        public ImmutableDictionary<string, string> Variables
        {
            get
            {
                return this.variables;
            }
        }

        public bool XmlHeader
        {
            get
            {
                return this.xmlHeader;
            }
        }

        public bool DocumentExposedMembers =>
            this.documentExposedMembers;

        public bool DocumentInternalMembers =>
            this.documentInternalMembers;

        public bool DocumentPrivateMembers =>
            this.documentPrivateMembers;

        public bool DocumentInterfaces =>
            this.documentInterfaces;

        public bool DocumentPrivateFields =>
            this.documentPrivateFields;
    }
}
