namespace Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

public static partial class BlazorWasmEmptyFacts
{
    public const string LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH = @"Properties/launchSettings.json";

    public static string GetLaunchSettingsJsonContents(string projectName) => @$"{{
  ""iisSettings"": {{
    ""iisExpress"": {{
      ""applicationUrl"": ""http://localhost:49299"",
      ""sslPort"": 44334
    }}
  }},
  ""profiles"": {{
    ""https"": {{
      ""commandName"": ""Project"",
      ""dotnetRunMessages"": true,
      ""launchBrowser"": true,
      ""inspectUri"": ""{{wsProtocol}}://{{url.hostname}}:{{url.port}}/_framework/debug/ws-proxy?browser={{browserInspectUri}}"",
      ""applicationUrl"": ""https://localhost:7299;http://localhost:5158"",
      ""environmentVariables"": {{
        ""ASPNETCORE_ENVIRONMENT"": ""Development""
      }}
    }},
  }}
}}
";
}
