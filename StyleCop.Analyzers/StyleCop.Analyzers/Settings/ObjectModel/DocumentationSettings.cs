// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Text.RegularExpressions;
    using LightJson;

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
        private string companyName;

        /// <summary>
        /// This is the backing field for the <see cref="CopyrightText"/> property.
        /// </summary>
        private string copyrightText;

        /// <summary>
        /// This is the cache for the <see cref="CopyrightText"/> property.
        /// </summary>
        private string copyrightTextCache;

        /// <summary>
        /// This is the backing field for the <see cref="Variables"/> property.
        /// </summary>
        private ImmutableDictionary<string, string>.Builder variables;

        /// <summary>
        /// This is the backing field for the <see cref="XmlHeader"/> property.
        /// </summary>
        private bool xmlHeader;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentExposedElements"/> property.
        /// </summary>
        private bool documentExposedElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInternalElements"/> property.
        /// </summary>
        private bool documentInternalElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateElements"/> property.
        /// </summary>
        private bool documentPrivateElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInterfaces"/> property.
        /// </summary>
        private bool documentInterfaces;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateFields"/> property.
        /// </summary>
        private bool documentPrivateFields;

        /// <summary>
        /// This is the backing field for the <see cref="FileNamingConvention"/> property.
        /// </summary>
        private FileNamingConvention fileNamingConvention;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSettings"/> class during JSON deserialization.
        /// </summary>
        protected internal DocumentationSettings()
        {
            this.companyName = DefaultCompanyName;
            this.copyrightText = DefaultCopyrightText;
            this.variables = ImmutableDictionary<string, string>.Empty.ToBuilder();
            this.xmlHeader = true;

            this.documentExposedElements = true;
            this.documentInternalElements = true;
            this.documentPrivateElements = false;
            this.documentInterfaces = true;
            this.documentPrivateFields = false;

            this.fileNamingConvention = FileNamingConvention.StyleCop;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSettings"/> class.
        /// </summary>
        /// <param name="documentationSettingsObject">The JSON object containing the settings.</param>
        protected internal DocumentationSettings(JsonObject documentationSettingsObject)
            : this()
        {
            foreach (var kvp in documentationSettingsObject)
            {
                switch (kvp.Key)
                {
                case "documentExposedElements":
                    this.documentExposedElements = kvp.ToBooleanValue();
                    break;

                case "documentInternalElements":
                    this.documentInternalElements = kvp.ToBooleanValue();
                    break;

                case "documentPrivateElements":
                    this.documentPrivateElements = kvp.ToBooleanValue();
                    break;

                case "documentInterfaces":
                    this.documentInterfaces = kvp.ToBooleanValue();
                    break;

                case "documentPrivateFields":
                    this.documentPrivateFields = kvp.ToBooleanValue();
                    break;

                case "companyName":
                    this.companyName = kvp.ToStringValue();
                    break;
                case "copyrightText":
                    this.copyrightText = kvp.ToStringValue();
                    break;

                case "variables":
                    kvp.AssertIsObject();
                    foreach (var child in kvp.Value.AsJsonObject)
                    {
                        string name = child.Key;

                        if (!Regex.IsMatch(name, "^[a-zA-Z0-9]+$"))
                        {
                            throw new InvalidSettingsException($"{name} contains invalid characters");
                        }

                        string value = child.ToStringValue();

                        this.variables.Add(name, value);
                    }

                    break;

                case "xmlHeader":
                    this.xmlHeader = kvp.ToBooleanValue();
                    break;

                case "fileNamingConvention":
                    this.fileNamingConvention = kvp.ToEnumValue<FileNamingConvention>();
                    break;

                default:
                    throw new InvalidSettingsException($"documentationRules should not contain a child named {kvp.Key}");
                }
            }
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
                if (this.copyrightTextCache == null)
                {
                    this.copyrightTextCache = this.BuildCopyrightText();
                }

                return this.copyrightTextCache;
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

        private string BuildCopyrightText()
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
}
