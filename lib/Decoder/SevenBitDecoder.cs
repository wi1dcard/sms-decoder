using System.IO;
using System.Text;

namespace SmsDecoder.Decoder {
    public class SevenBitDecoder : IDecoder {
        public string Decode (byte[] raw) {
            var deRaw = this.decompress (raw);
            return Encoding.ASCII.GetString (deRaw);
        }

        public byte[] Encode (string str) {
            var raw = Encoding.ASCII.GetBytes (str);
            return this.compress (raw);
        }

        protected byte[] decompress (byte[] raw) {
            byte leftBitsLen = 0;
            byte leftBits = 0;
            byte[] deBytes;
            byte sevenBits = 0;
            using (MemoryStream ms = new MemoryStream ()) {
                foreach (var b in raw) {
                    sevenBits = (byte) (((b << leftBitsLen) | leftBits) & 0x7F);
                    ms.WriteByte (sevenBits);
                    leftBits = (byte) (b >> (7 - leftBitsLen));
                    leftBitsLen++;
                    if (leftBitsLen == 7) {
                        ms.WriteByte (leftBits);
                        leftBits = 0;
                        leftBitsLen = 0;
                    }
                }
                ms.Position = 0;
                deBytes = new byte[ms.Length];
                ms.Read (deBytes, 0, (int) ms.Length);
            }
            return deBytes;
        }

        protected byte[] compress (byte[] raw) {
            int i = 0;
            byte sevenBits = 0;
            byte leftBits = 0;
            byte[] enBytes;
            using (MemoryStream ms = new MemoryStream ()) {
                foreach (var b in raw) {
                    sevenBits = (byte) (i & 7);
                    if (sevenBits == 0) {
                        leftBits = b;
                    } else {
                        byte eightBits = (byte) ((b << (8 - sevenBits)) | leftBits);
                        ms.WriteByte (eightBits);
                        leftBits = (byte) (b >> sevenBits);
                    }
                    i++;
                }
                ms.WriteByte (leftBits);
                enBytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read (enBytes, 0, (int) ms.Length);
            }
            return enBytes;
        }
    }
}