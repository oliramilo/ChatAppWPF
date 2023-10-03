using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FileServer
{
    public class FileServer
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Starting up File server...");
            ServiceHost host;
            NetTcpBinding binding = new NetTcpBinding();
            host = new ServiceHost(typeof(FileHandler));

            host.AddServiceEndpoint(typeof(FileServerInterface), binding, "net.tcp://0.0.0.0:8100/FileServer");

            host.Open();
            Console.WriteLine("File Service online.");
            Console.ReadLine();
            host.Close();
        }
    }
}
