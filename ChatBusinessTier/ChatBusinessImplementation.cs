using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatDLL;
using FileServer;
using MessageServer;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace ChatBusinessTier
{
    //
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Multiple,UseSynchronizationContext = false)]
    public class ChatBusinessImplementation : ChatBusinessInterface
    {

        public const string FILE_SERVER_ADDRESS = "net.tcp://localhost:8100/FileServer";
        public const string MESSAGE_SERVER_ADDRES = "net.tcp://localhost:8101/MessageServer";

        [DataMember]
        private Dictionary<string, bool> users = new Dictionary<string, bool>();
        [DataMember]
        private Dictionary<string, List<string>> chatrooms = new Dictionary<string, List<string>>();
        private FileServerInterface fileServer;
        private MessageServerInterface messageServer;

        public ChatBusinessImplementation()
        {
            // Open a channel factory to the Message server

            ChannelFactory<MessageServerInterface> messageChannelFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            messageChannelFactory = new ChannelFactory<MessageServerInterface>(tcp, MESSAGE_SERVER_ADDRES);
            messageServer = messageChannelFactory.CreateChannel();

            ChannelFactory<FileServerInterface> fileChannelFactory;
            NetTcpBinding fTCP = new NetTcpBinding();
            fileChannelFactory = new ChannelFactory<FileServerInterface>(fTCP, FILE_SERVER_ADDRESS);
            fileServer = fileChannelFactory.CreateChannel();
            Console.WriteLine("Created chat server business.");
        }

        // Check if user exists
        private bool userExists(string username)
        {
            return users.ContainsKey(username);
        }

        // Check if chat room exists
        private bool chatExists(string chatroom)
        {
            return chatrooms.ContainsKey(chatroom);
        }


        // Check if user is already logged in
        private bool userLoggedIn(string username)
        {
            // users[username] is true if logged in so check if one of the clients are logged in
            return userExists(username) && users[username] == true;
        }

        // Checks if user belongs to a chatroom specified
        private bool userInChat(string username, string roomName)
        {
            bool userInChat = false;
            if (chatExists(roomName) && chatrooms[roomName].Contains(username))
            {
                userInChat = true;
            }
            return userInChat;
        }


        // Log user in, user saves but sets log in status to true
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Login(string username)
        {
            bool loggedIn;
            // Check if user exists and is not logged in
            if (users.TryGetValue(username, out loggedIn))
            { 
                Console.WriteLine("Existing user " + username + " logged in.");

                if(loggedIn)
                {
                    Console.WriteLine("User " + username + " already logged in");
                    loggedIn = false;
                }
                else
                {
                    Console.WriteLine("User " + username + " added");
                    users[username] = true;
                    loggedIn = true;
                }
            }
            else 
            {
                Console.WriteLine("New user " + username + " added.");
                loggedIn = true;
                users.Add(username, true);
            }
            Console.WriteLine("Current users: " + users.Count);
            return loggedIn;
        }


        // Log user out of the client. Saves user but sets log in status to false
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Logout(string username)
        {
            bool logOut = false;
            // Check if user exists and is logged in
            if (users.ContainsKey(username) && users[username] == true)
            {
                users[username] = false;
                logOut = true;
            }

            return logOut;
        }


        // Creates room if not created already, return false if room already exists
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool CreateRoom(string roomName)
        {
            bool roomCreated = false;
            // check if chatroom already exists
            if (!chatExists(roomName))
            {
                roomCreated = true;
                // add chatroom and create new list of users
                chatrooms[roomName] = new List<string>();
            }
            return roomCreated;
        }


        // Add user to a room, if user is already in chat or user does not exist will return false
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool JoinRoom(string username, string roomName)
        {
            bool userJoined = false;
            if (chatExists(roomName) && !userInChat(username, roomName))
            {
                chatrooms[roomName].Add(username);
                userJoined = true;
            }
            else
            {
                Console.WriteLine("User failed to join room");
            }
            return userJoined;
        }


        // Remove users in the room, if user was not in the chat room returns false
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool LeaveRoom(string username, string roomName)
        {

            bool userRemoved = false;
            if (chatExists(roomName) && userInChat(username, roomName))
            {
                chatrooms[roomName].Remove(username);
                userRemoved = true;
            }
            return userRemoved;
        }

        // returns null if file not found in the chatroom or if chatroom does not exist. 
        [MethodImpl(MethodImplOptions.Synchronized)]
        public FileMessage DownloadFile(string chatroom, string filename)
        {
            FileMessage file = null;
            if(chatExists(chatroom))
            {
                file = fileServer.getFile(chatroom, filename);
            }
            return file;
        }


        // return false if file failed to upload or if chatroom does not exist
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool UploadFile(string chatroom, FileMessage fileMessage)
        {
            bool fileUploaded = false;

            if(chatExists(chatroom))
            {
                fileServer.uploadFile(fileMessage);
                fileUploaded = true;
            }
            return fileUploaded;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SendMessage(string username, string roomName, TextMessage message)
        {
            bool messageSent = false;
            if(chatExists(roomName) && userInChat(username, roomName))
            {
                if (message == null && messageServer == null)
                {
                    Console.WriteLine("Message null");
                }
                else
                {
                    messageServer.addMessageToChat(roomName, message);
                }
                messageSent = true;
            }
            else
            {
                Console.WriteLine("userinChat() or chatExists() returned false. User may not have access to this chatroom or chat does not exists");
            }

            if(messageSent == false)
            {
                Console.WriteLine("failed to send a message.");
            }
            //Thread.Sleep(1000);
            return messageSent;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SendPrivateMessage(string sender, string receiver, TextMessage message)
        {
            bool messageSent = false;
            if (userExists(sender) && userExists(receiver))
            {
                messageServer.sendPrivateMessage(receiver, message);
                messageSent = true;
            }
            return messageSent;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<TextMessage> GetMessagesForChatRoom(string roomName)
        {
            List<TextMessage> messages = null;
            if(chatExists(roomName))
            {
                messages = messageServer.getChatMessagesFor(roomName);
            }
            if(messages == null)
            {
                Console.WriteLine("Returned a null message for chatroom: " + roomName);
            }
            //Thread.Sleep(1000);
            return messages;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<FileMessage> GetFilesForChatRoom(string chatroom)
        {
            List<FileMessage> files = null;
            if(chatExists(chatroom))
            {
                files = fileServer.getFilesForChatRoom(chatroom);
            }

            return files;

        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<TextMessage> GetUserPrivateMessages(string user)
        {
            List<TextMessage> messages = null;
            if (userExists(user))
            {
               
                messages = messageServer.getPrivateMessageFor(user);
            }

            return messages;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<string> GetUsers()
        {
            return users.Keys.ToList();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<string> GetUsersInChat(string roomname)
        {
            return chatrooms[roomname];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Dictionary<string, List<string>> GetChatRooms()
        {
            return chatrooms;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool VerifyUserInChat(string user, string roomName)
        {
            bool verified = false;

            if(userExists(user) && userInChat(user, roomName))
            {
                verified = true;
            }

            return verified;
        }

    }
}
