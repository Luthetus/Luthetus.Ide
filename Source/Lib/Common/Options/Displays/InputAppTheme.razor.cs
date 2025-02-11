using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Options.Displays;

public partial class InputAppTheme : IDisposable
{
    [Inject]
    private IThemeService ThemeRecordsCollectionService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;

    [Parameter]
    public InputViewModel InputViewModel { get; set; } = InputViewModel.Empty;

    protected override void OnInitialized()
    {
        AppOptionsService.AppOptionsStateChanged += OnAppOptionsStateChanged;
        ThemeRecordsCollectionService.ThemeStateWrap.StateChanged += OnStateChanged;

        base.OnInitialized();
    }

    private async void OnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private void OnThemeSelectChanged(ChangeEventArgs changeEventArgs)
    {
        if (changeEventArgs.Value is null)
            return;

        var themeState = ThemeRecordsCollectionService.ThemeStateWrap.Value;

        var guidAsString = (string)changeEventArgs.Value;

        if (Guid.TryParse(guidAsString, out var guidValue))
        {
            var themesInScopeList = themeState.ThemeList.Where(x => x.ThemeScopeList.Contains(ThemeScope.App))
                .ToArray();

            var existingThemeRecord = themesInScopeList.FirstOrDefault(btr => btr.Key.Guid == guidValue);

            if (existingThemeRecord is not null)
                AppOptionsService.SetActiveThemeRecordKey(existingThemeRecord.Key);
        }
    }

    private bool CheckIsActiveValid(ThemeRecord[] themeRecordList, Key<ThemeRecord> activeThemeKey)
    {
        return themeRecordList.Any(btr => btr.Key == activeThemeKey);
    }

    private bool CheckIsActiveSelection(Key<ThemeRecord> themeKey, Key<ThemeRecord> activeThemeKey)
    {
        return themeKey == activeThemeKey;
    }

	public async void OnAppOptionsStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}

    public void Dispose()
    {
        AppOptionsService.AppOptionsStateChanged -= OnAppOptionsStateChanged;
        ThemeRecordsCollectionService.ThemeStateWrap.StateChanged -= OnStateChanged;
    }
}