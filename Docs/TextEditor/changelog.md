# Change Log

All notable changes to the `Luthetus.TextEditor` repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.6.0] - 2024-09-19
<details>
  <summary>Click to show changes</summary>

	### Fixed
    - Fix: text erroneously written out when holding non-shift modifier(s).
      	(i.e.: { 'Ctrl' + 'c' } to copy writing out the letter 'c' to the text editor)
    - Fix: maximum scrollWidth and scrollHeight becoming smaller (and how this impacts scrollLeft and scrollTop)
    - Fix: negative scrollLeft and negative scrollTop
</details>

## [2.5.0] - 2024-09-05
<details>
  <summary>Click to show changes</summary>

	### Fixed
    - Fix: deletion of text that spans multiple partitions.
</details>

## [2.4.0] - 2024-09-02
<details>
  <summary>Click to show changes</summary>

	### Fixed
    - Fix: deletion of lines will now scroll by the amount of lines deleted.
           Previously, this was breaking the virtualization result until one triggered a re-calculation.
    - Fix: Keybinds first try to match on a JavaScript 'event.key' so to speak. Then, as a fallback
           they will now try to match on 'event.code' so to speak.
           Previously, on Ubuntu, if one remapped the CapsLock key to Escape, it would not work
           in the IDE at various places. This has been fixed.
    - Text editor events now use structs to transmit event data. This is expected to be a large optimization,
          as it tends that high turnover 'class' objects bring performance issues due to the garbage collection overhead.
    - Text editor's OnKeyDownLateBatching event uses a fixed size array for batching events, rather than what previously
          was a List<T>. This is expected to be a large optimization, as it tends that high turnover 'class' objects bring
          performance issues due to the garbage collection overhead. As well, it tends to be the case that no more than
          3 or 4 keyboard events ever get batched together. So the fixed size array is '8' keyboard events can be made into
          a single batch.
</details>

## [2.3.0] - 2024-08-27
<details>
  <summary>Click to show changes</summary>

	### Fixed
    - Fix: line endings breaking due to a Post to the ITextEditorService which makes an edit,
      but then throws an exception within the same Post.
    - Fix: Gutter width changes causing the text editor measurements to be incorrect.
    - Use more recent dropdown code for text editor autocomplete and context menu.
	  The newer dropdown code moves itself so it stays on screen (if it initially rendered offscreen).
    - Fix: return focus to text editor after picking a menu option in autocomplete or context menu.
    - Start code snippet logic.
</details>

## [2.2.0] - 2024-08-20
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
