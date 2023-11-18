using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.Tests.Basis.Installations.Models;

/// <summary>
/// <see cref="LuthetusCommonOptions"/>
/// </summary>
public record LuthetusCommonOptionsTests
{
    /// <summary>
    /// <see cref="LuthetusCommonOptions.InitialThemeKey"/>
    /// </summary>
    [Fact]
    public void InitialThemeKey()
    {
        // Use default value
        {
            var initialThemeKeyDefault = ThemeFacts.VisualStudioDarkThemeClone.Key;
            var luthetusCommonOptions = new LuthetusCommonOptions();

            Assert.Equal(initialThemeKeyDefault, luthetusCommonOptions.InitialThemeKey);
        }
        
        // Init value
        {
            var initialThemeKey = ThemeFacts.VisualStudioLightThemeClone.Key;
            
            var luthetusCommonOptions = new LuthetusCommonOptions
            {
                InitialThemeKey = initialThemeKey
            };

            Assert.Equal(initialThemeKey, luthetusCommonOptions.InitialThemeKey);
        }
        
        // With value
        {
            var initialThemeKeyDefault = ThemeFacts.VisualStudioDarkThemeClone.Key;
            var luthetusCommonOptions = new LuthetusCommonOptions();

            Assert.Equal(initialThemeKeyDefault, luthetusCommonOptions.InitialThemeKey);
            
            var initialThemeKey = ThemeFacts.VisualStudioLightThemeClone.Key;

            luthetusCommonOptions = luthetusCommonOptions with
            {
                InitialThemeKey = initialThemeKey
            };

            Assert.Equal(initialThemeKey, luthetusCommonOptions.InitialThemeKey);
        }
    }

    /// <summary>
    /// <see cref="LuthetusCommonOptions.CommonFactories"/>
    /// </summary>
    [Fact]
    public void CommonFactories()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonOptions.DialogServiceOptions"/>
    /// </summary>
    [Fact]
    public void DialogServiceOptions()
    {
        // Use default value
        {
            var dialogServiceOptionsDefault = new DialogServiceOptions();
            var luthetusCommonOptions = new LuthetusCommonOptions();

            Assert.Equal(dialogServiceOptionsDefault, luthetusCommonOptions.DialogServiceOptions);
        }

        // Init value
        {
            var isMaximizedStyleCssString = "abc123";

            var dialogServiceOptions = new DialogServiceOptions
            {
                IsMaximizedStyleCssString = isMaximizedStyleCssString
            };

            var luthetusCommonOptions = new LuthetusCommonOptions
            {
                DialogServiceOptions = dialogServiceOptions
            };

            Assert.Equal(dialogServiceOptions, luthetusCommonOptions.DialogServiceOptions);
        }

        // With value
        {
            var dialogServiceOptions = new DialogServiceOptions();
            var luthetusCommonOptions = new LuthetusCommonOptions();
            Assert.Equal(dialogServiceOptions, luthetusCommonOptions.DialogServiceOptions);

            var isMaximizedStyleCssString = "abc123";
            
            dialogServiceOptions = dialogServiceOptions with
            {
                IsMaximizedStyleCssString = isMaximizedStyleCssString
            };

            luthetusCommonOptions = luthetusCommonOptions with
            {
                DialogServiceOptions = dialogServiceOptions
            };

            Assert.Equal(dialogServiceOptions, luthetusCommonOptions.DialogServiceOptions);
        }
    }
}
