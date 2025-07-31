using System.Diagnostics;
using KhronosRegistry;

namespace Vala;

public class Namespace : ISymbol
{
    public Dictionary<string, SimpleType> SimpleTypes { get; } = [];
    public Dictionary<string, Enum> Enums { get; } = [];
    public string Identifier { get; }
    
    // Transparent typedef might point on another transparent
    // typedef and so on...
    // This function returns resulting typename
    static string UnTypedef (Typedef typed)
    {
        return typed.BaseType switch
        {
            BasicType basic => BasicTypesConversion[basic.Name],
            OpaqueType opaque => Strings.Converter.SnakeToPascal (opaque.Name),
            Typedef typed2 when  typed2.Transparent => UnTypedef (typed2),
            Typedef typed2 when !typed2.Transparent => Strings.Converter.SnakeToPascal (typed2.Name),
            _ => throw new UnreachableException(),
        };
    }

    public static Dictionary<string, string> BasicTypesConversion => new (){
        ["char"]            = "char",
        ["int"]             = "int",   
        ["unsigned char"]   = "uchar",  
        ["unsigned int"]    = "uint",   
        ["intptr_t"]        = "intptr",  
        ["size_t"]          = "size_t",    
        ["float"]           = "float",     
        ["double"]          = "float",    

        ["int8_t"]          = "int8",    
        ["int16_t"]         = "int16",   
        ["int32_t"]         = "int32",   
        ["int64_t"]         = "int64",   
        ["uint8_t"]         = "uint8",   
        ["uint16_t"]        = "uint16",  
        ["uint32_t"]        = "uint32",  
        ["uint64_t"]        = "uint64",  
    };

    // TODO: Vala namespace shouldn't know about Khronos stuff
    public Namespace (KhronosRegistry.Namespace khrNamespace)
    {
        Identifier = khrNamespace.Name;
        
        foreach (var en in khrNamespace.Enums)
        {
            // if (enName == null)
            // {
            //     enName = en.Name[(khrNamespace.Prefix.Length+1) ..];
            // }
            var valaEn = new Enum(Strings.Converter.SnakeToPascal(en.Name));
            valaEn.Add(
                en.Members
                .Select(
                    m => {
                        var attrs = new List<IAttribute> ();
                        if (m.IsCNameOverriden ())
                        {
                            attrs.Add (new CCode() {
                                cname = m.CName
                            });
                        }
                        var res = new EnumMember(m.Name!, m.Value) {
                            Attributes = attrs
                        };
                        return res;
                    }
                )
            );
            valaEn.Attributes.Add(
                new CCode() {
                    cprefix = en.Prefix!.ToUpper(),
                    cname = en.CName,
                }
            );
            Enums.Add(valaEn.Identifier, valaEn);
        }

        foreach (var (khrName, khrType) in khrNamespace.Types)
        {
            switch (khrType)
            {
                case OpaqueType t:
                    var name = Strings.Converter.SnakeToPascal(khrName);
                    var simple = new SimpleType (name);
                    simple.Attributes.Add(
                        new CCode() {
                            cname = t.CName
                        }
                    );
                    SimpleTypes.Add(simple.Identifier, simple);
                    break;
                case Typedef t:
                    name = Strings.Converter.SnakeToPascal(khrName);
                    if (t.Transparent)
                    {
                        continue;
                    }
                    var parentType = UnTypedef(t);
                    simple = new SimpleType (name, parentType);
                    simple.Attributes.Add(
                        new CCode() {
                            cname = t.CName
                        }
                    );
                    if (Enums.TryGetValue(simple.Identifier, out var val))
                    {
                        // nothing
                    } else {
                        SimpleTypes.Add(simple.Identifier, simple);
                    }
                    break;
            }
        }

        // foreach (var tpd in khrNamespace.Typedefs)
        // {
        //     var name = Strings.Converter.SnakeToPascal(tpd.Name!);
        //     var simple = new SimpleType (name, tpd.BaseType);
        //     simple.Attributes.Add(
        //         new CCode() {
        //             cname = tpd.CName
        //         }
        //     );

        //     SimpleTypes.Add(simple);
        // }
    }

    public string AsSource()
    {
        var res = $"namespace {Identifier} {{\n\n";

        foreach (var (_, sim) in SimpleTypes)
        {
            res += $"{sim.AsSource()}\n";
        }
        foreach (var (_, en) in Enums)
        {
            res += $"{en.AsSource()}\n";
        }

        res += "}\n";
        
        return res;
    }
}
