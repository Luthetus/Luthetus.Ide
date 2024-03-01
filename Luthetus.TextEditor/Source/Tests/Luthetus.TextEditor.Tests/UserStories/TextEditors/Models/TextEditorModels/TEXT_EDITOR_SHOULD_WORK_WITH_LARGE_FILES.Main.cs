using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.Tests.UserStories.TextEditors.Models.TextEditorModels;

public partial class TEXT_EDITOR_SHOULD_WORK_WITH_LARGE_FILES
{
    [Fact]
    public void Construct_Model_With_Large_File()
    {
        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;

        var model = new TextEditorModel(
             resourceUri,
             resourceLastWriteTime,
             fileExtension,
             LARGE_FILE,
             null,
             null);

        throw new NotImplementedException();
    }
}
