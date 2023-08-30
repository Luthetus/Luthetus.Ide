using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates.BlazorServerEmptyCase;

public static partial class BlazorServerEmptyFacts
{
    public const string APP_SETTINGS_JSON_RELATIVE_FILE_PATH = @"appsettings.json";

    public static string GetAppSettingsJsonContents(string projectName) => @$"{{
  ""Logging"": {{
    ""LogLevel"": {{
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }}
  }},
  ""AllowedHosts"": ""*""
}}
";
}
