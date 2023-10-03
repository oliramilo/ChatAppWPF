using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ChatDLL;
namespace ChatBusinessTier
{
    [ServiceContract]
    public interface ChatBusinessInterface
    {

        // Login/Logout Users , Join/Leave/Create Rooms Operations
        [OperationContract]
        bool Login(string username);
        [OperationContract]
        bool Logout(string username);
        [OperationContract]
        bool CreateRoom(string roomName);
        [OperationContract]
        bool JoinRoom(string username, string roomName);
        [OperationContract]
        bool LeaveRoom(string username, string roomName);


        // Download/Upload File Operations
        [OperationContract]
        FileMessage DownloadFile(string chatroom, string filename);
        [OperationContract] 
        bool UploadFile(string chatroom, FileMessage file);


        // Send message Operations

        [OperationContract]
        bool SendMessage(string username, string roomName, TextMessage message);
        [OperationContract]
        bool SendPrivateMessage(string sender, string receiver, TextMessage message);



        // Retrieve data
        [OperationContract]
        List<TextMessage> GetMessagesForChatRoom(string roomName);

        [OperationContract]
        List<FileMessage> GetFilesForChatRoom(string chatroom);

        [OperationContract]
        List<TextMessage> GetUserPrivateMessages(string user);

        [OperationContract]
        List<string> GetUsers();


        [OperationContract]
        Dictionary<string, List<string>> GetChatRooms();

        [OperationContract]
        bool VerifyUserInChat(string user, string roomName);

        [OperationContract]
        List<string> GetUsersInChat(string roomname);


    }
}
