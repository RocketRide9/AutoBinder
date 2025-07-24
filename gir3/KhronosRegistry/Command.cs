namespace KhronosRegisty;

public class Command
{
    public Type ReturnType;
    public string Identifier;
    public CommandParam[] Params;
}

public class CommandParam
{
    public Type Type;
    public string Name;
}
