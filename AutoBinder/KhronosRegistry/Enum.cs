using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace KhronosRegistry;

// Name - pair name as present in .xml spec
// e.g. CL_PLATFORM_PROFILE from cl_platform_info
// tho PlatformInfo.Profile makes more sense for bindings
public class EnumMember
{
    /// <summary>
    /// Full name according to spec
    /// </summary>
    public string CName { get; }
    public string Value { get; }
    /// <summary>
    /// Short name, CName without prefix.
    /// </summary>
    public string? Name { get; set; }
    public Enum? Parent { get; set; }

    public bool IsCNameOverriden ()
    {
        return CName != Parent!.Prefix! + Name;
    }

    public EnumMember (string cname, string value)
    {
        CName = cname;
        Value = value;
    }

    public EnumMember (EnumMember other)
    {
        CName = other.CName;
        Value = other.Value;
        Name = other.Name;
    }
}

public class Enum 
{

    public string CName { get; set; }
    public string Name { get; set; }
    public string? Vendor { get; set; }
    public string? Comment { get; set; }
    public bool IsBitmask { get; set; }

    List<EnumMember> _members = [];
    public IReadOnlyList<EnumMember> Members {
        get => _members.AsReadOnly ();
    }

    public void AddMembers(params IEnumerable<EnumMember> members)
    {
        foreach (var m in members)
        {
            m.Parent = this;
            _members.Add(m);
        }
    }

    /// <summary>
    /// Prefix that identifies this enumeration.
    /// Includes namespace prefix and trailing `_`
    /// </summary>
    [DisallowNull]
    public string? Prefix {
        get => _prefix; 
        set {
            _prefix = value.ToUpper();

            foreach (var m in Members)
            {
                m.Name ??= m.CName[_prefix.Length..];
            }
        }
    }
    string? _prefix;
    public string NamespacePrefix { get; set; }

    public Enum(string cname, string? name, string nsPrefix) {
        CName = cname;
        NamespacePrefix = nsPrefix;
        if (name == null)
        {
            Name ??= CName[(NamespacePrefix.Length+1)..];
        } else {
            Name = name;
        }
    }
    
    [SetsRequiredMembers]
    public Enum(XElement node, string nsPrefix)
    {
        CName = node.Attribute("name")?.Value!;
        Vendor = node.Attribute("vendor")?.Value!;
        Comment = node.Attribute("comment")?.Value;
        // HACK: Name doesn't matter in this case. Just anything
        Name = CName;
        NamespacePrefix = nsPrefix;

        foreach (var en in node.Elements())
        {
            switch (en.Name.LocalName)
            {
                case "enum":
                    string val;
                    string name;
                    name = en.Attribute("name")!.Value;

                    IsBitmask = node.Attribute("type")?.Value == "bitmask";

                    val = en.Attribute("value")?.Value ?? "1<<" + en.Attribute("bitpos")!.Value;

                    
                    AddMembers(new EnumMember(name, val));
                    break;
                case "unused":
                case "comment":
                    // just skip it
                    break;
                default:
                    throw new Exception($"Unhandled enum element: {en.Name}");
            }
        }
    }
    
    public void ApplyNamespacePrefix (string nsPrefix)
    {
        // It's a time come up with a better name for an enum
        // if it doesn't already have one
        if (Name == null)
        {
            var nName = CName;

            Name = Strings.Converter.SnakeToPascal(nName[(nsPrefix.Length+1)..]);
        }

        // Find common prefix of members
        var common = nsPrefix + "_";
        var end = false;

        if (Members.Count <= 1)
        {
            // Can't find common prefix if there aren't enough
            // members in enum
            Prefix = common;
            return;
        }

        // find some member without canonical name.
        // it will be used to get next letters
        string path = "";
        foreach (var m in Members)
        {
            if (m.Name == null)
            {
                path = m.CName;
                break;
            }
        }

        if (path == "")
        {
            // Members already have names
            Prefix = common;
            return;
        }

        while (true)
        {
            var underIndex = path.IndexOf('_', common.Length);
            if (underIndex < 0)
            {
                // no '_' left
                break;
            }
            var nc = common + path[common.Length .. (underIndex + 1)];

            foreach (var m in Members)
            {
                if (m.Name != null)
                {
                    // member has a name
                    continue;
                }

                if (!m.CName.StartsWith(nc, StringComparison.InvariantCultureIgnoreCase))
                {
                    end = true;
                    break;
                }
            }

            if (end)
            {
                break;
            }

            common = nc;
        }

        Prefix = common;
    }

    // find a pair with given name
    // if found, return value
    // else return null
    public EnumMember? Find(string pairName)
    {
        foreach (var a in Members)
        {
            if (a.CName == pairName)
            {
                return a;
            }
        }

        return null;
    }
}

public class EnumBuilder
{
    
}
