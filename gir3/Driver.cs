using System.Xml.Linq;

class Driver {
    static void Main() {
        var doc = XDocument.Load("../../../../specs/cl.xml");
        var root = doc.Root!;
        var res = "";

        var reg = new KhronosRegisty.Registry(root);
        var srcWrite = new Vala.SourceWriter();
        Directory.CreateDirectory("../../../../vapi");
        var ostream = new StreamWriter(
            "../../../../vapi/cl.vapi",
            new FileStreamOptions (){
                Mode = FileMode.OpenOrCreate,
                Access = FileAccess.Write,
            }
        );
        srcWrite.Write(ostream, reg);
        ostream.Write(res);
        ostream.Close();
    }    
}
