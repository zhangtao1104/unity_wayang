namespace agora
{
    namespace unity
    {
        public interface IMainViewPersenter
        {
            void OnAwake();
            void OnStart();
            void OnUpdate();
            void OnApplicationPause(bool pause);
            void OnApplicationFocus(bool focus);
            void OnApplicationQuit();
            void OnDestroy();
            void Release();
            void OnWebSocketConnect();
            void OnWebSocketDisConnect();
            void OnWebSocketError();
            void ConnectWebSocket();
            void DisConnectWebSocket();
            string GetSDKVersion();
            void ResetWebSocket();
            void OnSendMessage();
        }
    }
}
