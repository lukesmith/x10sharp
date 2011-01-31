using System;
using System.Collections;
using System.IO.Ports;

namespace x10sharp.Controllers
{
    public class CM12UController : IX10Controller, IDisposable
    {
        private SerialPort serialPort = new SerialPort();
        private bool isReady;

        public event EventHandler<EventArgs> ControllerReady;

        public event EventHandler<EventArgs> CommandSent;

        public event EventHandler<DataEventArgs> DataReceived;

        /// <summary>
        /// States whether the controller is ready to send a command
        /// </summary>
        public bool IsReady
        {
            get { return this.isReady; }
        }

        public static byte[] CreateExtendedCommand(X10ExtendedCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            byte[] toSend = new byte[4];
            toSend[0] = CreateHeader(true, true, true, 0);
            toSend[1] = CreateFunctionCode(command.House, FunctionCode.ExtendedCode);
            toSend[2] = command.Data;
            toSend[3] = command.Command;

            return toSend;
        }

        public byte[] CreateCommand(X10Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            byte[] buffer = new byte[1];

            if (typeof(X10AddressCommand) == command.GetType())
            {
                X10AddressCommand addressCommand = command as X10AddressCommand;

                buffer = new byte[2];
                buffer[0] = CreateHeader(false, false, true, 0);
                buffer[1] = CreateAddressCode(addressCommand.House, addressCommand.Device);
            }
            else if (typeof(X10FunctionCommand) == command.GetType())
            {
                X10FunctionCommand functionCommand = command as X10FunctionCommand;

                buffer = new byte[2];
                buffer[0] = CreateHeader(false, true, true, functionCommand.Value);
                buffer[1] = CreateFunctionCode(functionCommand.House, functionCommand.Function);
            }
            else if (typeof(X10ExtendedCommand) == command.GetType())
            {
                buffer = CreateExtendedCommand((X10ExtendedCommand)command);
            }

            return buffer;
        }

        public bool SendCommand(X10Command command)
        {
            byte[] buffer = this.CreateCommand(command);

            int numberOfRetries = 0;
            bool transmittedSuccessfully = false;
            bool success = false;

            // wait for the interface to send a polling
            //////while (true)
            ////{
            ////    //if (_serialPort.ReadByte() == 0x5a)
            ////    {
            ////        //  break;
            ////    }
            ////}

            // send acknowledgement to stop the interface polling
            this.Send(0xc3);

            do
            {
                this.Send(buffer);

                byte response = (byte)this.serialPort.ReadByte();

                if (response == CreateChecksum(buffer))
                {
                    // send acknowledgement
                    transmittedSuccessfully = true;

                    this.DoCommandSent(EventArgs.Empty);

                    this.Send(0x00);

                    success = true;
                }
                else
                {
                    // invalid checksum, retransmit
                    numberOfRetries++;
                }
            }
            while (numberOfRetries <= 1 && !transmittedSuccessfully);

            this.isReady = false;

            // wait for the controller to be ready before sending the next command
            while (!this.IsReady)
            {
            }

            return success;
        }

        public bool SendCommand(X10CommandSet commandSet)
        {
            if (!this.SendCommand(commandSet.Address))
            {
                return false;
            }

            foreach (X10FunctionCommand command in commandSet.Functions)
            {
                if (!this.SendCommand(command))
                {
                    return false;
                }
            }

            return true;
        }

        public bool RunMacro(Macro macro)
        {
            foreach (X10CommandSet command in macro.Commands)
            {
                if (!this.SendCommand(command))
                {
                    return false;
                }
            }

            return true;
        }

        public void Configure(ConnectionSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (this.serialPort != null && !this.serialPort.IsOpen)
            {
                this.serialPort = new SerialPort(settings.Port, settings.BaudRate, settings.Parity, settings.DataBits, settings.StopBits);
                this.serialPort.DataReceived += this._serialPort_DataReceived;

                this.serialPort.Open();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected void DoCommandSent(EventArgs e)
        {
            if (this.CommandSent != null)
            {
                this.CommandSent(this, e);
            }
        }

        protected void DoControllerReady(EventArgs e)
        {
            this.isReady = true;

            if (this.ControllerReady != null)
            {
                this.ControllerReady(this, e);
            }
        }

        protected void DoDataReceived(DataEventArgs e)
        {
            if (this.DataReceived != null)
            {
                this.DataReceived(this, e);
            }
        }

        /// <summary>
        /// Creates the address code <see cref="byte"/> to send to the controller
        /// </summary>
        /// <param name="houseCode"><see cref="HouseCode"/></param>
        /// <param name="deviceCode"><see cref="DeviceCode"/></param>
        /// <returns><see cref="byte"/> to send</returns>
        private static byte CreateAddressCode(HouseCode houseCode, DeviceCode deviceCode)
        {
            // create code
            BitArray result = new BitArray(8);
            BitArray ba = new BitArray(new byte[] { (byte)deviceCode });
            result[3] = ba[3];
            result[2] = ba[2];
            result[1] = ba[1];
            result[0] = ba[0];

            ba = new BitArray(new byte[] { (byte)houseCode });
            result[7] = ba[3];
            result[6] = ba[2];
            result[5] = ba[1];
            result[4] = ba[0];

            byte[] byteArray = new byte[(int)Math.Ceiling((double)result.Length / 8)];
            result.CopyTo(byteArray, 0);

            return byteArray[0];
        }

        /// <summary>
        /// Creates the function code <see cref="byte"/> to send to the controller
        /// </summary>
        /// <param name="houseCode"><see cref="HouseCode"/></param>
        /// <param name="functionCode"><see cref="FunctionCode"/></param>
        /// <returns><see cref="byte"/> to send</returns>
        private static byte CreateFunctionCode(HouseCode houseCode, FunctionCode functionCode)
        {
            BitArray result = new BitArray(8);
            BitArray ba = new BitArray(new[] { (byte)functionCode });
            result[3] = ba[3];
            result[2] = ba[2];
            result[1] = ba[1];
            result[0] = ba[0];

            ba = new BitArray(new[] { (byte)houseCode });
            result[7] = ba[3];
            result[6] = ba[2];
            result[5] = ba[1];
            result[4] = ba[0];

            byte[] byteArray = new byte[(int)Math.Ceiling((double)result.Length / 8)];
            result.CopyTo(byteArray, 0);

            return byteArray[0];
        }

        /// <summary>
        /// Creates the X10 header.
        /// </summary>
        /// <param name="isExtendedTransmission">if set to <c>true</c> [is extended transmission].</param>
        /// <param name="isFunction">if set to <c>true</c> [is function].</param>
        /// <param name="maintainSynchronisation">if set to <c>true</c> [maintain synchronisation].</param>
        /// <param name="dimValue">The dim value, used for dimming a lamp module.</param>
        /// <returns></returns>
        private static byte CreateHeader(bool isExtendedTransmission, bool isFunction, bool maintainSynchronisation, int dimValue)
        {
            BitArray ba = new BitArray(8, false);
            
            if (isExtendedTransmission)
            {
                ba[0] = true;
            }

            if (isFunction)
            {
                ba[1] = true;
            }

            if (maintainSynchronisation)
            {
                ba[2] = true; // Bit 2 is always set to '1' to ensure that the interface is able to maintain synchronization.
            }

            byte[] b = { Convert.ToByte(dimValue) };
            var bb = new BitArray(b);
            ba[7] = bb[4];
            ba[6] = bb[3];
            ba[5] = bb[2];
            ba[4] = bb[1];
            ba[3] = bb[0];

            byte[] byteArray = new byte[(int)Math.Ceiling((double)ba.Length / 8)];
            ba.CopyTo(byteArray, 0);

            return byteArray[0];
        }

        /// <summary>
        /// Creates the checksum.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>Checksum <see cref="byte"/> value</returns>
        private static int CreateChecksum(byte[] buffer)
        {
            byte num1 = 0;
            for (int num3 = 0; num3 < buffer.Length; num3++)
            {
                byte num2 = buffer[num3];
                num1 = (byte)(num1 + num2);
            }

            return num1 & 0xff;
        }

        private void Send(byte buffer)
        {
            this.Send(new byte[] { buffer });
        }

        private void Send(byte[] buffer)
        {
            if (this.serialPort.IsOpen)
            {
                this.serialPort.DiscardInBuffer();
                this.serialPort.Write(buffer, 0, buffer.Length);
            }
            else
            {
                throw new System.IO.IOException("Port is not open");
            }
        }

        ////private byte[] WaitForResponse()
        ////{
        ////    while (_received)
        ////    {
        ////        int num1 = _serialPort.BytesToRead;
        ////        byte[] buffer2 = new byte[num1];
        ////        _serialPort.Read(buffer2, 0, num1);
                
        ////        _received = false;

        ////        return new byte[] { buffer2[num1 - 2], buffer2[num1 - 1] };
        ////    }

        ////    return null;
        ////}

        ////private bool _received;

        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                if (this.serialPort.IsOpen)
                {
                    this.serialPort.Close();
                }
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = sender as SerialPort;

            byte buffer = (byte)sp.ReadByte();
            this.DoDataReceived(new DataEventArgs(buffer));

            if (buffer == 0x55)
            {
                // controller is ready
                this.DoControllerReady(EventArgs.Empty);
            }

            ////_received = true;
        }
    }
}
