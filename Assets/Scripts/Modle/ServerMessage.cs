using LitJson;
using System;
using System.Text;

namespace agora
{
    namespace unity
    {
        public class ServerMessage
        {
            public int type
            {
                get;
                set;
            }

            public string device
            {
                get;
                set;
            }

            public string cmd 
            {
                get;
                set;
            }

            public JsonData info
            {
                get;
                set;
            }

            public JsonData extra
            {
                get;
                set;
            }

            public string ToString() 
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("type: " + type + ",");
                if (type != null)
                {
                    stringBuilder.Append("device: " + device + ",");
                }
                if (cmd != null)
                {
                    stringBuilder.Append("cmd: " + cmd + ",");
                }
                if (info.ToString() != null)
                {
                    stringBuilder.Append("info: " + info.ToString() + ",");
                }
                if (extra != null)
                {
                    stringBuilder.Append("extra: " + extra.ToString() + ",");
                }
                return stringBuilder.ToString();
            }
        }
    }
}
