using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatDLL
{
    [DataContract]
    public class TextMessage : Message
    {
        [DataMember]
        private string message;
        [DataMember]
        private string timeStamp;
        public TextMessage(string sender, string message) : base(sender)
        { 
            this.message = message;
            DateTime now  = DateTime.Now;
            this.timeStamp = now.ToLocalTime().ToString("dd/MM HH:mm");
        }


        public string getMessage()
        {
            return message;
        }

        public string getTimeStamp() 
        { 
            return timeStamp;
        }
    }
}
