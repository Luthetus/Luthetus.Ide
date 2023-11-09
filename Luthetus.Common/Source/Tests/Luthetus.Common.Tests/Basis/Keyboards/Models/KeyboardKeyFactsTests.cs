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
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.MatchPunctuationCharacter(char)"/>
    /// </summary>
    [Fact]
    public void MatchPunctuationCharacter()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.DirectionToFindMatchMatchingPunctuationCharacter(char)"/>
    /// </summary>
    [Fact]
    public void DirectionToFindMatchMatchingPunctuationCharacter()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.IsWhitespaceCode(string)"/>
    /// </summary>
    [Fact]
    public void IsWhitespaceCode()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.CheckIsAlternateContextMenuEvent(string, string, bool, bool)"/>
    /// </summary>
    [Fact]
    public void CheckIsAlternateContextMenuEvent()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="CheckIsContextMenuEvent(string, string, bool, bool)"/>
    /// </summary>
    [Fact]
    public void CheckIsContextMenuEventA()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="CheckIsContextMenuEvent(KeyboardEventArgs)"/>
    /// </summary>
    [Fact]
    public void CheckIsContextMenuEventB()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.IsMovementKey(string)"/>
    /// </summary>
    [Fact]
    public void IsMovementKey()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="KeyboardKeyFacts.ConvertWhitespaceCodeToCharacter(string)"/>
    /// </summary>
    [Fact]
    public void ConvertWhitespaceCodeToCharacter()
    {
        throw new NotImplementedException();
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