using System.Xml.Linq;

class Driver {
    static void Main() {
        var doc = XDocument.Load(
            "../../../../specs/cl.xml",
            LoadOptions.PreserveWhitespace
        );
        var root = doc.Root!;
        var res = "";

        var reg = new KhronosRegistry.Registry(root);
        var srcWrite = new Vala.SourceWriter();
        Directory.CreateDirectory("../../../../vapi");
        var ostream = new StreamWriter(
            "../../../../vapi/cl.vapi",
            new FileStreamOptions (){
                Mode = FileMode.OpenOrCreate | FileMode.Truncate,
                Access = FileAccess.Write,
            }
        );
        srcWrite.Write(ostream, reg);
        ostream.Write(res);
        ostream.Close();
    }    
}
