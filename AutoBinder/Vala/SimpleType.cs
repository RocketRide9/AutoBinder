namespace Vala;

public class SimpleType : ISymbol
{
    public string Identifier { get; }
    public List<IAttribute> Attributes { get; init; } = [];
    public string? ParentType { get; }

    public SimpleType (string name, string? parentType = null)
    {
        Identifier = name;
        ParentType = parentType;
    }

    public string AsSource()
    {
        string res = "";
        res += "[SimpleType]\n";
        foreach (var a in Attributes)
        {
            res += a.ToSource() + "\n";
        }
        res += $"public struct {Identifier}";

        if (ParentType != null)
        {
            res += $" : {ParentType}";
        }

        res += " {}\n";

        return res;
    }
}