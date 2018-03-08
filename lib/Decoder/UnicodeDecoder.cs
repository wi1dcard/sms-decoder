using System.IO;
using System.Text;

namespace SmsDecoder.Decoder {
    public class UnicodeDecoder : IDecoder {
        public string Decode (byte[] raw) {
            return Encoding.BigEndianUnicode.GetString (raw);
        }

        public byte[] Encode (string str) {
            return Encoding.BigEndianUnicode.GetBytes (str);
        }

    }
}