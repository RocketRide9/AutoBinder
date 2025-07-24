using System.Xml.Linq;

namespace KhronosRegisty;

public class Registry
{
    List<Type> Types { get; } = [];
    List<Command> Commands { get; } = [];
    
    // 1. Parse enums as they are presented in <enums> elements
    EnumsCollection FirstPassEnums { get; } = new();
    // 2. Parse features to get more info how enums are structured
    // e.g. `CL_MEM_DEVICE_HANDLE_LIST_KHR = 0x2051` is located inside <enums name="enums.2000"...>
    // and by looking at feature we get a more helpful enum identifier hint - `cl_mem_properties` or `MemProperties`
    public EnumsCollection Enums { get; } = new();
    public Namespace Namespace { get; }

    // TODO: remake this so it will scale better
    public Registry(XElement root)
    {
        Namespace = new ("cl", "OpenCL");
        if (root.Name != "registry")
        {
            throw new ArgumentException($"Expected `registry`, got `{root.Name}`");
        }
        
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
        var feat = root.Element("feature")!;
        if (feat.Attribute("name")!.Value != "CL_VERSION_1_0")
        {
            throw new Exception($"Expected `CL_VERSION_1_0`, got {feat.Attribute("name")!.Value}");
        }
        
        foreach (var req in feat.Elements())
        {
            var comment = req.Attribute("comment")?.Value!;
            var guides = ParserGuides.ParserGuides.Guides;

            guides.EnumCommentHints.TryGetValue (comment, out var value);
            var overrideEnName = value?.Name;
            var overrideEnCName = value?.CName;
            
            var bitmaskSuffix = " - bitfield";
            var isBitmask = comment.EndsWith(bitmaskSuffix);
            if (isBitmask)
            {
                comment = comment[..^bitmaskSuffix.Length];
            }

            // assume that require elements have the same name
            // othewise bail out
            if (req.Elements().First().Name == "enum")
            {
                foreach (var el in req.Elements())
                {
                    if (el.Name != "enum")
                    {
                        // bail out
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
                            // TODO: that's probably a hacky way to get
                            // Name of enum after it's has been added to a namespace 
                            // it assumes that namespace won't make a copy
                            var en2 = new Enum () {
                                CName = en.CName, 
                                Name = en.Name, 
                                IsBitmask = isBitmask,
                                NamespacePrefix = Namespace.Prefix
                            };
                            Enums.AddEnumIfNotExists (en2);
                            Enums.AddMember(en2.Name!, new(enMember));
                        }
                    } else {
                        var en2 = new Enum () {
                            CName = overrideEnCName ?? comment,
                            Name = overrideEnName,
                            IsBitmask = isBitmask,
                            NamespacePrefix = Namespace.Prefix
                        };
                        Enums.AddEnumIfNotExists (en2);
                        Enums.AddMember (en2.Name!, enMember);
                    }
                }
            }
        }
        foreach (var (_, en) in Enums.Enums)
        {
            Namespace.AddEnum (en);
        }
    }
}
