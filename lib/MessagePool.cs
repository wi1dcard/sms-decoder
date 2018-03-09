using System;
using System.Collections.Generic;
using System.Linq;

namespace SmsDecoder {
    /// <summary>
    /// message pool of message lists, contains many parts of long sms(s)
    /// </summary>
    public class MessagePool {
        private Dictionary<string, MessageList> pool;

        public delegate void MessageHandler (Message ml);

        /// <summary>
        /// when a (long) sms in pool are compeleted, this event will be trigger
        /// </summary>
        public event MessageHandler Message;

        public MessagePool (MessageHandler msgHandler) {
            this.pool = new Dictionary<string, MessageList> ();
            this.Message += msgHandler;
        }

        protected void onMessageCompleted (MessageList ml) {
            var m = ml.Merge ();
            this.pool.Remove (m.splitId);
            this.Message (m);
        }

        /// <summary>
        /// add a message into pool
        /// </summary>
        /// <param name="msg"></param>
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

        /// <summary>
        /// remove a message from pool
        /// </summary>
        /// <param name="msg"></param>
        public void Remove (Message msg) {
            pool[msg.splitId].Remove (msg);
        }
    }
}