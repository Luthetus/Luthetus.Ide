using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;

namespace Luthetus.TextEditor.Tests.Basis.Characters.Models;

/// <summary>
/// <see cref="RichCharacterExtensions"/>
/// </summary>
public class RichCharacterExtensionsTests
{
	/// <summary>
	/// <see cref="RichCharacterExtensions.GetCharacterKind(RichCharacter)"/>
	/// </summary>
	[Fact]
	public void GetCharacterKind()
	{
        // CharacterKind.Whitespace
        {
            var richCharacter = new RichCharacter
            {
                DecorationByte = (byte)GenericDecorationKind.None,
                Value = ' '
            };

            Assert.Equal(CharacterKind.Whitespace, richCharacter.GetCharacterKind());
        }

        // CharacterKind.Punctuation
        {
            var richCharacter = new RichCharacter
            {
                DecorationByte = (byte)GenericDecorationKind.None,
                Value = ';'
            };

            Assert.Equal(CharacterKind.Punctuation, richCharacter.GetCharacterKind());
         }

        // CharacterKind.LetterOrDigit
        {
            // Letter
            {
                var richCharacter = new RichCharacter
                {
                    DecorationByte = (byte)GenericDecorationKind.None,
                    Value = 'C'
                };

                Assert.Equal(CharacterKind.LetterOrDigit, richCharacter.GetCharacterKind());
            }

            // Digit
            {
                var richCharacter = new RichCharacter
                {
                    DecorationByte = (byte)GenericDecorationKind.None,
                    Value = '8'
                };

                Assert.Equal(CharacterKind.LetterOrDigit, richCharacter.GetCharacterKind());
            }
        }
	}
}