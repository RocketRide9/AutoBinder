using System.Xml.Linq;

namespace KhronosRegistry;

public interface IType {}

public class OpaqueType : IType
{
    public string CName { get; }
    public string Name { get; set; }

    internal OpaqueType (string cname, string name)
    {
        CName = cname;
        Name = name;
    }
}

public class Typedef : IType
{
    public string CName { get; }
    public string Name { get; set; }
    public IType BaseType { get; }
    public bool Transparent { get; }

    internal Typedef (string cname, string name, IType baseType, bool transparent)
    {
        CName = cname;
        Name = name;
        BaseType = baseType;
        Transparent = transparent;
    }

    public override int GetHashCode()
    {
        return CName.GetHashCode();
    }
}

public class BasicType : IType
{
    public string Name { get; }

    internal BasicType (string name)
    {
        Name = name;
    }
}

public class Ptr : IType
{
    public enum PtrType
    {
        Out,
        In,
        Unknown
    }

    public string Target { get; }

    public PtrType Type { get; }


    internal Ptr (string target, PtrType ptrType = PtrType.Unknown)
    {
        Target = target;
        Type = ptrType;
    }
}

class TypeCollection
{
    // HACK: BasicTypes need their own namespace
    // TODO: Still not sure if void should be included
    // "void",
    public static HashSet<string> BasicTypes => [
        "char", // i8
        "int",  // i32
        "unsigned char", // u8
        "unsigned int",  // u32
        "intptr_t", // isize - ptr size
        "size_t",   // isize - index size
        "float",    // f32
        "double",   // f64

        "int8_t",   // i8
        "int16_t",  // i16
        "int32_t",  // i32
        "int64_t",  // i64
        "uint8_t",  // u8
        "uint16_t", // u16
        "uint32_t", // u32
        "uint64_t", // u64
    ];

    HashSet<string> OpaqueTypes { get; } = [];
    Dictionary<string, string> TypeDefs { get; } = [];

    public TypeCollection() {}

    public void Register(XElement typedef)
    {
        var type = typedef.Element("type")?.Value;
        var name = typedef.Element("name")?.Value;

        var guides = ParserGuides.ParserGuides.Guides;

        if (name == null)
        {
            // conditional constant definition
            // TODO: handle it
            name = typedef.Attribute("name")!.Value;
            return;
        }

        foreach (var item in guides.TypeGuides)
        {
            if (item.MatchRegex.IsMatch(name))
            {
                if (item.Action == "Skip")
                {
                    return;
                }

                break;
            }
        }

        if (type == null || type == "void")
        {
            OpaqueTypes.Add(name);
        } else {
            TypeDefs.Add(name, type);
        }
    }

    // Name -> Type
    public Dictionary<string, IType> GetTypes(string nsPrefix)
    {
        var res = new Dictionary<string, IType> ();

        foreach (var opCName in OpaqueTypes)
        {
            // TODO: deduplicate
            string name;
            if (opCName.StartsWith(nsPrefix + "_")) {
                name = opCName[(nsPrefix.Length + 1)..];
            } else {
                name = opCName[nsPrefix.Length..];
            }
            res.Add(name, new OpaqueType(opCName, name));
        }

        foreach (var (tdCName, baseCName) in TypeDefs)
        {
            string tdname;
            if (tdCName.StartsWith(nsPrefix + "_")) {
                tdname = tdCName[(nsPrefix.Length + 1)..];
            } else {
                tdname = tdCName[nsPrefix.Length..];
            }

            string baseName;
            if (BasicTypes.Contains(baseCName))
            {
                baseName = baseCName;
            } else if (baseCName.StartsWith(nsPrefix + "_")) {
                baseName = baseCName[(nsPrefix.Length + 1)..];
            } else {
                baseName = baseCName[nsPrefix.Length..];
            }

            var transparent = false;
            var guides = ParserGuides.ParserGuides.Guides;
            foreach (var item in guides.TypeGuides)
            {
                transparent |= item.MatchRegex.IsMatch(tdCName);
            }

            // HACK: How basic types are handled is a big smelly hack.
            // There are a lot of assumptions that symbol names won't collide
            if (res.TryGetValue(baseName, out var val))
            {
                res.Add(tdname, new Typedef (tdCName, tdname, val, transparent));
            } else if (BasicTypes.Contains(baseName))
            {
                res.Add(tdname, new Typedef (tdCName, tdname, new BasicType (baseName), transparent));
            }
        }

        return res;
    }
}
