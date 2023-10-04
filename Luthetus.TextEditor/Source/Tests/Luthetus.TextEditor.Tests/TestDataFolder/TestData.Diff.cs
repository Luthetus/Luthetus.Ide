namespace Luthetus.TextEditor.Tests.TestDataFolder;

public static partial class TestData
{
    /// <summary>
    /// The input for test cases are grouped. These groups are for organizational purposes.
    /// </summary>
    public static class Diff
    {
        /// <summary>
        /// Input without any newline characters belong under this group.
        /// Input of a newline character followed by no further content belongs under this group.
        /// </summary>
        public static class SingleLineBaseCases
        {
            public static class NoLineEndings
            {
                public const string SAMPLE_000 = "abcdefk";
                public const string SAMPLE_010 = "bhdefck";
            }

            public static class WithLinefeedEnding
            {
                public const string SAMPLE_000 = "abcdefk\n";
                public const string SAMPLE_010 = "bhdefck\n";
            }

            public static class WithCarriageReturnEnding
            {
                public const string SAMPLE_000 = "abcdefk\r";
                public const string SAMPLE_010 = "bhdefck\r";
            }

            public static class WithCarriageReturnLinefeedEnding
            {
                public const string SAMPLE_000 = "abcdefk\r\n";
                public const string SAMPLE_010 = "bhdefck\r\n";
            }
        }

        /// <summary>
        /// Input with at least one newline character which is followed by any content (including another line ending character) afterwards belong under this group.
        /// </summary>
        public static class MultiLineBaseCases
        {
            public static class WithLinefeedEnding
            {
                public const string SAMPLE_000 = "abcdefk\nzabc";
                public const string SAMPLE_010 = "bhdefck\nzabc";
            }

            public static class WithCarriageReturnEnding
            {
                public const string SAMPLE_000 = "abcdefk\rzabc";
                public const string SAMPLE_010 = "bhdefck\rzabc";
            }

            public static class WithCarriageReturnLinefeedEnding
            {
                public const string SAMPLE_000 = "abcdefk\r\nzabc";
                public const string SAMPLE_010 = "bhdefck\r\nzabc";
            }

            public static class ManyEmptyLinesWithEqualLengthInput
            {
                public const string SAMPLE_000 = "\n\n\n\n\n";
                public const string SAMPLE_010 = "\n\n\n\n\n";
            }

            public static class ManyEmptyLinesWithUnequalLengthInput
            {
                public const string SAMPLE_000 = "\n\n\n";
                public const string SAMPLE_010 = "\n\n\n\n\n";
            }
        }

        /// <summary>
        /// <see cref="JustifiedCases"/> are made up from more complex input. An example could be an input does not provide a valid output and had been reported as a bug. That input which had the bug output should exist here for eternity, always to be asserted that the bug is still fixed.
        /// </summary>
        public static class JustifiedCases
        {
            /// <summary>
            /// Expected output:
            ///     Longest common subsequence: "bdefk"
            /// Observed output:
            ///     Longest common subsequence: "b"
            /// Resolved output:
            ///     Longest common subsequence: "bdefk\r"
            /// </summary>
            public static class Bug_000
            {
                public const string SAMPLE_000 = "abcdefk\r";
                public const string SAMPLE_010 = "bhdefck\r\n";
            }

            /// <summary>
            /// Expected output:
            ///     Longest common subsequence: "bdefk\n"
            /// Observed output:
            ///     Longest common subsequence: "b\n"
            /// Resolved output:
            ///     Longest common subsequence: "bdefk\n"
            /// </summary>
            public static class Bug_010
            {
                public const string SAMPLE_000 = "abcdefk\n\n\n\n";
                public const string SAMPLE_010 = "bhdefck\n";
            }

            /// <summary>
            /// Expected output:
            ///     Longest common subsequence: "Mary had a little lamb,\nIts fleece was white as snow..."
            /// Observed output:
            ///     Longest common subsequence: "Mary had a little lamb,\n"
            /// Resolved output:
            ///     Longest common subsequence: TODO
            /// </summary>
            public static class Bug_020
            {
                public const string SAMPLE_000 = "Mary had a little lamb,\nIts fleece was white as snow...";
                public const string SAMPLE_010 = "Mary had a little lamb,\nIts fleece was white as snow...";
            }

            /// <summary>
            /// Expected output:
            ///     Longest common subsequence: "Mary had a little lamb, white as snow..."
            /// Observed output:
            ///     Longest common subsequence: "Mary had a little lamb, white as snow..."
            ///     UserInterface: The after is rendering a presentation over the entire width of the third row. Yet there is no third row.
            /// Resolved output:
            ///     Longest common subsequence: TODO
            ///     UserInterface: TODO
            /// </summary>
            public static class Bug_030
            {
                public const string SAMPLE_000 = "Mary had a little lamb,\nwhite as snow...";
                public const string SAMPLE_010 = "Mary had a little lamb,\nIts fleece was white as snow...";
            }
        }
    }
}