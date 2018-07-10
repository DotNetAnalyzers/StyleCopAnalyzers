// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Text.RegularExpressions;
    using LightJson;

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
        /// The default value for the <see cref="DocumentationCulture"/> property.
        /// </summary>
        internal const string DefaultDocumentationCulture = "en-US";

        /// <summary>
        /// The default value for the <see cref="ExcludeFromPunctuationCheck"/> property.
        /// </summary>
        internal static readonly ImmutableArray<string> DefaultExcludeFromPunctuationCheck = ImmutableArray.Create("seealso");

        /// <summary>
        /// This is the backing field for the <see cref="CompanyName"/> property.
        /// </summary>
        private readonly string companyName;

        /// <summary>
        /// This is the backing field for the <see cref="GetCopyrightText(string)"/> method.
        /// </summary>
        private readonly string copyrightText;

        /// <summary>
        /// This is the backing field for the <see cref="HeaderDecoration"/> property.
        /// </summary>
        private readonly string headerDecoration;

        /// <summary>
        /// This is the backing field for the <see cref="Variables"/> property.
        /// </summary>
        private readonly ImmutableDictionary<string, string>.Builder variables;

        /// <summary>
        /// This is the backing field for the <see cref="XmlHeader"/> property.
        /// </summary>
        private readonly bool xmlHeader;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentExposedElements"/> property.
        /// </summary>
        private readonly bool documentExposedElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInternalElements"/> property.
        /// </summary>
        private readonly bool documentInternalElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateElements"/> property.
        /// </summary>
        private readonly bool documentPrivateElements;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentInterfaces"/> property.
        /// </summary>
        private readonly bool documentInterfaces;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentPrivateFields"/> property.
        /// </summary>
        private readonly bool documentPrivateFields;

        /// <summary>
        /// This is the backing field for the <see cref="FileNamingConvention"/> property.
        /// </summary>
        private readonly FileNamingConvention fileNamingConvention;

        /// <summary>
        /// This is the backing field for the <see cref="DocumentationCulture"/> property.
        /// </summary>
        private readonly string documentationCulture;

        /// <summary>
        /// This is the backing field for the <see cref="ExcludeFromPunctuationCheck"/> property.
        /// </summary>
        private readonly ImmutableArray<string> excludeFromPunctuationCheck;

        /// <summary>
        /// This is the cache for the <see cref="GetCopyrightText(string)"/> method.
        /// </summary>
        private string copyrightTextCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSettings"/> class during JSON deserialization.
        /// </summary>
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

            this.documentationCulture = DefaultDocumentationCulture;

            this.excludeFromPunctuationCheck = DefaultExcludeFromPunctuationCheck;
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

                case "headerDecoration":
                    this.headerDecoration = kvp.ToStringValue();
                    break;

                case "variables":
                    kvp.AssertIsObject();
                    foreach (var child in kvp.Value.AsJsonObject)
                    {
                        string name = child.Key;

                        if (!Regex.IsMatch(name, "^[a-zA-Z0-9]+$"))
                        {
                            continue;
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

                case "documentationCulture":
                    this.documentationCulture = kvp.ToStringValue();
                    break;

                case "excludeFromPunctuationCheck":
                    kvp.AssertIsArray();
                    var excludedTags = ImmutableArray.CreateBuilder<string>();
                    foreach (var value in kvp.Value.AsJsonArray)
                    {
                        excludedTags.Add(value.AsString);
                    }

                    this.excludeFromPunctuationCheck = excludedTags.ToImmutable();
                    break;

                default:
                    break;
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

        public string DocumentationCulture =>
            this.documentationCulture;

        public ImmutableArray<string> ExcludeFromPunctuationCheck
            => this.excludeFromPunctuationCheck;

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
