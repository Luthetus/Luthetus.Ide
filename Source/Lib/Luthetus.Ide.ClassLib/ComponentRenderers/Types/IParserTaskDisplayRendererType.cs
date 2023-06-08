using Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;

namespace Luthetus.Ide.ClassLib.ComponentRenderers.Types;

public interface IParserTaskDisplayRendererType
{
    public IParserTask ParserTask { get; set; }
}