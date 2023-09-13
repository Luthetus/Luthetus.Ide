namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.RazorClassLibCase;

public partial class RazorClassLibFacts
{
    public const string EXAMPLE_JS_INTEROP_CS_RELATIVE_FILE_PATH = @"ExampleJsInterop.cs";

    public static string GetExampleJsInteropCsContents(string projectName) => @$"using Microsoft.JSInterop;

namespace {projectName}
{{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class ExampleJsInterop : IAsyncDisposable
    {{
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public ExampleJsInterop(IJSRuntime jsRuntime)
        {{
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                ""import"", ""./_content/{projectName}/exampleJsInterop.js"").AsTask());
        }}

        public async ValueTask<string> Prompt(string message)
        {{
            var module = await moduleTask.Value;
            return await module.InvokeAsync<string>(""showPrompt"", message);
        }}

        public async ValueTask DisposeAsync()
        {{
            if (moduleTask.IsValueCreated)
            {{
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }}
        }}
    }}
}}";
}
