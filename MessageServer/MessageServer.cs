using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
/**
 * Message Server handles incoming requests for 
 * sending/receiving messages for chat rooms
 * **/
namespace MessageServer
{
    public class MessageServer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up Message server...");
            ServiceHost host;
            NetTcpBinding binding = new NetTcpBinding();
            host = new ServiceHost(typeof(MessageHandler));

            host.AddServiceEndpoint(typeof(MessageServerInterface), binding, "net.tcp://0.0.0.0:8101/MessageServer");

            host.Open();
            Console.WriteLine("Message Service online.");
            Console.ReadLine();
            host.Close();
        }
    }
}
