# Luthetus.Ide (Not yet released)
- A free and open source IDE being written with .NET: C#, Blazor UI, and a Photino host.

- Runs on Linux, Windows, and Mac

![Example GIF](./Images/Rewrite/introductoryGifLuthetusIde.gif)

## Demo:
https://luthetus.github.io/Luthetus.Website/

## Features

### Solution Explorer:
![solutionExplorerGif](./Images/Gifs/solutionExplorerGif.gif)
- Tracks the file system default namespace.
- Interpolate file system namespace when creating a templated C# file.
- Create codebehinds for Blazor components with the 'partial', 'ComponentBase', and 'using' automatically.
- Display unique icons for unique folders. Example: "wwwroot" is a globe, and "Properties" is a settings gear.
- Context Menu contains: Copy, Cut, Rename, Paste, Delete, "New Empty File", "New Templated File", and "New Directory".
- For .NET Solutions the Context Menu also gets: "Add New C# Project", and "Add Existing C# Project"
- For C# Projects the Context Menu also gets: "Add Project Reference", "Set As Startup Project", and "Remove (no files are deleted)"
- Nesting of 'codebehind' like files. So nesting "MyComponent.razor.cs" behind "MyComponent.razor".

### Input File Dialog:
![solutionExplorer](./Images/solutionExplorer.gif)
- Traverse the file system in order to select a file. This file must satisfy a predicate which is defined by the requester. So I can request for a C# Project and the user sees an input file dialog which asks the user to pick a C# Project.

### NuGet Package Manager
![solutionExplorer](./Images/solutionExplorer.gif)
- Query "azuresearch-usnc.nuget.org/"
- Pick the Project one wishes to add the NuGet Package reference to.
- Option for "Include Prerelease"
- List out the results of the HTTP request.
- Allow for picking of the version number foreach of the results individually.
- Button to add the NuGet Package at the selected version to the chosen Project.
- The gif for this will be the gif at the start of this file where I showcase its usage as to decrease how large this README is.

### Themes
![textEditor](./Images/themes.gif)
- Visual Studio Dark Theme Clone
- Visual Studio Light Theme Clone

## Why Am I Making this IDE?
- https://github.com/Luthetus/Luthetus.About

## References I find useful:
  - Scintilla Documentation: [https://www.scintilla.org/ScintillaDoc.html](https://www.scintilla.org/ScintillaDoc.html)

## Cloning and locally running the repo
Luthetus.Ide has references to other projects of mine.

I wanted to avoid git-submodules, as I wonder if they would make things more complicated or not. I want anyone to be able to clone and run this repository, without needing knowledge of submodules.

Therefore, to run this repository one must clone this repo itself, and a few of my other repositories of which Luthetus.Ide references.

Preferably the main branch would use the NuGet Packages of the projects which are referenced. I hope to make this change in the future.

Clone the following repos into the same folder:
- Luthetus.Common
  - [Source code](https://github.com/Luthetus/Luthetus.Common)
  - git https url: https://github.com/Luthetus/Luthetus.Common.git
- Luthetus.TextEditor
  - [Source code](https://github.com/Luthetus/Luthetus.TextEditor)
  - git https url: https://github.com/Luthetus/Luthetus.TextEditor.git
- Luthetus.CompilerServices
  - [Source code](https://github.com/Luthetus/Luthetus.CompilerServices)
  - git https url: https://github.com/Luthetus/Luthetus.CompilerServices.git
- Luthetus.Ide
  - [Source code](https://github.com/Luthetus/Luthetus.Ide)
  - git https url: https://github.com/Luthetus/Luthetus.Ide.git

Possible errors with this 'scuffed' approach I have at the moment are:
- Do to me not using submodules, the parent repository (Luthetus.Ide) does not track the commit which was being used by a child-repository (ex: Luthetus.TextEditor). I intend to keep the 'main' branches in sync, but I want to acknowledge possible errors. If there are others who wish to contribute to the code I'll make submodules a number one priority. As of right now, I'm the only developer on this so the unorganized nature of my approach is working, and effective for rapid changes. As the project grows, things would then need to be better organized.
- The C# projects use file paths to resolve project references. The error here would be that the file paths do not match. Open the C# project in a text editor, and fix the file paths.
- The .NET Solutions use file paths to include C# projects in a solution. The error here would be that the file paths do not match. Open the .NET Solution in a text editor, and fix the file paths.

# My Youtube Videos
You may be interested in visiting my [youtube channel](https://www.youtube.com/channel/UCzhWhqYVP40as1MFUesQM9w). I make videos about this repository there.