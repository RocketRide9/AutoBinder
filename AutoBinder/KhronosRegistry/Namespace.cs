namespace KhronosRegistry;

public class Namespace
{
    /// <summary>
    /// Namespace prefix found in function, type and constants names.
    /// Lowercase single word string.
    /// 
    /// E.g. cl, gl, egl, vk.
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// Namespace name. Bindings can use it or
    /// uppercased Prefix for namespace indentificator.
    /// 
    /// E.g. OpenCL, OpenGL, EGL, Vulkan.
    /// </summary>
    public string Name { get; }

    public Dictionary<string, IType> Types { get; set; } = [];

    public List<Enum> Enums { get; } = [];

    public Namespace (string prefix, string name)
    {
        Prefix = prefix;
        Name = name;
    }

    public void AddEnum (Enum en)
    {
        en.ApplyNamespacePrefix (Prefix);

        var verified = false;
        foreach (var item in Types)
        {
            if (en.Name == item.Key)
            {
                verified = true;
                break;
            }
        }

        if (!verified)
        {
            verified = TypeCollection.BasicTypes.Contains(en.CName);
        }

        if (!verified)
        {
            throw new Exception($"Couldn't find type such type: {en.Name}");
        }
        Enums.Add(en);
    }
}
