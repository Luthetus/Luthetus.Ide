namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;

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
