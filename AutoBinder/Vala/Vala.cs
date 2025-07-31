namespace Vala;


public interface ISymbol
{
    string Identifier { get; }

    /// <summary>
    /// </summary>
    /// <returns>
    /// Vala source of this object
    /// </returns>
    string AsSource();
}

public class SourceWriterContext
{
    public string Namespace { get; }
}
