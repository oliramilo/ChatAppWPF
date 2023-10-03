using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using ChatDLL;

namespace FileServer
{
    [ServiceContract]
    public interface FileServerInterface
    {
        [OperationContract]
        void uploadFile(FileMessage fileMessage);
        
        [OperationContract]
        List<FileMessage> getFilesForChatRoom(string chatRoom);


        [OperationContract]
        int getChatRoomFileCount(string chatroom);

        [OperationContract]
        FileMessage getFile(string chatroom, string fileName);

        [OperationContract]
        List<FileMessage> test();
    
    }
}
