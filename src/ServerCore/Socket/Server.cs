using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public sealed class Server {
    public Server() {
        // it didn't work in macOS.
        // string localHostName = Dns.GetHostName();
        // IPHostEntry ipHost = Dns.GetHostEntry(localHostName);
        // IPAddress localAddress = ipHost.AddressList[0];

        const int PORT_NUM = 7777;
        // same as IPAddress.Loopback
        IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), PORT_NUM);

        Socket listenSocket = new(
            endPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        try {
            listenSocket.Bind(endPoint);
            listenSocket.Listen(backlog: 10);

            while (true) {
                Console.WriteLine($"Waiting for client...");

                Socket connectedClientSocket = listenSocket.Accept();

                byte[] recvBuffer = new byte[1024];
                int recvLen = connectedClientSocket.Receive(recvBuffer);
                if (recvLen > 0) {
                    string recvStr = Encoding.UTF8.GetString(recvBuffer, 0, recvLen);
                    if (!string.IsNullOrEmpty(recvStr)) {
                        Console.WriteLine($"[From Client] {recvStr}");
                    }
                }

                byte[] sendBuffer = Encoding.UTF8.GetBytes("hello it's me. MMORPG Server!");
                int sendLen = connectedClientSocket.Send(sendBuffer);

                connectedClientSocket.Shutdown(SocketShutdown.Both);
                connectedClientSocket.Close(timeout: 2000);
            }
        }
        catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }
}