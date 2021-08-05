using System;
using System.Text;

namespace Commons.Cryptography {
    public static class HexTk {
        /* the number of byte values defined. */

        private const int ByteCount = 1 + byte.MaxValue - byte.MinValue;

        /* the mapping table using for hex decoding of the right half of a hex number. */

        private static readonly byte[] UnhexRight = new byte[ByteCount];

        /* the mapping table using for hex decoding of the left half of a hex number. */

        private static readonly byte[] UnhexLeft = new byte[ByteCount];

        private static readonly char[] HexDigits =
            {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        static HexTk() {
            // Set the mapping for the characters for the right half
            for (var i = 0; i < UnhexRight.Length; i++) {
                UnhexRight[i] = byte.MaxValue;
            }

            UnhexRight['0'] = 0x00;
            UnhexRight['1'] = 0x01;
            UnhexRight['2'] = 0x02;
            UnhexRight['3'] = 0x03;
            UnhexRight['4'] = 0x04;
            UnhexRight['5'] = 0x05;
            UnhexRight['6'] = 0x06;
            UnhexRight['7'] = 0x07;
            UnhexRight['8'] = 0x08;
            UnhexRight['9'] = 0x09;
            UnhexRight['a'] = 0x0a;
            UnhexRight['b'] = 0x0b;
            UnhexRight['c'] = 0x0c;
            UnhexRight['d'] = 0x0d;
            UnhexRight['e'] = 0x0e;
            UnhexRight['f'] = 0x0f;
            UnhexRight['A'] = 0x0A;
            UnhexRight['B'] = 0x0B;
            UnhexRight['C'] = 0x0C;
            UnhexRight['D'] = 0x0D;
            UnhexRight['E'] = 0x0E;
            UnhexRight['F'] = 0x0F;

            // the left half is a shift left of the right half
            for (var i = 0; i < UnhexRight.Length; i++) {
                UnhexLeft[i] = (byte) (UnhexRight[i] << 4);
            }
        }

        /// <summary>
        ///     generates a hexdump of the specified byte array, starting at the specified offset,
        ///     for the specified number of bytes, and without any decorations.
        /// </summary>
        /// <param name="source"> the byte array to be dumped. </param>
        /// <param name="offset"> the index where to begin the dump. </param>
        /// <param name="dataLen">
        ///     the number of bytes to be dumped. If the byte array contains less bytes than specified, only the
        ///     available bytes are dumped.
        /// </param>
        /// <returns> a String containing the hexdump of the byte array. </returns>
        public static string HexDump(
            byte[] source,
            int offset,
            int dataLen) {
            var sb = new StringBuilder();
            if (null != source) {
                var lastPos = Math.Min(source.Length, offset + dataLen);
                for (var pos = offset; pos < lastPos; pos++) {
                    sb.Append(source[pos].ToString("X2"));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     generates a hexdump of the specified bytes without any decorations.
        /// </summary>
        /// <param name="source"> the byte array to be dumped. </param>
        /// <returns> a String containing the hexdump of the byte array. </returns>
        public static string HexDump(byte[] source) {
            return null != source ? HexDump(source, 0, source.Length) : null;
        }

        /// <summary>
        ///     transform a hexdump into the corresponding byte array.
        /// </summary>
        /// <param name="hexDump"> a hexdump, i.e. a String that contains an even number of characters [0-9A-Fa-f]. </param>
        /// <returns>
        ///     the byte array represented by the hexdump, or a byte array of zero length if the Hexdump String argument is
        ///     <i>null</i> .
        /// </returns>
        public static byte[] UnHex(string hexDump) {
            if (null == hexDump) {
                return new byte[0];
            }

            if (0 != hexDump.Length % 2) {
                throw new ArgumentException(string.Format(AesResources.HexDump_OddLength, hexDump, hexDump.Length));
            }

            var hexChar = hexDump.ToCharArray();
            var unHex = new byte[hexChar.Length / 2];
            for (int i = 0, j = 0; i < hexChar.Length; i++, j++) {
                var left = UnhexLeft[hexChar[i]];
                var right = UnhexRight[hexChar[++i]];
                if (byte.MaxValue == right) {
                    throw new ArgumentException(string.Format(AesResources.HexDump_UnallowedCharacter, hexDump,
                        hexChar[i], i));
                }

                unHex[j] = (byte) (left | right);
            }

            return unHex;
        }

        public static string RandomHexString(int length) {
            var random = new Random();
            var numChars = new char[2 * length];
            for (var i = 0; i < 2 * length; i++) {
                var index = random.Next(0, HexDigits.Length);
                numChars[i] = HexDigits[index];
            }

            return new string(numChars);
        }
    }
}