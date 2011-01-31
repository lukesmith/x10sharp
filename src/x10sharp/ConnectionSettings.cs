using System.IO.Ports;

namespace x10sharp
{
    public class ConnectionSettings
    {
        private string port;
        private int baudRate = 4800;
        private int dataBits = 8;
        private StopBits stopBits = StopBits.One;
        private Parity parity = Parity.None;
        private Handshake handshaking = Handshake.None;

        public ConnectionSettings()
        {
        }

        public ConnectionSettings(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            this.port = portName;
            this.baudRate = baudRate;
            this.parity = parity;
            this.dataBits = dataBits;
            this.stopBits = stopBits;
        }

        public string Port
        {
            get { return this.port; }
            set { this.port = value; }
        }

        public int BaudRate
        {
            get { return this.baudRate; }
            set { this.baudRate = value; }
        }

        public int DataBits
        {
            get { return this.dataBits; }
            set { this.dataBits = value; }
        }

        public StopBits StopBits
        {
            get { return this.stopBits; }
            set { this.stopBits = value; }
        }

        public Parity Parity
        {
            get { return this.parity; }
            set { this.parity = value; }
        }

        public Handshake Handshaking
        {
            get { return this.handshaking; }
            set { this.handshaking = value; }
        }
    }
}
