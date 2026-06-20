using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Network
{
    public static class NetworkFunctions
    {
        public static IPEndPoint GetLocalIP(Action? errorMethod = null)
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect("8.8.8.8", 80); // doesn't actually send anything
            IPEndPoint? local = (IPEndPoint?)socket.LocalEndPoint;
            if (local == null)
            {
#if DEBUG
                Console.WriteLine("couldn't get local address using 8.8.8.8:80");
#endif
                if (errorMethod == null)
                {
                    Console.WriteLine($"Error something is wrong");
                    Console.WriteLine($"Error:");
                    Console.WriteLine($"{ErrorCodes.ErrorLocalFailed}");
                    throw new SocketException(10014, "Couldn't get local address");
                }
                else
                {
                    errorMethod.Invoke();
                    return new IPEndPoint(IPAddress.Parse("127.0 0.1"), 0);
                }
            }
            socket.Close();
            return local;
        }
    }
}
