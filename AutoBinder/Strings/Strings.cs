namespace Strings;

static class Converter
{
    public static string SnakeToPascal(string snake)
    {
        var parts = snake
            .Split('_')
            .Select(str => char.ToUpper(str[0]) + str[1..]);
        return string.Concat(parts);
    }
}