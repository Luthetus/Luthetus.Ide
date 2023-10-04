using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Keyboards.Models;

public static class KeyboardKeyFacts
{
    public static bool IsMetaKey(KeyboardEventArgs keyboardEventArgs)
    {
        return IsMetaKey(keyboardEventArgs.Key, keyboardEventArgs.Code);
    }

    public static bool IsMetaKey(string key, string code)
    {
        if (key.Length > 1 && !IsWhitespaceCode(code))
            return true;

        return false;
    }

    public static bool IsWhitespaceCharacter(char character)
    {
        switch (character)
        {
            case WhitespaceCharacters.TAB:
            case WhitespaceCharacters.CARRIAGE_RETURN:
            case WhitespaceCharacters.NEW_LINE:
            case WhitespaceCharacters.SPACE:
                return true;
            default:
                return false;
        }
    }

    public static bool IsPunctuationCharacter(char character)
    {
        switch (character)
        {
            case PunctuationCharacters.OPEN_CURLY_BRACE:
            case PunctuationCharacters.CLOSE_CURLY_BRACE:
            case PunctuationCharacters.OPEN_PARENTHESIS:
            case PunctuationCharacters.CLOSE_PARENTHESIS:
            case PunctuationCharacters.OPEN_SQUARE_BRACKET:
            case PunctuationCharacters.CLOSE_SQUARE_BRACKET:
            case PunctuationCharacters.BANG:
            case PunctuationCharacters.QUESTION_MARK:
            case PunctuationCharacters.PERIOD:
            case PunctuationCharacters.COMMA:
            case PunctuationCharacters.HASHTAG:
            case PunctuationCharacters.DOLLARS:
            case PunctuationCharacters.PERCENT:
            case PunctuationCharacters.AMPERSAND:
            case PunctuationCharacters.CARET:
            case PunctuationCharacters.STAR:
            case PunctuationCharacters.PLUS:
            case PunctuationCharacters.SEMICOLON:
            case PunctuationCharacters.EQUAL:
            case PunctuationCharacters.AT:
            case PunctuationCharacters.DASH:
            // TODO: Should 'PunctuationCharacters.UNDER_SCORE' count as punctuation? It makes expand selection (double mouse click) on a private field with the leading '_' convention annoying to select the whole word.
            //case PunctuationCharacters.UNDER_SCORE:
            case PunctuationCharacters.ACCENT:
            case PunctuationCharacters.TILDE:
            case PunctuationCharacters.PIPE:
            case PunctuationCharacters.COLON:
            case PunctuationCharacters.DOUBLE_QUOTE:
            case PunctuationCharacters.SINGLE_QUOTE:
            case PunctuationCharacters.OPEN_ARROW_BRACKET:
            case PunctuationCharacters.END_ARROW_BRACKET:
            case PunctuationCharacters.FORWARD_SLASH:
            case PunctuationCharacters.BACK_SLASH:
                return true;
            default:
                return false;
        }
    }

    public static char? MatchPunctuationCharacter(char character)
    {
        switch (character)
        {
            case PunctuationCharacters.OPEN_CURLY_BRACE:
                return PunctuationCharacters.CLOSE_CURLY_BRACE;
            case PunctuationCharacters.CLOSE_CURLY_BRACE:
                return PunctuationCharacters.OPEN_CURLY_BRACE;
            case PunctuationCharacters.OPEN_PARENTHESIS:
                return PunctuationCharacters.CLOSE_PARENTHESIS;
            case PunctuationCharacters.CLOSE_PARENTHESIS:
                return PunctuationCharacters.OPEN_PARENTHESIS;
            case PunctuationCharacters.OPEN_SQUARE_BRACKET:
                return PunctuationCharacters.CLOSE_SQUARE_BRACKET;
            case PunctuationCharacters.CLOSE_SQUARE_BRACKET:
                return PunctuationCharacters.OPEN_SQUARE_BRACKET;
            case PunctuationCharacters.OPEN_ARROW_BRACKET:
                return PunctuationCharacters.END_ARROW_BRACKET;
            case PunctuationCharacters.END_ARROW_BRACKET:
                return PunctuationCharacters.OPEN_ARROW_BRACKET;
            default:
                return null;
        }
    }

    public static int? DirectionToFindMatchMatchingPunctuationCharacter(char character)
    {
        switch (character)
        {
            case PunctuationCharacters.OPEN_CURLY_BRACE:
                return 1;
            case PunctuationCharacters.CLOSE_CURLY_BRACE:
                return -1;
            case PunctuationCharacters.OPEN_PARENTHESIS:
                return 1;
            case PunctuationCharacters.CLOSE_PARENTHESIS:
                return -1;
            case PunctuationCharacters.OPEN_SQUARE_BRACKET:
                return 1;
            case PunctuationCharacters.CLOSE_SQUARE_BRACKET:
                return -1;
            case PunctuationCharacters.OPEN_ARROW_BRACKET:
                return 1;
            case PunctuationCharacters.END_ARROW_BRACKET:
                return -1;
            default:
                return null;
        }
    }

    public static bool IsWhitespaceCode(string code)
    {
        switch (code)
        {
            case WhitespaceCodes.TAB_CODE:
            case WhitespaceCodes.CARRIAGE_RETURN_CODE:
            case WhitespaceCodes.ENTER_CODE:
            case WhitespaceCodes.SPACE_CODE:
                return true;
            default:
                return false;
        }
    }

    public static bool CheckIsAlternateContextMenuEvent(string key, string code, bool shiftWasPressed,
        bool altWasPressed)
    {
        var keyOne = "F10";
        var keyTwo = "f10";

        var wasShiftF10 = (key == keyOne || key == keyTwo)
                          && shiftWasPressed;

        var wasAltPeriod = key == "."
                           && altWasPressed;

        return wasShiftF10 || wasAltPeriod;
    }

    public static bool CheckIsContextMenuEvent(string key, string code, bool shiftWasPressed, bool altWasPressed)
    {
        var keyOne = "ContextMenu";

        return key == keyOne ||
               CheckIsAlternateContextMenuEvent(key, code, shiftWasPressed, altWasPressed);
    }

    public static bool CheckIsContextMenuEvent(KeyboardEventArgs keyboardEventArgs)
    {
        return CheckIsContextMenuEvent(
            keyboardEventArgs.Key,
            keyboardEventArgs.Code,
            keyboardEventArgs.ShiftKey,
            keyboardEventArgs.AltKey);
    }

    public static bool IsMovementKey(string key)
    {
        switch (key)
        {
            case MovementKeys.ARROW_LEFT:
            case MovementKeys.ARROW_DOWN:
            case MovementKeys.ARROW_UP:
            case MovementKeys.ARROW_RIGHT:
            case MovementKeys.HOME:
            case MovementKeys.END:
                return true;
            default:
                return false;
        }
    }

    public static char ConvertWhitespaceCodeToCharacter(string code)
    {
        switch (code)
        {
            case WhitespaceCodes.TAB_CODE:
                return '\t';
            case WhitespaceCodes.ENTER_CODE:
                return '\n';
            case WhitespaceCodes.SPACE_CODE:
                return ' ';
            case WhitespaceCodes.CARRIAGE_RETURN_CODE:
                return '\r';
            default:
                throw new ApplicationException($"Unrecognized Whitespace code of: {code}");
        }
    }

    public static bool IsLineEndingCharacter(char character)
    {
        return character switch
        {
            WhitespaceCharacters.NEW_LINE => true,
            WhitespaceCharacters.CARRIAGE_RETURN => true,
            _ => false,
        };
    }

    public static class MetaKeys
    {
        public const string BACKSPACE = "Backspace";
        public const string ESCAPE = "Escape";
        public const string DELETE = "Delete";
        public const string F10 = "F10";
        public const string PAGE_UP = "PageUp";
        public const string PAGE_DOWN = "PageDown";
    }

    public static class WhitespaceCharacters
    {
        public const char TAB = '\t';
        public const char CARRIAGE_RETURN = '\r';
        public const char NEW_LINE = '\n';
        public const char SPACE = ' ';
    }

    public static class WhitespaceCodes
    {
        public const string TAB_CODE = "Tab";

        // TODO: Get CARRIAGE_RETURN_CODE code
        public const string CARRIAGE_RETURN_CODE = "";
        public const string ENTER_CODE = "Enter";
        public const string SPACE_CODE = "Space";
    }

    /// <summary>
    /// Added characters that were found in
    /// https://www.scintilla.org/ScintillaDoc.html
    /// source code, CharacterType.h:79
    /// </summary>
    public static class PunctuationCharacters
    {
        public const char OPEN_CURLY_BRACE = '{';
        public const char CLOSE_CURLY_BRACE = '}';
        public const char OPEN_PARENTHESIS = '(';
        public const char CLOSE_PARENTHESIS = ')';
        public const char OPEN_SQUARE_BRACKET = '[';
        public const char CLOSE_SQUARE_BRACKET = ']';
        public const char BANG = '!';
        public const char QUESTION_MARK = '?';
        public const char PERIOD = '.';
        public const char COMMA = ',';
        public const char HASHTAG = '#';
        public const char DOLLARS = '$';
        public const char PERCENT = '%';
        public const char AMPERSAND = '&';
        public const char CARET = '^';
        public const char STAR = '*';
        public const char PLUS = '+';
        public const char SEMICOLON = ';';
        public const char EQUAL = '=';
        public const char AT = '@';
        public const char DASH = '-';
        public const char UNDER_SCORE = '_';
        public const char ACCENT = '`';
        public const char TILDE = '~';
        public const char PIPE = '|';
        public const char COLON = ':';
        public const char DOUBLE_QUOTE = '\"';
        public const char SINGLE_QUOTE = '\'';
        public const char OPEN_ARROW_BRACKET = '<';
        public const char END_ARROW_BRACKET = '>';
        public const char FORWARD_SLASH = '/';
        public const char BACK_SLASH = '\\';

        public static readonly ImmutableArray<char> AllBag = new char[]
        {
            OPEN_CURLY_BRACE,
            CLOSE_CURLY_BRACE,
            OPEN_PARENTHESIS,
            CLOSE_PARENTHESIS,
            OPEN_SQUARE_BRACKET,
            CLOSE_SQUARE_BRACKET,
            BANG,
            QUESTION_MARK,
            PERIOD,
            COMMA,
            HASHTAG,
            DOLLARS,
            PERCENT,
            AMPERSAND,
            CARET,
            STAR,
            PLUS,
            SEMICOLON,
            EQUAL,
            AT,
            DASH,
            // TODO: Should 'PunctuationCharacters.UNDER_SCORE' count as punctuation? It makes expand selection (double mouse click) on a private field with the leading '_' convention annoying to select the whole word.
            // PunctuationCharacters.UNDER_SCORE,
            ACCENT,
            TILDE,
            PIPE,
            COLON,
            DOUBLE_QUOTE,
            SINGLE_QUOTE,
            OPEN_ARROW_BRACKET,
            END_ARROW_BRACKET,
            FORWARD_SLASH,
            BACK_SLASH,
        }.ToImmutableArray();
    }

    public static class MovementKeys
    {
        public const string ARROW_LEFT = "ArrowLeft";
        public const string ARROW_DOWN = "ArrowDown";
        public const string ARROW_UP = "ArrowUp";
        public const string ARROW_RIGHT = "ArrowRight";
        public const string HOME = "Home";
        public const string END = "End";
    }

    public static class AlternateMovementKeys
    {
        public const string ARROW_LEFT = "h";
        public const string ARROW_DOWN = "j";
        public const string ARROW_UP = "k";
        public const string ARROW_RIGHT = "l";
    }
}