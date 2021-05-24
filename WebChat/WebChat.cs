using Newtonsoft.Json;
using System;
using System.Text;
using System.Windows.Forms;
using WebChat.Core;
using WebSocketSharp;

namespace WebChat
{
    public partial class WebChat : Form
    {
        private readonly WebSocket _socket = new("ws://localhost:9876/webchat");

        public WebChat()
        {
            _socket.OnOpen += Socket_OnOpen;
            _socket.OnClose += Socket_OnClose;
            _socket.OnError += Socket_OnError;
            _socket.OnMessage += Socket_OnMessage;

            InitializeComponent();
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e) 
            => rtbOutput.AppendText(e.Data);

        private void Socket_OnError(object sender, ErrorEventArgs e) 
            => MessageBox.Show(e.Exception.ToString());

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            rtbOutput.AppendText("Disconnected From Server!" + Environment.NewLine);
            btnConnect.Text = "Connect";
            txtSendMessage.Enabled = false;
            btnSend.Enabled = false;
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            rtbOutput.AppendText("Connected To Server!" + Environment.NewLine);
            btnConnect.Text = "Disconnect";
            txtSendMessage.Enabled = true;
            btnSend.Enabled = true;

            // Send Hello
            var hello = new Hello(OpCode.Hello);
            _socket.Send(JsonConvert.SerializeObject(hello));
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            if (btnSend.Enabled)
            {
                rtbOutput.AppendText("Disonnecting To Server..." + Environment.NewLine);
                _socket.Close();

                return;
            }

            rtbOutput.AppendText("Connecting To Server..." + Environment.NewLine);
            _socket.Connect();
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            _socket.Send(JsonConvert.SerializeObject(new Core.Message(OpCode.Message, txtSendMessage.Text)));
            txtSendMessage.Clear();
        }

        private void TxtSendMessage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            _socket.Send(JsonConvert.SerializeObject(new Core.Message(OpCode.Message, txtSendMessage.Text)));
            txtSendMessage.Clear();
        }

        private void RtbOutput_TextChanged(object sender, EventArgs e)
            => rtbOutput.ScrollToCaret();
    }
}
