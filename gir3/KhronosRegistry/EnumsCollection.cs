namespace KhronosRegisty;

public class EnumsCollection
{
    // Name -> Enum
    public Dictionary<string, Enum> Enums { get; set; } = [];

    public EnumMember FindMember (string pairName)
    {
        foreach (var en in Enums)
        {
            var member = en.Value.Find (pairName);
            if (member != null)
            {
                return member;
            }
        }

        throw new Exception ($"Couldn't find member with name: {pairName}");
    }

    public void AddEnumIfNotExists (Enum en)
    {
        if (Enums.TryGetValue (en.Name!, out _))
        {
            return;
        }

        Enums.Add (en.Name!, en);
    }

    public void AddMember (string enumName, EnumMember member)
    {
        if (Enums.TryGetValue (enumName, out var en))
        {
            en.AddMembers (member);
        } else {
            throw new ArgumentException($"{enumName} doesn't exist in this collection");
        }
    }
}