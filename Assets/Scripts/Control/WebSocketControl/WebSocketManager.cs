using System;
using BestHTTP;
using BestHTTP.WebSocket;
using BestHTTP.WebSocket.Frames;

namespace agora
{
    namespace unity
    {
        public class WebSocketManager : IWebSocket
        {
            private static string Tag = "web_socket";
            private WebSocket webSocket = null;

            public bool NeedReconnect
            {
                get;
                set;
            }

            public IWebSocketPresenter WebSocketPresenter
            {
                get;
                set;
            }
            
            public SOCKET_CONNECTION_STATUS SocketConnectionStatus
            {
                get;
                set;
            }

            public bool StartPingThread
            {
                get;
                set;
            }

            public int PingFrequency
            {
                get;
                set;
            }

            public Action<WebSocket> OnSocketConnect 
            {
                get;
                set;
            }

            public Action<WebSocket, UInt16, string> OnSocketDisConnect 
            {
                get;
                set;
            }

            public  Action<WebSocket, Exception> OnSocketError 
            {
                get;
                set;
            }

            public Action<WebSocket, string> OnSocketReceiveMessage 
            {
                get;
                set;
            }

            public Action<WebSocket, byte[]> OnSocketReceiveBianry 
            {
                get;
                set;
            }

            public Action<WebSocket, string> OnSocketErrorDescription 
            {
                get;
                set;
            }

            public WebSocketManager(WebSocketPresenter presenter)
            {
                NeedReconnect = false;
                SocketConnectionStatus = SOCKET_CONNECTION_STATUS.NONE;
                StartPingThread = true;
                PingFrequency = 2000;
                WebSocketPresenter = presenter;
            }

            public void InitWebSocket(string serverAdress, string deviceId)
            {
                try
                {
                    webSocket = new WebSocket(new Uri(serverAdress + deviceId));
                    webSocket.StartPingThread = this.StartPingThread;
                    webSocket.PingFrequency = this.PingFrequency;
                    webSocket.OnOpen = this.OnWebSocketConnect;
                    webSocket.OnBinary = this.OnWebSocketReceiveBianry;
                    webSocket.OnMessage = this.OnWebSocketReceiveMessage;
                    webSocket.OnClosed = this.OnWebSocketDisConnect;
                    webSocket.OnError = this.OnWebSocketError;
                    webSocket.OnErrorDesc = this.OnWebSocketErrorDescription;
                    webSocket.Open();
                    NeedReconnect = true;
                    Application.Logger.Info(Tag, "Init webSocket success");
                }
                catch (Exception e)
                {
                    Application.Logger.Error(Tag, "Init webSocket error catch exception = " + e.ToString());
                }
            }

            public int Connect()
            {
                if (webSocket == null)
                {
                    Application.Logger.Error(Tag, "Not init webSocket");
                    return (int)ERROR_CODE.ERROR_CODE_NOT_INIT_WEBSOCKET;
                }

                // webSocket.Open();
                // NeedReconnect = true;
                Application.Logger.Info(Tag, "Connect webSocket ......");
                return (int)ERROR_CODE.ERROR_OK;
            }

            public int DisConnect()
            {
                if (webSocket == null)
                {
                    Application.Logger.Error(Tag, "Not init webSocket");
                    return (int)ERROR_CODE.ERROR_CODE_NOT_INIT_WEBSOCKET;
                }

                webSocket.Close();
                Application.Logger.Info(Tag, "Close webSocket ");
                return (int)ERROR_CODE.ERROR_OK;
            }

            public void ReConnect()
            {

            }

            public void Release()
            {
                // DisConnect();
                // webSocket.OnOpen = null;
                // webSocket.OnBinary = null;
                // webSocket.OnMessage = null;
                // webSocket.OnClosed = null;
                // webSocket.OnError = null;
                // webSocket.OnErrorDesc = null;
                // webSocket = null;
            }

            public int Send(WebSocketFrame frame)
            {
                if (webSocket == null)
                {
                    Application.Logger.Error(Tag, "Not init webSocket");
                    return (int)ERROR_CODE.ERROR_CODE_NOT_INIT_WEBSOCKET;
                }

                webSocket.Send(frame);
                Application.Logger.Info(Tag, "Send message frame ");
                return (int)ERROR_CODE.ERROR_OK;
            }

            public int Send(byte[] buffer, ulong offset, ulong count)
            {
                if (webSocket == null)
                {
                    Application.Logger.Error(Tag, "Not init webSocket");
                    return (int)ERROR_CODE.ERROR_CODE_NOT_INIT_WEBSOCKET;
                }

                webSocket.Send(buffer, offset, count);
                Application.Logger.Info(Tag, "Send message byte  buffer = " + buffer + " ,offset = " + offset + " ,count = " + count);
                return (int)ERROR_CODE.ERROR_OK;       
            }

            public int Send(byte[] buffer)
            {
                if (webSocket == null)
                {
                    Application.Logger.Error(Tag, "Not init webSocket");
                    return (int)ERROR_CODE.ERROR_CODE_NOT_INIT_WEBSOCKET;
                }

                webSocket.Send(buffer);
                Application.Logger.Info(Tag, "Send message byte  buffer = " + buffer);
                return (int)ERROR_CODE.ERROR_OK;       
            }

            public int Send(string message)
            {
                if (webSocket == null)
                {
                    Application.Logger.Error(Tag, "[Upload] Not init webSocket");
                    return (int)ERROR_CODE.ERROR_CODE_NOT_INIT_WEBSOCKET;
                }

                webSocket.Send(message);
                Application.Logger.Info(Tag, "Send message string message = " + message);
                return (int)ERROR_CODE.ERROR_OK;       
            }

            public void OnWebSocketConnect(WebSocket webSocket)
            {
                if (WebSocketPresenter != null)
                {
                    WebSocketPresenter.OnWebSocketConnect(webSocket);
                }

                Application.Logger.Info(Tag, "OnWebSocketConnect ");
            }

            public void OnWebSocketDisConnect(WebSocket webSocket, UInt16 code, string message)
            {
                if (WebSocketPresenter != null)
                {
                    WebSocketPresenter.OnWebSocketDisConnect(webSocket, code, message);
                }

                Application.Logger.Info(Tag, "OnWebSocketDisConnect ");
            }

            public  void OnWebSocketError(WebSocket webSocket, Exception ex)
            {
                if (WebSocketPresenter != null)
                {
                    WebSocketPresenter.OnWebSocketError(webSocket, ex);
                }

                Application.Logger.Info(Tag, "OnWebSocketError ");
            }

            public void OnWebSocketReceiveMessage(WebSocket webSocket, string message)
            {
                if (WebSocketPresenter != null)
                {
                    WebSocketPresenter.OnWebSocketReceiveMessage(webSocket, message);
                }

                Application.Logger.Info(Tag, "[receive] OnWebSocketReceiveMessage " + message);       
            }
            
            public void OnWebSocketReceiveBianry(WebSocket webSocket, byte[] data)
            {
                if (WebSocketPresenter != null)
                {
                    WebSocketPresenter.OnWebSocketReceiveBianry(webSocket, data);
                }

                Application.Logger.Info(Tag, "OnWebSocketReceiveBianry " + data);   
            }

            public void OnWebSocketErrorDescription(WebSocket webSocket, string reason)
            {
                if (WebSocketPresenter != null)
                {
                    WebSocketPresenter.OnWebSocketErrorDescription(webSocket, reason);
                }

                Application.Logger.Info(Tag, "OnWebSocketErrorDescription  reason " + reason); 
            }
        }
    }
}
