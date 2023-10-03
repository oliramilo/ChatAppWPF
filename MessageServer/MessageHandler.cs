using ChatDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Threading;

namespace MessageServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class MessageHandler : MessageServerInterface
    {
        // Key string is the chatroom name, each have a list of chat text messages
        [DataMember]
        private Dictionary<string, List<TextMessage>> chatMessages = new Dictionary<string, List<TextMessage>>();

        // Key string is the user's name, each have a list of private chat messages 
        [DataMember]
        private Dictionary<string, List<TextMessage>> privateMessages = new Dictionary<string, List<TextMessage>>();
        public MessageHandler() 
        {           
        }

        public void addMessageToChat(string chatroom, TextMessage textMessage)
        {
            Console.WriteLine("Incoming message request from chatroom: " + chatroom + "\nFrom: " + textMessage.getSender());
            List<TextMessage> messages;
            if (chatMessages.TryGetValue(chatroom, out messages))
            {
                messages.Add(textMessage);
                Console.WriteLine("Added message to existing chat room: " + chatroom);
            }
            else
            {
                chatMessages.Add(chatroom, new List<TextMessage> { textMessage });
                Console.WriteLine("Added message to a new chat room: " + chatroom);
            }
        }

        public List<TextMessage> getChatMessagesFor(string chatroom) 
        {
            if (chatMessages.ContainsKey(chatroom))
            {
                Console.WriteLine("Requested to get messages for chatroom: " + chatroom);
                return chatMessages[chatroom];
            }
            else
            {
                Console.Write("Failed to find chat message for chatroom: " + chatroom);
            }
            return null;
        }


        public void sendPrivateMessage(string receiver, TextMessage textMessage)
        {
            Console.WriteLine("Incoming private message request from sender: " + textMessage.getSender()+ "\nMessage: " + textMessage.getMessage() + "Send to: " + receiver);
            if (privateMessages.ContainsKey(receiver))
            {
                privateMessages[receiver].Add(textMessage);
            }
            else
            {
                privateMessages[receiver] = new List<TextMessage>();
                privateMessages[receiver].Add(textMessage);
            }
        }


        public List<TextMessage> getPrivateMessageFor(string receiver)
        {
            List<TextMessage> pm = null;
            if(privateMessages.ContainsKey(receiver))
            {
                pm = privateMessages[receiver];
            }
            pm = pm == null ? new List<TextMessage>() : pm;
            return pm;
        }

    }
}
