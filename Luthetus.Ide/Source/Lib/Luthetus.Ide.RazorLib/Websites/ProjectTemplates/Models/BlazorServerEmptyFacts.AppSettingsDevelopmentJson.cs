namespace Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;

public static partial class BlazorServerEmptyFacts
{
    public const string APP_SETTINGS_DEVELOPMENT_JSON_RELATIVE_FILE_PATH = @"appsettings.Development.json";

    public static string GetAppSettingsDevelopmentJsonContents(string projectName) => @$"{{
  ""DetailedErrors"": true,
  ""Logging"": {{
    ""LogLevel"": {{
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }}
  }}
}}
";
}
