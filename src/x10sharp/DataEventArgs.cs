using System;

namespace x10sharp
{
    public class DataEventArgs : EventArgs
    {
        private readonly byte val;

        public DataEventArgs(byte val)
        {
            this.val = val;
        }

        public byte Value
        {
            get { return this.val; }
        }
    }
}