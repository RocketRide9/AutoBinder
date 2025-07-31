using System.Text.Json;

namespace Vala;

/*
public class EnumBuilder
{
    public List<EnumMember> Members { get; set; } = [];
    public string? Identifier { get; set; }
    public string? CPrefix { get; set; }
    public string? CName { get; set; }
    public bool IsFlags { get; set; } = false;

    public EnumBuilder() {}

    public Enum Build()
    {
        if (CName == null)
        {
            throw new InvalidOperationException("CName have to be always specified");
        }

        // Only cname was specified
        // Try to generate a good Identifier
        Identifier ??= Strings.Converter.SnakeToPascal(CName!);

        
    } 
}
 */

public class Enum : ISymbol
{
    public List<EnumMember> Members { get; } = [];
    public string Identifier { get; }
    public List<IAttribute> Attributes { get; init; } = [];

    public Enum(string identifier)
    {
        Identifier = identifier;
    }
    
    public void Add(params IEnumerable<EnumMember> members)
    {
        Members.AddRange(members);
    }
    
    public string AsSource()
    {
        string res = "";
        foreach (var a in Attributes)
        {
            res += a.ToSource() + "\n";
        }
        res += $"public enum {Identifier}";
        res += " {\n";
        
        foreach (var a in Members)
        {
            foreach (var attr in a.Attributes)
            {
                res += "    " + attr.ToSource() + "\n";
            }
            res += $"    {a.Name} = {a.Value},\n";
        }

        res += "}\n";

        return res;
    }
}

public class EnumMember
{
    public string Name { get; }
    public string Value { get; }
    public List<IAttribute> Attributes { get; init; } = [];

    public EnumMember(string name, string value)
    {
        (Name, Value) = (name, value);
    }
}
