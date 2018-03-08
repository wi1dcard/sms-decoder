using System;
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
                "0891683108502305F0440D91683197814047F70008511181000320238C0500033F02018D85957F4E2D658777ED4FE16D4B8BD500300030003000300030003000300030003000300030003000300030003000300030003000300030003000300030003000300030003000300030003000300030003000300030003000300030554A554A554A52066BB552066BB5600E4E488FD84E0D52066BB54EC04E489B3C0041004100614F60662F",
                "0891683108502305F0440D91683197814047F7000851118100034023240500033F02028C01621172314F60554A771F76845047768490174F6073A973A95566FF01",
            };

            foreach (var s in mstr) {
                pool.Add (SmsDecoder.Helper.Decode (s));
            }

        }
    }
}