﻿using System;
using System.IO;

namespace test {
    class Program {

        public static void onCompleteMsg (SmsDecoder.Message msg) {
            Console.WriteLine ("recv msg from : +{0}, content : {1}", msg.senderNumber, msg.content);
        }

        static void Main (string[] args) {
            var pool = new SmsDecoder.MessagePool (Program.onCompleteMsg);

            string[] mstr = new string[] {
                "07915892208800F0040B915892241883F800009921810170002B0341E211",
                "0791448720003023240DD0E474D81C0EBB010000111011315214000BE474D81C0EBB5DE3771B",
                "0891683108501755F5240CA1015698994600000881307031906423885C0A656C76844E2D56FD79FB52A875286237FF0C60A8597DFF016B228FCE4F7F75284E2D56FD79FB52A87F514E0A554657CEFF0C60A8768477ED4FE1968F673A7801662FFF1A00380039003600320037003830024E2D56FD79FB52A84E0D4F1A4EE54EFB4F5565B95F0F541160A87D2253D68BE55BC67801FF0C8BF752FF544A77E54ED64EBA3002",
            };

            foreach (var s in mstr) {
                pool.Add (SmsDecoder.Helper.Decode (s));
            }

        }
    }
}