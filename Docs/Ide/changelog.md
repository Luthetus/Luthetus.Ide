# Change Log

All notable changes to the "Luthetus.Ide" repository will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.9.6.9] - 2024-10-17
<details>
  <summary>Click to show changes</summary>

	- Text Editor: fix presentation layer background highlighting.
			You can now provide a singular text span that "spans" many lines,
			and the text editor will highlight that area.
			Previously, you had to break the singular text span yourself
			into individual text spans per line.
	- When viewing a C# file, one can have the nearest scope highlighted
			at its start and end points by adding the following
			(NOTE: the luthetusTextEditor.css was modified to now include .luth_te_brace_matching {...}
					so you might have to clear your cache to get the latest CSS to load.):
			
			<pre>
	private ViewModelDisplayOptions _viewModelDisplayOptions = new()
	{
	HeaderComponentType = typeof(TextEditorDevToolsAnchorDisplay),
	};

	&lt;TextEditorGroupDisplay TextEditorGroupKey="EditorIdeApi.EditorTextEditorGroupKey"
	ViewModelDisplayOptions="_viewModelDisplayOptions"/&gt;</pre>
		
		<li>
			Add property: LuthetusTextEditorConfig.AbsolutePathStandardizeFunc
			<pre>
	/// &lt;summary&gt;
	/// Func is given as an argument a string and IServiceProvider, the string will be
	/// made into a &lt;see cref="Luthetus.TextEditor.RazorLib.Lexers.Models.ResourceUri"/&gt;.
	///
	/// Returns the standardized format for the absolute path.
	///
	/// Example: "C:\a.txt" and "\a.txt" are two distinct resource uri's.
	///          With this Func, if one desires, they can alter
	///          "C:\a.txt" to remove the 'C:' from its string,
	///          or add 'C:' to the "\a.txt" or etc...
	///          to make these resource uris match one another.
	/// &lt;/summary&gt;
	public Func&lt;string, IServiceProvider, Task&lt;string&gt;&gt;? AbsolutePathStandardizeFunc { get; set; }
			</pre>
		- C# Parser:
			- Function Definition and Constructor Definition and Primary Constructor arguments
					are now added to the function body's scope.
					(previously they were being added to the scope that contained
						the function definition.).
			- Parse scope of keywords that create scope blocks.
					<pre>
	foreach (var someVariable in list)
	{
	/*
	scope is parsed here now, and the variable 'someVariable'
	is only defined within this code block.
	*/
	}</pre>
				- Progress was made on "single statement body code blocks"
					i.e.:
					<pre>
	foreach (var someVariable in list)
	Console.WriteLine(someVariable);</pre>
					
					A scope is made on the semicolon for now (more changes to this will be made in the future).
				- Drastic reduction to object allocations when parsing. Previously every
					scope held 3 or so dictionaries:
					<pre>
	VariableDeclarations&lt;string identifier, variableDeclarationNode&gt;
	FunctionDefinitions&lt;string identifier, functionDefinitionNode&gt;
	TypeDefinitions&lt;string identifier, typeDefinitionNode&gt;.</pre>
					
					Now, the 3 dictionaries are allocated per file, rather than
					per scope within a file.
					
					Depending on how many code blocks were in a file, this could
					end up being a substantial reduction to the amount of
					objects that are allocated.
					
					Also, the Scope.cs itself was made into a struct, with minimal
					information attached to it.
				- Tokens are now structs (previously they were sealed records)
				- Nodes now maintain their ChildList as an ISyntax[]
					(previously they were ImmutableArray&lt;ISyntax&gt;)
				- Reason for all the struct changes:
					the parser logic gets ran frequently,
					and is constantly constructing new syntax trees.
					So the faster we can delete the previous syntax tree
					the better (i.e.: not delete it via garbage collection
					which is hard on the cpu with how much turnover on the constructed objects there are
					in the parser code).
				
			</ul>
		
	</ul>
</details>

---

## [0.9.6.8] - 2024-10-08
<details>
  <summary>Click to show changes</summary>

	<ul>
		- Move cursor to text span, scroll cursor into view:
			'Output Panel', 'Code Search (Tool)', and 'Find All (Tool)'
		- Test Explorer "Send to Output panel" context menu option will open the Output panel,
			and will appear for individual tests, not just project test discovery.
		- Set execution terminal active from StartupControlDisplay.razor.
		- DotNetRunParseResult.cs add string message.
		- TestExplorerDisplay.razor maintain ElementDimensions.
		- Terminal panel Sync the spinner UI
		- Fix: New C# Project should not share the same terminal command key for the 'new project' command and the 'add to solution' command.
		- Fix: { 'Ctrl' + 'Space' } keybind to open autocomplete menu.
	</ul>
</details>

---

## [0.9.6.7] - 2024-10-07
<details>
  <summary>Click to show changes</summary>

	<ul>
		- Text Editor NuGet Package v3.2.0
			<a target="_blank" href="https://www.nuget.org/packages/Luthetus.TextEditor/">(nuget.org)</a>
		- Goto definition, respect the original cursor's syntax node kind.
		- C# Parser:
			- 'record struct' storage modifier kind
			- TypeClauseNode with '?'
			- Ambiguous property definition
			- Constructor invocation within an expression.
			- Record struct primary constructor
			- ExplicitCastNode.cs 'Ex: (MyClass)someObject'
			- Verbatim string progress
			- Constructor ': this(...) or base(...)'
			- 'params' keyword
			- Constructor where clause (progress)
			- For statement progress
			- Foreach statement progress
			- Lock statement progress
			- While statement progress
			- Do-while statement progress
			- Switch statement progress
			- Try-catch-finally statement progress

					Breadth first parsing progress
					(Parse all the definitions in a given scope prior to
					parsing an inner scope where appropriate i.e.: class definitions)

		- Never throw an exception in the parser, just show a diagnostic
		- Goto definition for 'SyntaxKind.ConstructorSymbol'
		- ContextMenu goto definition, scroll cursor into view
		- After context menu picked SetCursorShouldBlink(false)
		- Add: ThrottleOptimized&lt;TArgs&gt;
		- Drag optimizations (struct drag event actions)
	</ul>
</details>

---

## [0.9.6.6] - 2024-10-03
<details>
  <summary>Click to show changes</summary>

	- Text Editor NuGet Package v3.1.0
			<a target="_blank" href="https://www.nuget.org/packages/Luthetus.TextEditor/">(nuget.org)</a>
		- Fix: fatal exception from v0.9.6.5 update (when opening code search dialog).
</details>

---

## [0.9.6.5] - 2024-10-03
<details>
  <summary>Click to show changes</summary>

	- On startup, re-open the last open '.sln' file
		- Horizontally virtualize on a line by line basis, and only if the individual line is long.
		- Virtualization result is an array of lines (was a list)
		- Change: SetModelAndViewModelRangeAction to a struct
		- Change: TextEditorTextSpan to record-struct
		- TextEditorState.Main.cs uses private-mutable dictionaries to store
			text editor models/viewmodels.
			(They used to be ImmutableList(s))
		- Add: IAppDataService.cs
		- TextEditor: Add: ITextEditorDependentComponent.cs
				("low priority" rendering of accessory components to the text editor).
			Render header throttled at 1 second.
			Render footer throttled at 250 milliseconds.
		- Move git to its own project
		- Add: FooterJustifyEndComponent.cs
		- Fix: Git origin/branch only settable once
		- Add: Simple PythonCompilerService
</details>

---

## [0.9.6.4] - 2024-09-21
<details>
  <summary>Click to show changes</summary>

		- Text Editor NuGet Package v2.7.0
			<a target="_blank" href="https://www.nuget.org/packages/Luthetus.TextEditor/">(nuget.org)</a>
		
		- Common: BrowserResizeInterop
		
		- TextEditor: User Agent resize events trigger remeasure
		
		- TextEditor: Changing text editor's font-size triggers re-measure 
		
		- TextEditor: After re-remeasuring, reload the virtualization result.
		
		- TextEditor: InputTextEditorFontSize.razor changes
		
		- TextEditor: ScrollbarSection.razor vertical reset point while dragging
		
		- TextEditor: DISTANCE_TO_RESET_SCROLL_POSITION is 300px
</details>

---

## [0.9.6.3] - 2024-09-19
<details>
  <summary>Click to show changes</summary>

	<ul>
		- Text Editor NuGet Package v2.6.0
			<a target="_blank" href="https://www.nuget.org/packages/Luthetus.TextEditor/">(nuget.org)</a>
		
		- Fix: Text Editor, scrollWidth and scrollHeight becoming smaller (and how this impacts scrollLeft and scrollTop)
		
		- Fix: Text Editor, negative scrollLeft and negative scrollTop
		
	</ul>
</details>

---

## [0.9.6.2] - 2024-09-18
<details>
  <summary>Click to show changes</summary>

	<ul>
		- Add to CommonOptions.cs: ResizeHandleWidthInPixels, and ResizeHandleHeightInPixels.
		
		- Notifications render width and height based on character width, and character height units of measurement,
			rather than a fixed pixel value. This lets the notifications scale with font-size.
		
	</ul>
</details>

---

## [0.9.6.1] - 2024-09-17
<details>
  <summary>Click to show changes</summary>

	<ul>
		- Don't move scrollbar for terminal unless the cursor position changed
		
		- Implement the TerminalWebsite progress
		
		- Clear button no longer affects an active command
		
		- Add: ITerminal.ClearFireAndForget()
		
		- Fix: IDE settings into a table
		
		- Change: common inputs into a table
		
	</ul>
</details>

---

## [0.9.6.0] - 2024-09-13
<details>
  <summary>Click to show changes</summary>

	<ul>
		- Update Photino.Blazor NuGet Package to v3.1.9 (from v2.6.0).
			The version of Ubuntu tested with this change had massive usability improvements,
			and appears to perform better too.
			Previously, caret browsing was turned on, and it wasn't obvious how to turn it off,
			this was creating a whole mess of problems and no longer seems an issue.
		
	</ul>
</details>

---

## [0.9.5.1] - 2024-09-07
<details>
  <summary>Click to show changes</summary>

		- Fix tab drag off text editor group sometimes cause fatal exception due
			to null reference exception.
		
		- Fix text erroneously written out when holding non-shift modifier(s).
			(i.e.: { 'Ctrl' + 'c' } to copy writing out the letter 'c' to the text editor)
		
		- Given the change "Fix text erroneously written out when holding non-shift modifier(s)."
			all previously added hacks to fix this have been removed for the more correct implementation.
		
		- Add: DropdownHelper.cs, less code duplication of dropdown logic.
		
		- Sort codebehinds '.cs' '.css'; DragInitializer.razor is an example where it isn't sorted. (Linux specific)
		
		- Invoke codebehinds for the C# project immediate children.
		
		- Fix add '.razor.cs' for an existing '.razor' file, on linux it won't show until restart IDE.
		
		- Fix remove '.razor.cs' for an existing '.razor' file, but the expansion chevron still renders.
		
		- Sort RelatedFilesQuickPick { 'F7' }
		
		- RelatedFilesQuickPick set initial menu index to the opened file
		
</details>

---

## [0.9.5.0] - 2024-09-05
<details>
  <summary>Click to show changes</summary>

		- Fix: deletion of text that spans multiple partitions.
		
</details>

---

## [0.9.4.0] - 2024-09-02
<details>
  <summary>Click to show changes</summary>

		- Home key has indentation logic
		
		- Fix cursor blinking
		
		- Fix change keymap without having to reload
		
		- Track additionally, the 'Key' of a keyboard event args (previously only was tracking the 'Code')
		
		- Change ITextEditorWork implementations and ResourceUri to structs
		
		- When running the IDE natively, disable various browser keybinds;
			for example: 'F5' won't refresh the webview.
		
		- Fix various 'Vim' keybind bugs. It isn't fully functional yet.
		
		- Add: WidgetDisplay.razor
		
		- ContextSwitchDisplay.razor progress (keybind: Ctrl + Tab)
		
		- CommandBarDisplay.razor progress (keybind: Ctrl + P)
		
		- Use more recent dropdown code for text editor autocomplete and context menu.
			The newer dropdown code moves itself so it stays on screen (if it initially rendered offscreen).
		
		- Fix: return focus to text editor after picking a menu option in autocomplete or context menu.
		
		- Start code snippet logic.
		
		- Fix: line endings breaking due to a Post to the ITextEditorService which makes an edit,
					but then throws an exception within the same Post.
		
		- Fix: Gutter width changes causing the text editor measurements to be incorrect.
		
		- Fix: deletion of lines will now scroll by the amount of lines deleted.
				Previously, this was breaking the virtualization result until one triggered a re-calculation.
		
		- Fix: Keybinds first try to match on a JavaScript 'event.key' so to speak. Then, as a fallback
					they will now try to match on 'event.code' so to speak.
					Previously, on Ubuntu, if one remapped the CapsLock key to Escape, it would not work
					in the IDE at various places. This has been fixed.
		
		- Text editor events now use structs to transmit event data. This is expected to be a large optimization,
				as it tends that high turnover 'class' objects bring performance issues due to the garbage collection overhead.
		
		- Text editor's OnKeyDownLateBatching event uses a fixed size array for batching events, rather than what previously
				was a List&lt;T&gt;. This is expected to be a large optimization, as it tends that high turnover 'class' objects bring
				performance issues due to the garbage collection overhead. As well, it tends to be the case that no more than
				3 or 4 keyboard events ever get batched together. So the fixed size array is '8' keyboard events can be made into
				a single batch.
		
</details>

---

## [0.9.3.0] - 2024-08-16
<details>
  <summary>Click to show changes</summary>

		- 
			<a target="_blank" href="https://www.nuget.org/packages/Luthetus.TextEditor/">
				Text Editor NuGet Package v2.0.0
			</a>

			--(
			<a target="_blank" href="https://github.com/Luthetus/Luthetus.Ide/blob/main/Docs/TextEditor/installation.md">
				installation.md
			</a>
			)
		
		- Fix cursor "randomly" losing focus
		
		- Re-write virtualization in C# (it was previously done with JavaScript)
		
		- Change RichCharacter.cs to a struct (it was previously a class).
		
		- Change ITextEditorModel.RichCharacterList to an array (it was previously an ImmutableList).
		
		- Fix typing at start of file (position index 0) a non letter or digit.
		
		- Fix text editor context menu crashing when closing
		
		- Fix out of sync syntax highlighting.
		
		- IDE uses 60% less memory after various struct/array optimizations.
		
		- IDE "feels" an order of magnitude faster after various Blazor optimizations and
			struct/array optimizations (which reduce the garbage collection overhead thus improving performance greatly).
		
		- Fix terminal ContinueWithFunc not firing.
		
		- Click Output panel diagnostic to open file.
		
		- Send test output to Output panel for it to be parsed for any errors by using
			right click menu on test explorer tree view node.
		
		- Keybind { Ctrl + . } for quick actions / refactors context menu.
		
		- Refactor: generate parameterless constructor when cursor on a property within a class.
		
		- Add: TerminalWebsite.cs implementation of ITerminal to avoid confusion when running website demo.
		
</details>

---

## [0.9.2.0] - 2024-08-11
<details>
  <summary>Click to show changes</summary>

		- Bug in this version: The text editor appears to be "randomly" losing focus.
			I presume I can fix this, but I am sitting on too many code changes at the moment,
			so I'll accept these changes then look at this bug.
		
		- Cursor blinking is purposefully broken at the moment. It was
			causing rerenders to all the actively rendered text editors because I wanted them
			to synchronize the blinking. I still want it synchronized but I just want to revisit the
			implementation I think it could be better.
		
		- Use RenderFragment(s) instead of Blazor components were applicable
			to avoid the overhead of a component, while still using razor markup.
		
		- Rewrite terminal code. (this rewrite is still in progress, not finished).
		
		- If a file does not save properly, make it obvious to the user
		
		- Only invoke 'FinalizePost' in the events if there were no unhandled exceptions.
		
		- Rename 'IEditContext.cs' to 'ITextEditorEditContext.cs'
		
		- Change Luthetus libraries to net8.0
		
		- Reference Fluxor v6 NuGet package
		
		- <a href="https://github.com/tryphotino/photino.Blazor/issues/124"
				target="_blank">
				Having issues with upgrading Photino.Blazor from v2.6 to a higher version
			</a>

			For me, I can run 'dotnet run -c Release' on v2.6
			But if I try to 'dotnet publish -c Release' then 
			'cd bin/Release/net8.0/publish/' then 'dotnet ./Luthetus.Ide.Photino.dll'
			I get 'Unhandled exception. System.MissingMethodException: Method not found: 'PhotinoNET.PhotinoWindow Photino.Blazor.PhotinoBlazorApp.get_MainWindow()'.
	Aborted (core dumped)'
			If I use v3 something then I get Load("/") "/" not found
			or something.

			I hate writing notes to myself right before I go to bed but hopefully
			this is enough to jog my memory after getting some sleep.

			(it worked on Windows, but not on Ubuntu when using v3)
		
</details>

---

## [0.9.1.0] - 2024-07-24
<details>
	<summary>Click to show changes</summary>

	- IBackgroundTask service received immense optimizations.
			Some of these optimizations include: no Task.Delay between
			background task invocations, and attempt to run
			a task synchronously, and only await it if it did not finish
			synchronously.

	- The text editor's 'IEditContext' received immense optimizations.
			Some of these optimizations include: do not instantiate a 'Func'
			for every method that takes an 'IEditContext' as a parameter.

	- 'Find All' tool shows results in a tree view.
			As well, it shows multiple results per file,
			preview text for each result,
			and moves cursor to the respective result within the file.

	- '@@onkeydown="EventUtil.AsNonRenderingEventHandler&lt;KeyboardEventArgs&gt;(ReceiveOnKeyDown)"'
			This avoids unnecessary rendering due to implicit state has changed in the Blazor events.
			Note: the exact version this was added in is uncertain. It was recent though.
</details>

---

## [0.9.0.0] - 2024-07-18
<details>
  <summary>Click to show changes</summary>

	### FIX
	- IDE is language "neutral".
	  All .NET support was moved to its own project 'Luthetus.Extensions.DotNet.csproj'.
	  This allows one to pick and choose which programming languages the IDE supports.
</details>

---

## [0.8.6.0] - 2024-07-05
<details>
  <summary>Click to show changes</summary>

	### FIX
	- If a context menu is rendered off-screen, then it is repositioned.
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
