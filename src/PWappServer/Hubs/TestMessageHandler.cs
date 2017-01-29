using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketManager;

namespace PWappServer.Hubs
{
    public class TestMessageHandler : WebSocketHandler
    {
        public TestMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);

            var message = new Message()
            {
                MessageType = MessageType.Text,
                Data = $"{socketId} is now connected"
            };

            await SendMessageToAllAsync(message);

           
        }

        public async Task SendMessage(string socketId, string message)
        {
            await InvokeClientMethodToAllAsync("receiveMessage", socketId, message);
        }

        public async Task SendMessageToId(string socketId, string message)
        {
            await InvokeClientMethodAsync(socketId, "receiveMessage", message);
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);

            await base.OnDisconnected(socket);

            var message = new Message()
            {
                MessageType = MessageType.Text,
                Data = $"{socketId} disconnected"
            };

            WebSocketSessions.Sessions
    .RemoveAll(a => a.ConnectionId == socketId);

            



            var reciveirId = WebSocketSessions.Sessions.Where(u => u.UserName == socketId);

            //   StaticClass.Sessions.RemoveAt(ConnectionInfo.(x => thingy));

            /*
            var index = StaticClass.Sessions.IndexOf("VesselId");
            if (index > -1)
                StaticClass.Sessions.RemoveAt(reciveirId[0]);
                */

            await SendMessageToAllAsync(message);

         //   await WebSocketConnectionManager.RemoveSocket(socketId);
        }
    }
}
