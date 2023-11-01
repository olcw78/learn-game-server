using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener {
    private const int DEFAULT_BACKLOG = 4;

    private readonly Socket _socket;
    private readonly IPEndPoint _endPoint;

    private event Action<Socket> _onAcceptHandler;


    public Listener(IPAddress ipAddress, int portNum, Action<Socket> onAcceptHandler) {
        _onAcceptHandler += onAcceptHandler;
        // it didn't work in macOS.
        // string localHostName = Dns.GetHostName();
        // IPHostEntry ipHost = Dns.GetHostEntry(localHostName);
        // IPAddress localAddress = ipHost.AddressList[0];

        _endPoint = new IPEndPoint(ipAddress, portNum);
        _socket = new Socket(
            _endPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        _socket.Bind(_endPoint);
        _socket.Listen(backlog: DEFAULT_BACKLOG);

        SocketAsyncEventArgs args = new();
        args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
        RegisterAccept(args);
    }

    void RegisterAccept(SocketAsyncEventArgs args) {
        Console.WriteLine($"Waiting for client...");

        args.AcceptSocket = null;

        bool bPending = _socket.AcceptAsync(args);
        if (!bPending) {
            OnAcceptCompleted(null, args);
        }
    }

    void OnAcceptCompleted(object sender, SocketAsyncEventArgs args) {
        if (args.SocketError == SocketError.Success) {
            _onAcceptHandler.Invoke(args.AcceptSocket);
        }
        else {
            Console.WriteLine(args.SocketError.ToString());
        }

        RegisterAccept(args);
    }
}