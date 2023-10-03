using ChatDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MessageServer
{
    [ServiceContract]
    public interface MessageServerInterface
    {
        [OperationContract]
        void addMessageToChat(string chatroom,TextMessage textMessage);
        [OperationContract]
        List<TextMessage> getChatMessagesFor(string chatroom);
        [OperationContract]
        void sendPrivateMessage(string user,TextMessage textMessage);
        [OperationContract]
        List<TextMessage> getPrivateMessageFor(string receiver);

    }
}
