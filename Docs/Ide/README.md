# Luthetus.Ide (Not yet released)
- A free and open source IDE being written with .NET: C#, Blazor UI, and a Photino host.

- Runs on Linux, Windows, and Mac

![Example GIF](../../Images/Ide/Gifs/ide0.3.0.gif)

## Demo:
https://luthetus.github.io/Luthetus.Website/

## Features

### Solution Explorer:
![solutionExplorerGif](../../Images/Ide/Gifs/solutionExplorerGif.gif)
- Tracks the file system default namespace.
- Interpolate file system namespace when creating a templated C# file.
- Create codebehinds for Blazor components with the 'partial', 'ComponentBase', and 'using' automatically.
- Display unique icons for unique folders. Example: "wwwroot" is a globe, and "Properties" is a settings gear.
- Context Menu contains: Copy, Cut, Rename, Paste, Delete, "New Empty File", "New Templated File", and "New Directory".
- For .NET Solutions the Context Menu also gets: "Add New C# Project", and "Add Existing C# Project"
- For C# Projects the Context Menu also gets: "Add Project Reference", "Set As Startup Project", and "Remove (no files are deleted)"
- Nesting of 'codebehind' like files. So nesting "MyComponent.razor.cs" behind "MyComponent.razor".

### Input File Dialog:
![inputFileDialogGif](../../Images/Ide/Gifs/inputFileDialogGif.gif)
- Traverse the file system in order to select a file. This file must satisfy a predicate which is defined by the requester. So I can request for a C# Project and the user sees an input file dialog which asks the user to pick a C# Project.

### NuGet Package Manager
![nugetPackageManagerGif](../../Images/Ide/Gifs/nugetPackageManagerGif.gif)
- Query "azuresearch-usnc.nuget.org/"
- Pick the Project one wishes to add the NuGet Package reference to.
- Option for "Include Prerelease"
- List out the results of the HTTP request.
- Allow for picking of the version number foreach of the results individually.
- Button to add the NuGet Package at the selected version to the chosen Project.
- The gif for this will be the gif at the start of this file where I showcase its usage as to decrease how large this README is.

### Settings
![settingsGif](../../Images/Ide/Gifs/settingsGif.gif)
- Application Settings
  - Font-Size
  - Font-Family
  - Icon-Size
  - Theme
- Text Editor Settings
  - Font-Size
  - Font-Family
  - Cursor-Width
  - Show Newlines
  - Use Monospace Optimizations (you should not turn off monospace optimizations because the non-monospace logic is currently very unoptimized as of 2023-08-16)
  - Show Whitespace
  - Theme
  - Keymap (Default, or Vim)
