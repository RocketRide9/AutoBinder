using System.Collections.Generic;

namespace GL {
    public static class Core {
        static public Dictionary<string, string> TypeMap = new (){
            ["GLuint"] = "uint",
            ["GLsizei"] = "int",
            ["GLboolean"] = "bool",
            ["GLbitfield"] = "uint",
        };
    }
}
