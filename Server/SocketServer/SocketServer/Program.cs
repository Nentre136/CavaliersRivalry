
using SocketServer.Servers;

namespace SocketServer
{
    class Progame
    {
        static void Main(string[] args)
        {
            Server server = new Server(8888);
            Console.Read();
        }
    }
}