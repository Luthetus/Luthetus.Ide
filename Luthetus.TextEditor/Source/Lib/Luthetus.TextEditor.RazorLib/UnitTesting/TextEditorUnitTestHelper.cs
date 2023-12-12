using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.TextEditor.RazorLib.UnitTesting;

public class TextEditorUnitTestHelper
{
    /// <summary>
    /// To create an instance of <see cref="TextEditorUnitTestHelper"/>,
    /// one should invoke <see cref="AddLuthetusTextEditorUnitTesting(IServiceCollection, LuthetusHostingInformation)"/>,
    /// then build the <see cref="IServiceProvider"/>, and provide the built serviceProvider to this constructor.
    /// </summary>
    public TextEditorUnitTestHelper(IServiceProvider serviceProvider)
    {
        AutocompleteIndexer = serviceProvider.GetRequiredService<IAutocompleteIndexer>();
        TextEditorService = serviceProvider.GetRequiredService<ITextEditorService>();
    }

    public IAutocompleteIndexer AutocompleteIndexer { get; }
    public ITextEditorService TextEditorService { get; }

    /// <summary>This method is not an extension method due to its niche nature.</summary>
    public static IServiceCollection AddLuthetusTextEditorUnitTesting(
        IServiceCollection services,
        LuthetusHostingInformation hostingInformation)
    {
        return services.AddLuthetusTextEditor(hostingInformation, inTextEditorOptions => inTextEditorOptions with
        {
            AddLuthetusCommon = false,
        });
    }
}
