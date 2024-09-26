namespace Luthetus.CompilerServices.Python.Facts;

public partial class PythonLanguageFacts
{
    public class Whitespace
    {
        public const char CARRIAGE_RETURN_CHAR = '\r';
        public const char LINE_FEED_CHAR = '\n';

        public string CARRIAGE_RETURN_LINE_FEED_SUBSTRING =>
            $"{CARRIAGE_RETURN_CHAR}{LINE_FEED_CHAR}";
    }
}