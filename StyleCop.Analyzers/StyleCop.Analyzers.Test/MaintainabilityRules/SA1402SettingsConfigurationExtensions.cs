// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Linq;

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
