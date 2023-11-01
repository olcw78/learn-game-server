using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public sealed class Server {
    const int PORT_NUM = 7777;
    private readonly Listener _listener;

    public Server() {
        _listener = new Listener(IPAddress.Loopback, PORT_NUM, OnAcceptHandler);

        Console.WriteLine("Start Listening...");
        while (true) {
        }
    }

    static void OnAcceptHandler(Socket connectedClientSocket) {
        try {
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
        catch (Exception e) {
            Console.WriteLine(e.Message);
        }
    }
}