using System;
using BestHTTP;
using BestHTTP.WebSocket;
namespace agora
{
    namespace unity
    {
        public interface IApplicationContract
        {
            void OnWebSocketConnect(WebSocket webSocket);
            void OnWebSocketDisConnect(WebSocket webSocket, UInt16 code, string message);
            void OnWebSocketError(WebSocket webSocket, Exception ex);
            void OnWebSocketReceiveMessage(WebSocket webSocket, string message);
            void OnWebSocketReceiveBianry(WebSocket webSocket, byte[] data);
            void OnWebSocketErrorDescription(WebSocket webSocket, string reason);  
            void SetMainViewPersenter(IMainViewPersenter mainViewPersenter);
        }
    }
}
