
using System;
using BestHTTP;
using BestHTTP.WebSocket;
using BestHTTP.WebSocket.Frames;

namespace agora
{
    namespace unity
    {
        public interface IWebSocketPresenter
        {
            int Connect();
            int DisConnect();
            void ReConnect();
            int Send(WebSocketFrame frame);
            int Send(byte[] buffer, ulong offset, ulong count);
            int Send(byte[] buffer);
            int Send(string message);
            void InitWebSocket(string serverAdress, string deviceId);
            void OnWebSocketConnect(WebSocket webSocket);
            void OnWebSocketDisConnect(WebSocket webSocket, UInt16 code, string message);
            void OnWebSocketError(WebSocket webSocket, Exception ex);
            void OnWebSocketReceiveMessage(WebSocket webSocket, string message);
            void OnWebSocketReceiveBianry(WebSocket webSocket, byte[] data);
            void OnWebSocketErrorDescription(WebSocket webSocket, string reason);  
            void Release();
        }
    }
}
