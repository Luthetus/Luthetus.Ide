using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.Installations.Models;

public record LuthetusTextEditorOptionsTests
{
	[Fact]
	public void InitialThemeKey()
	{
		//public Key<ThemeRecord>? InitialThemeKey { get; init; }
		throw new NotImplementedException();
	}

	[Fact]
	public void CustomThemeRecordBag()
	{
		//public ImmutableArray<ThemeRecord>? CustomThemeRecordBag { get; init; } = LuthetusTextEditorCustomThemeFacts.AllCustomThemesBag;
		throw new NotImplementedException();
	}

	[Fact]
	public void InitialTheme()
	{
		//public ThemeRecord InitialTheme { get; init; } = ThemeFacts.VisualStudioDarkThemeClone;
		throw new NotImplementedException();
	}

	[Fact]
	public void AutocompleteServiceFactory()
	{
		//public Func<IServiceProvider, IAutocompleteService> AutocompleteServiceFactory { get; init; } = serviceProvider => new WordAutocompleteService(serviceProvider.GetRequiredService<IAutocompleteIndexer>());
		throw new NotImplementedException();
	}

	[Fact]
	public void AutocompleteIndexerFactory()
	{
		//public Func<IServiceProvider, IAutocompleteIndexer> AutocompleteIndexerFactory { get; init; } = serviceProvider => new WordAutocompleteIndexer(serviceProvider.GetRequiredService<ITextEditorService>());
		throw new NotImplementedException();
	}
	
	[Fact]
	public void SettingsComponentRendererType()
	{
		//public Type SettingsComponentRendererType { get; init; } = typeof(TextEditorSettings);
		throw new NotImplementedException();
	}

	[Fact]
	public void SettingsDialogComponentIsResizable()
	{
		//public bool SettingsDialogComponentIsResizable { get; init; } = true;
		throw new NotImplementedException();
	}

	[Fact]
	public void FindComponentRendererType()
	{
		//public Type FindComponentRendererType { get; init; } = typeof(TextEditorFindDisplay);
		throw new NotImplementedException();
	}

	[Fact]
	public void FindDialogComponentIsResizable()
	{
		//public bool FindDialogComponentIsResizable { get; init; } = true;
		throw new NotImplementedException();
	}

	[Fact]
	public void FindProviderBag()
	{
		//public ImmutableArray<ITextEditorFindProvider> FindProviderBag { get; init; } = FindFacts.DefaultFindProvidersBag;
		throw new NotImplementedException();
	}

	[Fact]
	public void AddLuthetusCommon()
	{
		//public bool AddLuthetusCommon { get; init; } = true;
		throw new NotImplementedException();
	}
}