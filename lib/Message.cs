using System;
using System.Linq;
using SmsDecoder.Decoder;

namespace SmsDecoder {
    public class Message {
        internal Message () {

        }

        public string smscNumber;
        public string senderNumber;
        public DateTime dateTime;
        public string content;
        public bool isSplit;
        public string splitId;
        public int splitCount;
        public int splitIndex;

        protected string byteReverse (byte[] num) {
            string hex = BitConverter.ToString (num).Replace ("-", "");
            var reversed = Enumerable.Range (0, hex.Length)
                .Where (x => x % 2 == 0)
                .Select (x => hex.Substring (x + 1, 1) + hex.Substring (x, 1));
            return String.Join ("", reversed).Replace ("F", "");
        }

        protected bool[] getBits (object data) {
            var size = System.Runtime.InteropServices.Marshal.SizeOf (data);
            var bits = new bool[size * 8];
            var dataInt = Int64.Parse (data.ToString ());
            for (int i = 0; i < bits.Length; i++) {
                int mask = 1 + (int) Math.Pow (2, i);
                bits[i] = (dataInt & mask) != 0;
            }
            return bits;
        }

        public static Message Decode (PDU pdu) {
            var message = new Message ();
            IDecoder msgDecoder;
            var esBits = message.getBits (pdu.encodingScheme);
            if (!esBits[6] && !esBits[7]) {
                if (esBits[5]) {
                    msgDecoder = new UnicodeDecoder ();
                } else {
                    var bits = Convert.ToByte (esBits[3]) << 1 | Convert.ToByte (esBits[2]);
                    switch (bits) {
                        case 0:
                            msgDecoder = new SevenBitDecoder ();
                            break;
                        case 1:
                            msgDecoder = new EightBitDecoder ();
                            break;
                        case 2:
                            msgDecoder = new UnicodeDecoder ();
                            break;
                        case 3:
                        default:
                            throw new Exception ("Unknown PDU Message Encoding Scheme!");
                    }
                }
            } else {
                throw new Exception ("Unknown PDU Message Encoding Scheme!");
            }
            message.content = msgDecoder.Decode (pdu.data).Trim ('\0');

            message.smscNumber = message.byteReverse (pdu.smscNumber);
            message.senderNumber = message.byteReverse (pdu.senderNumber);

            var timeStampAsString = message.byteReverse (pdu.timeStamp);
            var dateTimeAsString = "20" + timeStampAsString.Substring (0, 12);
            message.dateTime = DateTime.ParseExact (dateTimeAsString, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            // var timeZone = int.Parse (timeStampAsString.Substring (12, 2));
            // if ((timeZone >> 7) == 0) {
            //     message.dateTimeUtc = message.dateTime - new TimeSpan (timeZone / 4, 0, 0);
            // }

            message.isSplit = pdu.dataHeader.length != 0 && pdu.dataHeader.splitLength != 0;
            if (message.isSplit) {
                message.splitCount = pdu.dataHeader.splitCount;
                message.splitIndex = pdu.dataHeader.splitIndex;
                message.splitId = BitConverter.ToString (pdu.dataHeader.splitId);
            }
            return message;
        }
    }
}