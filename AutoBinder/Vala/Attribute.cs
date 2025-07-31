namespace Vala;

public interface IAttribute {
    private static string Write(IAttribute attr)
    {
        var type = attr.GetType();
        var props = type.GetProperties();
        var parts = new List<string>();

        foreach (var p in props)
        {
            if (p.GetValue(attr) != null)
            {
                parts.Add($"{p.Name} = \"{p.GetValue(attr)}\"");
            }
        }

        if (parts.Count == 0)
        {
            return $"[{type.Name}]";
        } else {
            var initializer = "(";
            initializer += string.Join(", ", parts);
            initializer += ')';

            return $"[{type.Name} {initializer}]";
        }
    }

    string ToSource()
    {
        return Write(this);
    }
}

public class Flags : IAttribute {}

public class CCode : IAttribute
{
    public string? cname { get; set; }
    public string? cprefix { get; set; }
    public bool? has_type_id { get; set; }
}

public class Version : IAttribute
{
    public string? since { get; set; }
    public string? deprecated_since { get; set; }
    public bool? deprecated { get; set; }
}

