namespace SmsDecoder.Decoder {
    public interface IDecoder {
        string Decode (byte[] raw);
    }
}