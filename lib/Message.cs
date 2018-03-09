using System;
using System.Linq;
using SmsDecoder.Decoder;

namespace SmsDecoder {
    /// <summary>
    /// message class contains a decoded sms
    /// </summary>
    public class Message {
        internal Message () {

        }

        /// <summary>
        /// an address(number) of sms center
        /// </summary>
        public string smscNumber;

        /// <summary>
        /// an address(number) of sms sender
        /// </summary>
        public string senderNumber;

        /// <summary>
        /// timestamp of sms center, this value should be sms sent time of local timezone by default
        /// </summary>
        public DateTime dateTime;

        /// <summary>
        /// sms content, maybe a part
        /// </summary>
        public string content;

        /// <summary>
        /// for long sms which be split, this value will be true
        /// </summary>
        public bool isSplit;

        /// <summary>
        /// unique identifier of long sms which be split
        /// </summary>
        public string splitId;

        /// <summary>
        /// number of parts that long sms split into
        /// </summary>
        public int splitCount;

        /// <summary>
        /// index of split long sms, when <see cref="isSplit"/> is true
        /// </summary>
        public int splitIndex;

        /// <summary>
        /// reverse bytes, for example: 0x0F0E -> 0xF0E0
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        protected string byteReverse (byte[] num) {
            string hex = BitConverter.ToString (num).Replace ("-", "");
            var reversed = Enumerable.Range (0, hex.Length)
                .Where (x => x % 2 == 0)
                .Select (x => hex.Substring (x + 1, 1) + hex.Substring (x, 1));
            return String.Join ("", reversed).Replace ("F", "");
        }

        /// <summary>
        /// get all bits of a number
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// decode message from parsed pdu data
        /// </summary>
        /// <param name="pdu"></param>
        /// <returns></returns>
        public static Message Decode (PDU pdu) {
            var message = new Message ();
            IDecoder msgDecoder;
            var esBits = message.getBits (pdu.encodingScheme);
            //if (!esBits[6] && !esBits[7]) {
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
            //} else {
            //    throw new Exception ("Unknown PDU Message Encoding Scheme!");
            //}
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