using System;
using System.Collections.Generic;
using System.Linq;

namespace SmsDecoder {
    /// <summary>
    /// message list of parts of long sms
    /// </summary>
    public class MessageList : List<Message> {
        /// <summary>
        /// add a message into list
        /// </summary>
        /// <param name="item"></param>
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

        /// <summary>
        /// merge parts of split messages
        /// </summary>
        /// <returns></returns>
        public Message Merge () {
            var fullMsg = new Message () {
                smscNumber = this [0].smscNumber,
                senderNumber = this [0].senderNumber,
                dateTime = this [0].dateTime,
                content = "",
                isSplit = false,
                splitId = this[0].splitId,
                splitCount = this[0].splitCount
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