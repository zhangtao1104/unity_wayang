using System;
using BestHTTP;
using BestHTTP.WebSocket;
using BestHTTP.WebSocket.Frames;

namespace agora
{
    namespace unity
    {
        public class WebSocketPresenter : IWebSocketPresenter
        {
            public IWebSocket WebSocket
            {
                get;
                set;
            }

            public IApplicationContract ApplicationContract
            {
                get;
                set;
            }
            
            public WebSocketPresenter(IApplicationContract contract)
            {
                WebSocket = new WebSocketManager(this);
                ApplicationContract = contract;
            }

            public void InitWebSocket(string serverAdress, string deviceId)
            {
                WebSocket.InitWebSocket(serverAdress, deviceId);
            }

            public int Connect()
            {
                return WebSocket.Connect();
            }

            public int DisConnect()
            {
                return WebSocket.DisConnect();
            }

            public void ReConnect()
            {
                WebSocket.ReConnect();
            }

            public int Send(WebSocketFrame frame)
            {
                return WebSocket.Send(frame);
            }

            public int Send(byte[] buffer, ulong offset, ulong count)
            {
                return WebSocket.Send(buffer, offset, count);
            }

            public int Send(byte[] buffer)
            {
                return WebSocket.Send(buffer);
            }

            public int Send(string message)
            {
                return WebSocket.Send(message);
            }

            public void Release()
            {
                DisConnect();
                WebSocket = null;
                ApplicationContract = null;
            }

            public  void OnWebSocketConnect(WebSocket webSocket)
            {
                ApplicationContract.OnWebSocketConnect(webSocket);
            }

            public void OnWebSocketDisConnect(WebSocket webSocket, UInt16 code, string message)
            {
                ApplicationContract.OnWebSocketDisConnect(webSocket, code, message);
            }

            public void OnWebSocketError(WebSocket webSocket, Exception ex)
            {
                ApplicationContract.OnWebSocketError(webSocket, ex);
            }

            public void OnWebSocketReceiveMessage(WebSocket webSocket, string message)
            {
                ApplicationContract.OnWebSocketReceiveMessage(webSocket, message);
            }

            public void OnWebSocketReceiveBianry(WebSocket webSocket, byte[] data)
            {
                ApplicationContract.OnWebSocketReceiveBianry(webSocket, data);
            }

            public void OnWebSocketErrorDescription(WebSocket webSocket, string reason)
            {
                ApplicationContract.OnWebSocketErrorDescription(webSocket, reason);
            }
        }
    }
}
