# Luthetus.TextEditor (v1.3.0)

## Installation

### Source Code
The .NET Solution: [Luthetus.TextEditor.Installation.sln](../Source/Tutorials/Installation/Luthetus.TextEditor.Installation.sln),
was made by following steps described here. So, the completed result can be found there.

### Goal

- Reference the `Luthetus.TextEditor` Nuget Package
- Register the `Services`
- Reference the `CSS`
- Reference the `JavaScript`
- In `App.razor` render the `<Fluxor.Blazor.Web.StoreInitializer/>`
- In `MainLayout.razor` render the `<Luthetus.TextEditor.RazorLib.LuthetusTextEditorInitializer/>` Blazor component

### Steps
- Reference the `Luthetus.TextEditor` NuGet Package

Use your preferred way to install NuGet Packages to install `Luthetus.TextEditor`.

The nuget.org link to the NuGet Package is here: https://www.nuget.org/packages/Luthetus.TextEditor

- Register the `Services`

Go to the file that you register your services and add the following lines of C# code.

> *NOTE:* In many C# Project templates, the services are registered in `Program.cs`.

> *NOTE:* The service registration is incredibly verbose and I intend to improve it in later versions.

```csharp
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewDisplays;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.Common.RazorLib;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Luthetus.TextEditor.RazorLib;
using Fluxor;

var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
    typeof(TreeViewTextDisplay),
    typeof(TreeViewReflectionDisplay),
    typeof(TreeViewPropertiesDisplay),
    typeof(TreeViewInterfaceImplementationDisplay),
    typeof(TreeViewFieldsDisplay),
    typeof(TreeViewExceptionDisplay),
    typeof(TreeViewEnumerableDisplay));

var commonRendererTypes = new LuthetusCommonComponentRenderers(
    typeof(CommonBackgroundTaskDisplay),
    typeof(CommonErrorNotificationDisplay),
    typeof(CommonInformativeNotificationDisplay),
    typeof(TreeViewExceptionDisplay),
    typeof(TreeViewMissingRendererFallbackDisplay),
    watchWindowTreeViewRenderers,
    null,
    typeof(CompilerServiceBackgroundTaskDisplay));

// TODO: Move registration of "ILuthetusCommonComponentRenderers" to LuthetusCommon
builder.Services.AddSingleton<ILuthetusCommonComponentRenderers>(_ => commonRendererTypes);

builder.Services.AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueue>();
builder.Services.AddSingleton<ICommonBackgroundTaskMonitor, CommonBackgroundTaskMonitor>();

builder.Services.AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueue>();
builder.Services.AddSingleton<ITextEditorBackgroundTaskMonitor, TextEditorBackgroundTaskMonitor>();

builder.Services.AddSingleton<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueue>();
builder.Services.AddSingleton<ICompilerServiceBackgroundTaskMonitor, CompilerServiceBackgroundTaskMonitor>();

builder.Services
    .AddLuthetusTextEditor()
    .AddFluxor(options =>
        options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly));
```

- If you are running a `Blazor Server Side` Application, also add these lines of code:

```csharp
builder.Services.AddSingleton<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueue>();

builder.Services.AddHostedService<CommonQueuedHostedService>();
builder.Services.AddHostedService<TextEditorQueuedHostedService>();
builder.Services.AddHostedService<CompilerServiceQueuedHostedService>();
```

- If you are running a `Blazor WebAssembly` Application, also add these lines of code:
```csharp
builder.Services.AddSingleton<CommonQueuedHostedService>();
builder.Services.AddSingleton<TextEditorQueuedHostedService>();
builder.Services.AddSingleton<CompilerServiceQueuedHostedService>();

builder.Services.AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueueSingleThreaded>();
builder.Services.AddSingleton<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueueSingleThreaded>();
```

- Reference the `CSS`

Go to the file that you reference CSS files from and add the following CSS references.

> *NOTE:* In many Blazor ServerSide apps this file would be `Pages/_Layout.cshtml`. As for Blazor WASM apps, this file tends to be `wwwroot/index.html`

```html
<link href="_content/Luthetus.Common/luthetusCommon.css" rel="stylesheet" />
<link href="_content/Luthetus.TextEditor/luthetusTextEditor.css" rel="stylesheet" />
```

> *NOTE:* `luthetusCommon.css` is not necessary to reference, if one has already done so.

- Reference the `JavaScript`

Go to the file that you reference JavaScript files from and add the following JavaScript reference below the Blazor framework JavaScript reference

> *NOTE:* In many Blazor ServerSide apps this file would be `Pages/_Layout.cshtml`. As for Blazor WASM apps, this file tends to be `wwwroot/index.html`

```html
<script src="_content/Luthetus.Common/luthetusCommon.js"></script>
<script src="_content/Luthetus.TextEditor/luthetusTextEditor.js"></script>
```

> *NOTE:* `luthetusCommon.js` is not necessary to reference, if one has already done so.

- In `App.razor` render the `<Fluxor.Blazor.Web.StoreInitializer/>` Blazor component

> *NOTE:* Luthetus repositories use the state management library named `Fluxor` ([github link](https://github.com/mrpmorris/Fluxor)).

```html
<!-- App.razor -->

<Fluxor.Blazor.Web.StoreInitializer/>

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

- In `MainLayout.razor` render both the `<Luthetus.Common.RazorLib.LuthetusCommonInitializer />` and the  `<Luthetus.TextEditor.RazorLib.LuthetusTextEditorInitializer/>` Blazor component

> *NOTE:* The placement of the `<Luthetus.Common.RazorLib.LuthetusCommonInitializer/>` Blazor component should be wrapped in an encompassing div. This allows one to cascade css. A later tutorial is intended to show this as to keep the installation tutorial more to the point.

```html
@inherits LayoutComponentBase

<PageTitle>Luthetus.Common.Installation.ServerSide</PageTitle>

<Luthetus.Common.RazorLib.LuthetusCommonInitializer />
<Luthetus.TextEditor.RazorLib.LuthetusTextEditorInitializer />

<div class="page">
    <div class="sidebar">
        <NavMenu />
    </div>

    <main>
        <div class="top-row px-4">
            <a href="https://docs.microsoft.com/aspnet/" target="_blank">About</a>
        </div>

        <article class="content px-4">
            @Body
        </article>
    </main>
</div>
```

# Next tutorial: [usage.md](./usage.md)