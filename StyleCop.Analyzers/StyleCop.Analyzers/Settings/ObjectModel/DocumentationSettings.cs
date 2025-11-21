// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using LightJson;
    using StyleCop.Analyzers.Lightup;

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
        private readonly ImmutableDictionary<string, string> variables;

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
        private readonly InterfaceDocumentationMode documentInterfaces;

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
        /// This is backing field for the <see cref="DocumentationCultureInfo"/> property.
        /// </summary>
        private readonly CultureInfo documentationCultureInfo;

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
            this.headerDecoration = string.Empty;
            this.variables = ImmutableDictionary<string, string>.Empty;
            this.xmlHeader = true;

            this.documentExposedElements = true;
            this.documentInternalElements = true;
            this.documentPrivateElements = false;
            this.documentInterfaces = InterfaceDocumentationMode.All;
            this.documentPrivateFields = false;

            this.fileNamingConvention = FileNamingConvention.StyleCop;

            this.documentationCulture = DefaultDocumentationCulture;
            this.documentationCultureInfo = CultureInfo.InvariantCulture;

            this.excludeFromPunctuationCheck = DefaultExcludeFromPunctuationCheck;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSettings"/> class.
        /// </summary>
        /// <param name="documentationSettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal DocumentationSettings(JsonObject documentationSettingsObject, AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            bool? documentExposedElements = null;
            bool? documentInternalElements = null;
            bool? documentPrivateElements = null;
            InterfaceDocumentationMode? documentInterfaces = null;
            bool? documentPrivateFields = null;
            string companyName = null;
            string copyrightText = null;
            string headerDecoration = null;
            ImmutableDictionary<string, string>.Builder variables = null;
            bool? xmlHeader = null;
            FileNamingConvention? fileNamingConvention = null;
            string documentationCulture = null;
            ImmutableArray<string>.Builder excludeFromPunctuationCheck = null;

            foreach (var kvp in documentationSettingsObject)
            {
                switch (kvp.Key)
                {
                case "documentExposedElements":
                    documentExposedElements = kvp.ToBooleanValue();
                    break;

                case "documentInternalElements":
                    documentInternalElements = kvp.ToBooleanValue();
                    break;

                case "documentPrivateElements":
                    documentPrivateElements = kvp.ToBooleanValue();
                    break;

                case "documentInterfaces":
                    documentInterfaces = ParseDocumentInterfacesValue(kvp);
                    break;

                case "documentPrivateFields":
                    documentPrivateFields = kvp.ToBooleanValue();
                    break;

                case "companyName":
                    companyName = kvp.ToStringValue();
                    break;

                case "copyrightText":
                    copyrightText = kvp.ToStringValue();
                    break;

                case "headerDecoration":
                    headerDecoration = kvp.ToStringValue();
                    break;

                case "variables":
                    kvp.AssertIsObject();
                    variables = ImmutableDictionary.CreateBuilder<string, string>();
                    foreach (var child in kvp.Value.AsJsonObject)
                    {
                        string name = child.Key;

                        if (!IsValidVariableName(name))
                        {
                            continue;
                        }

                        string value = child.ToStringValue();

                        variables.Add(name, value);
                    }

                    break;

                case "xmlHeader":
                    xmlHeader = kvp.ToBooleanValue();
                    break;

                case "fileNamingConvention":
                    fileNamingConvention = kvp.ToEnumValue<FileNamingConvention>();
                    break;

                case "documentationCulture":
                    documentationCulture = kvp.ToStringValue();
                    break;

                case "excludeFromPunctuationCheck":
                    kvp.AssertIsArray();
                    excludeFromPunctuationCheck = ImmutableArray.CreateBuilder<string>();
                    foreach (var value in kvp.Value.AsJsonArray)
                    {
                        excludeFromPunctuationCheck.Add(value.AsString);
                    }

                    break;

                default:
                    break;
                }
            }

            documentExposedElements ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.documentation.documentExposedElements");
            documentInternalElements ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.documentation.documentInternalElements");
            documentPrivateElements ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.documentation.documentPrivateElements");
            documentInterfaces ??= TryGetDocumentInterfacesValue(analyzerConfigOptions);
            documentPrivateFields ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.documentation.documentPrivateFields");

            companyName ??= AnalyzerConfigHelper.TryGetStringValue(analyzerConfigOptions, "stylecop.documentation.companyName");
            copyrightText ??= AnalyzerConfigHelper.TryGetMultiLineStringValue(analyzerConfigOptions, "stylecop.documentation.copyrightText")
                ?? AnalyzerConfigHelper.TryGetMultiLineStringValue(analyzerConfigOptions, "file_header_template");
            headerDecoration ??= AnalyzerConfigHelper.TryGetStringValue(analyzerConfigOptions, "stylecop.documentation.headerDecoration");

            xmlHeader ??= AnalyzerConfigHelper.TryGetBooleanValue(analyzerConfigOptions, "stylecop.documentation.xmlHeader");
            fileNamingConvention ??= AnalyzerConfigHelper.TryGetStringValue(analyzerConfigOptions, "stylecop.documentation.fileNamingConvention") switch
            {
                "stylecop" => FileNamingConvention.StyleCop,
                "metadata" => FileNamingConvention.Metadata,
                _ => null,
            };

            documentationCulture ??= AnalyzerConfigHelper.TryGetStringValue(analyzerConfigOptions, "stylecop.documentation.documentationCulture");
            excludeFromPunctuationCheck ??= AnalyzerConfigHelper.TryGetStringListValue(analyzerConfigOptions, "stylecop.documentation.excludeFromPunctuationCheck")?.ToBuilder();

            this.documentExposedElements = documentExposedElements.GetValueOrDefault(true);
            this.documentInternalElements = documentInternalElements.GetValueOrDefault(true);
            this.documentPrivateElements = documentPrivateElements.GetValueOrDefault(false);
            this.documentInterfaces = documentInterfaces ?? InterfaceDocumentationMode.All;
            this.documentPrivateFields = documentPrivateFields.GetValueOrDefault(false);
            this.companyName = companyName ?? DefaultCompanyName;
            this.copyrightText = copyrightText ?? DefaultCopyrightText;
            this.headerDecoration = headerDecoration ?? string.Empty;
            this.variables = variables?.ToImmutable() ?? ImmutableDictionary<string, string>.Empty;
            this.xmlHeader = xmlHeader.GetValueOrDefault(true);
            this.fileNamingConvention = fileNamingConvention.GetValueOrDefault(FileNamingConvention.StyleCop);
            this.documentationCulture = documentationCulture ?? DefaultDocumentationCulture;
            this.documentationCultureInfo = this.documentationCulture == DefaultDocumentationCulture ? CultureInfo.InvariantCulture : new CultureInfo(this.documentationCulture);
            this.excludeFromPunctuationCheck = excludeFromPunctuationCheck?.ToImmutable() ?? DefaultExcludeFromPunctuationCheck;
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

        public bool DocumentExposedElements =>
            this.documentExposedElements;

        public bool DocumentInternalElements =>
            this.documentInternalElements;

        public bool DocumentPrivateElements =>
            this.documentPrivateElements;

        public InterfaceDocumentationMode DocumentInterfaces =>
            this.documentInterfaces;

        public bool DocumentPrivateFields =>
            this.documentPrivateFields;

        public FileNamingConvention FileNamingConvention =>
            this.fileNamingConvention;

        public string DocumentationCulture =>
            this.documentationCulture;

        public ImmutableArray<string> ExcludeFromPunctuationCheck
            => this.excludeFromPunctuationCheck;

        public CultureInfo DocumentationCultureInfo
            => this.documentationCultureInfo;

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

        private static InterfaceDocumentationMode ParseDocumentInterfacesValue(KeyValuePair<string, JsonValue> kvp)
        {
            if (kvp.Value.IsBoolean)
            {
                return kvp.Value.AsBoolean ? InterfaceDocumentationMode.All : InterfaceDocumentationMode.None;
            }

            if (kvp.Value.IsString)
            {
                return kvp.ToEnumValue<InterfaceDocumentationMode>();
            }

            throw new StyleCop.Analyzers.InvalidSettingsException($"{kvp.Key} must contain a boolean or string value");
        }

        private static InterfaceDocumentationMode? TryGetDocumentInterfacesValue(AnalyzerConfigOptionsWrapper analyzerConfigOptions)
        {
            var value = AnalyzerConfigHelper.TryGetStringValue(analyzerConfigOptions, "stylecop.documentation.documentInterfaces");
            if (value is null)
            {
                return null;
            }

            if (bool.TryParse(value, out var boolValue))
            {
                return boolValue ? InterfaceDocumentationMode.All : InterfaceDocumentationMode.None;
            }

            switch (value.ToLowerInvariant())
            {
            case "all":
                return InterfaceDocumentationMode.All;
            case "exposed":
                return InterfaceDocumentationMode.Exposed;
            case "none":
                return InterfaceDocumentationMode.None;
            default:
                return null;
            }
        }

        private static bool IsValidVariableName(string name)
        {
            // Equivalent to Regex.IsMatch(prefix, "^[a-zA-Z0-9]+$")
            for (var i = 0; i < name.Length; i++)
            {
                if (name[i] is not ((>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or (>= '0' and <= '9')))
                {
                    return false;
                }
            }

            return name.Length > 0;
        }

        private KeyValuePair<string, bool> BuildCopyrightText(string fileName)
        {
            bool canCache = true;

            string Evaluator(Match match)
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
            }

            string pattern = Regex.Escape("{") + "(?<Property>[a-zA-Z0-9]+)" + Regex.Escape("}");
            string expanded = Regex.Replace(this.copyrightText, pattern, Evaluator);
            return new KeyValuePair<string, bool>(expanded, canCache);
        }
    }
}
