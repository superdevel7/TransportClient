namespace CompiledTechnologies.Transport
{
    using System.IO;
    using System.Text;

    public class ValueOutputStream : BinaryWriter
    {
        internal static UnicodeEncoding myEncoding = new UnicodeEncoding(true, false);
        private int theBytesWritten;

        public ValueOutputStream(Stream outp) : base(outp, myEncoding)
        {
            theBytesWritten = 0;
        }

        internal void WritePadded(byte[] b)
        {
            int length = b.Length;
            theBytesWritten += length;
            Write(b);
            while ((length % 4) != 0)
            {
                theBytesWritten++;
                Write((byte)0);
                length++;
            }
        }

        internal void WritePadded(char[] s)
        {
            int length = s.Length;
            theBytesWritten += length * 2;
            Write(s);
            if ((length % 2) != 0)
            {
                theBytesWritten += 2;
                Write('\0');
            }
        }

        internal void WriteR(double d)
        {
            theBytesWritten += 8;
            Write(ValueInputStream.Reverse(d));
        }

        internal void WriteR(int i)
        {
            theBytesWritten += 4;
            Write(ValueInputStream.Reverse(i));
        }

        internal void WriteR(uint i)
        {
            theBytesWritten += 4;
            Write(ValueInputStream.Reverse(i));
        }

        public void WriteValue(string s)
        {
            string str = s;
            int num3 = str.Length;
            WriteR(num3);
            WritePadded(str.ToCharArray());
        }

        public int BytesWritten
        {
            get
            {
                return theBytesWritten;
            }
        }
    }
}
