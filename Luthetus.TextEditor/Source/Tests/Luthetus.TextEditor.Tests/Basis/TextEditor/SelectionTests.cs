using Xunit;

namespace Luthetus.TextEditor.Tests.Basics.TextEditor;

public class SelectionTests
{
    [Fact]
    public void SELECT_TEXT()
    {
    }

    [Fact]
    public void SELECT_TEXT_THEN_ARROW_LEFT()
    {
    }

    [Fact]
    public void SELECT_TEXT_THEN_ARROW_DOWN()
    {
    }

    [Fact]
    public void SELECT_TEXT_THEN_ARROW_UP()
    {
    }

    [Fact]
    public void SELECT_TEXT_THEN_ARROW_RIGHT()
    {
    }

    [Fact]
    public void SELECT_TEXT_THEN_HOME()
    {
    }

    [Fact]
    public void SELECT_TEXT_THEN_END()
    {
    }

    [Fact]
    public void SELECT_ALL()
    {
    }

    [Fact]
    public void SELECT_ALL_THEN_ARROW_LEFT()
    {
    }

    /// <summary>
    /// BUG: from [3.2.0] where this
    /// would position the user's cursor
    /// out of bounds. The next movement they made
    /// would then cause the app to crash.
    /// </summary>
    [Fact]
    public void SELECT_ALL_THEN_ARROW_RIGHT()
    {
    }

    [Fact]
    public void EXPAND_SELECTION()
    {
    }

    [Fact]
    public void COPY_SELECTION()
    {
    }

    [Fact]
    public void CUT_SELECTION()
    {
    }
}