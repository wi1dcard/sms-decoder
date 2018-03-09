﻿using System;
using System.IO;

namespace test {
    class Program {
        /// <summary>
        /// event on a message receive completed
        /// </summary>
        /// <param name="msg"></param>
        public static void onCompleteMsg (SmsDecoder.Message msg) {
            Console.WriteLine ("Receiving sms from : +{0}, content : {1}", msg.senderNumber, msg.content);
        }

        static void Main (string[] args) {
            var pool = new SmsDecoder.MessagePool (Program.onCompleteMsg);
            var poolDecode = new Action<string> ((sms) => pool.Add (SmsDecoder.Helper.Decode (sms)));

            foreach (var s in args) {
                poolDecode (s);
            }
            if (args.Length > 0) {
                return;
            }

            Console.WriteLine ("Waiting for PDU hex string...");
            do {
                string sms = Console.ReadLine ();
                if (string.IsNullOrWhiteSpace (sms)) {
                    break;
                }
                poolDecode (sms);
            }
            while (true);
        }
    }
}