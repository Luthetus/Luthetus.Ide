namespace Luthetus.Ide.Wasm.Facts;

public partial class InitialSolutionFacts
{
    public const string PERSON_DISPLAY_RAZOR_ABSOLUTE_FILE_PATH = @"/BlazorCrudApp/BlazorCrudApp.Wasm/Persons/PersonDisplay.razor";
    public const string PERSON_DISPLAY_RAZOR_CONTENTS = @"<div class=""bca_person"">
	<div class=""bca_person-title"">
		@Person.DisplayName
	</div>

	<div class=""bca_person-body"">
		<button class=""bca_button""
				@onclick=""SomeMethod"">
		    SomeMethod
		</button>
	</div>
</div>";
}
