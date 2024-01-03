using Xunit;
using Luthetus.TextEditor.RazorLib.Characters.Models;

namespace Luthetus.TextEditor.Tests.Basis.Characters.Models;

/// <summary>
/// <see cref="CharacterKindHelper"/>
/// </summary>
public class CharacterKindHelperTests
{
	/// <summary>
	/// <see cref="CharacterKindHelper.CharToCharacterKind(char)"/>
	/// </summary>
	[Fact]
	public void CharToCharacterKind()
	{
        // CharacterKind.Whitespace
        {
            Assert.Equal(CharacterKind.Whitespace, CharacterKindHelper.CharToCharacterKind(' '));
        }

        // CharacterKind.Punctuation
        {
            Assert.Equal(CharacterKind.Punctuation, CharacterKindHelper.CharToCharacterKind(';'));
        }

        // CharacterKind.LetterOrDigit
        {
            // Letter
            {
                Assert.Equal(CharacterKind.LetterOrDigit, CharacterKindHelper.CharToCharacterKind('C'));
            }

            // Digit
            {
                Assert.Equal(CharacterKind.LetterOrDigit, CharacterKindHelper.CharToCharacterKind('8'));
            }
        }
    }
}