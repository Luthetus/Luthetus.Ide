using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.SearchEngines.Displays;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using Luthetus.TextEditor.RazorLib.Options.Displays;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public record LuthetusTextEditorOptions
{
    public Key<ThemeRecord>? InitialThemeKey { get; init; }
    public ImmutableArray<ThemeRecord>? CustomThemeRecordList { get; init; } = LuthetusTextEditorCustomThemeFacts.AllCustomThemesList;
    public ThemeRecord InitialTheme { get; init; } = ThemeFacts.VisualStudioDarkThemeClone;
    /// <summary>Default value if left null is: <see cref="WordAutocompleteService"/></summary>
    public Func<IServiceProvider, IAutocompleteService> AutocompleteServiceFactory { get; init; } = serviceProvider => new WordAutocompleteService((WordAutocompleteIndexer)serviceProvider.GetRequiredService<IAutocompleteIndexer>());
    /// <summary>Default value if left null is: <see cref="WordAutocompleteIndexer"/></summary>
    public Func<IServiceProvider, IAutocompleteIndexer> AutocompleteIndexerFactory { get; init; } = serviceProvider => new WordAutocompleteIndexer();
    public Type SettingsComponentRendererType { get; init; } = typeof(TextEditorSettings);
    public bool SettingsDialogComponentIsResizable { get; init; } = true;
    public Type FindAllComponentRendererType { get; init; } = typeof(TextEditorSearchEngineDisplay);
    public bool FindAllDialogComponentIsResizable { get; init; } = true;
    public ImmutableArray<ITextEditorSearchEngine> SearchEngineList { get; init; } = SearchEngineFacts.DefaultSearchEngineList;
    public Func<string, IServiceProvider, Task> OpenInEditorAsyncFunc { get; init; } = null;
    /// <summary>Default value is <see cref="true"/>. If one wishes to configure Luthetus.Common themselves, then set this to false, and invoke <see cref="Common.RazorLib.Installations.Models.ServiceCollectionExtensions.AddLuthetusCommonServices(IServiceCollection, Func{LuthetusCommonOptions, LuthetusCommonOptions}?)"/> prior to invoking Luthetus.TextEditor's</summary>
    public bool AddLuthetusCommon { get; init; } = true;
}