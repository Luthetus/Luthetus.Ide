# Luthetus.Website

### One can gauge their interest in the repository with this one minute GIF:
![Example GIF](./Images/Rewrite/introductoryGifLuthetusWebsite.gif)

## Demo:
https://luthetus.github.io/Luthetus.Website/

## Recent Changes:

### (2023-06-19) 
-DisplayTracker.cs (namespace: Luthetus.TextEditor.RazorLib.ViewModel). This tracks whether the ViewModel is currently rendered and is expected to be a great improvement to performance.

### (2023-06-11) 
-Many code optimizations were made to allow for the IDE repository run better.

### (2023-06-05) 
-Prefer Diagnostics over symbols for mouse hover tooltip (if they overlap).
-Parser improvements.
-Out of date Symbols are removed.
- ![Example GIF](./Images/Rewrite/preferDiagnostics.gif)

### (2023-06-04) 
- Semantic Explorer
- ![Example GIF](./Images/Rewrite/semanticExplorer.gif)

### (2023-06-03) 
- Cross file go-to definition has been started.
- ![Example GIF](./Images/Rewrite/crossFileGotoDefinition.gif)

### (2023-06-02) 
- .razor file go-to definition has been started.
- ![Example GIF](./Images/Rewrite/gotoDefinitionRazor.gif)
- Input file dialog
- ![Example GIF](./Images/Rewrite/inputFile.gif)

### (2023-06-01) 
- C# go-to definition has been started. More needs done here, for example cross file goto-definition logic.
- ![Example GIF](./Images/Rewrite/gotoDefinition.gif)

### (2023-05-31) 
- Logic added for recognizing Razor attribute directives. Such as @onclick.
- Logic to recognize variables. This logic is currently limited to the variable being declared inside the .razor file.
- Use the C# Compiler Service in .razor files.
- ![Example GIF](./Images/Rewrite/2023-05-31.gif)

### (2023-05-30) 
- Parser improvements for "var contextual keyword"
- Parser improvements for "variable symbol identification"
- ![Example GIF](./Images/Rewrite/conditionalVarProgress.gif)
- BackgroundTaskQueueSingleThreaded was added for the WASM host. This fixes a lot that broke when going from ServerSide to a WASM host. An example being, creation of new files.