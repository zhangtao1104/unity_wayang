using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace agora 
{
    namespace unity
    {
        public class ServerMessageFactory
        {
            private static string TAG = "ServerMessageFactory";
            public static ServerMessage CreateServerMessage(TYPE type, string device, string cmd, Dictionary<string, object> info, Dictionary<string, object> extra) 
            {
                Application.Logger.Info(TAG, System.Reflection.MethodBase.GetCurrentMethod().Name);
                ServerMessage message = new ServerMessage();
                message.type = (int)type;
                message.device = device;
                message.cmd = cmd;
                if (info != null)
                {
                    message.info = new JsonData();
                    foreach(KeyValuePair<string, object> a in info)
                    {
                        if (a.Value is int)
                        {
                            message.info[a.Key] = (int)a.Value;
                        }
                        else if (a.Value is string)
                        {
                            message.info[a.Key] = (string)a.Value;
                        }
                        else if (a.Value is bool)
                        {
                            message.info[a.Key] = (bool)a.Value;
                        }
                        else if (a.Value is float)
                        {
                            message.info[a.Key] = (float)a.Value;
                        }
                    }
                }
                if (extra != null)
                {
                    message.extra = new JsonData();
                    foreach(KeyValuePair<string, object> a in extra)
                    {
                        if (a.Value is int)
                        {
                            message.extra[a.Key] = (int)a.Value;
                        }
                        else if (a.Value is string)
                        {
                            message.extra[a.Key] = (string)a.Value;
                        }
                        else if (a.Value is bool)
                        {
                            message.extra[a.Key] = (bool)a.Value;
                        }
                        else if (a.Value is float)
                        {
                            message.extra[a.Key] = (float)a.Value;
                        }
                    }
                }
                return message;
            }
        }
    }
}
