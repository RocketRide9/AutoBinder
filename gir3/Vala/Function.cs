using System.Collections.Generic;

namespace Vala
{
    public class Function : ISymbol
    {
        string returnType;
        public string Identifier => name;
        public string CName => name;
        string name;
        List<FuncParameter> parameters;

        public Function(string returnType, string name, List<FuncParameter> parameters)
        {
            this.returnType = returnType;
            this.name = name;
            this.parameters = parameters;
        }

        public override string ToString()
        {
            return string.Format(
                "public {0} {1}(\n{2}\n);\n",
                returnType,
                name,
                string.Join(", \n", parameters.Select(p => "    " + p))
            );
        }

        public string AsSource()
        {
            throw new NotImplementedException();
        }
    }

    public class FuncParameter
    {
        string type;
        string name;
        Dictionary<string, string> typeConversion = GL.Core.TypeMap;

        public FuncParameter(string type, string name)
        {
            if (typeConversion.ContainsKey(type))
            {
                this.type = typeConversion[type];
            }
            else
            {
                this.type = type;
            }

            this.name = name;
        }

        public override string ToString()
        {
            return $"{type} {name}";
        }
    }
}
