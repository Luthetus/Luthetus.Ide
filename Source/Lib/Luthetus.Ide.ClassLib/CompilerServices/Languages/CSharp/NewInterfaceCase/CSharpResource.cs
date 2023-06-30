using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.NewInterfaceCase;

public class CSharpResource
{
    public CSharpResource(
        TextEditorModelKey modelKey,
        ResourceUri resourceUri,
        CSharpCompilerService cSharpCompilerService)
    {
        ModelKey = modelKey;
        ResourceUri = resourceUri;
        CSharpCompilerService = cSharpCompilerService;
    }

    public TextEditorModelKey ModelKey { get; }
    public ResourceUri ResourceUri { get; }
    public CSharpCompilerService CSharpCompilerService { get; }

    /// <returns>
    /// The <see cref="Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.ISyntaxNode"/>
    /// which represents the resource in the compilation result.
    /// </returns>
    public async Task GetRootSyntaxNodeAsync()
    {
        //CSharpCompilerService.Compilation.RootSyntaxNode;
    }
}
