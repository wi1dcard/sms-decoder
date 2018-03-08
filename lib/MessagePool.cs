using System;
using System.Collections.Generic;
using System.Linq;

namespace SmsDecoder {
    public class MessagePool {
        private Dictionary<string, MessageList> pool;

        public delegate void MessageHandler (Message ml);

        public event MessageHandler Message;

        public MessagePool (MessageHandler msgHandler) {
            this.pool = new Dictionary<string, MessageList> ();
            this.Message += msgHandler;
        }

        protected void onMessageCompleted (MessageList ml) {
            var m = ml.Merge ();
            this.Message (m);
        }

        public void Add (Message msg) {
            MessageList list;
            if (!msg.isSplit) {
                this.Message (msg);
                return;
            }
            if (!pool.ContainsKey (msg.splitId)) {
                list = pool[msg.splitId] = new MessageList ();
                list.MessageCompleted += this.onMessageCompleted;
            } else {
                list = pool[msg.splitId];
            }
            list.Add (msg);
        }

        public void Remove (Message msg) {
            pool[msg.splitId].Remove (msg);
        }
    }
}