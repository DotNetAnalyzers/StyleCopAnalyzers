// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public enum SA1402SettingsConfiguration
    {
        /// <summary>
        /// Provide no custom settings
        /// </summary>
        KeepDefaultConfiguration,

        /// <summary>
        /// Provide custom settings that configure the tested type as being a top level type
        /// </summary>
        ConfigureAsTopLevelType,

        /// <summary>
        /// Provide custom settings that configure the tested type as not being a top level type
        /// </summary>
        ConfigureAsNonTopLevelType,
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1649:File name must match first type name.", Justification = "Extension method for enum above.")]
    public static class SA1402SettingsConfigurationExtensions
    {
        public static string GetSettings(this SA1402SettingsConfiguration configuration, string keyword)
        {
            if (configuration == SA1402SettingsConfiguration.KeepDefaultConfiguration)
            {
                return null;
            }

            var keywords = new List<string> { "class", "interface", "struct", "enum", "delegate" };
            if (configuration == SA1402SettingsConfiguration.ConfigureAsNonTopLevelType)
            {
                keywords.Remove(keyword);
            }

            var keywordsStr = string.Join(", ", keywords.Select(x => "\"" + x + "\""));

            var settings = $@"
{{
  ""settings"": {{
    ""maintainabilityRules"": {{
      ""topLevelTypes"": [{keywordsStr}]
    }}
  }}
}}";

            return settings;
        }
    }
}
