using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace x10sharp
{
    public class X10CommandSet
    {
        private readonly List<X10FunctionCommand> functions = new List<X10FunctionCommand>();
        private HouseCode houseCode = HouseCode.A;
        private DeviceCode deviceCode = DeviceCode.One;

        public string DeviceName { get; set; }

        [XmlAttribute]
        public HouseCode HouseCode
        {
            get { return this.houseCode; }
            set { this.houseCode = value; }
        }

        [XmlAttribute]
        public DeviceCode Device
        {
            get { return this.deviceCode; }
            set { this.deviceCode = value; }
        }

        public Collection<X10FunctionCommand> Functions
        {
            get
            {
                foreach (var function in this.functions)
                {
                    function.House = this.HouseCode;
                }

                return new Collection<X10FunctionCommand>(this.functions);
            }
        }

        public X10AddressCommand Address
        {
            get { return new X10AddressCommand(this.HouseCode, this.Device); }
        }
    }
}