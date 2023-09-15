using Luthetus.TextEditor.RazorLib.Installation.Models;

namespace Luthetus.Ide.RazorLib.InstallationCase.Models;

public record LuthetusIdeOptions
{
    /// <summary>Default value is <see cref="true"/>. If one wishes to configure Luthetus.TextEditor themselves, then set this to false, and invoke <see cref="TextEditor.RazorLib.Installation.Models.ServiceCollectionExtensions.AddLuthetusTextEditor(Microsoft.Extensions.DependencyInjection.IServiceCollection, Func{LuthetusTextEditorOptions, LuthetusTextEditorOptions}?)"/> prior to invoking Luthetus.TextEditor's</summary>
    public bool AddLuthetusTextEditor { get; init; } = true;
}