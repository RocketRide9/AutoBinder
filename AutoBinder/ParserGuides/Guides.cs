using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
public class EnumCommentGuides (
    string? action = null,
    string? name = null,
    string? cname = null,
    string? parentType = null,
    List<AttributeDescription>? attributes = null
) {
    // Possible values: Parse, Skip
    public string Action { get; set; } = action ?? "Parse";
    public string? Name { get; set; } = name;
    public string? CName { get; set; } = cname;
    public string? ParentType { get; set; } = parentType;
    public List<AttributeDescription>? Attributes { get; set; } = attributes;
}

public class TypeGuide
{
    // HACK: "Each parameter in the deserialization
    // constructor on type must bind to an object property
    // or field on deserialization"
    public string MatchPattern { get; }
    public Regex MatchRegex { get; }
    // Possible values: Parse, Skip
    public string Action { get; }
    public string Transparent { get; }

    [JsonConstructor]
    public TypeGuide (string matchPattern, string? action, string? transparent)
    {
        MatchPattern = matchPattern;
        MatchRegex = new(matchPattern);
        Action = action ?? "Parse";
        Transparent = transparent ?? "false";
    }
}

public class Guides
{
    public Dictionary<string, EnumCommentGuides> EnumCommentGuides { get; }

    public Overrides Overrides { get; }

    public List<TypeGuide> TypeGuides { get; }

    [JsonConstructor]
    public Guides (
        Dictionary<string, EnumCommentGuides> enumCommentGuides,
        Overrides overrides,
        List<TypeGuide> typeGuides
    ) {
        (EnumCommentGuides, Overrides) = (enumCommentGuides, overrides);
        TypeGuides = typeGuides;
    }

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
    public string CName { get; }
    public string Name { get; }

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