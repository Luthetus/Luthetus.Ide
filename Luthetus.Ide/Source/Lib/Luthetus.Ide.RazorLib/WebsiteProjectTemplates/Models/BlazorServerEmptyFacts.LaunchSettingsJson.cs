namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;

public static partial class BlazorServerEmptyFacts
{
    public const string LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH = @"Properties/launchSettings.json";

    public static string GetLaunchSettingsJsonContents(string projectName) => @$"{{
  ""iisSettings"": {{
    ""iisExpress"": {{
      ""applicationUrl"": ""http://localhost:17381"",
      ""sslPort"": 44346
    }}
  }},
  ""profiles"": {{
    ""http"": {{
      ""commandName"": ""Project"",
      ""dotnetRunMessages"": true,
      ""launchBrowser"": true,
      ""applicationUrl"": ""http://localhost:5082"",
      ""environmentVariables"": {{
        ""ASPNETCORE_ENVIRONMENT"": ""Development""
      }}
    }},
    ""https"": {{
      ""commandName"": ""Project"",
      ""dotnetRunMessages"": true,
      ""launchBrowser"": true,
      ""applicationUrl"": ""https://localhost:7194;http://localhost:5082"",
      ""environmentVariables"": {{
        ""ASPNETCORE_ENVIRONMENT"": ""Development""
      }}
    }},
    ""IIS Express"": {{
      ""commandName"": ""IISExpress"",
      ""launchBrowser"": true,
      ""environmentVariables"": {{
        ""ASPNETCORE_ENVIRONMENT"": ""Development""
      }}
    }}
  }}
}}
";
}
