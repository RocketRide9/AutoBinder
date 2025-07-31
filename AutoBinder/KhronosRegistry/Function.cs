using System.Text.RegularExpressions;

class Function
{
    static (string type, uint stars, string name) ParseArgument(string arg)
    {
        var reg = new Regex(@"^\s*(?:const )?\s*([ \w]+?)\s*([\* ]+?)\s*(\w+)\s*$");

        var match = reg.Match(arg);

        var type = match.Captures[1].Value;
        var stars = (uint)match.Captures[2].Value.Count(c => c == '*');
        var name = match.Captures[3].Value;

        return (type, stars, name);
    }
}