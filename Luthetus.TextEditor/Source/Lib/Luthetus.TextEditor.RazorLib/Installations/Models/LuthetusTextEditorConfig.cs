using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.SearchEngines.Displays;
using Luthetus.TextEditor.RazorLib.SearchEngines.Models;
using Luthetus.TextEditor.RazorLib.Options.Displays;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public record LuthetusTextEditorConfig
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
    /// <summary>
    /// The go-to definition implementation makes use of <see cref="RegisterModelAction"/>.<br/>
    /// 
    /// In the case that a symbol's definition exists within a resource that does not have
    /// an already existing Model, then this is invoked to create that instance, so that
    /// go-to definition can then be performed.<br/>
    /// 
    /// The Func takes in the resource uri that needs a model.
    /// </summary>
    public Action<ResourceUri>? RegisterModelAction { get; set; }
    /// <summary>
    /// The go-to definition implementation makes use of <see cref="RegisterModelAction"/>.<br/>
    /// 
    /// In the case that a symbol's definition exists within a resource that does not have
    /// an already existing ViewModel, then this is invoked to create that instance, so that
    /// go-to definition can then be performed.<br/>
    /// 
    /// The Func takes in the resource uri that needs a ViewModel.
    /// </summary>
    public Action<ResourceUri>? RegisterViewModelAction { get; set; }
    public Action<Key<TextEditorViewModel>>? ShowViewModelAction { get; set; }
    /// <summary>Default value is <see cref="true"/>. If one wishes to configure Luthetus.Common themselves, then set this to false, and invoke <see cref="Common.RazorLib.Installations.Models.ServiceCollectionExtensions.AddLuthetusCommonServices(IServiceCollection, Func{LuthetusCommonConfig, LuthetusCommonConfig}?)"/> prior to invoking Luthetus.TextEditor's</summary>
    public bool AddLuthetusCommon { get; init; } = true;
}