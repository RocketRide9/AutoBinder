using System.Text.Json;
using System.Text.Json.Serialization;

namespace ParserGuides;

public static class ParserGuides
{
    static Guides? _guides = null;
    public static Guides Guides {
        get {
            if (_guides == null)
            {
                string jsonString = File.ReadAllText("../../../ParserGuides/Guides.json");
                var opts = new JsonSerializerOptions() {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                _guides = JsonSerializer.Deserialize<Guides>(jsonString, opts)!;
            }

            return _guides;
        }
    }
}