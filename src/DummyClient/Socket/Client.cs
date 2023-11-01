using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient;

public sealed class Client {
    const int PORT_NUM = 7777;

    public Client() {
        // it didn't work in macOS.
        // string localHostName = Dns.GetHostName();
        // IPHostEntry ipHost = Dns.GetHostEntry(localHostName);
        // IPAddress localAddress = ipHost.AddressList[0];

        // same as IPAddress.Loopback
        IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), PORT_NUM);

        while (true) {
            Socket clientSocket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                clientSocket.Connect(endPoint);

                System.Console.WriteLine($"[Client] connected to {clientSocket.RemoteEndPoint.ToString()}");

                byte[] sendBuffer = Encoding.UTF8.GetBytes("Hello, Server from client!");
                int sendLen = clientSocket.Send(sendBuffer);

                byte[] recvBuffer = new byte[1024];
                int recvLen = clientSocket.Receive(recvBuffer);

                string recvStr = Encoding.UTF8.GetString(recvBuffer, 0, recvLen);
                System.Console.WriteLine($"[Client] from server: {recvStr}");

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close(timeout: 2000);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            Thread.Sleep(1000);
        }
    }
}