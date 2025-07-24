using System.Xml.Linq;

class Parser {
    public Parser() {}
    int indent_lvl = 0;
    // string res;
    
    string indent {
        get {
            return new string (' ', indent_lvl * 4);
        }
    }
    
    public void parse(ref string res, XElement node) {
        foreach (var el in node.Elements()) {
            if (el.Name == "enums") {
                parse_enum(ref res, el);
            }
            if (el.Name == "commands") {
                parse_commands(ref res, el);
            }
        }
    }
    
    string extract_text(XElement element) {
        return element.Nodes()
            .Where(v => v.NodeType == System.Xml.XmlNodeType.Text)
            .Select(v => (v as XText).Value)
            .Single()
            .Trim();
    }
    
    void parse_commands(ref string res, XElement cmds) {
        foreach (var cmd in cmds.Elements()) {
            var proto = cmd.Element("proto")!;
            var return_type = proto.Element("ptype")?.Value ?? extract_text(proto);
            var name = proto.Element("name")!.Value;
            
            var funcParams = new List<Vala.FuncParameter>();
            foreach (var param in cmd.Elements("param")) {
                funcParams.Add(
                    new (
                        param.Element("ptype")?.Value ?? extract_text(param),
                        param.Element("name")!.Value
                    )
                );
            }
            
            res += new Vala.Function(return_type, name, funcParams);
        }
    }
    
    void parse_enum(ref string res, XElement enums) {
        var has_enum = false;
        foreach (var entry in enums.Elements()) {
            if (entry.Name == "enum") {
                has_enum = true;
            }
        }
        
        if (!has_enum) {
            res += "/* empty enums group */\n";
            
            return;
        }
        
        if (enums.Attribute("group") == null) {
            res += "/*\nunnamed enums group\n";
            if (enums.Attribute("vendor") != null) {
                res += string.Format("vendor: {0}\n", enums.Attribute("vendor")!.Value);
            }
            if (enums.Attribute("comment") != null) {
                res += string.Format("comment: {0}\n", enums.Attribute("comment")!.Value);
            }
        }
        
        var tmp = string.Format( "public enum {0} {{\n", enums.Attribute("group")?.Value ?? "" );
        res += tmp;
        indent_lvl += 1;
        
        
        foreach (var entry in enums.Elements()) {
            if (entry.Name == "enum") {
                res += string.Format( "    {0} = {1},\n", entry.Attribute("name")!.Value, entry.Attribute("value")!.Value );
            }
        }
        indent_lvl -= 1;
        res += "}\n";
        
        if (enums.Attribute("group") == null) {
            res += "*/\n";
        }
    }
}
