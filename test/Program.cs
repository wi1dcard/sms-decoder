using System;
using System.IO;

namespace test {
    class Program {
        /// <summary>
        /// event on a message receive completed
        /// </summary>
        /// <param name="msg"></param>
        public static void onCompleteMsg (SmsDecoder.Message msg) {
            Console.WriteLine ("recv msg from : +{0}, content : {1}", msg.senderNumber, msg.content);
        }

        static void Main (string[] args) {
            var pool = new SmsDecoder.MessagePool (Program.onCompleteMsg);

            foreach (var s in args) {
                pool.Add (SmsDecoder.Helper.Decode (s));
            }

            Console.WriteLine ("Waiting for PDU hex string...");
            do {
                string sms = Console.ReadLine ();
                if (string.IsNullOrWhiteSpace (sms)) {
                    break;
                }
                pool.Add (SmsDecoder.Helper.Decode (sms));
            } while (true);
        }
    }
}