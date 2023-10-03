using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatDLL
{
    [DataContract]
    public abstract class Message
    {
        [DataMember]
        protected string sender;
        
        public Message(string sender)
        {
            this.sender = sender;
        }

        public string getSender()
        {
            return sender;
        }
    }
}
