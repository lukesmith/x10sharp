using System.Xml.Serialization;

namespace x10sharp
{
    public abstract class X10Command
    {
        private HouseCode houseCode = HouseCode.A;
        
        [XmlIgnore]
        public HouseCode House
        {
            get { return this.houseCode; }
            set { this.houseCode = value; }
        }
    }
}