using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.TextEditor.RazorLib.Autocompletes.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Installations.Models;

public record LuthetusTextEditorConfig
{
    /// <summary>
    /// The initial theme for the text editor is NOT the same as <see cref="LuthetusCommonConfig.InitialThemeKey"/>.
    /// The text editor and application theme are separate.
    /// </summary>
    public Key<ThemeRecord>? InitialThemeKey { get; init; } = ThemeFacts.VisualStudioDarkThemeClone.Key;
    /// <summary>
    /// By default the only themes are clones of the application "Visual Studio"('s) colors.
    /// That is to say, 2 themes total, one light, one dark.
    /// <br/><br/>
    /// Adding more themes here will allow them to be selected from the settings theme dropdown.
    /// </summary>
    public ImmutableArray<ThemeRecord>? CustomThemeRecordList { get; init; } = LuthetusTextEditorCustomThemeFacts.AllCustomThemesList;
    /// <summary>
    /// Default value if left null is: <see cref="WordAutocompleteService"/>
    /// </summary>
    public Func<IServiceProvider, IAutocompleteService> AutocompleteServiceFactory { get; init; } = serviceProvider => new WordAutocompleteService((WordAutocompleteIndexer)serviceProvider.GetRequiredService<IAutocompleteIndexer>());
    /// <summary>
    /// Default value if left null is: <see cref="WordAutocompleteIndexer"/>
    /// </summary>
    public Func<IServiceProvider, IAutocompleteIndexer> AutocompleteIndexerFactory { get; init; } = serviceProvider => new WordAutocompleteIndexer();
    /// <summary>
    /// When a user wants to customize the text editor, this settings dialog will be rendered.
    /// </summary>
    public SettingsDialogConfig SettingsDialogConfig { get; init; } = new();
    /// <summary>
    /// When a user hits the keymap { Control + , } then a dialog will be rendered where they can
    /// search for text through "all files". Where "all files" relates to the implementation details
    /// of the given find all dialog.
    /// </summary>
    public FindAllDialogConfig FindAllDialogConfig { get; init; } = new();
    /// <summary>
    /// The <see cref="ITextEditorSearchEngine"/>(s) are used by the <see cref="FindAllDialogConfig"/>
    /// to search through all the files and find matching text.
    /// </summary>
    public ImmutableArray<ITextEditorSearchEngine> SearchEngineList { get; init; } = SearchEngineFacts.DefaultSearchEngineList;
    /// <summary>
    /// The go-to definition implementation makes use of <see cref="RegisterModelFunc"/>.<br/>
    /// 
    /// In the case that a symbol's definition exists within a resource that does not have
    /// an already existing <see cref="TextEditorModel"/>, then this is invoked to create that
    /// instance, so that go-to definition can then be performed.<br/>
    /// 
    /// The Func takes in the resource uri that needs a model.
    /// </summary>
    public Func<RegisterModelArgs, Task>? RegisterModelFunc { get; set; }
    /// <summary>
    /// The go-to definition implementation makes use of <see cref="RegisterModelFunc"/>.<br/>
    /// 
    /// In the case that a symbol's definition exists within a resource that does not have
    /// an already existing <see cref="TextEditorViewModel"/>, then this is invoked to create that
    /// instance, so that go-to definition can then be performed.<br/>
    /// 
    /// The Func takes in the resource uri that needs a ViewModel.
    /// </summary>
    public Func<TryRegisterViewModelArgs, Task<Key<TextEditorViewModel>>>? TryRegisterViewModelFunc { get; set; }
    /// <summary>
    /// The go-to definition implementation makes use of <see cref="TryShowViewModelFunc"/>.<br/>
    /// 
    /// In the case that a symbol's definition exists within a resource that does not have
    /// an already existing ViewModel, then this is invoked to create that instance, so that
    /// go-to definition can then be performed.<br/>
    /// 
    /// The Func takes in the resource uri that needs a ViewModel.
    /// </summary>
    public Func<TryShowViewModelArgs, Task<bool>>? TryShowViewModelFunc { get; set; }
    /// <summary>
    /// Default value is <see cref="true"/>.<br/>
    /// 
    /// If one wishes to configure Luthetus.Common themselves, then set this to false, and invoke
    /// <see cref="Common.RazorLib.Installations.Models.ServiceCollectionExtensions.AddLuthetusCommonServices(IServiceCollection, LuthetusHostingInformation, Func{LuthetusCommonConfig, LuthetusCommonConfig}?)"/>
    /// prior to invoking Luthetus.TextEditor's
    /// </summary>
    public bool AddLuthetusCommon { get; init; } = true;
}