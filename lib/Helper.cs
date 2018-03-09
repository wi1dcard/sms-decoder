using System;
using System.Linq;

namespace SmsDecoder {
    public static class Helper {
        /// <summary>
        /// convert hex string to byte array
        /// </summary>
        /// <param name="hex">eg: "FFFF"</param>
        /// <returns>eg: [255, 255]</returns>
        public static byte[] StringToByteArray (string hex) {
            return Enumerable.Range (0, hex.Length)
                .Where (x => x % 2 == 0)
                .Select (x => Convert.ToByte (hex.Substring (x, 2), 16))
                .ToArray ();
        }

        /// <summary>
        /// one-key decode pdu data from byte array to <see cref="Message"/>
        /// </summary>
        /// <param name="rawPduData"></param>
        /// <returns></returns>
        public static Message Decode (byte[] rawPduData) {
            var p = SmsDecoder.PDU.Parse (rawPduData);
            var m = SmsDecoder.Message.Decode (p);
            return m;
        }

        /// <summary>
        /// one-key decode pdu data from hex string to <see cref="Message"/>
        /// </summary>
        /// <param name="hexPduString"></param>
        /// <returns></returns>
        public static Message Decode (string hexPduString) {
            return Helper.Decode (Helper.StringToByteArray (hexPduString));
        }
    }
}