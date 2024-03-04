using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.Tests.Adhoc;

public partial class AdhocTest
{
    [Fact]
    public void PARTITITION_CliWrapIntegratedTerminalSourceText()
    {
        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;

        var model = new TextEditorModel(
             resourceUri,
             resourceLastWriteTime,
             fileExtension,
             _cliWrapIntegratedTerminalSourceText,
             null,
             null);

        Assert.Equal(_cliWrapIntegratedTerminalSourceText, model.GetAllText());

        throw new NotImplementedException();
    }
}
