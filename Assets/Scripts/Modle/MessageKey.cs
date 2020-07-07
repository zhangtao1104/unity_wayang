using System;

namespace agora
{
    namespace unity
    {
        public enum TYPE
        {
            CMD_MESSAGE = 1,
            CALLBACK_MESSAGE = 4,
            UPLOAD_MESSAGE = 5,
            NONAPI_MESSAGE = 6,
        }

        class MessageKey
        {
            public static string Type = "type";
            public static string Device = "device";
            public static string Cmd = "cmd";
            public static string Info = "info";
            public static string Extra = "extra"; 
        }
    }
}
