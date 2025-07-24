namespace Vala;

public class Namespace : ISymbol
{
    public List<Enum> Enums { get; } = [];
    public string Identifier { get; }
    
    // TODO: Vala namespace shouldn't know about Khronos stuff
    public Namespace (KhronosRegisty.Namespace khrNamespace)
    {
        Identifier = khrNamespace.Name;
        
        foreach (var en in khrNamespace.Enums)
        {
            var enName = en.Name ?? en.CName;
            // if (enName == null)
            // {
            //     enName = en.Name[(khrNamespace.Prefix.Length+1) ..];
            // }
            var valaEn = new Enum(enName);
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
            valaEn.Attributes.Add(new CCode(){cprefix = en.Prefix!.ToUpper()});
            Enums.Add(valaEn);
        }
    }

    public string AsSource()
    {
        var res = $"namespace {Identifier} {{\n";
        foreach (var en in Enums)
        {
            res += $"{en.AsSource()}\n";
        }
        res += "}\n";
        
        return res;
    }
}
