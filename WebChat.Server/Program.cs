using System;
using WebSocketSharp.Server;

namespace WebChat.Server
{
    class Program
    {
        public static readonly WebSocketServer ServerSocket = new("ws://localhost:9876");
        public static bool disconnectRequested = false;

        static void Main()
        {
            ServerSocket.AddWebSocketService<WebChat>("/webchat");

            ServerSocket.Start();

            Console.WriteLine("Server Started! Press any key to stop the server.");
            Console.ReadKey();

            ServerSocket.Stop();
        }
    }
}
