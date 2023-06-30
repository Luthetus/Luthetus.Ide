using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.NewInterfaceCase;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.NewInterfaceCase;

public class CSharpCompilerServiceTests
{
    [Fact]
    public void Aaa()
    {
        var cSharpCompilerService = new CSharpCompilerService();

        var resourceUri = new ResourceUri("Program.cs");

        //var textEditorModel = new TextEditorModel(
        //    resourceUri,
        //    DateTime.UtcNow,
        //    ".cs",
        //    "content",
        //    null);

        //cSharpCompilerService.RegisterModel();
    }
}
