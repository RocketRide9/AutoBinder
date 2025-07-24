using System.Text.Json.Serialization;

namespace ParserGuides;

[method: JsonConstructor]
public class AttributeDescription (
    string name,
    Dictionary<string, string>? properties
)
{
    public string Name { get; } = name;
    public Dictionary<string, string>? Properties { get; } = properties;

}

[method: JsonConstructor]
public class EnumCommentHint (
    string? name,
    string? cname,
    string? parentType,
    List<AttributeDescription>? attributes
) {
    public string? Name { get; } = name;
    public string? CName { get; } = cname;
    public string? ParentType { get; } = parentType;
    public List<AttributeDescription>? Attributes { get; } = attributes;
}

public class Guides
{
    public Dictionary<string, EnumCommentHint> EnumCommentHints { get; }

    public Overrides Overrides { get; }

    [JsonConstructor]
    public Guides (Dictionary<string, EnumCommentHint> enumCommentHints, Overrides overrides)
        => (EnumCommentHints, Overrides) = (enumCommentHints, overrides);

    public List<Enum> FindEnums (string memberName)
    {
        var res = new List<Enum>();
        foreach (var en in Overrides.Enums)
        {
            foreach (var mem in en.Members)
            {
                if (mem == memberName)
                {
                    res.Add(en);
                }
            }
        }

        return res;
    }
}

public class Overrides
{
    public List<Enum> Enums { get; }
    public List<EnumMember> EnumMembers { get; }

    [JsonConstructor]
    public Overrides (List<Enum> enums, List<EnumMember> enumMembers)
        => (Enums, EnumMembers) = (enums, enumMembers);
}

public class EnumMember
{
    public string CName { get;}
    public string Name { get;}

    [JsonConstructor]
    public EnumMember (string cname, string name)
    {
        CName = cname;
        Name = name;
    }
}

public class Enum
{
    public string Name { get;}
    public string CName { get;}
    public List<string> Members { get;}

    [JsonConstructor]
    public Enum (string name, string cname, List<string> members)
        => (Name, CName, Members) = (name, cname, members);
}