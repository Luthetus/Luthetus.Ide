# Change Log

All notable changes to the `Luthetus.TextEditor` repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.2.0] - ????-??-??
<details>
  <summary>Click to show changes</summary>

	### Fixed
	- Home keybind understands indentation
	- Fix cursor blinking
    - Fix change keymap without having to reload
    - Track additionally, the 'Key' of a keyboard event args (previously only was tracking the 'Code')
    - Change ITextEditorWork implementations and ResourceUri to structs
    - Fix various 'Vim' keybind bugs. It isn't fully functional yet.
</details>

## [2.1.0] - 2024-08-15
<details>
  <summary>Click to show changes</summary>

	### Fixed
	- Fix text editor context menu crashing when closing
    - Fix out of sync syntax highlighting.
</details>

## [2.0.0] - 2024-08-14
<details>
  <summary>Click to show changes</summary>

	### Fixed
	- Fix cursor "randomly" losing focus
    - Re-write virtualization in C# (it was previously done with JavaScript)
    - Change RichCharacter.cs to a struct (it was previously a class).
    - Change ITextEditorModel.RichCharacterList to an array (it was previously an ImmutableList).
    - Fix typing at start of file (position index 0) a non letter or digit.
	- Text Editor uses 60% less memory after various struct/array optimizations.
    - Text Editor "feels" an order of magnitude faster after various Blazor optimizations and
      struct/array optimizations (which reduce the garbage collection overhead thus improving performance greatly).
	
	### Bugs
	- If one opens the text editor's context menu with the dedicated 'ContextMenu' button on the keyboard,
	  or the accessability keybind { Shift + F10 }. To then hit the 'Escape' key or the 'ArrowLeft'
	  key to close the context menu, is causing an unhandled exception.
	- The syntax highlighting is out of sync. (its seems to be "lagging" behind by 1 event) 
</details>

## [1.3.0] - 2023-08-20
<details>
  <summary>Click to show changes</summary>

	### Fixed
	- Most documentation issues I found are fixed.
	- All NuGet Packages mentioned are at version 1.3.0 as well to avoid confusion.
	- .NET 6 Blazor-WASM worked after following documentation start to finish.
	- .NET 6 Blazor-ServerSide after following documentation start to finish.
	
	### Bugs
	- .NET 6 is hard-coded as the target framework. Therefore, .NET 7 apps are not working. I need to remedy this in later versions.
</details>
