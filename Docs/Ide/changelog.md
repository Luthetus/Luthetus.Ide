# Change Log

All notable changes to the "Luthetus.Ide" repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [?.?.?] - ????-??-??
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Update /Docs/Ide/changelog.md
</details>

---

## [0.8.5.0] - 2024-06-19
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Added Lexer support for C# char type.
	- Added Lexer support for C# escape characters (both string and char types).
	- Breadth first C# parser (used to be depth first).
	  This lets the parser see same scope definitions that occur
	  below in the text, relative to where its being
	  referenced from.
	- Re-written undo/redo. In short, only changes are tracked
	  now. Whereas previously a copy of the entire file was being stored.
	- Various text editor optimizations.
	  One of which is not to re-calculate the virtualization
	  result if the user's action did not cause an edit.
	  In otherwords, any movement keys for the cursor
	  won't re-calculate the virtualization result.
	- Enqueueing a background task is a "synchronous" method, (it used to be async).
</details>

---

## [0.8.4.0] - 2024-05-12
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Optimization: scrolling can be done to a C# model with IEditContext. This is a massive optimization,
	  instead of every change to the text editor scrollbar invoking JavaScript interop to set
	  the scrollbar, the IEditContext will gather many events that ask to modify the scrollbar,
	  and batch them into a single JavaScript interop call to set the scrollbar.
	- Change: Re-write ThrottleAsync.cs
	- Change: A lot of synchronous UI code was re-written to be async. The goal is that all UI code should be async from top to bottom (generally speaking).
	- Fix: MostCharactersOnASingleLineTuple
	- Fix: .NET Solution text editor syntax highlighting (right click the .NET Solution in the treeview and pick the menu option to open it in the text editor)
</details>

---

## [0.8.3.0] - 2024-05-05
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Fix Linux solution explorer paths. (issue was relating to '\' vs '/' directory separator).
	- Fix Linux copy and paste.
	- Fix get local storage.
	- Fix unit test output. (still isn't perfect)
	- Fix insertion of text when text-editor-cursor has a selection
	- Fix text editor mouse wheel scrolling
	- Fix text editor bring cursor into view.
	- Batch terminal output. (this is a massive performance improvement,
	  instead of writing line by line, if many lines of output need to be written,
	  they all are written in one operation.)
	- Parse the output of the terminal on a 'by-command-basis'.
	  One can run 'dotnet run' with a .net cli output parser,
	  then run 'git status' with a git cli output parser, as an example.
	- Git integration progress. For example, a button that runs "git status",
	  parses the terminal output, and creates a tree view of all the changed files.
	  The git integration is still in progress though. (more usage of the '.git'
	  folder instead of just parsing the terminal so much is likely a path to take).
</details>

---

## [0.8.2.0] - 2024-04-27
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Integrated Terminal.
	- Moving github publish action to this repo itself (old website is located in the repo: Luthetus/Luthetus.Website).
</details>

---

## [0.8.1.0] - 2024-03-17
<details>
  <summary>Click to show changes</summary>

	### FIX
	- TextEditorDynamicViewModelAdapter.cs: Drag a text editor tab to dock it on a panel, or out into a dialog, and vice versa.
	- IDynamicViewModel.cs: Interface to implement drag, drop, or dock for a UI element.
</details>

---

## [0.8.0.0] - 2024-03-11
<details>
  <summary>Click to show changes</summary>

	### FIX
	- TextEditorModelModifier.Partition.cs: Text Editor content is partitioned in 4096 character chunks.
	- DirtyResourceUriInteractiveIconDisplay.razor.cs: Text Editor models which are unsaved now appear in bottom right.
	- PolymorphicTabDisplay.razor.cs (2024-03-15: renamed to TabDisplay.razor.cs and moved) PolymorphicUi has been started.
	  One can see it in action by dragging a text editor tab off of the editor, and then letting go. This will turn that tab into a dialog.
	- ThrottleController.cs has been started. All UI events are passed through the same throttle. Furthermore, each UI event
	  can specify its own throttle delay, while maintaining the execution by order in which the UI events occurred.
	- ThrottleEventOnKeyDown.cs Text Editor event batching. By way of the previously mentioned ThrottleController,
	  consecutive UI events which are batchable with one another. Will merge into a batch event. For example, if 20 onkeydown
	  events are waiting in the throttle controller, then 1 onkeydownBatch event will occur which contains all 20 onkeydown events.
	- SolutionExplorerContextMenu.razor.cs: Solution Explorer multi-select context menu options. 
	  If one selects many files/directories, then the 'delete' context menu option will be available. This option will
	  delete all the selected files/directories.
	- WASM Performance issues: with this update it seems I introduced a UI thread blockage. Holding down any key while typing
	  in the text editor will freeze the UI as a result (more or less) until one lets go. I wanted to fix this issue before updating
	  the website, but there are so many vital changes in this update, that I want to make them known.
	- Native IDE Performance issues: I find any file with more than 10,000 characters in it is a bit laggy. I'm still able to myself, use
	  the IDE to develop the IDE, but large files aren't too great of an experience at the moment. I wrote partitioning for text editor
	  content in chunks of 4,096 characters. But, still there are many more optimizations I have planned for large files optimization.
</details>

---

## [0.7.7.0] - 2024-02-23
<details>
  <summary>Click to show changes</summary>

	### FIX
	- The diagnostics should render properly now. Prior to this, if one opened "Program.cs",
	  then opened any other file, the diagnostics from "Program.cs" may have rendered in the other file
	  that was opened after the fact.
</details>

---

## [0.7.6.0] - 2024-02-22
<details>
  <summary>Click to show changes</summary>

	### FIX
	- { Ctrl + f } (find within texteditor) will scroll the found results
	  into view as one iterates over the search results.
	- Add 'Tools' dropdown to header.
	- 'BackgroundTasks' dialog shows all the background tasks that the
	  IDE is performing, as they happen. For example, open the dialog,
	  move it so you can see the text editor, then click and drag your cursor
	  within the text editor. Each on mouse move event fires a 'BackgroundTask'
	  named "te_HandleContentOnMouseMove"
	- Progress with debugger integration 'IntegratedTerminalDisplay.razor.cs'.
	  This component came about because, after attaching to a process ID with
	  <a href="https://github.com/Samsung/netcoredbg" target="_blank">github: Samsung/netcoredbg</a>
	  I wanted to use the CLI to give commands, like a command to put a breakpoint
	  for example. But when running netcoredbg, the program immediately
	  would return, as opposed to reading user input.
	  I wrote a simple C program that prompts the user for their name.
	  When the C program then tries to read standard input, they await a
	  SemaphoreSlim, and an input HTML element gets rendered.
	  Once the input HTML element receives an 'Enter' keystroke,
	  the standard input is set as the HTML element value,
	  and the SemaphoreSlim is released by the UI thread. Then
	  the CLI program reads standard input.
</details>

---

## [0.7.5.0] - 2024-02-04
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Fix Fluxor related deadlocks in Photino hosted app version of the IDE.
	  I made a PR to the Fluxor repo: <a href="https://github.com/mrpmorris/Fluxor/pull/469" target="_blank">https://github.com/mrpmorris/Fluxor/pull/469</a>
	  This seems to only happen in the Photino hosted app.
	  I hope to hear back from the creator of Fluxor, I might just be
	  doing something silly on my end to even encounter this.
</details>

---

## [0.7.4.0] - 2024-02-03
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Fix the IDE freezing "randomly":
	  I wrote the <a href="https://github.com/Luthetus/Luthetus.Ide/blob/main/Luthetus.Common/Source/Lib/Luthetus.Common.RazorLib/Reactives/Models/Throttle.cs" target="_blank">Throttle.cs</a> initially as async.
	  But if the source that wants to be throttled awaits the throttle timer,
	  its just sort of nonsense, right?
	  I believe this was locking.
	  Now its a fire and forget, so the invoker doesn't have to await the
	  throttle.
</details>

---

## [0.7.3.0] - 2024-02-02
<details>
  <summary>Click to show changes</summary>

	### FIX
	- A preview text editor when using 'Code Search' ({ Ctrl + , } written out: "Control + Comma").
	- The most recently interacted with Dialog will render above any other. (this refers to two dialogs overlapping).
	- A newly opened dialog will be set as focused (this is for keyboard usage ease of use).
	- Open a find overlay within a texteditor: { Ctrl + f }
</details>

---

## [0.7.2.0] - 2024-01-31
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Add usings when autocompleting a type
	- GUI editor for C# Compiler Service. (follows the text editor cursor)
	- A file cannot be deleted unless it is first given deletion rights
	- 'Ctrl + ,' progress (only searches on files are being done.
	  Clicking a resulting file will open that file in the text editor)
	- 'Ctrl + Shift + F' progress (the search query, find all button,
      and file system path to seach in are the only UI elements
      in effect currently). A list gets made at the bottom of the dialog
      wth the results, and clicking a result will open that file in the text editor.
</details>

---

## [0.7.1.0] - 2024-01-26
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Import repositories from GitHub
</details>

---

## [0.7.0.0] - 2024-01-23
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Added all storage types. (struct, class, interface, enum, record)
	- Added Primary constructors (for records only at the moment)
	-  Object initialization (new Type { Property = Value, })
	- Improved 'var' contextual keyword. (detecting it as a keyword or an identifier)
	- Generic Type syntax highlighting. (List&lt;int&gt; myVariable;)
	- The following syntaxes are now expressions: constructor invocation, variable references, function invocation.
	- If parser throws an exception, still draw the valid Lexer syntax highlighting. (lexer syntax highlighting does the keywords, and more syntactic related things)
	- Added access modifiers to type definition. (public, "protected internal", protected, internal, "private protected", private).
	  Usage of access modifiers is not yet implemented, only the parsing of them.
	- Added the "partial" modifier to type definition. 
	  Usage of the modifier is not yet implemented, only the parsing of it.
</details>

---

## [0.6.0.0] - 2024-01-19
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Fixed 'Go To Definition' (F12 when in text editor)
	- Fixed 'Go To Matching Character' ("Ctrl + ]" when in text editor. Holding shift will select the text.)
	- Improved the C# Compiler Service, namespaces were changed the most.
</details>

---

## [0.5.0.0] - 2024-01-14
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Tab characters are rendered properly when scrolling horizontally in the text editor.
	- Fix the majority of bugs that came about from the re-write of the Text Editor for immutability.
	- Progress on tree view for multi-selecting nodes
</details>

---

## [0.4.0.0] - 2024-01-02
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Unit Test Explorer
	- Re-write text editor for immutability
</details>

---

## [0.2.0.0] - 2023-10-21
<details>
  <summary>Click to show changes</summary>

	### FIX
	- C# Autocompletion via Binder
	- C# Parsing improvements
</details>

---

## [0.1.1.0] - 2023-10-04
<details>
  <summary>Click to show changes</summary>

	### FIX
	- Keymaps to navigate user's focus with the keyboard to PanelTabs.
	- (Example:) "Ctrl + Alt + S" => Set focus to solution explorer. Also, if the solution explorer isn't the active tab, make it the active tab.
	- The active contexts panel tab (default position is in bottom panel) will
  	show the keymap available given the user's focus. Use the "Inspect Element" like
	  functionality to lock the Context so you can see the keymap without losing it once
	  you click on the active contexts panel tab to view the keymap.
</details>
