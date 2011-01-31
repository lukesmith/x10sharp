namespace x10sharp
{
    public class X10ExtendedCommand : X10Command
    {
        public X10ExtendedCommand()
        {
        }

        public X10ExtendedCommand(HouseCode houseCode)
        {
            this.House = houseCode;
        }

        public byte Data { get; set; }

        public byte Command { get; set; }
    }
}
