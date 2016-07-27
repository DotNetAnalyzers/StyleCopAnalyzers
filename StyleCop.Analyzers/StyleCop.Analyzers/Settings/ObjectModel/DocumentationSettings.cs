// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Generic;
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
        /// The default value for the <see cref="GetCopyrightText(string)"/> method.
        /// </summary>
        internal const string DefaultCopyrightText = "Copyright (c) {companyName}. All rights reserved.";

        /// <summary>
        /// This is the backing field for the <see cref="CompanyName"/> property.
        /// </summary>
        [JsonProperty("companyName", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string companyName;

        /// <summary>
        /// This is the backing field for the <see cref="GetCopyrightText(string)"/> method.
        /// </summary>
        [JsonProperty("copyrightText", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string copyrightText;

        /// <summary>
        /// This is the cache for the <see cref="GetCopyrightText(string)"/> method.
        /// </summary>
        private string copyrightTextCache;

        /// <summary>
        /// This is the backing field for the <see cref="HeaderDecoration"/> property.
        /// </summary>
        [JsonProperty("headerDecoration", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private string headerDecoration;

        /// <summary>
        /// This is the backing field for the <see cref="Variables"/> property.
        /// </summary>
        [JsonProperty("variables", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private ImmutableDictionary<string, string>.Builder variables;

        /// <summary>
        /// This is the backing field for the <see cref="XmlHeader"/> property.
        /// </summary>
        [JsonProperty("xmlHeader", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool xmlHeader;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentExposedElements"/> property.
        /// </summary>
        [JsonProperty("documentExposedElements", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool documentExposedElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInternalElements"/> property.
        /// </summary>
        [JsonProperty("documentInternalElements", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool documentInternalElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateElements"/> property.
        /// </summary>
        [JsonProperty("documentPrivateElements", DefaultValueHandling = DefaultValueHandling.Include)]
        private bool documentPrivateElements;

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
        /// This is the backing field for the <see cref="FileNamingConvention"/> property.
        /// </summary>
        [JsonProperty("fileNamingConvention", DefaultValueHandling = DefaultValueHandling.Include)]
        private FileNamingConvention fileNamingConvention;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSettings"/> class during JSON deserialization.
        /// </summary>
        [JsonConstructor]
        protected internal DocumentationSettings()
        {
            this.companyName = DefaultCompanyName;
            this.copyrightText = DefaultCopyrightText;
            this.headerDecoration = null;
            this.variables = ImmutableDictionary<string, string>.Empty.ToBuilder();
            this.xmlHeader = true;

            this.documentExposedElements = true;
            this.documentInternalElements = true;
            this.documentPrivateElements = false;
            this.documentInterfaces = true;
            this.documentPrivateFields = false;

            this.fileNamingConvention = FileNamingConvention.StyleCop;
        }

        public string CompanyName
        {
            get
            {
                return this.companyName;
            }
        }

        public string HeaderDecoration
        {
            get
            {
                return this.headerDecoration;
            }
        }

        public ImmutableDictionary<string, string> Variables
        {
            get
            {
                return this.variables.ToImmutable();
            }
        }

        public bool XmlHeader
        {
            get
            {
                return this.xmlHeader;
            }
        }

        public bool DocumentExposedElements =>
            this.documentExposedElements;

        public bool DocumentInternalElements =>
            this.documentInternalElements;

        public bool DocumentPrivateElements =>
            this.documentPrivateElements;

        public bool DocumentInterfaces =>
            this.documentInterfaces;

        public bool DocumentPrivateFields =>
            this.documentPrivateFields;

        public FileNamingConvention FileNamingConvention =>
            this.fileNamingConvention;

        public string GetCopyrightText(string fileName)
        {
            string copyrightText = this.copyrightTextCache;
            if (copyrightText != null)
            {
                return copyrightText;
            }

            var expandedCopyrightText = this.BuildCopyrightText(fileName);
            if (!expandedCopyrightText.Value)
            {
                // Unable to cache the copyright text due to use of a {fileName} variable.
                return expandedCopyrightText.Key;
            }

            this.copyrightTextCache = expandedCopyrightText.Key;
            return this.copyrightTextCache;
        }

        private KeyValuePair<string, bool> BuildCopyrightText(string fileName)
        {
            bool canCache = true;
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

                        if (key == "fileName")
                        {
                            // The 'fileName' built-in variable is only applied when the user did not include an
                            // explicit value for a custom 'fileName' variable.
                            canCache = false;
                            return fileName;
                        }

                        break;
                    }

                    return "[InvalidReference]";
                };

            string expanded = Regex.Replace(this.copyrightText, pattern, evaluator);
            return new KeyValuePair<string, bool>(expanded, canCache);
        }
    }
}
