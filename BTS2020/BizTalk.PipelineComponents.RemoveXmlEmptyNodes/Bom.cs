namespace na.removeXmlEmptyNodes
{
    using System;
    using System.IO;
    using System.Linq;

    public static class Bom
    {
        public static int GetCursor(Stream stream)
        {
            byte[] match = new byte[4];
            match[2] = 0xfe;
            match[3] = 0xff;
            if (IsMatch(stream, match))
            {
                return 4;
            }
            byte[] buffer2 = new byte[4];
            buffer2[0] = 0xff;
            buffer2[1] = 0xfe;
            if (IsMatch(stream, buffer2))
            {
                return 4;
            }
            byte[] buffer3 = new byte[] { 0xfe, 0xff };
            if (IsMatch(stream, buffer3))
            {
                return 2;
            }
            byte[] buffer4 = new byte[] { 0xff, 0xfe };
            if (IsMatch(stream, buffer4))
            {
                return 2;
            }
            byte[] buffer5 = new byte[] { 0xef, 0xbb, 0xbf };
            return (!IsMatch(stream, buffer5) ? 0 : 3);
        }

        private static bool IsMatch(Stream stream, byte[] match)
        {
            stream.Position = 0L;
            byte[] buffer = new byte[match.Length];
            stream.Read(buffer, 0, buffer.Length);
            return !buffer.Where<byte>((t, i) => (t != match[i])).Any<byte>();
        }
    }
}

