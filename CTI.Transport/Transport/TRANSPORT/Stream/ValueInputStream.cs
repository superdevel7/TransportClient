namespace CompiledTechnologies.Transport
{
    using System;
    using System.IO;
    using System.Text;

    public class ValueInputStream : BinaryReader
    {
        internal static UnicodeEncoding myEncoding = new UnicodeEncoding(true, false);
        private int theBytesRead;
        private Connection theConnection;

        public ValueInputStream(Stream inp) : base(inp, myEncoding)
        {
            theBytesRead = 0;
        }

        public ValueInputStream(Stream inp, Connection c) : base(inp, myEncoding)
        {
            theBytesRead = 0;
            theConnection = c;
        }

        internal byte[] ReadBytesPadded(int n)
        {
            theBytesRead += n;
            byte[] buffer = ReadBytes(n);
            while ((n % 4) != 0)
            {
                theBytesRead++;
                ReadByte();
                n++;
            }
            return buffer;
        }

        internal char[] ReadCharsPadded(int n)
        {
            theBytesRead += 2 * n;
            char[] chArray = this.ReadChars(n);
            if ((n % 2) != 0)
            {
                theBytesRead += 2;
                ReadChar();
            }
            return chArray;
        }

        internal double ReadDoubleR()
        {
            theBytesRead += 8;
            return Reverse(ReadDouble());
        }

        internal int ReadInt32R()
        {
            theBytesRead += 4;
            return Reverse(ReadInt32());
        }

        internal uint ReadUInt32R()
        {
            theBytesRead += 4;
            return Reverse(ReadUInt32());
        }

        public string ReadValue()
        {
            try
            {
                return new string(ReadCharsPadded(ReadInt32R()));
            }
            catch (Exception)
            {
                throw new IOException("Invalid Teleport value type code (attempt to read forward failed)");
            }
        }

        internal static unsafe double Reverse(double d)
        {
            byte* numPtr = (byte*)&d;
            byte num = numPtr[0];
            numPtr[0] = numPtr[7];
            numPtr[7] = num;
            num = numPtr[1];
            numPtr[1] = numPtr[6];
            numPtr[6] = num;
            num = numPtr[2];
            numPtr[2] = numPtr[5];
            numPtr[5] = num;
            num = numPtr[3];
            numPtr[3] = numPtr[4];
            numPtr[4] = num;
            return d;
        }

        internal static unsafe int Reverse(int i)
        {
            byte* numPtr = (byte*)&i;
            byte num = numPtr[0];
            numPtr[0] = numPtr[3];
            numPtr[3] = num;
            num = numPtr[1];
            numPtr[1] = numPtr[2];
            numPtr[2] = num;
            return i;
        }

        internal static unsafe uint Reverse(uint i)
        {
            byte* numPtr = (byte*)&i;
            byte num = numPtr[0];
            numPtr[0] = numPtr[3];
            numPtr[3] = num;
            num = numPtr[1];
            numPtr[1] = numPtr[2];
            numPtr[2] = num;
            return i;
        }

        public int BytesRead
        {
            get
            {
                return theBytesRead;
            }
        }
    }
}

