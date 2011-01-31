using System.Xml.Serialization;

namespace x10sharp
{
    public class X10AddressCommand : X10Command
    {
        private DeviceCode deviceCode = DeviceCode.One;

        public X10AddressCommand()
        {
        }

        public X10AddressCommand(HouseCode houseCode, DeviceCode deviceCode)
        {
            this.House = houseCode;
            this.Device = deviceCode;
        }

        [XmlAttribute]
        public DeviceCode Device
        {
            get { return this.deviceCode; }
            set { this.deviceCode = value; }
        }
    }
}
