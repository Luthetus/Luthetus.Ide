using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.TestExplorers;

public partial class TestExplorerDisplay : ComponentBase
{
	private const string DOTNET_TEST_LIST_TESTS_COMMAND = "dotnet test -t";

	private void InvokeDotNetTestListTestsCommand()
	{
	}
}