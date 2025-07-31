using System.Diagnostics;
using System.Xml.Linq;

namespace KhronosRegistry;

public class Registry
{
    List<Type> Types { get; } = [];
    List<Command> Commands { get; } = [];
    
    // 1. Parse enums as they are presented in <enums> elements
    EnumsCollection FirstPassEnums { get; } = new();
    // 2. Parse features to get more info how enums are structured
    // e.g. `CL_MEM_DEVICE_HANDLE_LIST_KHR = 0x2051` is located inside <enums name="enums.2000"...>
    // and by looking at feature we get a more helpful enum identifier hint - `cl_mem_properties` or `MemProperties`
    EnumsCollection Enums { get; } = new();
    public Namespace Namespace { get; }

    public Registry(XElement root)
    {
        Namespace = new ("cl", "OpenCL");
        if (root.Name != "registry")
        {
            throw new ArgumentException($"Expected `registry`, got `{root.Name}`");
        }

        ParseTypes (root);

        ParseEnums (root);
    }

    void ParseTypes (XElement root)
    {
        var types = new TypeCollection();
        foreach (var ts in root.Elements("types"))
        {
            foreach (var t in ts.Elements("type"))
            {
                if (t.Attribute("category")!.Value == "define")
                {
                    types.Register(t);
                }
            }
        }
        Namespace.Types = types.GetTypes(Namespace.Prefix);
    }

    void ParseFeatureEnums (XElement feat)
    {
        foreach (var req in feat.Elements())
        {
            if (req.Elements().First().Name != "enum")
            {
                // skip non enum <require>
                continue;
            }

            var comment = req.Attribute("comment")?.Value!;
            var guides = ParserGuides.ParserGuides.Guides;

            guides.EnumCommentGuides.TryGetValue (comment, out var over);
            if (over?.Action == "Skip")
            {
                continue;
            }
            var overrideEnName = over?.Name;
            var overrideEnCName = over?.CName;
            
            var bitmaskSuffix = " - bitfield";
            var isBitmask = comment.EndsWith(bitmaskSuffix);
            if (isBitmask)
            {
                comment = comment[..^bitmaskSuffix.Length];
            }
            foreach (var el in req.Elements())
            {
                if (el.Name != "enum")
                {
                    // we assume that elements in <require> have the same type
                    throw new Exception($"expected 'enum', got {el.Name}");
                }

                var memberName = el.Attribute("name")!.Value;
                var enMember = FirstPassEnums.FindMember(memberName);
                
                enMember.Name = guides
                    .Overrides
                    .EnumMembers
                    .Find(v => v.CName == memberName)?
                    .Name;

                var overrideEnums = guides.FindEnums(memberName);
                if (overrideEnums.Count != 0)
                {
                    foreach (var en in overrideEnums)
                    {
                        var en2 = new Enum (en.CName, en.Name, Namespace.Prefix) {
                            IsBitmask = isBitmask,
                        };
                        Enums.AddEnumIfNotExists (en2);
                        Enums.AddMember(en2.Name!, new(enMember));
                    }
                } else {
                    var en2 = new Enum (overrideEnCName ?? comment, overrideEnName, Namespace.Prefix) {
                        IsBitmask = isBitmask
                    };
                    Enums.AddEnumIfNotExists (en2);
                    Enums.AddMember (en2.Name!, enMember);
                }
            }
        }
    }

    void ParseEnums (XElement root)
    {
        // First pass
        foreach (var ens in root.Elements("enums"))
        {
            var en = new Enum(ens, Namespace.Prefix);
            FirstPassEnums.AddEnumIfNotExists(en);
        }

        if (FirstPassEnums.Enums.Count == 0)
        {
            throw new Exception("Didn't find any <enums> elements");
        }

        // Second pass
        var features = root.Elements("feature");
        // if (feat.Attribute("name")!.Value != "CL_VERSION_1_0")
        // {
        //     throw new Exception($"Expected `CL_VERSION_1_0`, got {feat.Attribute("name")!.Value}");
        // }
        
        foreach (var f in features)
        {
            ParseFeatureEnums(f);
        }
        
        foreach (var (_, en) in Enums.Enums)
        {
            Namespace.AddEnum (en);
        }
    }
}
