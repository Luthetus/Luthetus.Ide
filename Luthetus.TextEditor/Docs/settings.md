# Blazor.Text.Editor (Some parts of this are outdated. I think this markdown file was made with v5.0.0 I will update this markdown file to v7.0.0 when I find time)

## Settings

### Goal

- Render the optional static dialog containing all the settings available for change.
- Render the resizable and moveable version of the settings dialog.
- Render each setting individually where desired.
- Inform about optional automatic local storage integration for settings.

### Steps
- I will assume you have seen the previous tutorials during this one.

- In Index.razor put an HTML button element with your styling of choice. I will put mine above where the `TextEditorDisplay` was put in previous tutorials. See the following code snippet.

```html
<button class="btn btn-primary">
    
</button>

<TextEditorDisplay TextEditorKey="IndexTextEditorKey"/>
```

- Give the button text of `"Text Editor Settings"`. As well an @onclick which invokes the unimplemented method `OpenTextEditorSettingsDialog`. See the following code snippet.

```html
<button class="btn btn-primary"
        @onclick="OpenTextEditorSettingsDialog">
    Text Editor Settings
</button>

<TextEditorDisplay TextEditorKey="IndexTextEditorKey"/>
```

```csharp
@code {
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
    private static readonly TextEditorKey IndexTextEditorKey = 
        TextEditorKey.NewTextEditorKey();

    protected override void OnInitialized()
    {
        TextEditorService.RegisterCSharpTextEditor(
            IndexTextEditorKey,
            string.Empty);
        
        base.OnInitialized();
    }

    private void OpenTextEditorSettingsDialog()
    {
        throw new NotImplementedException();
    }
}
```

- Implement the method `OpenTextEditorSettingsDialog` to contain `TextEditorService.ShowSettingsDialog();`. See the following code snippet

```csharp
private void OpenTextEditorSettingsDialog()
{
    TextEditorService.ShowSettingsDialog();
}
```

- Now run the program and click the button. You will see the static settings dialog as shown in this gif.

![Static settings dialog](/Images/Gifs/20_static-dialog.gif)

- Modify the line `TextEditorService.ShowSettingsDialog();`. Pass in as an argument `true` for the optional parameter named `isResizable`.

- Now run the program and click the button. You will see the resizable and moveable settings dialog as shown in this gif.

![Dynamic settings dialog](/Images/Gifs/20_dynamic-dialog.gif)

- Add the following using statement to Index.razor if it is not already there.

```html
@using BlazorTextEditor.RazorLib.HelperComponents
```

- Above where the button is to open the settings dialog. Render the razor components shown in the following markup.

```html
<TextEditorInputFontSize/>
<TextEditorInputShowNewLines/>
<TextEditorInputShowWhitespace/>
<TextEditorInputTheme/>
```

- Now run the program and you will find every settings input from the dialog will be on the page itself. `You can place these inputs wherever` you'd like as `everything is Dependency Injected`.

![Inline settings dialog](/Images/Gifs/20_inline-settings.gif)

- The nuget package by default will integrate with JavaScript local storage. One can turn this off inorder to reference the NuGet Package from a C# Project which cannot dependency inject the IJSRuntime.

- Add an HTML button element, with styling of your choice, on the Index.razor page. Give the button text of `"Read Local Storage"` and an @onclick that invoke the unimplemented method: `async Task ReadLocalStorage()` see the following code snippets.

```html
<button class="btn btn-primary"
        @onclick="ReadLocalStorage">
    Read Local Storage
</button>
```

```csharp
private async Task ReadLocalStorage()
{
    
}
```

- Implement `ReadLocalStorage()` to `await TextEditorService.SetTextEditorOptionsFromLocalStorageAsync();`. See the following code snippet.

```csharp
private async Task ReadLocalStorage()
{
    await TextEditorService.SetTextEditorOptionsFromLocalStorageAsync();
}
```

- Run the application and then modify your settings. I will set my theme to the light theme. 

- Afterwards reload the webpage, but do not clear your cache. Once the webpage loads proceed to click the `Read Local Storage` button. Your theme should change to the light theme as the default was dark theme and your local storage had light theme stored.

- As of v5.0.0 of this NugetPackage the following are stored in local storage @onchange

- local storage: int? FontSizeInPixels
- local storage: Theme? Theme
- local storage: bool? ShowWhitespace
- local storage: bool? ShowNewlines

- As of writing this tutorial however, I am suddenly unable to get my theme from local storage. It just keeps being unrecognized and using the Unset theme.

- A good way to go about making use of the local storage local. Is to OnAfterRenderAsync of a 'top level component'. In the if(firstRender) { await readLocalStorage(); }