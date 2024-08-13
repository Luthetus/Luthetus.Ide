# Luthetus.TextEditor (v2.0.0)

## Installation

The .NET Solution: [Luthetus.Tutorials.sln](../../Source/Tutorials/Luthetus.Tutorials.sln),
was made by following steps described here. So, the completed result can be found there.

### Goal

![tutorial_Usage-CSharpCompilerServiceTextEditor.gif](../../Images/TextEditor/Gifs/text-editor-tutorial-result.gif)

- Reference the `Luthetus.TextEditor` Nuget Package
- Register the `Services`
- Reference the `CSS`
- Reference the `JavaScript`
- In `App.razor` render the `<Fluxor.Blazor.Web.StoreInitializer/>`
- In `MainLayout.razor` render the `<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer />` and the `<Luthetus.TextEditor.RazorLib.Installations.Displays.LuthetusTextEditorInitializer />` Blazor components

### Steps
- Reference the `Luthetus.TextEditor` NuGet Package

Use your preferred way to install NuGet Packages to install `Luthetus.TextEditor`.

The nuget.org link to the NuGet Package is here: https://www.nuget.org/packages/Luthetus.TextEditor

- Register the `Services`

Go to the file that you register your services and add the following lines of C# code.

```csharp

using Fluxor;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;


// Use either Wasm or ServerSide depending on how your app is being hosted.
var luthetusHostingKind = LuthetusHostingKind.Wasm;
// var luthetusHostingKind = LuthetusHostingKind.ServerSide;

var hostingInformation = new LuthetusHostingInformation(
    luthetusHostingKind,
    LuthetusPurposeKind.TextEditor,
    new BackgroundTaskService());

services.AddLuthetusTextEditor(hostingInformation);
        
services
    .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistry>()
    .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistry>();

services.AddFluxor(options => options.ScanAssemblies(
    typeof(LuthetusCommonConfig).Assembly,
    typeof(LuthetusTextEditorConfig).Assembly));

// If NOT running ServerSide then one needs to run:
// 'hostingInformation.StartBackgroundTaskWorkers(host.Services);'
//
// Some builders have 'Build()' invoked then fluent API into their 'Run()' or 'RunAsync()'.
//
// This might require one to capture the 'Build()' result
//
// This can be done anywhere, so long as the services have been built

var host = builder.Build();
hostingInformation.StartBackgroundTaskWorkers(host.Services);

await host.RunAsync();
```

- Reference the `CSS`

Go to the file that you reference CSS files from and add the following CSS references.

```html
<link href="_content/Luthetus.Common/luthetusCommon.css" rel="stylesheet" />
<link href="_content/Luthetus.TextEditor/luthetusTextEditor.css" rel="stylesheet" />
```

- Reference the `JavaScript`

Go to the file that you reference JavaScript files from and add the following JavaScript reference below the Blazor framework JavaScript reference

```html
<script src="_content/Luthetus.Common/luthetusCommon.js"></script>
<script src="_content/Luthetus.TextEditor/luthetusTextEditor.js"></script>
```

- In `App.razor` add the following towards the top of the file:

```html
<Fluxor.Blazor.Web.StoreInitializer />

<!--
    The Luthetus components here, can be more wherever.
    Preferably, these are in one's LayoutComponentBase.
    As here they cannot receive any cascading css.
-->
<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer />
<Luthetus.TextEditor.RazorLib.Installations.Displays.LuthetusTextEditorInitializer />
```

> *NOTE:* Luthetus repositories use the state management library named `Fluxor` ([github link](https://github.com/mrpmorris/Fluxor)).

```html
<!-- App.razor -->

<Fluxor.Blazor.Web.StoreInitializer />

<!--
    The Luthetus components here, can be more wherever.
    Preferably, these are in one's LayoutComponentBase.
    As here they cannot receive any cascading css.
-->
<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer />
<Luthetus.TextEditor.RazorLib.Installations.Displays.LuthetusTextEditorInitializer />

<Router AppAssembly="@typeof(App).Assembly" AdditionalAssemblies="new [] { typeof(MainLayout).Assembly }">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)"/>
        <FocusOnNavigate RouteData="@routeData" Selector="h1"/>
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

# Next tutorial: [usage.md](./usage.md)