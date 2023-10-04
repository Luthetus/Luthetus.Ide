namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplates.Models;

public static partial class BlazorWasmEmptyFacts
{
    public const string PROGRAM_CS_RELATIVE_FILE_PATH = @"Program.cs";

    public static string GetProgramCsContents(string projectName) => @$"using {projectName};
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>(""#app"");
builder.RootComponents.Add<HeadOutlet>(""head::after"");

builder.Services.AddScoped(sp => new HttpClient {{ BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }});

await builder.Build().RunAsync();
";
}
