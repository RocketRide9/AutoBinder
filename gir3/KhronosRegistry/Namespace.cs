namespace KhronosRegisty;

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

    public List<Enum> Enums { get; } = [];

    public Namespace (string prefix, string name)
    {
        Prefix = prefix;
        Name = name;
    }

    public void AddEnum (Enum en)
    {
        en.ApplyNamespacePrefix (Prefix);
        Enums.Add(en);
    }
}
