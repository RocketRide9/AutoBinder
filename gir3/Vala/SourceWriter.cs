namespace Vala;

public class SourceWriter
{
    public void Write(StreamWriter writer, KhronosRegisty.Registry registry)
    {
        var namesp = new Namespace(registry.Namespace);
        writer.Write(namesp.AsSource());
    }
}
