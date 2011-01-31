using System.Xml.Serialization;

namespace x10sharp
{
    public class X10FunctionCommand : X10Command
    {
        public X10FunctionCommand()
        {
        }

        public X10FunctionCommand(FunctionCode function)
        {
            this.Function = function;
        }

        public X10FunctionCommand(FunctionCode function, byte value)
        {
            this.Function = function;
            this.Value = value;
        }

        [XmlAttribute]
        public FunctionCode Function { get; set; }

        [XmlAttribute]
        public byte Value { get; set; }
    }
}
