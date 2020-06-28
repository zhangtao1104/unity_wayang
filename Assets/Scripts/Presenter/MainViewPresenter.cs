using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace agora
{
    namespace unity
    {
        public class MainViewPersenter : IMainViewPersenter
        {
            private ApplicationContract ApplicationContract
            {
                get;
                set;
            }
            
            public MainView MainView
            {
                get;
                set;
            }

            public MainViewPersenter(MainView View)
            {
                MainView = View;
                ApplicationContract = ApplicationContract.GetInstance();
                ApplicationContract.SetMainViewPersenter(this);
            }

            public void OnAwake()
            {
                ApplicationContract.OnAwake();
            }

            public void OnStart()
            {
                ApplicationContract.OnStart();
            }

            public void OnUpdate()
            {
                ApplicationContract.OnUpdate();
            }

            public void OnApplicationPause(bool pause)
            {
                ApplicationContract.OnApplicationPause(pause);
            }

            public void OnApplicationFocus(bool focus)
            {
               ApplicationContract.OnApplicationFocus(focus); 
            }

            public void OnApplicationQuit()
            {
                ApplicationContract.OnApplicationQuit();
            }

            public void OnWebSocketConnect()
            {
                MainView.OnUpdateConnectServerUI(true);
            }

            public void OnWebSocketDisConnect()
            {
                MainView.OnUpdateConnectServerUI(false);
            }
            public void OnWebSocketError()
            {
                MainView.OnUpdateConnectServerUI(false);
            }

            public void OnDestroy()
            {
                ApplicationContract.OnDestroy();   
            }

            public void DisConnectWebSocket()
            {
                ApplicationContract.DisConnect();
            }

            public void ConnectWebSocket()
            {
                ApplicationContract.ConnectWebSocket();
            }

            public string GetSDKVersion()
            {
                return ApplicationContract.GetSDKVersion();
            }

            public void ResetWebSocket()
            {
                
                ApplicationContract.DisConnect();
                ApplicationContract.ConnectWebSocket();
            }

            public void Release()
            {
                ApplicationContract.Release();
                ApplicationContract = null;
            }

            public void OnSendMessage()
            {
               ApplicationContract.OnSendMessageClick(); 
            }

            public void OnJoinChannel()
            {
                ApplicationContract.OnJoinChannelClicked();
            }
            public void OnLeaveChannel()
            {
                ApplicationContract.OnLeaveChannelClicked();
            }
        }
    }
}
