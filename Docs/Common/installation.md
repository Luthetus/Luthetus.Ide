# Luthetus.Common (v1.4.0)

## Installation

### Source Code
The .NET Solution: [Luthetus.Common.Usage.sln](../../Source/Tutorials/Common/Luthetus.Common.Usage.sln),
was made by following steps described here. So, the completed result can be found there.

### Goal

- Reference the `Luthetus.Common` Nuget Package
- Register the `Services`
- Reference the `CSS`
- Reference the `JavaScript`
- In `App.razor` render the `<Fluxor.Blazor.Web.StoreInitializer/>`
- In `MainLayout.razor` render the `<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer/>` Blazor component

### Steps
- Reference the `Luthetus.Common` NuGet Package

Use your preferred way to install NuGet Packages to install `Luthetus.Common`.

The nuget.org link to the NuGet Package is here: https://www.nuget.org/packages/Luthetus.Common

- Register the `Services`

Go to the file that you register your services and add the following lines of C# code.

> *NOTE:* In many C# Project templates, the services are registered in `Program.cs`.

```csharp
// using Luthetus.Common.RazorLib.Installations.Models;
// using Fluxor;

var luthetusHostingInformation = new LuthetusHostingInformation(
    //LuthetusHostingKind.Wasm,
    // OR
    // LuthetusHostingKind.ServerSide,
    new BackgroundTaskService());

return services
    .AddLuthetusCommonServices(luthetusHostingInformation) // 
    .AddFluxor(options => options.ScanAssemblies(
        typeof(LuthetusCommonOptions).Assembly));
```

- Reference the `CSS`

Go to the file that you reference CSS files from and add the following CSS reference.

> *NOTE:* In many Blazor ServerSide apps this file would be `Pages/_Layout.cshtml`. As for Blazor WASM apps, this file tends to be `wwwroot/index.html`

```html
<link href="_content/Luthetus.Common/luthetusCommon.css" rel="stylesheet" />
```

- Reference the `JavaScript`

Go to the file that you reference JavaScript files from and add the following JavaScript reference below the Blazor framework JavaScript reference

> *NOTE:* In many Blazor ServerSide apps this file would be `Pages/_Layout.cshtml`. As for Blazor WASM apps, this file tends to be `wwwroot/index.html`

```html
<script src="_content/Luthetus.Common/luthetusCommon.js"></script>
```

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

- In `MainLayout.razor` render the `<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer />` Blazor component

> *NOTE:* The placement of the `<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer/>` Blazor component should be wrapped in an encompassing div. This allows one to cascade css. A later tutorial is intended to show this, as to keep the installation tutorial more to the point.

```html
@inherits LayoutComponentBase

<PageTitle>Luthetus.Common.Installation.ServerSide</PageTitle>

<Luthetus.Common.RazorLib.Installations.Displays.LuthetusCommonInitializer/>

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