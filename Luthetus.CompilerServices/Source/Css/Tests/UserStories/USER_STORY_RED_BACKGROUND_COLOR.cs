namespace Luthetus.CompilerServices.Lang.Css.Tests.UserStories;

/// <summary>
/// User Story Description:
/// User wants to give a &lt;div class="red-background"&gt;Sample Text&lt;div&gt;
/// HTML element a red background. They have a css class listed in the HTML
/// element's 'class' attribute named: 'red-background'. They want this
/// css class to apply the red background.
/// </summary>
public class USER_STORY_RED_BACKGROUND_COLOR
{
    [Fact]
    public void Enact()
    {
        string sourceText = @".red-background {
    background-color: red;
}".ReplaceLineEndings("\n");

        throw new NotImplementedException();
    }
}