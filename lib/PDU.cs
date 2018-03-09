using System;
using System.IO;

namespace SmsDecoder {
    /// <summary>
    /// pdu parsed data, for more: doc/sms_pdu-mode.pdf
    /// </summary>
    public class PDU {
        private PDU () {

        }

        public byte smscByteLength;
        public byte smscType;
        public byte[] smscNumber;
        public byte pduHeader;
        public bool includedDataHeader {
            get {
                return (pduHeader & 0x40) == 0x40;
            }
        }
        public byte senderLength;
        public byte senderByteLength {
            get {
                return (byte) (senderLength % 2 == 0 ? senderLength / 2 : senderLength / 2 + 1);
            }
        }
        public byte senderType;
        public byte[] senderNumber;
        public byte protocalId;
        public byte encodingScheme;
        public byte[] timeStamp;
        public byte dataLength;
        public DataHeader dataHeader;
        public byte[] data;

        public class DataHeader {
            public byte length;
            public byte type;
            public byte splitLength;
            public byte[] splitId;
            public byte splitCount;
            public byte splitIndex;
        }

        public static PDU Parse (byte[] rawPdu) {
            PDU parsed;
            using (var ms = new MemoryStream (rawPdu)) {
                parsed = PDU.Parse (ms);
            }
            return parsed;
        }
        
        public static PDU Parse (Stream rawPdu) {
            PDU pdu = new PDU ();
            rawPdu.Position = 0;
            pdu.smscByteLength = (byte) rawPdu.ReadByte ();
            pdu.smscType = (byte) rawPdu.ReadByte ();
            pdu.smscNumber = new byte[pdu.smscByteLength - 1];
            rawPdu.Read (pdu.smscNumber, 0, pdu.smscByteLength - 1);

            pdu.pduHeader = (byte) rawPdu.ReadByte ();
            pdu.senderLength = (byte) rawPdu.ReadByte ();
            pdu.senderType = (byte) rawPdu.ReadByte ();
            pdu.senderNumber = new byte[pdu.senderByteLength];
            rawPdu.Read (pdu.senderNumber, 0, pdu.senderByteLength);

            pdu.protocalId = (byte) rawPdu.ReadByte ();
            pdu.encodingScheme = (byte) rawPdu.ReadByte ();
            pdu.timeStamp = new byte[7];
            rawPdu.Read (pdu.timeStamp, 0, 7);

            pdu.dataLength = (byte) rawPdu.ReadByte ();
            var dataHeader = pdu.dataHeader = new DataHeader ();
            int dataLengthExcludeHeader = pdu.dataLength;
            if (pdu.includedDataHeader) {
                dataHeader.length = (byte) rawPdu.ReadByte ();
                dataHeader.type = (byte) rawPdu.ReadByte ();
                dataHeader.splitLength = (byte) rawPdu.ReadByte ();
                dataHeader.splitId = new byte[dataHeader.splitLength - 2];
                rawPdu.Read (dataHeader.splitId, 0, dataHeader.splitLength - 2);
                dataHeader.splitCount = (byte) rawPdu.ReadByte ();
                dataHeader.splitIndex = (byte) rawPdu.ReadByte ();
                dataLengthExcludeHeader -= dataHeader.length + 1;
            }
            pdu.data = new byte[dataLengthExcludeHeader];
            rawPdu.Read (pdu.data, 0, dataLengthExcludeHeader);

            return pdu;
        }
    }
}