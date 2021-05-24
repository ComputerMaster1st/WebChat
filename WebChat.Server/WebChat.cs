using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebChat.Core;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WebChat.Server
{
    public class WebChat : WebSocketBehavior
    {
        private string Header(string id = null) 
            => string.Format("{0} - {1}:", DateTime.UtcNow.ToString("hh:mm tt"), id ?? ID);

        protected override void OnOpen()
            => Console.WriteLine($"Client Connected: {ID}");

        protected override void OnClose(CloseEventArgs e)
            => Console.WriteLine($"Client Disconnected: {ID}");

        protected override void OnError(ErrorEventArgs e)
        {
            Console.WriteLine($"ERROR: {e.Message}");
            Console.WriteLine($"EXCEPTION: {e.Exception}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine($"Server Received Message From {ID}: {e.Data}");
            ParseJson(e.Data);
        }

        private void ParseJson(string json)
        {
            var request = JsonConvert.DeserializeObject<dynamic>(json);
            var op = (OpCode)request.Op;

            switch (op)
            {
                case OpCode.Hello:
                    // Tell new user who's in the server
                    var output = new StringBuilder()
                        .AppendLine(Header("Server"))
                        .AppendFormat("There are {0} users logged in!", Sessions.Count).AppendLine()
                        .AppendLine();

                    foreach (var session in Sessions.Sessions)
                    {
                        if (session.ID == ID)
                            continue;

                        output.AppendLine(session.ID);
                    }

                    Send(output.ToString());

                    // alert everyone that a new user has connected
                    Sessions.Broadcast($"{Header("Server") + Environment.NewLine}User Joined Server! ID: {ID}");
                    break;
                case OpCode.Message:
                    var msg = JsonConvert.DeserializeObject<Message>(json);
                    Sessions.Broadcast(Header() + Environment.NewLine + msg.Text);
                    break;
            }
        }
    }
}
