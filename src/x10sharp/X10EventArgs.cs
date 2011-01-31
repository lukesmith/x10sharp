using System;
using System.Collections.ObjectModel;

namespace x10sharp
{
    public class X10EventArgs : EventArgs
    {
        private readonly byte[] buffer = new byte[0];

        public X10EventArgs(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public ReadOnlyCollection<byte> Buffer
        {
            get { return new ReadOnlyCollection<byte>(this.buffer); }
        }
    }
}
