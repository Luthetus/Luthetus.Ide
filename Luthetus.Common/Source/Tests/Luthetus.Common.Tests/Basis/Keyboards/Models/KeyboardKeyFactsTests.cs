using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components.Web;
using static Luthetus.Common.RazorLib.Keyboards.Models.KeyboardKeyFacts;

namespace Luthetus.Common.Tests.Basis.Keyboards.Models;

/// <summary>
/// <see cref="KeyboardKeyFacts"/>
/// </summary>
public class KeyboardKeyFactsTests
{
    /// <summary>
    /// <see cref="IsMetaKey(KeyboardEventArgs)"/>
    /// </summary>
    [Fact]
    public void IsMetaKeyA()
    {
        Assert.True(IsMetaKey(new KeyboardEventArgs
        {
            Code = "F1",
            Key = "F1",
        }));

        Assert.False(IsMetaKey(new KeyboardEventArgs
        {
            Code = WhitespaceCodes.TAB_CODE,
            // Key and Code for 'tab' are the same
            Key = WhitespaceCodes.TAB_CODE,
        }));
    }

    /// <summary>
    /// <see cref="IsMetaKey(string, string)"/>
    /// </summary>
    [Fact]
    public void IsMetaKeyB()
    {
        Assert.True(IsMetaKey(
            "F1",
            "F1"));

        Assert.False(IsMetaKey(
            WhitespaceCodes.TAB_CODE,
            // Key and Code for 'tab' are the same
            WhitespaceCodes.TAB_CODE));
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.IsWhitespaceCharacter(char)"/>
    /// </summary>
    [Fact]
    public void IsWhitespaceCharacter()
    {
        foreach (var whitespaceCharacter in WhitespaceCharacters.AllBag)
        {
            Assert.True(KeyboardKeyFacts.IsWhitespaceCharacter(whitespaceCharacter));
        }

        foreach (var punctuationCharacter in PunctuationCharacters.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsWhitespaceCharacter(punctuationCharacter));
        }

        // Letter
        Assert.False(KeyboardKeyFacts.IsWhitespaceCharacter('a'));

        // Digit
        Assert.False(KeyboardKeyFacts.IsWhitespaceCharacter('5'));
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.IsPunctuationCharacter(char)"/>
    /// </summary>
    [Fact]
    public void IsPunctuationCharacter()
    {
        foreach (var punctuationCharacter in PunctuationCharacters.AllBag)
        {
            Assert.True(KeyboardKeyFacts.IsPunctuationCharacter(punctuationCharacter));
        }

        foreach (var whitespaceCharacter in WhitespaceCharacters.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsPunctuationCharacter(whitespaceCharacter));
        }

        // Letter
        Assert.False(KeyboardKeyFacts.IsPunctuationCharacter('a'));

        // Digit
        Assert.False(KeyboardKeyFacts.IsPunctuationCharacter('5'));
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.MatchPunctuationCharacter(char)"/>
    /// </summary>
    [Fact]
    public void MatchPunctuationCharacter()
    {
        Assert.Equal(PunctuationCharacters.CLOSE_CURLY_BRACE, KeyboardKeyFacts.MatchPunctuationCharacter(PunctuationCharacters.OPEN_CURLY_BRACE));
        Assert.Equal(PunctuationCharacters.OPEN_CURLY_BRACE, KeyboardKeyFacts.MatchPunctuationCharacter(PunctuationCharacters.CLOSE_CURLY_BRACE));
        Assert.Equal(PunctuationCharacters.CLOSE_PARENTHESIS, KeyboardKeyFacts.MatchPunctuationCharacter(PunctuationCharacters.OPEN_PARENTHESIS));
        Assert.Equal(PunctuationCharacters.OPEN_PARENTHESIS, KeyboardKeyFacts.MatchPunctuationCharacter(PunctuationCharacters.CLOSE_PARENTHESIS));
        Assert.Equal(PunctuationCharacters.CLOSE_SQUARE_BRACKET, KeyboardKeyFacts.MatchPunctuationCharacter(PunctuationCharacters.OPEN_SQUARE_BRACKET));
        Assert.Equal(PunctuationCharacters.OPEN_SQUARE_BRACKET, KeyboardKeyFacts.MatchPunctuationCharacter(PunctuationCharacters.CLOSE_SQUARE_BRACKET));
        Assert.Equal(PunctuationCharacters.CLOSE_ARROW_BRACKET, KeyboardKeyFacts.MatchPunctuationCharacter(PunctuationCharacters.OPEN_ARROW_BRACKET));
        Assert.Equal(PunctuationCharacters.OPEN_ARROW_BRACKET, KeyboardKeyFacts.MatchPunctuationCharacter(PunctuationCharacters.CLOSE_ARROW_BRACKET));
        
        // Letter
        Assert.Null(KeyboardKeyFacts.MatchPunctuationCharacter('a'));

        // Digit
        Assert.Null(KeyboardKeyFacts.MatchPunctuationCharacter('1'));

        // Punctuation
        Assert.Null(KeyboardKeyFacts.MatchPunctuationCharacter('/'));

        // Whitespace
        Assert.Null(KeyboardKeyFacts.MatchPunctuationCharacter(WhitespaceCharacters.SPACE));
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.DirectionToFindMatchingPunctuationCharacter(char)"/>
    /// </summary>
    [Fact]
    public void DirectionToFindMatchMatchingPunctuationCharacter()
    {
        Assert.Equal(1, DirectionToFindMatchingPunctuationCharacter(PunctuationCharacters.OPEN_CURLY_BRACE));
        Assert.Equal(-1, DirectionToFindMatchingPunctuationCharacter(PunctuationCharacters.CLOSE_CURLY_BRACE));
        Assert.Equal(1, DirectionToFindMatchingPunctuationCharacter(PunctuationCharacters.OPEN_PARENTHESIS));
        Assert.Equal(-1, DirectionToFindMatchingPunctuationCharacter(PunctuationCharacters.CLOSE_PARENTHESIS));
        Assert.Equal(1, DirectionToFindMatchingPunctuationCharacter(PunctuationCharacters.OPEN_SQUARE_BRACKET));
        Assert.Equal(-1, DirectionToFindMatchingPunctuationCharacter(PunctuationCharacters.CLOSE_SQUARE_BRACKET));
        Assert.Equal(1, DirectionToFindMatchingPunctuationCharacter(PunctuationCharacters.OPEN_ARROW_BRACKET));
        Assert.Equal(-1, DirectionToFindMatchingPunctuationCharacter(PunctuationCharacters.CLOSE_ARROW_BRACKET));

        // Letter
        Assert.Null(DirectionToFindMatchingPunctuationCharacter('a'));

        // Digit
        Assert.Null(DirectionToFindMatchingPunctuationCharacter('1'));

        // Punctuation
        Assert.Null(DirectionToFindMatchingPunctuationCharacter('/'));
        
        // Whitespace
        Assert.Null(DirectionToFindMatchingPunctuationCharacter(WhitespaceCharacters.SPACE));
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.IsWhitespaceCode(string)"/>
    /// </summary>
    [Fact]
    public void IsWhitespaceCode()
    {
        foreach (var whitespaceCode in WhitespaceCodes.AllBag)
        {
            Assert.True(KeyboardKeyFacts.IsWhitespaceCode(whitespaceCode));
        }

        foreach (var movementKey in MovementKeys.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsWhitespaceCode(movementKey));
        }

        foreach (var punctuationCharacter in PunctuationCharacters.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsWhitespaceCode(punctuationCharacter.ToString()));
        }

        foreach (var whitespaceCharacter in WhitespaceCharacters.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsWhitespaceCode(whitespaceCharacter.ToString()));
        }

        // Letter
        Assert.False(KeyboardKeyFacts.IsWhitespaceCode("a"));

        // Digit
        Assert.False(KeyboardKeyFacts.IsWhitespaceCode("5"));
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(string, string, bool, bool)"/>
    /// </summary>
    [Fact]
    public void CheckIsAlternateContextMenuEvent()
    {
        // Presuming the code remains as: { "F10" + ShiftKey }
        // where "F10" is the Key (2023-11-20)

        Assert.True(KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(
            "F10",
            string.Empty,
            true,
            false));

        Assert.False(KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(
            "F10",
            string.Empty,
            false,
            false));

        Assert.False(KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(
            "a",
            "KeyA",
            true,
            false));

        Assert.False(KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(
            "a",
            "KeyA",
            false,
            false));

        // "ContextMenu" key is not the alternative, therefore these are false
        {
            Assert.False(KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(
                "ContextMenu",
                "ContextMenu",
                true,
                false));

            Assert.False(KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(
                "ContextMenu",
                "ContextMenu",
                false,
                false));
        }
    }

    /// <summary>
    /// <see cref="CheckIsContextMenuEvent(string, string, bool, bool)"/>
    /// </summary>
    [Fact]
    public void CheckIsContextMenuEventA()
    {
        // Default contextmenu
        {
            Assert.True(CheckIsContextMenuEvent(
                "ContextMenu",
                "ContextMenu",
                false,
                false));

            Assert.True(CheckIsContextMenuEvent(
                "ContextMenu",
                "ContextMenu",
                true,
                false));
        }

        // Alternate contextmenu
        {
            Assert.False(CheckIsContextMenuEvent(
                "F10",
                string.Empty,
                false,
                false));

            Assert.True(CheckIsContextMenuEvent(
                "F10",
                string.Empty,
                true,
                false));
        }

        // NOT-contextmenu
        {
            Assert.False(CheckIsContextMenuEvent(
                "a",
                "KeyA",
                false,
                false));

            Assert.False(CheckIsContextMenuEvent(
                "a",
                "KeyA",
                true,
                false));
        }
    }

    /// <summary>
    /// <see cref="CheckIsContextMenuEvent(KeyboardEventArgs)"/>
    /// </summary>
    [Fact]
    public void CheckIsContextMenuEventB()
    {
        // Default contextmenu
        {
            Assert.True(CheckIsContextMenuEvent(new KeyboardEventArgs
            {
                Key = "ContextMenu",
                Code = "ContextMenu",
                ShiftKey = false,
                AltKey = false,
            }));

            Assert.True(CheckIsContextMenuEvent(new KeyboardEventArgs
            {
                Key = "ContextMenu",
                Code = "ContextMenu",
                ShiftKey = true,
                AltKey = false,
            }));
        }

        // Alternate contextmenu
        {
            Assert.False(CheckIsContextMenuEvent(new KeyboardEventArgs
            {
                Key = "F10",
                Code = string.Empty,
                ShiftKey = false,
                AltKey = false,
            }));

            Assert.True(CheckIsContextMenuEvent(new KeyboardEventArgs
            {
                Key = "F10",
                Code = string.Empty,
                ShiftKey = true,
                AltKey = false,
            }));
        }

        // NOT-contextmenu
        {
            Assert.False(CheckIsContextMenuEvent(new KeyboardEventArgs
            {
                Key = "a",
                Code = "KeyA",
                ShiftKey = false,
                AltKey = false,
            }));

            Assert.False(CheckIsContextMenuEvent(new KeyboardEventArgs
            {
                Key = "a",
                Code = "KeyA",
                ShiftKey = true,
                AltKey = false,
            }));
        }
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.IsMovementKey(string)"/>
    /// </summary>
    [Fact]
    public void IsMovementKey()
    {
        foreach (var movementKey in MovementKeys.AllBag)
        {
            Assert.True(KeyboardKeyFacts.IsMovementKey(movementKey));
        }

        foreach (var whitespaceCode in WhitespaceCodes.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsMovementKey(whitespaceCode));
        }

        foreach (var punctuationCharacter in PunctuationCharacters.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsMovementKey(punctuationCharacter.ToString()));
        }

        foreach (var whitespaceCharacter in WhitespaceCharacters.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsMovementKey(whitespaceCharacter.ToString()));
        }

        // Letter
        Assert.False(KeyboardKeyFacts.IsMovementKey("a"));

        // Digit
        Assert.False(KeyboardKeyFacts.IsMovementKey("5"));
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(string)"/>
    /// </summary>
    [Fact]
    public void ConvertWhitespaceCodeToCharacter()
    {
        Assert.Equal('\t', KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(WhitespaceCodes.TAB_CODE));
        Assert.Equal('\n', KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(WhitespaceCodes.ENTER_CODE));
        Assert.Equal(' ', KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(WhitespaceCodes.SPACE_CODE));

        // Letter
        Assert.ThrowsAny<Exception>(
            () => KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter("KeyA"));

        // Digit
        Assert.ThrowsAny<Exception>(
            () => KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter("Digit1"));
        
        // Punctuation
        Assert.ThrowsAny<Exception>(
            () => KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter("Slash"));
        
        // Modifier
        Assert.ThrowsAny<Exception>(
            () => KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter("ControlLeft"));
        
        // F-Key
        Assert.ThrowsAny<Exception>(
            () => KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter("F1"));
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.IsLineEndingCharacter(char)"/>
    /// </summary>
    [Fact]
    public void IsLineEndingCharacter()
    {
        Assert.True(KeyboardKeyFacts.IsLineEndingCharacter(WhitespaceCharacters.NEW_LINE));
        Assert.True(KeyboardKeyFacts.IsLineEndingCharacter(WhitespaceCharacters.CARRIAGE_RETURN));

        foreach (var punctuationCharacter in PunctuationCharacters.AllBag)
        {
            Assert.False(KeyboardKeyFacts.IsLineEndingCharacter(punctuationCharacter));
        }
    }
}