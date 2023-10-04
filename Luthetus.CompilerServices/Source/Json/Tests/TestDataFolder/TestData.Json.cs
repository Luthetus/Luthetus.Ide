namespace Luthetus.CompilerServices.Lang.Json.Tests.TestDataFolder;

public static partial class TestData
{
    public static class Json
    {
        public const string EXAMPLE_TEXT_ARRAY_AS_TOP_LEVEL = @"[
  {
    ""Name"": ""Joyce"",
    ""Sex"": ""F"",
    ""Age"": 11,
    ""Height"": 51.3,
    ""Weight"": 50.5
  },
  {
    ""Name"": ""Thomas"",
    ""Sex"": ""M"",
    ""Age"": 11,
    ""Height"": 57.5,
    ""Weight"": 85
  }
]";

        /// <summary>
        /// https://www.w3schools.com/js/js_json_arrays.asp
        /// </summary>
        public const string EXAMPLE_TEXT_OBJECT_WITH_ARRAY = @"{
""name"":""John"",
""age"":30,
""cars"":[""Ford"", ""BMW"", ""Fiat""]
}";

        /// <summary>
        /// Adhoc test data
        /// </summary>
        public const string EXAMPLE_ADHOC = @"{
    ""firstName"": ""Hunter"",
    ""lastName"": ""Freeman"",
    ""IIS Express"": {
        ""commandName"": ""IISExpress"",
        ""launchBrowser"": true,
        ""environmentVariables"": {
            ""ASPNETCORE_ENVIRONMENT"": ""Development""
        }
    }
}";

        /// <summary>
        /// launchSettings.json of a Blazor ServerSide application
        /// </summary>
        public const string EXAMPLE_TEXT_LAUNCH_SETTINGS = @"{
  ""iisSettings"": {
        ""windowsAuthentication"": false,
        ""anonymousAuthentication"": true,
        ""iisExpress"": {
            ""applicationUrl"": ""http://localhost:62895"",
            ""sslPort"": 44378
        }
    },
    ""profiles"": {
        ""BlazorTextEditor.Demo.ServerSide"": {
            ""commandName"": ""Project"",
            ""dotnetRunMessages"": true,
            ""launchBrowser"": true,
            ""applicationUrl"": ""https://localhost:7250;http://localhost:5106"",
            ""environmentVariables"": {
                ""ASPNETCORE_ENVIRONMENT"": ""Development""
            }
        },
        ""IIS Express"": {
            ""commandName"": ""IISExpress"",
            ""launchBrowser"": true,
            ""environmentVariables"": {
                ""ASPNETCORE_ENVIRONMENT"": ""Development""
            }
        }
    }
}
";
        /// <summary>
        /// launchSettings.json of a Blazor ServerSide application
        /// </summary>
        public const string EXAMPLE_TEXT_WITH_COMMENTS = @"{
  ""testNumberSyntaxHighlighting"": 10.73,
  ""testIntegerSyntaxHighlighting"": 951,
  ""iisSettings"": {
        ""windowsAuthentication"": false,
        // JSON with Comments
        ""anonymousAuthentication"": true,
        ""iisExpress"": {
            ""applicationUrl"": ""http://localhost:62895"",
            /*         
                JSON with Comments
            */
            ""sslPort"": 44378
        }
    },
    ""profiles"": {
        ""BlazorTextEditor.Demo.ServerSide"": {
            ""commandName"": ""Project"",
            ""dotnetRunMessages"": true,
            ""launchBrowser"": true,
            ""applicationUrl"": ""https://localhost:7250;http://localhost:5106"",
            ""environmentVariables"": {
                ""ASPNETCORE_ENVIRONMENT"": ""Development""
            }
        },
        ""IIS Express"": {
            ""commandName"": ""IISExpress"",
            ""launchBrowser"": true,
            ""environmentVariables"": {
                ""ASPNETCORE_ENVIRONMENT"": ""Development""
            }
        }
    }
}
";
    }
}