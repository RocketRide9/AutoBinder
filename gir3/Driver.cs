using System.Xml.Linq;

class Driver {
    static void Main() {
        var doc = XDocument.Load("../../../../specs/cl.xml");
        var root = doc.Root!;
        var res = "";

        var reg = new KhronosRegisty.Registry(root);
        var srcWrite = new Vala.SourceWriter();
        var ostream = new StreamWriter("../../../../vapi/cl.vapi");
        srcWrite.Write(ostream, reg);
        ostream.Write(res);
        ostream.Close();
    }    
}
