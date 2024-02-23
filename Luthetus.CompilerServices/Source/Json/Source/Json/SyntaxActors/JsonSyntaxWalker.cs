using Luthetus.CompilerServices.Lang.Json.Json.SyntaxEnums;
using Luthetus.CompilerServices.Lang.Json.Json.SyntaxObjects;

namespace Luthetus.CompilerServices.Lang.Json.Json.SyntaxActors;

public class JsonSyntaxWalker
{
    public List<JsonPropertyKeySyntax> PropertyKeySyntaxes { get; } = new();
    public List<JsonBooleanSyntax> BooleanSyntaxes { get; } = new();
    public List<JsonIntegerSyntax> IntegerSyntaxes { get; } = new();
    public List<JsonNullSyntax> NullSyntaxes { get; } = new();
    public List<JsonNumberSyntax> NumberSyntaxes { get; } = new();
    public List<JsonStringSyntax> StringSyntaxes { get; } = new();

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
        PropertyKeySyntaxes.Add(jsonPropertyKeySyntax);
    }

    private void VisitJsonBooleanSyntax(JsonBooleanSyntax jsonBooleanSyntax)
    {
        BooleanSyntaxes.Add(jsonBooleanSyntax);
    }

    private void VisitJsonIntegerSyntax(JsonIntegerSyntax jsonIntegerSyntax)
    {
        IntegerSyntaxes.Add(jsonIntegerSyntax);
    }
    private void VisitJsonNullSyntax(JsonNullSyntax jsonNullSyntax)
    {
        NullSyntaxes.Add(jsonNullSyntax);
    }

    private void VisitJsonNumberSyntax(JsonNumberSyntax jsonNumberSyntax)
    {
        NumberSyntaxes.Add(jsonNumberSyntax);
    }

    private void VisitJsonStringSyntax(JsonStringSyntax jsonStringSyntax)
    {
        StringSyntaxes.Add(jsonStringSyntax);
    }

}