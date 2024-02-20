using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.Ide.Tests.Basis.Installations.Models;

public record LuthetusIdeConfigTests
{
    /// <summary>Default value is <see cref="true"/>. If one wishes to configure Luthetus.TextEditor themselves, then set this to false, and invoke <see cref="TextEditor.RazorLib.Installations.Models.ServiceCollectionExtensions.AddLuthetusTextEditor(Microsoft.Extensions.DependencyInjection.IServiceCollection, Func{LuthetusTextEditorConfig, LuthetusTextEditorConfig}?)"/> prior to invoking Luthetus.TextEditor's</summary>
    public bool AddLuthetusTextEditor { get; init; } = true;
}