using System;
using BestHTTP;
using BestHTTP.WebSocket;
using BestHTTP.WebSocket.Frames;
using System.Text;

namespace agora
{
    namespace unity
    {
        public class ApplicationContract : IApplicationContract
        {
            private string TAG = "ApplicationContract";
            private IMainViewPersenter MainViewPersenter
            {
                get;
                set;
            }

            private RtcEnginePresenter rtcEnginePresenter
            {
                get;
                set;
            }

            private IWebSocketPresenter WebSocketPresenter
            {
                get;
                set;
            }

            private MessageComand MessageComand
            {
                get;
                set;
            }

            public static ApplicationContract ApplicationContractInstance = null;
            public static ApplicationContract GetInstance()
            {
                if (ApplicationContractInstance == null)
                {
                    ApplicationContractInstance = new ApplicationContract();
                }
                return ApplicationContractInstance;
            }

            private ApplicationContract()
            {  
                WebSocketPresenter = new WebSocketPresenter(this);
                MessageComand = new MessageComand();
                rtcEnginePresenter = new RtcEnginePresenter(this);
            }

            public static string GetSDKVersion()
            {
                return RtcEnginePresenter.GetSdkVersion();
            }
            
            public void OnAwake()
            {
                ConnectWebSocket();
            }

            public void OnStart()
            {

            }

            public void OnUpdate()
            {
                if (MessageComand != null)
                {
                    ServerMessage serverMessage = MessageComand.GetMessageFromQueue(MessageType.SERVER);
                    if (serverMessage != null)
                    {
                        rtcEnginePresenter.OnExecuteServerMessage(serverMessage);
                    }

                    ServerMessage clientMessage = MessageComand.GetMessageFromQueue(MessageType.CLIENT);
                    if (clientMessage != null)
                    {
                        Application.Logger.Info(TAG, "Message: " + clientMessage.ToString());
                        Send(System.Text.Encoding.UTF8.GetBytes(MessageComand.ServerMessageToJson(clientMessage)));
                    }
                }
            }

            public void UploadMessageToServer(ServerMessage message)
            {
                MessageComand.UploadMessageToServer(message);
            }

            public void OnApplicationPause(bool pause)
            {
                Application.Logger.Info(TAG, "OnApplicationPause pause = " + pause);
            }

            public void OnApplicationFocus(bool focus)
            {

            }

            public void OnApplicationQuit()
            {
                Release();
            }

            public void OnDestroy()
            {

            }

            public int DisConnect()
            {
                return WebSocketPresenter.DisConnect();
            }

            public void ReConnect()
            {
                WebSocketPresenter.ReConnect();
            }

            public int Send(WebSocketFrame frame)
            {
                return WebSocketPresenter.Send(frame);
            }

            public int Send(byte[] buffer, ulong offset, ulong count)
            {
                return WebSocketPresenter.Send(buffer, offset, count);
            }

            public int Send(byte[] buffer)
            {
                return WebSocketPresenter.Send(buffer);
            }

            public int Send(string message)
            {
                return WebSocketPresenter.Send(message);
            }

            public void SetMainViewPersenter(IMainViewPersenter mainViewPersenter)
            {
                MainViewPersenter = mainViewPersenter;
            }

            public void ConnectWebSocket()
            {
                WebSocketPresenter.InitWebSocket(Application.WebSocket_Url, Application.DeviceID);
            }

            public void OnWebSocketConnect(WebSocket webSocket)
            {
                //Application.Logger.Info(TAG, System.Reflection.MethodBase.GetCurrentMethod().Name);
                //string s = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"create\", \"info\":{\"context\":\"\",\"appId\":\"5db0d12c40354100abd7a8a0adaa1fb8\",\"handler\":\"\"},\"extra\":{}}";
                //MessageComand.SendJsonToMessageQueue(MessageType.SERVER, s);

                //string s1 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"joinChannel\", \"info\":{\"context\":\"\",\"appId\":\"345445\",\"handler\":\"\"},\"extra\":{}}";
                //MessageComand.SendJsonToMessageQueue(MessageType.SERVER, s1);

                //string s2 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\":\"setVideoEncoderConfiguration\", \"info\": {\"dimensions\":{\"width\": 1280, \"height\": 720}, \"frameRate\": 15, \"bitrate\": 0, \"minBitrate\": -1, \"orientationMode\": 0, \"minFrameRate\": -1, \"degradationPreference\": 0}}}";
                //MessageComand.SendJsonToMessageQueue(MessageType.SERVER, s2);

                MainViewPersenter.OnWebSocketConnect();
            }

            public void OnWebSocketDisConnect(WebSocket webSocket, UInt16 code, string message)
            {
                //Application.Logger.Info(TAG, System.Reflection.MethodBase.GetCurrentMethod().Name);
                MainViewPersenter.OnWebSocketDisConnect();
            }

            public void OnWebSocketError(WebSocket webSocket, Exception ex)
            {
                Application.Logger.Info(TAG, System.Reflection.MethodBase.GetCurrentMethod().Name);
                if (MainViewPersenter != null)
                {
                    MainViewPersenter.OnWebSocketDisConnect();
                }
            }

            public void OnWebSocketReceiveMessage(WebSocket webSocket, string message)
            {  
                Application.Logger.Info(TAG, System.Reflection.MethodBase.GetCurrentMethod().Name + "  message = " + message);
                MessageComand.SendJsonToMessageQueue(MessageType.SERVER, message);
            }

            public void OnWebSocketReceiveBianry(WebSocket webSocket, byte[] data)
            {

            }

            public void OnWebSocketErrorDescription(WebSocket webSocket, string reason)
            {
                
            }

            public void Release()
            {
                rtcEnginePresenter.release();
                MessageComand.Release();
                DisConnect();
                WebSocketPresenter = null;
                MainViewPersenter = null;
                MessageComand = null;
                rtcEnginePresenter = null;
            }

            public void OnSendMessageClick()
            {
                string s = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"create\", \"info\":{\"context\":\"\",\"appId\":\"fda6a89b2857451f8d3479a2fda2fbdf\",\"handler\":\"\"},\"extra\":{}}";
                string s1 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"setChannelProfile\", \"info\":{\"profile\":\"1\"},\"extra\":{}}";
                string s2 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"setClientRole\", \"info\":{\"role\":\"0\"},\"extra\":{}}";
                string s3 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"setParameters\", \"info\":{\"parameter\":\"{\"rtc.log_filter\": 65535}\"},\"extra\":{}}";
                string s4 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"enableAudio\", \"info\":{\"parameter\":\"{\"rtc.log_filter\": 65535}\"},\"extra\":{}}";
                string s5 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"enableLocalAudio\", \"info\":{\"enabled\":\"true\"},\"extra\":{}}";
                string s6 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"disableAudio\", \"info\":{\"enabled\":\"true\"},\"extra\":{}}";
                string s7 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"setAudioProfile\", \"info\":{\"profile\":\"1\",\"scenario\":\"1\"},\"extra\":{}}";
                string s8 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"joinChannel\", \"info\":{\"token\":\"\",\"channelName\":\"123\",\"optionalInfo\":\"1\",\"optionalUid\":\"0\"},\"extra\":{}}";
                string s9 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"leaveChannel\", \"info\":{\"enabled\":\"true\"},\"extra\":{}}";
                string s10 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"setRemoteVoicePosition\", \"info\":{\"enabled\":\"true\"},\"extra\":{}}";
                Send(s);
                //Send(s1);
                //Send(s2);
                //Send(s3);
                //Send(s4);
                //Send(s5);
                //Send(s6);
                //Send(s7);
                //Send(s8);
                //Send(s9);
                //Send(s10);
            }

            public void OnJoinChannelClicked()
            {
                string s = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"createChannel\", \"info\":{\"channelId\":\"yifantt\"},\"extra\":{}}";
                string s1 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"joinChannel\", \"info\":{\"channelId\":\"yifantt\", \"token\":\"\",\"info\":\"1\",\"uid\":111111,\"autoSubscribeAudio\":true, \"autoSubscribeVideo\":true},\"extra\":{}}";
                string s2 = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"publish\", \"info\":{\"channelId\":\"yifantt\"},\"extra\":{}}";
                Send(s);
                Send(s1);
                Send(s2);
            }
            public void OnLeaveChannelClicked()
            {
                string s = "{\"type\":1, \"device\":\"Unity_1001\", \"cmd\" :\"leaveChannel\", \"info\":{\"channelId\":\"yifantt\"},\"extra\":{}}";
                Send(s);
            }
        } 
    }
}