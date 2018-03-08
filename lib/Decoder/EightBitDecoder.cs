using System;
using System.Text;

namespace SmsDecoder.Decoder {
    public class EightBitDecoder : IDecoder {
        public string Decode (byte[] raw) =>
            throw new Exception ("Cannot decode User-defined coding!");
            
    }
}