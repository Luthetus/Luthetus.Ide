# Luthetus.TextEditor (v4.9.0)

## Installation

The .NET Solution: [Luthetus.Tutorials.sln](../../Source/Tutorials/Luthetus.Tutorials.sln),
was made by following steps described here. So, the completed result can be found there.

### Goal

![tutorial_Usage-CSharpCompilerServiceTextEditor.gif](../../Images/TextEditor/Gifs/endResult.gif)

- Reference the `Luthetus.TextEditor` Nuget Package
- Register the `Services`
- Reference the `CSS`
- Reference the `JavaScript`
- In `MainLayout.razor` render the `<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer />` and the `<Luthetus.TextEditor.RazorLib.Installations.Displays.LuthetusTextEditorInitializer />` Blazor components

### Steps
- Reference the `Luthetus.TextEditor` NuGet Package

Use your preferred way to install NuGet Packages to install `Luthetus.TextEditor`.

The nuget.org link to the NuGet Package is here: https://www.nuget.org/packages/Luthetus.TextEditor

- Register the `Services`

Go to the file that you register your services and add the following lines of C# code.

```csharp
/* using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models; */

// Use either Wasm or ServerSide depending on how your app is being hosted.
// var luthetusHostingKind = LuthetusHostingKind.ServerSide;
var luthetusHostingKind = LuthetusHostingKind.Wasm;

var hostingInformation = new LuthetusHostingInformation(
    luthetusHostingKind,
    LuthetusPurposeKind.TextEditor,
    new BackgroundTaskService());

services.AddLuthetusTextEditor(hostingInformation);

// CompilerServiceRegistry
//
// You can create your own implementation of:
// - ICompilerServiceRegistry
// - IDecorationMapperRegistry
//
// In order to support any file extension.
//
// The "Default" ones will treat all file extensions
// as plain text.
//
return services
    .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistryDefault>()
    .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistryDefault>();
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
<!--
    The Luthetus components here, can be moved wherever.
    Preferably, these are in one's LayoutComponentBase.
    As here they cannot receive any cascading css.
-->
<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer />
<Luthetus.TextEditor.RazorLib.Installations.Displays.LuthetusTextEditorInitializer />
```

- My Entire App.razor file as of this step:

```html
<!-- App.razor -->
 
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