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

        // backlog: 최대 대기수
        _socket.Listen(backlog: DEFAULT_BACKLOG);

        // 동접이 많아 pending 이 항상 false 일 경우..
        for (int i = 0; i < 10; i++) {
            SocketAsyncEventArgs args = new();

            // ThreadPool 에서 I/O Thread 를 하나 가져와서 Completed Event 를 호출해준다.
            // if pending == true:
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }
    }

    void RegisterAccept(SocketAsyncEventArgs args) {
        Console.WriteLine("Waiting for client...");

        args.AcceptSocket = null;

        bool bPending = _socket.AcceptAsync(args);

        // ThreadPool 갈 필요 없이 바로 실행.
        if (!bPending) {
            OnAcceptCompleted(null, args);
        }
    }

    void OnAcceptCompleted(object sender, SocketAsyncEventArgs args) {
        Console.WriteLine($"Client Connected! pending");

        if (args.SocketError == SocketError.Success) {
            _onAcceptHandler.Invoke(args.AcceptSocket);
        }
        else {
            Console.WriteLine(args.SocketError.ToString());
        }

        RegisterAccept(args);
    }

    void OnAcceptCompleted_NoPending(object sender, SocketAsyncEventArgs args) {
        Console.WriteLine($"Client Connected! no pending");

        if (args.SocketError == SocketError.Success) {
            _onAcceptHandler.Invoke(args.AcceptSocket);
        }
        else {
            Console.WriteLine(args.SocketError.ToString());
        }

        RegisterAccept(args);
    }
}