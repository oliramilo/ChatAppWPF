using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ChatBusinessTier;
using ChatDLL;
using FileServer;
using MessageServer;

namespace TestServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up test server");
            string address = "net.tcp://localhost:8102/BusinessTier";
            ChatBusinessInterface server;
            ChannelFactory<ChatBusinessInterface> channelFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            channelFactory = new ChannelFactory<ChatBusinessInterface>(tcp, address);
            server = channelFactory.CreateChannel();

            
            Console.WriteLine("Connected to Business server");

            string user1 = "supermman2017";
            string user2 = "supermman2018";

            string room1 = "room1";
            string room2 = "room2";
            string room3 =  "The boys";

            server.CreateRoom(room1);
            server.CreateRoom(room2);

            server.Login(user1);
            server.Login(user1);
            server.Login(user2);


            server.JoinRoom(user1, room1);
            server.JoinRoom(user1, room2);
            server.JoinRoom(user2, room2);

            List<string> users = server.GetUsers();



            server.SendMessage(user1, room1, new TextMessage(user2, "Hello world!!!"));

            server.SendMessage(user1, room1, new TextMessage(user2, "Hello world!!!"));

            server.SendMessage(user1, room1, new TextMessage(user2, "Hello world!!!"));


            server.SendMessage(user1, room2, new TextMessage(user2, "Bye world!!!"));

            server.SendMessage(user1, room2, new TextMessage(user2, "Bye world!!!"));

            server.SendMessage(user1, room2, new TextMessage(user2, "Bye world!!!"));



            List<TextMessage> messages = server.GetMessagesForChatRoom(room2);

            foreach (TextMessage message in messages)
            {
                Console.WriteLine("Message: " + message.getMessage());
            }

            server.SendPrivateMessage(user1, user2, new TextMessage(user1, "Well well well"));
            server.SendPrivateMessage(user1, user2, new TextMessage(user1, "World"));

            List<TextMessage> user1Messages = server.GetUserPrivateMessages(user2);
            foreach (TextMessage message in user1Messages)
            {
                Console.WriteLine(message.getSender() + ": " +  message.getMessage());    
            }


            byte[] data = new byte[1024]; 
            FileMessage fm = new FileMessage(user1,room1,"IMAGE.jpg",".jpg",data);
            server.UploadFile(room1,fm);

            FileMessage retrieveFiled = server.DownloadFile(room1, "IMAGE.jpg");
            Console.WriteLine(retrieveFiled.getFileName());

            Console.ReadLine();
            List<string> updatedUsers = server.GetUsers();
            foreach (string user in updatedUsers)
            {
                Console.WriteLine($"{user}");
            }

            Console.WriteLine("Starting up test server again");
            ChatBusinessInterface server2;
            ChannelFactory<ChatBusinessInterface> channelFactory2;
            NetTcpBinding tcp2 = new NetTcpBinding();
            channelFactory2 = new ChannelFactory<ChatBusinessInterface>(tcp2, address);
            server2 = channelFactory.CreateChannel();
            List<string> newList = server2.GetUsers();
            Console.WriteLine("User count: " + newList.Count);
            foreach (string user in newList) 
            {
                Console.WriteLine("user: " + user);
            }

            Console.ReadLine();
            server.CreateRoom(room3);



            server.LeaveRoom(user1, room1);
            Console.ReadLine();
        }
    }
}
