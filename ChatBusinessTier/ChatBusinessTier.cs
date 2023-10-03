using ChatDLL;
using FileServer;
using MessageServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatBusinessTier
{
    public class ChatBusinessTier
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up Business Tier server...");
            ServiceHost host;
            NetTcpBinding binding = new NetTcpBinding();
            host = new ServiceHost(typeof(ChatBusinessImplementation));

            host.AddServiceEndpoint(typeof(ChatBusinessInterface), binding, "net.tcp://0.0.0.0:8102/BusinessTier");

            host.Open();
            Console.WriteLine("Business Tier Service online.");
            Console.ReadLine();
            host.Close();
        }
    }
}
