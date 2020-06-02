using UnityEngine;
using System;
using BestHTTP;
using BestHTTP.WebSocket;
using BestHTTP.WebSocket.Frames;

namespace agora
{
    namespace unity
    {
        public enum SOCKET_CONNECTION_STATUS
        {
            NONE = 0,
            CONNECT = 1,
            DISCONNECT = 2,
        }

        public interface IWebSocket
        {
            IWebSocketPresenter WebSocketPresenter
            {
                get;
                set;
            }
            bool NeedReconnect
            {
                get;
                set;
            }

            SOCKET_CONNECTION_STATUS SocketConnectionStatus
            {
                get;
                set;
            }

            bool StartPingThread
            {
                get;
                set;
            }

            int PingFrequency
            {
                get;
                set;
            }

            Action<WebSocket> OnSocketConnect 
            {
                get;
                set;
            }

            Action<WebSocket, UInt16, string> OnSocketDisConnect 
            {
                get;
                set;
            }

            Action<WebSocket, Exception> OnSocketError 
            {
                get;
                set;
            }

            Action<WebSocket, string> OnSocketReceiveMessage 
            {
                get;
                set;
            }

            Action<WebSocket, byte[]> OnSocketReceiveBianry 
            {
                get;
                set;
            }

            Action<WebSocket, string> OnSocketErrorDescription 
            {
                get;
                set;
            }

            int Connect();
            int DisConnect();
            void ReConnect();
            int Send(WebSocketFrame frame);
            int Send(byte[] buffer, ulong offset, ulong count);
            int Send(byte[] buffer);
            int Send(string message);
            void OnWebSocketConnect(WebSocket webSocket);
            void OnWebSocketDisConnect(WebSocket webSocket, UInt16 code, string message);
            void OnWebSocketError(WebSocket webSocket, Exception ex);
            void OnWebSocketReceiveMessage(WebSocket webSocket, string message);
            void OnWebSocketReceiveBianry(WebSocket webSocket, byte[] data);
            void OnWebSocketErrorDescription(WebSocket webSocket, string reason);
            void InitWebSocket(string serverAdress, string deviceId);
            void Release();
        }
    }
}

