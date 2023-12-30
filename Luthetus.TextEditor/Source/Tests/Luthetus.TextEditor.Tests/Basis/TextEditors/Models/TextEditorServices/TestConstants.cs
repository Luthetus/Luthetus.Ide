using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

public class TestConstants
{
    /// <summary>
    /// This sample text is used for testing.
    /// <br/><br/>
    /// It contains separate lines that start with letter, digit, whitespace, and punctuation
    /// Similarly, separate lines that end with punctuation, letter, whitespace, digit.
    /// <br/><br/>
    /// Note: there is a line which contains only a space character, this note is here to try to
    /// avoid confusion if one does not see it.
    /// <br/><br/>
    /// Do not use a verbatim string here, it will use operating system dependent line endings,
    /// which then cannot be asserted in the unit tests.
    /// </summary>
    public const string SOURCE_TEXT = "Hello World!\n7 Pillows\n \n,abc123";

    public const int NEGATIVE_ROW_INDEX = -4;
    public const int FIRST_ROW_INDEX = 0;
    public const int ROW_INDEX_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW = 1;
    public const int LAST_ROW_INDEX = 3;
    public const int LARGE_OUT_OF_BOUNDS_ROW_INDEX = 5;
    public const int ROW_COUNT = 4;
    public const int LENGTH_OF_FIRST_ROW = 12;
    public const int LENGTH_OF_ROW_WHICH_IS_BETWEEN_FIRST_AND_LAST_ROW = 9;
    public const int LENGTH_OF_LAST_ROW = 7;
}
