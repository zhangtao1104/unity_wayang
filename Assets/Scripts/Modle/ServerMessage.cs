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

            public Int64 sequence
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

            public override string ToString() 
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
                if (sequence != null)
                {
                    stringBuilder.Append("sequence: " + sequence + ",");
                }
                if (info != null && info.ToString() != null)
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
