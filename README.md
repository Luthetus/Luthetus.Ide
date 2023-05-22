# Blazor Studio (Not yet released)
- A free and open source IDE written using .NET - a Photino host, Blazor UI, and C#

- Runs on Linux, Windows, and Mac

## This gif serves to be a quick showcase of what some of the IDE is so people can gauge their interest in the Repository. The gif is 1 minute and 30 seconds long.

![startupProject](/Images/startupProject.gif)

## Features

I will go through the various features by text description and then an accompanying gif. I'll zoom in a bit more for these gifs and keep them short.

Window component that mimics those found on an operating system.
- Resizable at all 'cardinal' angles (N-E-S-W)
- Resizable at all diagonal angles
- Moveable by dragging on the window's titlebar.
- Maximizable by clicking the maximize icon in the top right. This then changes the icon to a restore icon which will restore the exact position it was at prior to the maximize.
- Close button to close the window.
![windowComponent](/Images/windowComponent.gif)

Generic ContextMenu Blazor Component
![windowComponent](/Images/contextMenuComponent.gif)

Generic TreeView Blazor Component
- Keyboard support for movement: { ArrowLeft, ArrowDown, ArrowUp, ArrowRight, Home, End }
- Maintains the expanded state of child nodes when collapsing their parent and then expanding it again.
- Maintains the state of the TreeView decoupled from the lifecycle of the Blazor Component itself.
- To minimize size of this README, the gif of the "Solution Explorer" serves to be the gif of the "Generic TreeView" as the solution explorer is a usage of it.

Solution Explorer
- Tracks the file system default namespace.
- Interpolate file system namespace when creating a templated C# file.
- Create codebehinds for Blazor components with the 'partial', 'ComponentBase', and 'using' automatically.
- Display unique icons for unique folders. Example: "wwwroot" is a globe, and "Properties" is a settings gear.
- Context Menu contains: Copy, Cut, Rename, Paste, Delete, "New Empty File", "New Templated File", and "New Directory".
- For .NET Solutions the Context Menu also gets: "Add New C# Project", and "Add Existing C# Project"
- For C# Projects the Context Menu also gets: "Add Project Reference", "Set As Startup Project", and "Remove (no files are deleted)"
- Nesting of 'codebehind' like files. So nesting "MyComponent.razor.cs" behind "MyComponent.razor".
![solutionExplorer](/Images/solutionExplorer.gif)

Input File Dialog
- Traverse the file system in order to select a file. This file must satisfy a predicate which is defined by the requester. So I can request for a C# Project and the user sees an input file dialog which asks the user to pick a C# Project.

Resizable Grid
- Resizable Row
- Resizable Column

NuGet Package Manager
- Query "azuresearch-usnc.nuget.org/"
- Pick the Project one wishes to add the NuGet Package reference to.
- Option for "Include Prerelease"
- List out the results of the HTTP request.
- Allow for picking of the version number foreach of the results individually.
- Button to add the NuGet Package at the selected version to the chosen Project.
- The gif for this will be the gif at the start of this file where I showcase its usage as to decrease how large this README is.

Text Editor
- Has its own public repository and is a NuGet Package itself. Find source code at: https://github.com/huntercfreeman/Blazor.Text.Editor
- Syntax Highlighting
- Text Selection
- Keyboard movement: { ArrowLeft, ArrowDown, ArrowUp, ArrowRight, Home, End }
- Ctrl key modified Keyboard movement
- Shift key modified Keyboard movement
- Autocomplete
- Undo and Redo logic
- Commands: { "Copy Selection", "Cut Selection", "Paste Clipboard", "Save Changes", "Select All", "Undo", "Redo", "ScrollLineDown", "ScrollLineUp", "ScrollPageDown", "ScrollPageUp", "CursorMovePageBottom", "CursorMovePageTop" }
- Custom made scrollbar components.
- Horizontal and Vertical virtualization
- Helper components out of the box which allow the user to modify their settings. These helper components will dispatch an event and therefore you can watch the text editor change as you change settings.
- Tabs come out of the box with the TextEditorGroupDisplay.razor
- Automatic local storage integration to save the user's settings.
- Tracks what line endings are being used in a file and displays it to the user: { '\r', '\n', "\r\n" }
- Allows user to pick what line ending they wish to type when hitting the "Enter" key
- Footer displays file extension, file length, line count, current line, current column, line ending in file, line ending which will be typed.
![textEditor](/Images/textEditor.gif)

Dropdown Component
- Vertical Dropdown
- Horizontal Dropdown
- Submenus

Themes
- Visual Studio Dark Theme Clone
- Visual Studio Light Theme Clone
![textEditor](/Images/themes.gif)

Why Am I Making this IDE?
- I'm making this IDE because I enjoy doing so. I imagine Sisyphus happy.
![textEditor](/Images/clipart3420085.png)

## Resources I found helpful

#### youtube.com resources:
  - [Josh Varty - Learn Roslyn Now ( playlist )](https://youtube.com/playlist?list=PLxk7xaZWBdUT23QfaQTCJDG6Q1xx6uHdG)
  - [Mark Rendle - Automate yourself out of a job with Roslyn ( video )](https://www.youtube.com/watch?v=V4zqk4-LL1M)
  - [Immo Landwerth - Building a Compiler ( playlist )](https://youtube.com/playlist?list=PLRAdsfhKI4OWNOSfS7EUu5GRAVmze1t2y)
  - [Adam Fowler - Create a Database from the ground up in C++ ( playlist )](https://youtube.com/playlist?list=PLWoOSZbmib_cr7zRfAkPkoa9m2uYsYDug)
  - [Brian Beckman - Don't fear the Monad ( video )](https://www.youtube.com/watch?v=ZhuHCtR3xq8)

#### website resources:
  - Unicode Technical Site: [https://unicode.org/main.html](https://unicode.org/main.html)
  - Scintilla Documentation: [https://www.scintilla.org/ScintillaDoc.html](https://www.scintilla.org/ScintillaDoc.html)

# My Youtube Videos
You may be interested in visiting my [youtube channel](https://www.youtube.com/channel/UCzhWhqYVP40as1MFUesQM9w). I make videos about this repository there.

# Cloning and locally running the repo

BlazorStudio is "plug and play" or "clone and run" that is to say. So, just 
- Clone the source code 
- Run the C# Project named `BlazorStudio.Photino`

as shown in the following gif.
![cloneBlazorStudio.gif](/Images/RootREADME/cloneGifIntro.gif)
