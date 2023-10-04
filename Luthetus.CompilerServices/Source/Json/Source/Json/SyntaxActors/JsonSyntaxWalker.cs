using Luthetus.CompilerServices.Lang.Json.Json.SyntaxEnums;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxObjects;

namespace Luthetus.CompilerServices.Lang.Json.Json.SyntaxActors;

public class JsonSyntaxWalker
{
    public List<JsonPropertyKeySyntax> JsonPropertyKeySyntaxes { get; } = new();
    public List<JsonBooleanSyntax> JsonBooleanSyntaxes { get; } = new();
    public List<JsonIntegerSyntax> JsonIntegerSyntaxes { get; } = new();
    public List<JsonNullSyntax> JsonNullSyntaxes { get; } = new();
    public List<JsonNumberSyntax> JsonNumberSyntaxes { get; } = new();
    public List<JsonStringSyntax> JsonStringSyntaxes { get; } = new();

    public void Visit(IJsonSyntax jsonSyntax)
    {
        // TODO: Trying to update the text editor demo. Am getting an exception about null references. Revisit this after finished.
        if (jsonSyntax?.ChildJsonSyntaxes is null)
            return;

        foreach (var child in jsonSyntax.ChildJsonSyntaxes)
            Visit(child);

        switch (jsonSyntax.JsonSyntaxKind)
        {
            case JsonSyntaxKind.PropertyKey:
                VisitJsonPropertyKeySyntax((JsonPropertyKeySyntax)jsonSyntax);
                break;
            case JsonSyntaxKind.Boolean:
                VisitJsonBooleanSyntax((JsonBooleanSyntax)jsonSyntax);
                break;
            case JsonSyntaxKind.Integer:
                VisitJsonIntegerSyntax((JsonIntegerSyntax)jsonSyntax);
                break;
            case JsonSyntaxKind.Null:
                VisitJsonNullSyntax((JsonNullSyntax)jsonSyntax);
                break;
            case JsonSyntaxKind.Number:
                VisitJsonNumberSyntax((JsonNumberSyntax)jsonSyntax);
                break;
            case JsonSyntaxKind.String:
                VisitJsonStringSyntax((JsonStringSyntax)jsonSyntax);
                break;
        }
    }

    private void VisitJsonPropertyKeySyntax(JsonPropertyKeySyntax jsonPropertyKeySyntax)
    {
        JsonPropertyKeySyntaxes.Add(jsonPropertyKeySyntax);
    }

    private void VisitJsonBooleanSyntax(JsonBooleanSyntax jsonBooleanSyntax)
    {
        JsonBooleanSyntaxes.Add(jsonBooleanSyntax);
    }

    private void VisitJsonIntegerSyntax(JsonIntegerSyntax jsonIntegerSyntax)
    {
        JsonIntegerSyntaxes.Add(jsonIntegerSyntax);
    }
    private void VisitJsonNullSyntax(JsonNullSyntax jsonNullSyntax)
    {
        JsonNullSyntaxes.Add(jsonNullSyntax);
    }

    private void VisitJsonNumberSyntax(JsonNumberSyntax jsonNumberSyntax)
    {
        JsonNumberSyntaxes.Add(jsonNumberSyntax);
    }

    private void VisitJsonStringSyntax(JsonStringSyntax jsonStringSyntax)
    {
        JsonStringSyntaxes.Add(jsonStringSyntax);
    }

}