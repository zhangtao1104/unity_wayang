using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;

namespace agora
{
    namespace unity
    {
        public class AgoraChannelControl
        {
            private IRtcEngine mRtcEngine;
            private readonly Dictionary<string, AgoraChannel> channelDictionary = new Dictionary<string, AgoraChannel>();
            public void SetRtcEngine(IRtcEngine rtcEngine)
            {
                mRtcEngine = rtcEngine;
            }

            

            public ServerMessage createChannel(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = new AgoraChannel(mRtcEngine, channelId);

                channelDictionary.Add(channelId, channel);

                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            //public ServerMessage joinChannel(ServerMessage message)
            //{
            //    //string channelId = (string)message.info["channelId"];

            //    //string token = (string)(message.info["token"]);
            //    //string channelId = (string)(message.info["channelName"]);
            //    //string optionalInfo = (string)(message.info["optionalInfo"]);
            //    //long optionalUid = JSON.ParseNumberToLong(message.info["optionalUid"]);
            //    //int ret = mRtcEngine.JoinChannelByKey(token, channelName, optionalInfo, (uint)optionalUid);
            //    //streamViewManager.AddLocalStreamView();

            //    //Dictionary<string, object> infoData = new Dictionary<string, object>();
            //    //infoData.Add("error", ret);
            //    return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            //}
        }
    }
}