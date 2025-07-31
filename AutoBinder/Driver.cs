using System.Xml.Linq;

class Driver {
    static void Main() {
        var doc = XDocument.Load(
            "../../../../specs/cl.xml",
            LoadOptions.PreserveWhitespace
        );
        var root = doc.Root!;
        var reg = new KhronosRegistry.Registry(root);

        Directory.CreateDirectory("../../../../vapi");
        File.CreateText("../../../../vapi/cl.vapi").Close();
        var ostream = new StreamWriter(
            "../../../../vapi/cl.vapi"
        );

        var srcWrite = new Vala.SourceWriter();
        srcWrite.Write(ostream, reg);
        ostream.Close();
    }    
}
