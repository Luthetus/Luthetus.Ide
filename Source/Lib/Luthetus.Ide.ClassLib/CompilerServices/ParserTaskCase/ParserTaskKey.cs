namespace Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;

public record ParserTaskKey(Guid Guid)
{
    public static readonly ParserTaskKey Empty = new(Guid.Empty);

    public static ParserTaskKey NewParserTaskKey()
    {
        return new(Guid.NewGuid());
    }
}