using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using Websockets.Universal;
using Newtonsoft.Json;

namespace AdaW10.WebSockets
{
    class WebSocketClient
    {
        private Websockets.IWebSocketConnection connection;
        private bool Failed;
        private bool Echo;
        private Conversation _conversation;
        private DirectLineClient _client;

        public async void DoTest(string connectionURL)
        {
            connection = Websockets.WebSocketFactory.Create();
            connection.OnLog += Connection_OnLog;
            connection.OnError += Connection_OnError;
            connection.OnMessage += Connection_OnMessage;
            connection.OnOpened += Connection_OnOpened;

            connection.Open(connectionURL);
        }



        private void Connection_OnOpened()
        {
            Debug.WriteLine("Opened !");

            _client.Conversations.PostActivity(_conversation.ConversationId, new Activity("message")
            {
                From = new ChannelAccount("AdaUWP"),
                Text = "bonjour!",
            });
        }

        async void Timeout()
        {
            await Task.Delay(120000);
            Failed = true;
            Debug.WriteLine("Timeout");
        }

        private void Connection_OnMessage(string obj)
        {
            Activity activity = JsonConvert.DeserializeObject<Activity>(obj);

            switch (activity.Text)
            {
                case "take picture":
                default:
                    return;
            }
        }

        private void Connection_OnError(string obj)
        {
            Debug.Write("ERROR " + obj);
            Failed = true;
        }

        private void Connection_OnLog(string obj)
        {
            Debug.Write(obj);
        }

        internal void Setup(DirectLineClient client, Conversation conversation)
        {
            _client = client;
            _conversation = conversation;
            WebsocketConnection.Link();
        }
    }
}
