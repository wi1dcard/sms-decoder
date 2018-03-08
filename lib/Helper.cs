using System;
using System.Linq;

namespace SmsDecoder {
    public static class Helper {
        public static byte[] StringToByteArray (string hex) {
            return Enumerable.Range (0, hex.Length)
                .Where (x => x % 2 == 0)
                .Select (x => Convert.ToByte (hex.Substring (x, 2), 16))
                .ToArray ();
        }

        public static Message Decode (byte[] rawPduData) {
            var p = SmsDecoder.PDU.Parse (rawPduData);
            var m = SmsDecoder.Message.Decode (p);
            return m;
        }

        public static Message Decode (string hexPduString) {
            return Helper.Decode (Helper.StringToByteArray (hexPduString));
        }
    }
}