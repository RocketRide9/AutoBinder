using System.Text.Json;

namespace ParserGuides;

public static class ParserGuides
{
    static Guides? _guides = null;
    public static Guides Guides {
        get {
            if (_guides == null)
            {
                string jsonString = File.ReadAllText("../../../ParserGuides/Guides.json");
                _guides = JsonSerializer.Deserialize<Guides>(jsonString)!;
            }

            return _guides;
        }
    }
}