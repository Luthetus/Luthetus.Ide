using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.Ide.RazorLib.CompilerServices.Models;

namespace Luthetus.Ide.Tests.Basis.CompilerServices.Models;

/// <summary>
/// <see cref="CompilerServiceRegistry"/>
/// </summary>
public class CompilerServiceRegistryTests
{
    /// <summary>
    /// <see cref="CompilerServiceRegistry(ITextEditorService, IBackgroundTaskService, IEnvironmentProvider, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="CompilerServiceRegistry.Map"/>
    /// <see cref="CompilerServiceRegistry.CSharpCompilerService"/>
    /// <see cref="CompilerServiceRegistry.CSharpProjectCompilerService"/>
    /// <see cref="CompilerServiceRegistry.CssCompilerService"/>
    /// <see cref="CompilerServiceRegistry.DotNetSolutionCompilerService"/>
    /// <see cref="CompilerServiceRegistry.FSharpCompilerService"/>
    /// <see cref="CompilerServiceRegistry.JavaScriptCompilerService"/>
    /// <see cref="CompilerServiceRegistry.JsonCompilerService"/>
    /// <see cref="CompilerServiceRegistry.RazorCompilerService"/>
    /// <see cref="CompilerServiceRegistry.TypeScriptCompilerService"/>
    /// <see cref="CompilerServiceRegistry.XmlCompilerService"/>
    /// <see cref="CompilerServiceRegistry.CCompilerService"/>
    /// <see cref="CompilerServiceRegistry.DefaultCompilerService"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="CompilerServiceRegistry.GetCompilerService(string)"/>
    /// </summary>
    [Fact]
    public void GetCompilerService()
    {
        throw new NotImplementedException();
    }
}
