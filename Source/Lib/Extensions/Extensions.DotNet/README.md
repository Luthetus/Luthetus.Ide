Luthetus.CompilerServices.RazorLib.csproj is the
way for a compiler service non-razor lib to provide razor components.

For example,
one might primarily code in C#, but occassionally have to write JavaScript.
Perhaps, this individual would prefer to load the JavaScript CompilerService,
BUT NOT load the JavaScript specific UI components.

I'm not certain on the best way to go about this "separate the compiler service
from its UI components' idea. But this is how I'll start to try.

Essentially I need to move all the .NET specific code that is in the
Luthetus.Ide.RazorLib.csproj to this project instead.

After I do that, I need to make the code generic so that any compiler service
can decide to implement that blazor component.

For example, the solution explorer. I imagine that the name "Solution Explorer"
could be changed based on some blazor parameter, and that
the nodes in the tree could be populated with different logic, instead
of from reading a .sln file and etc...

I have no idea how I would do that???? These components seem very specialized,
and I think I even prefer it that way.

But do I really want 2 projects per programming language,
one for the compiler service and one for the UI?

Move these:
[ ] /CSharpProjects/
[ ] /DotNetSolutions/
[ ] /Nugets/

[ ] /CommandLines/Models/DotNetCliCommandFormatter.cs
[ ] /CommandLines/Models/DotNetCliOutputParser.cs
[ ] /ComponentRenderers/Models/INuGetPackageManagerRendererType.cs
[ ] /ComponentRenderers/Models/IRemoveCSharpProjectFromSolutionRendererType.cs
[ ] /ComponentRenderers/Models/ITreeViewCSharpProjectNugetPackageReferenceRendererType.cs
[ ] /ComponentRenderers/Models/ITreeViewCSharpProjectToProjectReferenceRendererType.cs
[ ] /ComponentRenderers/Models/ITreeViewSolutionFolderRendererType.cs
[ ] /Menus/Models/IMenuOptionsFactory.RemoveCSharpProjectReferenceFromSolution()
[ ] /Menus/Models/IMenuOptionsFactory.AddProjectToProjectReference()
[ ] /Menus/Models/IMenuOptionsFactory.RemoveProjectToProjectReference()
[ ] /Menus/Models/IMenuOptionsFactory.MoveProjectToSolutionFolder()
[ ] /Menus/Models/IMenuOptionsFactory.RemoveNuGetPackageReferenceFromProject()
[ ] Repeat what is done for IMenuOptionsFactory.cs to its implementation MenuOptionsFactory.cs
[ ] /Namespaces/Models/TreeViewHelperCSharpClass.cs
[ ] /Namespaces/Models/TreeViewHelperCSharpProject.cs
[ ] /Namespaces/Models/TreeViewHelperRazorMarkup.cs
[ ] /Shareds/Displays/Internals/IdePromptOpenSolutionDisplay.razor
[ ] /TestExplorers/ ###### Should /TestExplorers/ be moved????
[ ] /Namespaces/ ###### Should /Namespaces/ be moved????

I need to make a 'Workspace' concept in order to permit arbitrary grouping of directories
into a tree view.

I moved over '/CSharpProjects/' and it is immediately apparent that this will be a headache.
Some of the code in '/CSharpProjects/' relies on various bits and pieces of code that exist within
the IDE, so I'll get a circular reference if I try to take the shortcut of having
the IDE reference my new project for time being

