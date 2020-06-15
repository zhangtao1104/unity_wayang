using System;

namespace agora
{
    namespace unity
    {
        public class Application
        {
            public static string DEFAULT_WEB_SOCKET_URL
            {
                get 
                {
                    return "ws://114.236.93.153:8083/iov/websocket/dual?topic=";
                }
            }

            public static string DEFAULT_DEVICE_ID
            {
                get
                {
                    return "Unity_1001";
                }
            }

            public static LoggerHelper Logger = new LoggerHelper();
            public static string AppVersion
            {
                get 
                {
                    return "v1.0.0";
                }
            }

            private static string appId = "5db0d12c40354100abd7a8a0adaa1fb8";
            public static string AppId 
            {
                get 
                {
                    return appId;
                }

                set
                {
                    appId = value;
                }
            }

            private static string webSocket_Url = "ws://114.236.93.153:8083/iov/websocket/dual?topic=";
            public static string WebSocket_Url 
            {
                get
                {
                    return webSocket_Url;
                }

                set
                {
                    webSocket_Url = value;
                }
            }

            private static string deviceID = "Unity_1001";
            public static string DeviceID
            {
                get
                {
                    return deviceID;
                }

                set
                {
                    deviceID = value;
                }
            }

            public static string Wayang_Title
            {
                get
                {
                    return "Wayang:";
                }
            }
        }
    }
}
