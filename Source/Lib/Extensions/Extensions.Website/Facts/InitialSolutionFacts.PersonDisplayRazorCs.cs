namespace Luthetus.Ide.Wasm.Facts;

public partial class InitialSolutionFacts
{
    public const string PERSON_DISPLAY_RAZOR_CS_ABSOLUTE_FILE_PATH = @"/BlazorCrudApp/BlazorCrudApp.Wasm/Persons/PersonDisplay.razor.cs";
    public const string PERSON_DISPLAY_RAZOR_CS_CONTENTS = @"using Microsoft.AspNetCore.Components;

namespace BlazorCrudApp.Wasm.Persons;

public partial class PersonDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public Person Person { get; set; } = null!;

	public void SomeMethod()
	{
		if (true)
			return;
	}
}";
}
