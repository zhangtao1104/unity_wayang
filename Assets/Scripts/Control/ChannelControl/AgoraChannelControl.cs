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
            private ChannelStreamViewManager channelStreamViewManager = new ChannelStreamViewManager();
            private RtcEngineControl rtcEngineControl;
            private readonly Dictionary<string, AgoraChannel> channelDictionary = new Dictionary<string, AgoraChannel>();

            public AgoraChannelControl(RtcEngineControl rtcEngineControl)
            {
                this.rtcEngineControl = rtcEngineControl;
            }

            public void SetRtcEngine(IRtcEngine rtcEngine)
            {
                mRtcEngine = rtcEngine;
            }

            
            private void SetChannelCallback(AgoraChannel channel)
            {
                channel.ChannelOnWarning = OnChannelOnWarningHandler;
                channel.ChannelOnError = OnChannelOnErrorHandler;
                channel.ChannelOnJoinChannelSuccess = OnChannelOnJoinChannelSuccessHandler;
                channel.ChannelOnReJoinChannelSuccess = OnChannelOnReJoinChannelSuccessHandler;
                channel.ChannelOnLeaveChannel = OnChannelOnLeaveChannelHandler;
                channel.ChannelOnClientRoleChanged = OnChannelOnClientRoleChangedHandler;
                channel.ChannelOnUserJoined = OnChannelOnUserJoinedHandler;
                channel.ChannelOnUserOffLine = OnChannelOnUserOffLineHandler;
                channel.ChannelOnConnectionLost = OnChannelOnConnectionLostHandler;
                channel.ChannelOnRequestToken = OnChannelOnRequestTokenHandler;
                channel.ChannelOnTokenPrivilegeWillExpire = OnChannelOnTokenPrivilegeWillExpireHandler;
                channel.ChannelOnRtcStats = OnChannelOnRtcStatsHandler;
                channel.ChannelOnNetworkQuality = OnChannelOnNetworkQualityHandler;
                channel.ChannelOnRemoteVideoStats = OnChannelOnRemoteVideoStatsHandler;
                channel.ChannelOnRemoteAudioStats = OnChannelOnRemoteAudioStatsHandler;
                channel.ChannelOnRemoteAudioStateChanged = OnChannelOnRemoteAudioStateChangedHandler;
                channel.ChannelOnActiveSpeaker = OnChannelOnActiveSpeakerHandler;
                channel.ChannelOnVideoSizeChanged = OnChannelOnVideoSizeChangedHandler;
                channel.ChannelOnRemoteVideoStateChanged = OnChannelOnRemoteVideoStateChangedHandler;
                channel.ChannelOnStreamMessage = OnChannelOnStreamMessageHandler;
                channel.ChannelOnStreamMessageError = OnChannelOnStreamMessageErrorHandler;
                channel.ChannelOnMediaRelayStateChanged = OnChannelOnMediaRelayStateChangedHandler;
                channel.ChannelOnMediaRelayEvent = OnChannelOnMediaRelayEventHandler;
                channel.ChannelOnRtmpStreamingStateChanged = OnChannelOnRtmpStreamingStateChangedHandler;
                channel.ChannelOnTranscodingUpdated = OnChannelOnTranscodingUpdatedHandler;
                channel.ChannelOnStreamInjectedStatus = OnChannelOnStreamInjectedStatusHandler;
                channel.ChannelOnRemoteSubscribeFallbackToAudioOnly = OnChannelOnRemoteSubscribeFallbackToAudioOnlyHandler;
                channel.ChannelOnConnectionStateChanged = OnChannelOnConnectionStateChangedHandler;
            }

            public ServerMessage createChannel(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];

                if (!channelDictionary.ContainsKey(channelId))
                {
                    AgoraChannel channel = new AgoraChannel(mRtcEngine, channelId);
                    SetChannelCallback(channel);
                    channelDictionary.Add(channelId, channel);
                }
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", 0 },
                    { "error", 0 }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage releaseChannel(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                if (channelDictionary.ContainsKey(channelId))
                {
                    AgoraChannel channel = channelDictionary[channelId];
                    channel.ReleaseChannel();
                    channelDictionary.Remove(channelId);
                }

                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", 0 },
                    { "error", 0 }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage joinChannel(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string token = (string)message.info["token"];
                string info = (string)message.info["info"];
                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                bool autoSubscribeAudio = (bool)message.info["autoSubscribeAudio"];
                bool autoSubscribeVideo = (bool)message.info["autoSubscribeVideo"];

                ChannelMediaOptions options = new ChannelMediaOptions(autoSubscribeAudio, autoSubscribeVideo);
                int ret = channel.JoinChannel(token, info, (uint)uid, options);
                channelStreamViewManager.ChannelAddLocalStreamView();

                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage joinChannelWithUserAccount(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string token = (string)message.info["token"];
                string userAccount = (string)message.info["userAccount"];
                bool autoSubscribeAudio = (bool)message.info["autoSubscribeAudio"];
                bool autoSubscribeVideo = (bool)message.info["autoSubscribeVideo"];

                ChannelMediaOptions options = new ChannelMediaOptions(autoSubscribeAudio, autoSubscribeVideo);
                int ret = channel.JoinChannelWithUserAccount(token, userAccount, options);
                channelStreamViewManager.ChannelAddLocalStreamView();
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage leaveChannel(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                int ret = channel.LeaveChannel();
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage publish(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                int ret = channel.Publish();
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage unpublish(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                int ret = channel.Unpublish();
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage channelId(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string ret = channel.ChannelId();
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", 0 }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getCallId(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string ret = channel.GetCallId();
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", 0 }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage renewToken(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string token = (string)message.info["token"];
                int ret = channel.RenewToken(token);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setEncryptionSecret(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string secret = (string)message.info["secret"];
                int ret = channel.SetEncryptionSecret(secret);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);

            }

            public ServerMessage setEncryptionMode(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string encryptionMode = (string)message.info["encryptionMode"];
                int ret = channel.SetEncryptionMode(encryptionMode);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setClientRole(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                int clientRole = (int)message.info["role"];
                int ret = channel.SetClientRole((CLIENT_ROLE_TYPE)clientRole);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setRemoteUserPriority(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                int userPriority = (int)message.info["userPriority"];

                int ret = channel.SetRemoteUserPriority((uint)uid, (PRIORITY_TYPE)userPriority);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setRemoteVoicePosition(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                double pan = (double)message.info["pan"];
                double gain = (double)message.info["gain"];
                int ret = channel.SetRemoteVoicePosition((uint)uid, pan, gain);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setRemoteRenderMode(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                int renderMode = (int)message.info["renderMode"];
                int mirrorMode = (int)message.info["mirrorMode"];
                int ret = channel.SetRemoteRenderMode((uint)uid, renderMode, mirrorMode);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setDefaultMuteAllRemoteAudioStreams(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                bool mute = (bool)message.info["mute"];
                int ret = channel.SetDefaultMuteAllRemoteAudioStreams(mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setDefaultMuteAllRemoteVideoStreams(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                bool mute = (bool)message.info["mute"];
                int ret = channel.SetDefaultMuteAllRemoteVideoStreams(mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage muteAllRemoteAudioStreams(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                bool mute = (bool)message.info["mute"];
                int ret = channel.MuteAllRemoteAudioStreams(mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage adjustUserPlaybackSignalVolume(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                long userId = JSON.ParseNumberToLong(message.info["userId"]);
                int volume = (int)message.info["volume"];
                int ret = channel.AdjustUserPlaybackSignalVolume((uint)userId, volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage muteRemoteAudioStream(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                long userId = JSON.ParseNumberToLong(message.info["userId"]);
                bool mute = (bool)message.info["mute"];
                int ret = channel.MuteRemoteAudioStream((uint)userId, mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage muteAllRemoteVideoStreams(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                bool mute = (bool)message.info["mute"];
                int ret = channel.MuteAllRemoteVideoStreams(mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage muteRemoteVideoStream(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                long userId = JSON.ParseNumberToLong(message.info["userId"]);
                bool mute = (bool)message.info["mute"];
                int ret = channel.MuteRemoteVideoStream((uint)userId, mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setRemoteVideoStreamType(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                long userId = JSON.ParseNumberToLong(message.info["userId"]);
                int streamType = (int)message.info["streamType"];
                int ret = channel.SetRemoteVideoStreamType((uint)userId, (REMOTE_VIDEO_STREAM_TYPE)streamType);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setRemoteDefaultVideoStreamType(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                int streamType = (int)message.info["streamType"];
                int ret = channel.SetRemoteDefaultVideoStreamType((REMOTE_VIDEO_STREAM_TYPE)streamType);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage createDataStream(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                bool reliable = (bool)message.info["reliable"];
                bool ordered = (bool)message.info["ordered"];
                int ret = channel.CreateDataStream(reliable, ordered);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage sendStreamMessage(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                int streamId = (int)message.info["streamId"];
                string data = (string)message.info["data"];
                long length = JSON.ParseNumberToLong(message.info["length"]);
                int ret = channel.SendStreamMessage(streamId, data, length);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage addPublishStreamUrl(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string url = (string)message.info["url"];
                bool transcodingEnabled = (bool)message.info["transcodingEnabled"];
                int ret = channel.AddPublishStreamUrl(url, transcodingEnabled);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage removePublishStreamUrl(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string url = (string)message.info["url"];
                int ret = channel.RemovePublishStreamUrl(url);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setLiveTranscoding(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                LiveTranscoding liveTranscoding = JSON.JsonToObject<LiveTranscoding>(message.info["liveTranscoding"].ToJson());
                int ret = channel.SetLiveTranscoding(liveTranscoding);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage addInjectStreamUrl(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string url = (string)message.info["url"];
                InjectStreamConfig streamConfig = JSON.JsonToObject<InjectStreamConfig>(message.info["streamConfig"].ToJson());
                int ret = channel.AddInjectStreamUrl(url, streamConfig);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage removeInjectStreamUrl(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                string url = (string)message.info["url"];
                int ret = channel.RemoveInjectStreamUrl(url);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage startChannelMediaRelay(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                ChannelMediaRelayConfiguration channelMediaRelayConfiguration = JSON.JsonToObject<ChannelMediaRelayConfiguration>(message.info["channelMediaRelayConfiguration"].ToJson());
                int ret = channel.StartChannelMediaRelay(channelMediaRelayConfiguration);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage updateChannelMediaRelay(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                ChannelMediaRelayConfiguration channelMediaRelayConfiguration = JSON.JsonToObject<ChannelMediaRelayConfiguration>(message.info["channelMediaRelayConfiguration"].ToJson());
                int ret = channel.UpdateChannelMediaRelay(channelMediaRelayConfiguration);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage stopChannelMediaRelay(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                int ret = channel.StopChannelMediaRelay();
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", ret }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getConnectionState(ServerMessage message)
            {
                string channelId = (string)message.info["channelId"];
                AgoraChannel channel = channelDictionary[channelId];

                int ret = (int)channel.GetConnectionState();
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "return", ret },
                    { "error", 0 }
                };
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            void OnChannelOnWarningHandler(string channelId, int warn, string message)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "warn", warn },
                    { "message", message }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnWarningHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnErrorHandler(string channelId, int err, string message)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "err", err },
                    { "message", message }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnErrorHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnJoinChannelSuccessHandler(string channelId, uint uid, int elapsed)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "elapsed", elapsed }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnJoinChannelSuccessHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnReJoinChannelSuccessHandler(string channelId, uint uid, int elapsed)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "elapsed", elapsed }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnReJoinChannelSuccessHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnLeaveChannelHandler(string channelId, RtcStats rtcStats)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnLeaveChannelHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);

                channelStreamViewManager.ChannelRemoveAllRemoteStreamViews(channelId);
                channelStreamViewManager.ChannelRemoveLocalStreamView();
            }

            void OnChannelOnClientRoleChangedHandler(string channelId, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "oldRole", (int)oldRole },
                    { "newRole", (int)newRole }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnClientRoleChangedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnUserJoinedHandler(string channelId, uint uid, int elapsed)
            {
                channelStreamViewManager.ChannelAddRemoteStreamView(channelId, uid);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "elapsed", elapsed }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnUserJoinedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnUserOffLineHandler(string channelId, uint uid, USER_OFFLINE_REASON reason)
            {
                channelStreamViewManager.ChannelRemoveRemoteStreamView(channelId, uid);
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "reason", (int)reason }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnUserOffLineHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnConnectionLostHandler(string channelId)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnConnectionLostHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnRequestTokenHandler(string channelId)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnRequestTokenHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnTokenPrivilegeWillExpireHandler(string channelId, string token)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "token", token }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnTokenPrivilegeWillExpireHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnRtcStatsHandler(string channelId, RtcStats rtcStats)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "duration", rtcStats.duration },
                    { "txBytes", rtcStats.txBytes },
                    { "rxBytes", rtcStats.rxBytes },
                    { "txAudioBytes", rtcStats.txAudioBytes },
                    { "txVideoBytes", rtcStats.txVideoBytes },
                    { "rxAudioBytes", rtcStats.rxAudioBytes },
                    { "rxVideoBytes", rtcStats.rxVideoBytes },
                    { "txKBitRate", rtcStats.txKBitRate },
                    { "rxKBitRate", rtcStats.rxKBitRate },
                    { "rxAudioKBitRate", rtcStats.rxAudioKBitRate },
                    { "txAudioKBitRate", rtcStats.txAudioKBitRate },
                    { "rxVideoKBitRate", rtcStats.rxVideoKBitRate },
                    { "txVideoKBitRate", rtcStats.txVideoKBitRate },
                    { "lastmileDelay", (int)rtcStats.lastmileDelay },
                    { "txPacketLossRate", (int)rtcStats.txPacketLossRate },
                    { "rxPacketLossRate", (int)rtcStats.rxPacketLossRate },
                    { "userCount", rtcStats.userCount },
                    { "cpuAppUsage", rtcStats.cpuAppUsage },
                    { "cpuTotalUsage", rtcStats.cpuTotalUsage },
                    { "gatewayRtt", rtcStats.gatewayRtt },
                    { "memoryAppUsageRatio", rtcStats.memoryAppUsageRatio },
                    { "memoryTotalUsageRatio", rtcStats.memoryTotalUsageRatio },
                    { "memoryAppUsageInKbytes", rtcStats.memoryAppUsageInKbytes }

                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnRtcStatsHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnNetworkQualityHandler(string channelId, uint uid, int txQuality, int rxQuality)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "txQuality", txQuality },
                    { "rxQuality", rxQuality }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnNetworkQualityHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnRemoteVideoStatsHandler(string channelId, RemoteVideoStats remoteVideoStats)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", remoteVideoStats.uid },
                    { "delay", remoteVideoStats.delay },
                    { "width", remoteVideoStats.width },
                    { "height", remoteVideoStats.height },
                    { "receivedBitrate", remoteVideoStats.receivedBitrate },
                    { "decoderOutputFrameRate", remoteVideoStats.decoderOutputFrameRate },
                    { "rendererOutputFrameRate", remoteVideoStats.rendererOutputFrameRate },
                    { "packetLossRate", remoteVideoStats.packetLossRate },
                    { "rxStreamType", (int)remoteVideoStats.rxStreamType },
                    { "totalFrozenTime", remoteVideoStats.totalFrozenTime },
                    { "frozenRate", remoteVideoStats.frozenRate },
                    { "totalActiveTime", remoteVideoStats.totalActiveTime }

                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnRemoteVideoStatsHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnRemoteAudioStatsHandler(string channelId, RemoteAudioStats remoteAudioStats)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", remoteAudioStats.uid },
                    { "quality", remoteAudioStats.quality },
                    { "networkTransportDelay", remoteAudioStats.networkTransportDelay },
                    { "jitterBufferDelay", remoteAudioStats.jitterBufferDelay },
                    { "audioLossRate", remoteAudioStats.audioLossRate },
                    { "numChannels", remoteAudioStats.numChannels },
                    { "receivedSampleRate", remoteAudioStats.receivedSampleRate },
                    { "receivedBitrate", remoteAudioStats.receivedBitrate },
                    { "totalFrozenTime", remoteAudioStats.totalFrozenTime },
                    { "frozenRate", remoteAudioStats.frozenRate }

                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnRemoteAudioStatsHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnRemoteAudioStateChangedHandler(string channelId, uint uid, REMOTE_AUDIO_STATE state, REMOTE_AUDIO_STATE_REASON reason, int elapsed)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "state", (int)state },
                    { "reason", (int)reason },
                    { "elapsed", elapsed }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnRemoteAudioStateChangedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnActiveSpeakerHandler(string channelId, uint uid)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnActiveSpeakerHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnVideoSizeChangedHandler(string channelId, uint uid, int width, int height, int rotation)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "width", width },
                    { "height", height },
                    { "rotation", rotation }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnVideoSizeChangedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnRemoteVideoStateChangedHandler(string channelId, uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "state", (int)state },
                    { "reason", (int)reason },
                    { "elapsed", elapsed }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnRemoteVideoStateChangedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnStreamMessageHandler(string channelId, uint uid, int streamId, string data, int length)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "streamId", streamId },
                    { "data", data },
                    { "length", length }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnStreamMessageHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnStreamMessageErrorHandler(string channelId, uint uid, int streamId, int code, int missed, int cached)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "streamId", streamId },
                    { "code", code },
                    { "missed", missed },
                    { "cached", cached }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnStreamMessageErrorHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnMediaRelayStateChangedHandler(string channelId, CHANNEL_MEDIA_RELAY_STATE state, CHANNEL_MEDIA_RELAY_ERROR code)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "state", (int)state },
                    { "code", (int)code }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnMediaRelayStateChangedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnMediaRelayEventHandler(string channelId, CHANNEL_MEDIA_RELAY_EVENT events)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "events", (int)events }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnMediaRelayEventHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnRtmpStreamingStateChangedHandler(string channelId, string url, RTMP_STREAM_PUBLISH_STATE state, RTMP_STREAM_PUBLISH_ERROR errCode)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "url", url },
                    { "state", (int)state },
                    { "errCode", (int)errCode }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnRtmpStreamingStateChangedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnTranscodingUpdatedHandler(string channelId)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnTranscodingUpdatedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnStreamInjectedStatusHandler(string channelId, string url, uint uid, int status)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "url", url },
                    { "uid", uid },
                    { "status", status }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnStreamInjectedStatusHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnRemoteSubscribeFallbackToAudioOnlyHandler(string channelId, uint uid, bool isFallbackOrRecover)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "uid", uid },
                    { "isFallbackOrRecover", isFallbackOrRecover }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnRemoteSubscribeFallbackToAudioOnlyHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }

            void OnChannelOnConnectionStateChangedHandler(string channelId, CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "channelId", channelId },
                    { "state", (int)state },
                    { "reason", (int)reason }
                };
                ServerMessage msg = ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelOnConnectionStateChangedHandler", 0, infoData, null);
                rtcEngineControl.UploadMessageToServer(msg);
            }
        }
    }
}