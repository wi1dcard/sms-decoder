using System;
using System.Collections.Generic;
using System.Linq;

namespace SmsDecoder {
    public class MessageList : List<Message> {
        public new void Add (Message item) {
            foreach (var m in this) {
                if (m == item) {
                    throw new Exception ("Message Exists!");
                }
                if (m.splitIndex == item.splitIndex) {
                    this.Remove (m);
                }
            }
            base.Add (item);
            if (!this[0].isSplit || this.Count == this [0].splitCount) {
                this.MessageCompleted (this);
            }
        }

        public delegate void MessageCompletedHandler (MessageList ml);

        public event MessageCompletedHandler MessageCompleted;

        public Message Merge () {
            var fullMsg = new Message () {
                smscNumber = this [0].smscNumber,
                senderNumber = this [0].senderNumber,
                dateTime = this [0].dateTime,
                content = "",
                isSplit = false
            };
            this.Sort ((x, y) => {
                if (x.splitIndex == y.splitIndex) {
                    return 0;
                }
                return x.splitIndex > y.splitIndex ? 1 : -1;
            });
            this.AsEnumerable ().All ((m) => { fullMsg.content += m.content; return true; });
            return fullMsg;
        }
    }
}