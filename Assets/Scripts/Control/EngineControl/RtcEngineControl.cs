using System;
using System.Reflection;
using agora_gaming_rtc;
using agora_utilities;
using LitJson;
using System.Collections.Generic;
using UnityEngine.UI;

namespace agora
{
    namespace unity
    {
        public class RtcEngineControl : BaseInterface
        {
            private string TAG = "RtcEngineControl";
            private IRtcEngine mRtcEngine = null;
            private IAudioEffectManager audioEffectManager = null;
            private IVideoDeviceManager videoDeviceManager = null;
            private IAudioRecordingDeviceManager audioRecordingDeviceManager = null;
            private IAudioPlaybackDeviceManager audioPlaybackDeviceManager = null;
            private AudioRawDataManager audioRawDataManager = null;
            private VideoRawDataManager videoRawDataManager = null;
            private PacketObserver packetObserver = null;
            private MetadataObserver metadataObserver = null;
            private StreamViewManager streamViewManager = null;
            private AgoraChannelControl agoraChannelControl = null;
            private RtcEnginePresenter rtcEnginePresenter {
                get;
                set;
            }

            public MessageComand messageComand 
            {
                get;
                set;
            }

            public RtcEngineControl()
            {
                agoraChannelControl = new AgoraChannelControl(this);
            }

            public void SetRtcEnginePresenter(RtcEnginePresenter presenter)
            {
                rtcEnginePresenter = presenter;
            }

            public void OnExecuteServerMessage(ServerMessage message)
            {
                try 
                {
                    if (message != null)
                    {
                        //Application.Logger.Info(TAG, "OnExecuteServerMessage message : " + message.ToString());

                        if (message.info != null && message.info.Keys.Contains("channelId"))
                        {
                            string methodName = message.cmd;
                            MethodInfo method = agoraChannelControl.GetType().GetMethod(methodName);
                            Object info = method.Invoke(agoraChannelControl, new Object[] { message });
                            UploadMessageToServer((ServerMessage)info);
                        }
                        else
                        {
                            string methodName = message.cmd;
                            MethodInfo method = this.GetType().GetMethod(methodName);
                            Object info = method.Invoke(this, new Object[] { message });
                            UploadMessageToServer((ServerMessage)info);
                        }
                    }
                }
                catch (Exception e)
                {
                    Application.Logger.Error(TAG, "OnExecuteServerMessage catch exception " + e);
                }
            }

            public void UploadMessageToServer(ServerMessage message)
            {
                rtcEnginePresenter.UploadMessageToServer(message);
                //Application.Logger.Info(TAG, "UploadMessageToServer message :" + message.ToString());
            }

            public ServerMessage create(ServerMessage message)
            {
                string appId = (string)(message.info["appId"]);
                if (message.info.Keys.Contains("areaCode"))
                {
                    int areaCode = (int)message.info["areaCode"];
                    RtcEngineConfig config = new RtcEngineConfig(appId, (AREA_CODE)areaCode);
                    mRtcEngine = IRtcEngine.GetEngine(config);
                }
                else
                {
                    mRtcEngine = IRtcEngine.GetEngine(appId);
                }

                //mRtcEngine.EnableAudio();
                //mRtcEngine.EnableVideo();
                //mRtcEngine.EnableVideoObserver();
                //mRtcEngine.SetMultiChannelWant(true);

                audioEffectManager = AudioEffectManagerImpl.GetInstance(mRtcEngine);
                videoDeviceManager = VideoDeviceManager.GetInstance(mRtcEngine);
                audioRecordingDeviceManager = AudioRecordingDeviceManager.GetInstance(mRtcEngine);
                audioPlaybackDeviceManager = AudioPlaybackDeviceManager.GetInstance(mRtcEngine);
                audioRawDataManager = AudioRawDataManager.GetInstance(mRtcEngine);
                videoRawDataManager = VideoRawDataManager.GetInstance(mRtcEngine);
                packetObserver = PacketObserver.GetInstance(mRtcEngine);
                metadataObserver = MetadataObserver.GetInstance(mRtcEngine);
                streamViewManager = new StreamViewManager();
                agoraChannelControl.SetRtcEngine(mRtcEngine);
                InitCallback();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", 0);
                infoData.Add("error", 0);
                mRtcEngine.SetLogFile("/Users/dyf/Documents/log.txt");
                mRtcEngine.SetParameters("{\"rtc.log_filter\": 65535}");
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public void InitCallback() 
            {
                mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccessHandler;
                mRtcEngine.OnLeaveChannel = OnLeaveChannelHandler;
                mRtcEngine.OnReJoinChannelSuccess = OnReJoinChannelSuccessHandler;
                mRtcEngine.OnConnectionLost = OnConnectionLostHandler;
                mRtcEngine.OnConnectionInterrupted = OnConnectionInterruptedHandler;
                mRtcEngine.OnRequestToken = OnRequestTokenHandler;
                mRtcEngine.OnUserJoined = OnUserJoinedHandler;
                mRtcEngine.OnUserOffline = OnUserOfflineHandler;
                mRtcEngine.OnVolumeIndication = OnVolumeIndicationHandler;
                mRtcEngine.OnUserMutedAudio = OnUserMutedAudioHandler;
                mRtcEngine.OnWarning = OnSDKWarningHandler;
                mRtcEngine.OnError = OnSDKErrorHandler;
                mRtcEngine.OnRtcStats = OnRtcStatsHandler;
                mRtcEngine.OnAudioMixingFinished = OnAudioMixingFinishedHandler;
                mRtcEngine.OnAudioRouteChanged = OnAudioRouteChangedHandler;
                mRtcEngine.OnFirstRemoteVideoDecoded = OnFirstRemoteVideoDecodedHandler;
                mRtcEngine.OnVideoSizeChanged = OnVideoSizeChangedHandler;
                mRtcEngine.OnClientRoleChanged = OnClientRoleChangedHandler;
                mRtcEngine.OnUserMuteVideo = OnUserMuteVideoHandler;
                mRtcEngine.OnMicrophoneEnabled = OnMicrophoneEnabledHandler;
                mRtcEngine.OnApiExecuted = OnApiExecutedHandler;
                mRtcEngine.OnLastmileQuality = OnLastmileQualityHandler;
                mRtcEngine.OnFirstLocalAudioFrame = OnFirstLocalAudioFrameHandler;
                mRtcEngine.OnFirstRemoteAudioFrame = OnFirstRemoteAudioFrameHandler;
                mRtcEngine.OnAudioQuality = OnAudioQualityHandler;
                mRtcEngine.OnStreamInjectedStatus = OnStreamInjectedStatusHandler;
                mRtcEngine.OnStreamUnpublished = OnStreamUnpublishedHandler;
                mRtcEngine.OnStreamPublished = OnStreamPublishedHandler;
                mRtcEngine.OnStreamMessageError = OnStreamMessageErrorHandler;
                mRtcEngine.OnStreamMessage = OnStreamMessageHandler;
                mRtcEngine.OnConnectionBanned = OnConnectionBannedHandler;
                mRtcEngine.OnConnectionStateChanged = OnConnectionStateChangedHandler;
                mRtcEngine.OnTokenPrivilegeWillExpire = OnTokenPrivilegeWillExpireHandler;
                mRtcEngine.OnActiveSpeaker = OnActiveSpeakerHandler;
                mRtcEngine.OnVideoStopped = OnVideoStoppedHandler;
                mRtcEngine.OnFirstLocalVideoFrame = OnFirstLocalVideoFrameHandler;
                mRtcEngine.OnFirstRemoteVideoFrame = OnFirstRemoteVideoFrameHandler;
                mRtcEngine.OnUserEnableVideo = OnUserEnableVideoHandler;
                mRtcEngine.OnUserEnableLocalVideo = OnUserEnableLocalVideoHandler;
                mRtcEngine.OnRemoteVideoStateChanged = OnRemoteVideoStateChangedHandler;
                mRtcEngine.OnLocalPublishFallbackToAudioOnly = OnLocalPublishFallbackToAudioOnlyHandler;
                mRtcEngine.OnRemoteSubscribeFallbackToAudioOnly = OnRemoteSubscribeFallbackToAudioOnlyHandler;
                mRtcEngine.OnNetworkQuality = OnNetworkQualityHandler;
                mRtcEngine.OnLocalVideoStats = OnLocalVideoStatsHandler;
                mRtcEngine.OnRemoteVideoStats = OnRemoteVideoStatsHandler;
                mRtcEngine.OnRemoteAudioStats = OnRemoteAudioStatsHandler;
                mRtcEngine.OnAudioDeviceStateChanged = OnAudioDeviceStateChangedHandler;
                mRtcEngine.OnCameraReady = OnCameraReadyHandler;
                mRtcEngine.OnCameraFocusAreaChanged = OnCameraFocusAreaChangedHandler;
                mRtcEngine.OnCameraExposureAreaChanged = OnCameraExposureAreaChangedHandler;
                mRtcEngine.OnRemoteAudioMixingBegin = OnRemoteAudioMixingBeginHandler;
                mRtcEngine.OnRemoteAudioMixingEnd = OnRemoteAudioMixingEndHandler;
                mRtcEngine.OnAudioEffectFinished = OnAudioEffectFinishedHandler;
                mRtcEngine.OnVideoDeviceStateChanged = OnVideoDeviceStateChangedHandler;
                mRtcEngine.OnRemoteVideoTransportStats = OnRemoteVideoTransportStatsHandler;
                mRtcEngine.OnRemoteAudioTransportStats = OnRemoteAudioTransportStatsHandler;
                mRtcEngine.OnTranscodingUpdated = OnTranscodingUpdatedHandler;
                mRtcEngine.OnAudioDeviceVolumeChanged = OnAudioDeviceVolumeChangedHandler;
                mRtcEngine.OnMediaEngineStartCallSuccess = OnMediaEngineStartCallSuccessHandler;
                mRtcEngine.OnMediaEngineLoadSuccess = OnMediaEngineLoadSuccessHandler;
                mRtcEngine.OnAudioMixingStateChanged = OnAudioMixingStateChangedHandler;
                mRtcEngine.OnFirstRemoteAudioDecoded = OnFirstRemoteAudioDecodedHandler;
                mRtcEngine.OnLocalVideoStateChanged = OnLocalVideoStateChangedHandler;
                mRtcEngine.OnRtmpStreamingStateChanged = OnRtmpStreamingStateChangedHandler;
                mRtcEngine.OnNetworkTypeChanged = OnNetworkTypeChangedHandler;
                mRtcEngine.OnLastmileProbeResult = OnLastmileProbeResultHandler;
                mRtcEngine.OnLocalUserRegistered = OnLocalUserRegisteredHandler;
                mRtcEngine.OnUserInfoUpdated = OnUserInfoUpdatedHandler;
                mRtcEngine.OnLocalAudioStateChanged = OnLocalAudioStateChangedHandler;
                mRtcEngine.OnRemoteAudioStateChanged = OnRemoteAudioStateChangedHandler;
                mRtcEngine.OnLocalAudioStats = OnLocalAudioStatsHandler;
                mRtcEngine.OnChannelMediaRelayStateChanged = OnChannelMediaRelayStateChangedHandler;
                mRtcEngine.OnChannelMediaRelayEvent = OnChannelMediaRelayEventHandler;
            }

            public void OnAudioRawDataCallback()
            {
                audioRawDataManager.SetOnMixedAudioFrameCallback(OnMixedAudioFrameHandler);
                audioRawDataManager.SetOnPlaybackAudioFrameBeforeMixingCallback(OnPlaybackAudioFrameBeforeMixingHandler);
                audioRawDataManager.SetOnPlaybackAudioFrameCallback(OnPlaybackAudioFrameHandler);
                audioRawDataManager.SetOnRecordAudioFrameCallback(OnRecordAudioFrameHandler);
            }

            public void OnVideoRawDataCallback()
            {
                videoRawDataManager.SetOnCaptureVideoFrameCallback(OnCaptureVideoFrameHandler);
                videoRawDataManager.SetOnRenderVideoFrameCallback(OnRenderVideoFrameHandler);
            }

            public ServerMessage destroy(ServerMessage message)
            {
                Application.Logger.Info(TAG, "destroy");
                IRtcEngine.Destroy();
                mRtcEngine = null;
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", 0);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public void Destroy()
            {
                if (mRtcEngine != null)
                {
                    Application.Logger.Info(TAG, "Destroy");
                    IRtcEngine.Destroy();
                    mRtcEngine = null;
                }
            }

            public static string GetSdkVersion()
            {
                return IRtcEngine.GetSdkVersion();
            }

            public ServerMessage getDocumentPath(ServerMessage message)
            {
                string docPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", docPath);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.NONAPI_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getSdkVersion(ServerMessage message)
            {
                string version = IRtcEngine.GetSdkVersion();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", version);
                infoData.Add("version", version);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setMultiChannelWant(ServerMessage message)
            {
                bool want = (bool)message.info["want"];
                int ret = mRtcEngine.SetMultiChannelWant(want);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage joinChannel(ServerMessage message)
            {
                string token = (string)(message.info["token"]);
                string channelName = (string)(message.info["channelName"]);
                string optionalInfo = (string)(message.info["optionalInfo"]);
                long optionalUid = JSON.ParseNumberToLong(message.info["optionalUid"]);
                int ret = mRtcEngine.JoinChannelByKey(token, channelName, optionalInfo, (uint)optionalUid);
                streamViewManager.AddLocalStreamView();
                
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage setLocalVoicePitch(ServerMessage message)
            {
                double pitch = (double)(message.info["pitch"]);
                int ret = mRtcEngine.SetLocalVoicePitch(pitch);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
                     
            public ServerMessage setRemoteVoicePosition(ServerMessage message)
            {
                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                double pan = (double)(message.info["pan"]);
                double gain = (double)(message.info["gain"]);
                int ret = audioEffectManager.SetRemoteVoicePosition((uint)uid, pan, gain);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage setVoiceOnlyMode(ServerMessage message)
            {
                return null;
            }  

            public ServerMessage leaveChannel(ServerMessage message)
            {
                streamViewManager.RemoveLocalStreamView();
                int ret = mRtcEngine.LeaveChannel();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage enableLastmileTest(ServerMessage message)
            {
                int ret = mRtcEngine.EnableLastmileTest();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
            public ServerMessage disableLastmileTest(ServerMessage message)
            {
                int ret = mRtcEngine.DisableLastmileTest();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   

            public ServerMessage enableVideo(ServerMessage message)
            {
                int ret = mRtcEngine.EnableVideo();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage enableVideoObserver(ServerMessage message)
            {
                int ret = mRtcEngine.EnableVideoObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage disableVideo(ServerMessage message)
            {
                int ret = mRtcEngine.DisableVideo();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           

            public ServerMessage disableVideoObserver(ServerMessage message)
            {
                int ret = mRtcEngine.DisableVideoObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage enableLocalVideo(ServerMessage message)
            {
                bool enabled = (bool)(message.info["enabled"]);
                int ret = mRtcEngine.EnableLocalVideo(enabled);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   

            public ServerMessage enableLocalAudio(ServerMessage message)
            {
                bool enabled = (bool)(message.info["enabled"]);
                int ret = mRtcEngine.EnableLocalAudio(enabled);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            

            public ServerMessage startPreview(ServerMessage message)
            {
                int ret = mRtcEngine.StartPreview();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 
                       
            public ServerMessage stopPreview(ServerMessage message)
            {
                int ret = mRtcEngine.StopPreview();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage enableAudio(ServerMessage message)
            {
                int ret = mRtcEngine.EnableAudio();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage disableAudio(ServerMessage message)
            {
                int ret = mRtcEngine.DisableAudio();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage setParameters(ServerMessage message)
            {
                string parameter = (string)(message.info["parameter"]);
                int ret = mRtcEngine.SetParameters(parameter);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   

            public ServerMessage getCallId(ServerMessage message)
            {
                string ret = (string)mRtcEngine.GetCallId();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            // caller free the returned char * (through freeObject)

            public ServerMessage rate(ServerMessage message)
            {
                string callId = (string)(message.info["callId"]);
                int rating = (int)(message.info["rating"]);
                string description = (string)(message.info["description"]);
                int ret = mRtcEngine.Rate(callId, rating, description);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage complain(ServerMessage message)
            {
                string callId = (string)(message.info["callId"]);
                string description = (string)(message.info["description"]);
                int ret = mRtcEngine.Complain(callId, description);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage setEnableSpeakerphone(ServerMessage message)
            {
                bool enabled = (bool)(message.info["enabled"]);
                int ret = mRtcEngine.SetEnableSpeakerphone(enabled);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage isSpeakerphoneEnabled(ServerMessage message)
            {
                bool enabled = mRtcEngine.IsSpeakerphoneEnabled();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", enabled);
                infoData.Add("enable", enabled);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage setDefaultAudioRoutetoSpeakerphone(ServerMessage message)
            {
                bool enabled = (bool)(message.info["defaultToSpeaker"]); 
                int ret = mRtcEngine.SetDefaultAudioRouteToSpeakerphone(enabled);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage enableAudioVolumeIndication(ServerMessage message)
            {
                int interval = (int)(message.info["interval"]);
                int smooth = (int)message.info["smooth"];
                bool report_vad = (bool)message.info["report_vad"];
                int ret = mRtcEngine.EnableAudioVolumeIndication(interval, smooth, report_vad);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }        
            public ServerMessage startAudioRecording(ServerMessage message)
            {
                string filePath = (string)message.info["filePath"];
                AUDIO_RECORDING_QUALITY_TYPE quality = (AUDIO_RECORDING_QUALITY_TYPE)(int)(message.info["quality"]);
                int ret = mRtcEngine.StartAudioRecording(filePath, quality);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }         
            public ServerMessage startAudioRecording2(ServerMessage message)
            {
                return null;
            }           
            public ServerMessage stopAudioRecording(ServerMessage message)
            {
                int ret = mRtcEngine.StopAudioRecording();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage startAudioMixing(ServerMessage message)
            {
                string filePath = (string)message.info["filePath"];
                bool loopback = (bool)message.info["loopback"];
                bool replace = (bool)message.info["replace"];
                int cycle = (int)message.info["cycle"];
                int ret = mRtcEngine.StartAudioMixing(filePath, loopback, replace, cycle);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage stopAudioMixing(ServerMessage message)
            {
                int ret = mRtcEngine.StopAudioMixing();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage pauseAudioMixing(ServerMessage message)
            {
                int ret = mRtcEngine.PauseAudioMixing();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage resumeAudioMixing(ServerMessage message)
            {
                int ret = mRtcEngine.ResumeAudioMixing();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage adjustAudioMixingVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int ret = mRtcEngine.AdjustAudioMixingVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage getAudioMixingDuration(ServerMessage message)
            {
                int ret = mRtcEngine.GetAudioMixingDuration();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getAudioMixingCurrentPosition(ServerMessage message)
            {
                int ret = mRtcEngine.GetAudioMixingCurrentPosition();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("position", ret);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage muteLocalAudioStream(ServerMessage message)
            {
                bool muted = (bool)message.info["muted"];
                int ret = mRtcEngine.MuteLocalAudioStream(muted);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage muteAllRemoteAudioStreams(ServerMessage message)
            {
                bool muted = (bool)message.info["muted"];
                int ret = mRtcEngine.MuteAllRemoteAudioStreams(muted);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage muteRemoteAudioStream(ServerMessage message)
            {
                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                bool muted = (bool)message.info["muted"];
                int ret = mRtcEngine.MuteRemoteAudioStream((uint)uid, muted);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage switchCamera(ServerMessage message)
            {
                int ret = mRtcEngine.SwitchCamera();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setVideoProfile(ServerMessage message)
            {
                int profile = (int)message.info["profile"];
                bool swap = (bool)message.info["swapWidthAndHeight"];
                int ret = mRtcEngine.SetVideoProfile((VIDEO_PROFILE_TYPE)profile, swap);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage muteLocalVideoStream(ServerMessage message)
            {
                bool muted = (bool)message.info["muted"];
                int ret = mRtcEngine.MuteLocalVideoStream(muted);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage muteAllRemoteVideoStreams(ServerMessage message)
            {
                bool muted = (bool)message.info["muted"];
                int ret = mRtcEngine.MuteAllRemoteVideoStreams(muted);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage muteRemoteVideoStream(ServerMessage message)
            {
                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                bool muted = (bool)message.info["muted"];
                int ret = mRtcEngine.MuteRemoteVideoStream((uint)uid, muted);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            

            public ServerMessage setLogFile(ServerMessage message)
            {
                string filePath = (string)message.info["filePath"];
                int ret = mRtcEngine.SetLogFile(filePath);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage renewToken(ServerMessage message)
            {
                string token = (string)message.info["token"];
                int ret = mRtcEngine.RenewToken(token);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage setChannelProfile(ServerMessage message)
            {
                int profile = (int)message.info["profile"];
                int ret = mRtcEngine.SetChannelProfile((CHANNEL_PROFILE)profile);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setClientRole(ServerMessage message)
            {
                int role = (int)message.info["role"];
                int ret = mRtcEngine.SetClientRole((CLIENT_ROLE_TYPE)role);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
            public ServerMessage enableDualStreamMode(ServerMessage message)
            {
                bool enable = (bool)message.info["enabled"];
                int ret = mRtcEngine.EnableDualStreamMode(enable);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   

            public ServerMessage setEncryptionMode(ServerMessage message)
            {
                string encryptionMode = (string)message.info["encryptionMode"];
                int ret = mRtcEngine.SetEncryptionMode(encryptionMode);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage setEncryptionSecret(ServerMessage message)
            {
                string secret = (string)message.info["secret"];
                int ret = mRtcEngine.SetEncryptionSecret(secret);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 
           
            public ServerMessage createDataStream(ServerMessage message)
            {
                bool reliable = (bool)message.info["reliable"];
                bool ordered = (bool)message.info["ordered"];
                int ret = mRtcEngine.CreateDataStream(reliable, ordered);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                int error = 0;
                if (ret < 0)
                {
                    error = ret;
                }
                infoData.Add("return", ret);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            // TODO! supports general data later. now only string is supported           
            public ServerMessage sendStreamMessage(ServerMessage message)
            {
                
                int streamId = (int)message.info["streamId"];
                string messages = (string)message.info["message"];
                byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(messages);
                int ret = mRtcEngine.SendStreamMessage(streamId, messageBytes);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
            public ServerMessage setSpeakerphoneVolume(ServerMessage message)
            {
                bool volume = (bool)message.info["volume"];
                int ret = mRtcEngine.SetEnableSpeakerphone(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage adjustRecordingSignalVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int ret = mRtcEngine.AdjustRecordingSignalVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
            public ServerMessage adjustPlaybackSignalVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int ret = mRtcEngine.AdjustPlaybackSignalVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
           
            public ServerMessage enableInEarMonitoring(ServerMessage message)
            {
                bool enabled = (bool)message.info["enabled"];
                int ret = mRtcEngine.EnableInEarMonitoring(enabled);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage enableWebSdkInteroperability(ServerMessage message)
            {
                bool enabled = (bool)message.info["enabled"];
                int ret = mRtcEngine.EnableWebSdkInteroperability(enabled);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
          
            public ServerMessage startEchoTest(ServerMessage message)
            {
                int ret = mRtcEngine.StartEchoTest();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
          
            public ServerMessage stopEchoTest(ServerMessage message)
            {
                int ret = mRtcEngine.StopEchoTest();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setRemoteVideoStreamType(ServerMessage message)
            {
                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                int streamType = (int)message.info["streamType"];
                int ret = mRtcEngine.SetRemoteVideoStreamType((uint)uid, (REMOTE_VIDEO_STREAM_TYPE)streamType);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setMixedAudioFrameParameters(ServerMessage message)
            {
                int sampleRate = (int)message.info["sampleRate"];
                int samplesPerCall = (int)message.info["samplesPerCall"];
                int ret = mRtcEngine.SetMixedAudioFrameParameters(sampleRate, samplesPerCall);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setAudioMixingPosition(ServerMessage message)
            {
                int position = (int)message.info["position"];
                int ret = mRtcEngine.SetAudioMixingPosition(position);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            // setLogFilter: deprecated           
            public ServerMessage setLogFilter(ServerMessage message)
            {
                int fileter = (int)message.info["filter"];
                int ret = mRtcEngine.SetLogFilter((LOG_FILTER)fileter);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }         
          
            public ServerMessage updateTexture(ServerMessage message)
            {
                return null;
            }

            public ServerMessage deleteTexture(ServerMessage message)
            {
                return null;
            }            
            public ServerMessage updateVideoRawData(ServerMessage message)
            {
                return null;
            }           
            public ServerMessage addUserVideoInfo(ServerMessage message)
            {
                
                return null;
            }            
         
            public ServerMessage setPlaybackDeviceVolume(ServerMessage message)
            {
                return null;
            }    

            public ServerMessage getEffectsVolume(ServerMessage message)
            {
                double ret = audioEffectManager.GetEffectsVolume();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("volume", ret);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);

            }           
            public ServerMessage setEffectsVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int ret = audioEffectManager.SetEffectsVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
            public ServerMessage playEffect(ServerMessage message)
            {
                int soundId = (int)message.info["soundId"];
                string filePath = (string)message.info["filePath"];
                int loopCount = (int)message.info["loopCount"];
                double pitch = (double)message.info["pitch"];
                double pan = (double)message.info["pan"];
                int gain = (int)message.info["gain"];
                bool publish = (bool)message.info["publish"];
                int ret = audioEffectManager.PlayEffect(soundId, filePath, loopCount, pitch, pan, gain, publish);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage stopEffect(ServerMessage message)
            {
                int soundId = (int)message.info["soundId"];
                int ret = audioEffectManager.StopEffect(soundId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
            public ServerMessage stopAllEffects(ServerMessage message)
            {
                int ret = audioEffectManager.StopAllEffects();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            
            public ServerMessage preloadEffect(ServerMessage message)
            {
                int soundId = (int)message.info["soundId"];
                string filePath = (string)message.info["filePath"];
                int ret = audioEffectManager.PreloadEffect(soundId, filePath);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage unloadEffect(ServerMessage message)
            {
                int soundId = (int)message.info["soundId"];
                int ret = audioEffectManager.UnloadEffect(soundId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage pauseEffect(ServerMessage message)
            {
                int soundId = (int)message.info["soundId"];
                int ret = audioEffectManager.PauseEffect(soundId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage pauseAllEffects(ServerMessage message)
            {
                int ret = audioEffectManager.PauseAllEffects();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   

            public ServerMessage resumeEffect(ServerMessage message)
            {
                int soundId = (int)message.info["soundId"];
                int ret = audioEffectManager.ResumeEffect(soundId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage resumeAllEffects(ServerMessage message)
            {
                int ret = audioEffectManager.ResumeAllEffects();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   

            public ServerMessage setDefaultMuteAllRemoteAudioStreams(ServerMessage message)
            {
                bool mute = (bool)message.info["mute"];
                int ret = mRtcEngine.SetDefaultMuteAllRemoteAudioStreams(mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  

            public ServerMessage setDefaultMuteAllRemoteVideoStreams(ServerMessage message)
            {
                bool mute = (bool)message.info["mute"];
                int ret = mRtcEngine.SetDefaultMuteAllRemoteVideoStreams(mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }          

            public ServerMessage getConnectionState(ServerMessage message)
            {
                CONNECTION_STATE_TYPE ret = mRtcEngine.GetConnectionState();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", (int)ret);
                infoData.Add("state", (int)ret);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   

            public ServerMessage setAudioProfile(ServerMessage message)
            {
                int audioProfile = (int)message.info["audioProfileType"];
                int scenario = (int)message.info["scenario"];
                int ret = mRtcEngine.SetAudioProfile((AUDIO_PROFILE_TYPE)audioProfile, (AUDIO_SCENARIO_TYPE)scenario);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }            

            public ServerMessage setVideoEncoderConfiguration(ServerMessage message)
            {
                // "minFrameRate": -1, "degradationPreference": 0 } } }
                string s = message.ToString();
                Application.Logger.Info(TAG, "setVideoEncoderConfiguration  called message.info = " + message.info.ToJson());
                VideoEncoderConfiguration videoEncoderConfiguration = new VideoEncoderConfiguration();
                videoEncoderConfiguration.dimensions.width = (int)message.info["config"]["dimensionWidth"];
                videoEncoderConfiguration.dimensions.height = (int)message.info["config"]["dimensionHeight"];
                videoEncoderConfiguration.frameRate = (FRAME_RATE)(int)message.info["config"]["frameRate"];
                videoEncoderConfiguration.bitrate = (int)message.info["config"]["bitrate"];
                videoEncoderConfiguration.minBitrate = (int)message.info["config"]["minBitrate"];
                videoEncoderConfiguration.orientationMode = (ORIENTATION_MODE)(int)message.info["config"]["orientationMode"];
                videoEncoderConfiguration.minFrameRate = (int)message.info["config"]["minFrameRate"];
                videoEncoderConfiguration.degradationPreference = (DEGRADATION_PREFERENCE)(int)message.info["config"]["degradationPreference"];

                Dictionary<string, object> infoData = new Dictionary<string, object>();
                int ret = mRtcEngine.SetVideoEncoderConfiguration(videoEncoderConfiguration);
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage adjustAudioMixingPlayoutVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int ret = mRtcEngine.AdjustAudioMixingPlayoutVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage adjustAudioMixingPublishVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int ret = mRtcEngine.AdjustAudioMixingPublishVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setVolumeOfEffect(ServerMessage message)
            {
                int soundId = (int)message.info["soundId"];
                int volume = (int)message.info["volume"];
                int ret = mRtcEngine.SetVolumeOfEffect(soundId, volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setRecordingAudioFrameParameters(ServerMessage message)
            {
                int sampleRate = (int)message.info["sampleRate"];
                int channel = (int)message.info["channel"];
                int mode = (int)message.info["mode"];
                int samplesPerCall = (int)message.info["samplesPerCall"];
                int ret = mRtcEngine.SetRecordingAudioFrameParameters(sampleRate, channel, (RAW_AUDIO_FRAME_OP_MODE_TYPE)mode, samplesPerCall);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage setPlaybackAudioFrameParameters(ServerMessage message)
            {
                int sampleRate = (int)message.info["sampleRate"];
                int channel = (int)message.info["channel"];
                int mode = (int)message.info["mode"];
                int samplesPerCall = (int)message.info["samplesPerCall"];
                int ret = mRtcEngine.SetPlaybackAudioFrameParameters(sampleRate, channel, (RAW_AUDIO_FRAME_OP_MODE_TYPE)mode, samplesPerCall);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 
            public ServerMessage setLocalPublishFallbackOption(ServerMessage message)
            {
                int streamFallbackOptions = (int)message.info["streamFallbackOptions"];
                int ret = mRtcEngine.SetLocalPublishFallbackOption((STREAM_FALLBACK_OPTIONS)streamFallbackOptions);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage setRemoteSubscribeFallbackOption(ServerMessage message)
            {
                int streamFallbackOptions = (int)message.info["streamFallbackOptions"];
                int ret = mRtcEngine.SetRemoteSubscribeFallbackOption((STREAM_FALLBACK_OPTIONS)streamFallbackOptions);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setRemoteDefaultVideoStreamType(ServerMessage message)
            {
                int remoteVideoStreamType = (int)message.info["remoteVideoStreamType"];
                int ret = mRtcEngine.SetRemoteDefaultVideoStreamType((REMOTE_VIDEO_STREAM_TYPE)remoteVideoStreamType);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage addPublishStreamUrl(ServerMessage message)
            {
                string url = (string)message.info["url"];
                bool transcodingEnabled = (bool)message.info["transcodingEnabled"];
                int ret = mRtcEngine.AddPublishStreamUrl(url, transcodingEnabled);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage removePublishStreamUrl(ServerMessage message)
            {
                string url = (string)message.info["removePublishStreamUrl"];
                int ret = mRtcEngine.RemovePublishStreamUrl(url);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getErrorDescription(ServerMessage message)
            {
                int code = (int)message.info["code"];
                string ret = IRtcEngine.GetErrorDescription(code);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("description", ret);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setLiveTranscoding(ServerMessage message)
            {   
                LiveTranscoding liveTranscoding = JSON.JsonToObject<LiveTranscoding>(message.info["liveTranscoding"].ToJson());
                // LiveTranscoding liveTranscoding = new LiveTranscoding();
                // liveTranscoding.width = (int)message.info["liveTranscoding"]["width"];
                // liveTranscoding.height = (int)message.info["liveTranscoding"]["height"];
                // liveTranscoding.videoBitrate = (int)message.info["liveTranscoding"]["videoBitrate"];
                // liveTranscoding.videoFramerate = (int)message.info["liveTranscoding"]["videoFramerate"];
                // liveTranscoding.lowLatency = (bool)message.info["liveTranscoding"]["lowLatency"];
                // liveTranscoding.videoGop = (int)message.info["liveTranscoding"]["videoGop"];
                // liveTranscoding.videoCodecProfile = (VIDEO_CODEC_PROFILE_TYPE)(int)message.info["liveTranscoding"]["videoCodecProfile"];
                // liveTranscoding.backgroundColor = (uint)message.info["liveTranscoding"]["backgroundColor"];
                // liveTranscoding.userCount = (uint)message.info["liveTranscoding"]["userCount"];

                // TranscodingUser transcodingUser = new TranscodingUser();
                // transcodingUser.uid = (uint)message.info["liveTranscoding"]["transcodingUser"]["uid"];
                // transcodingUser.x = (int)message.info["liveTranscoding"]["transcodingUser"]["x"];
                // transcodingUser.y = (int)message.info["liveTranscoding"]["transcodingUser"]["y"];
                // transcodingUser.width = (int)message.info["liveTranscoding"]["transcodingUser"]["width"];
                // transcodingUser.height = (int)message.info["liveTranscoding"]["transcodingUser"]["height"];
                // transcodingUser.zOrder = (int)message.info["liveTranscoding"]["transcodingUser"]["zOrder"];
                // transcodingUser.alpha = (double)message.info["liveTranscoding"]["transcodingUser"]["alpha"];
                // transcodingUser.audioChannel = (int)message.info["liveTranscoding"]["transcodingUser"]["audioChannel"];
                // liveTranscoding.transcodingUsers = transcodingUser;
                // liveTranscoding.transcodingExtraInfo = (string)message.info["liveTranscoding"]["transcodingExtraInfo"];
                // liveTranscoding.metadata = (string)message.info["liveTranscoding"]["metadata"];

                // RtcImage watermark = new RtcImage();
                // watermark.url = (string)message.info["liveTranscoding"]["watermark"]["url"];
                // watermark.x = (int)message.info["liveTranscoding"]["watermark"]["x"];
                // watermark.y = (int)message.info["liveTranscoding"]["watermark"]["y"];
                // watermark.width = (int)message.info["liveTranscoding"]["watermark"]["width"];
                // watermark.height = (int)message.info["liveTranscoding"]["watermark"]["height"];
                // liveTranscoding.watermark = watermark;

                // RtcImage backgroundImage = new RtcImage();
                // backgroundImage.url = (string)message.info["liveTranscoding"]["backgroundImage"]["url"];
                // backgroundImage.x = (int)message.info["liveTranscoding"]["backgroundImage"]["x"];
                // backgroundImage.y = (int)message.info["liveTranscoding"]["backgroundImage"]["y"];
                // backgroundImage.width = (int)message.info["liveTranscoding"]["backgroundImage"]["width"];
                // backgroundImage.height = (int)message.info["liveTranscoding"]["backgroundImage"]["height"];
                // liveTranscoding.backgroundImage = backgroundImage;  

                // liveTranscoding.audioSampleRate = (AUDIO_SAMPLE_RATE_TYPE)(int)message.info["liveTranscoding"]["audioSampleRate"];
                // liveTranscoding.audioBitrate = (int)message.info["liveTranscoding"]["audioBitrate"];
                // liveTranscoding.audioChannels = (int)message.info["liveTranscoding"]["audioChannels"];
                // liveTranscoding.audioCodecProfile = (AUDIO_CODEC_PROFILE_TYPE)(int)message.info["liveTranscoding"]["audioCodecProfile"];
                int ret = mRtcEngine.SetLiveTranscoding(liveTranscoding);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage createAVideoDeviceManager(ServerMessage message)
            {
                bool createSuccess = videoDeviceManager.CreateAVideoDeviceManager();
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("createSuccess", createSuccess);
                infoData.Add("return", createSuccess);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage releaseAVideoDeviceManager(ServerMessage message)
            {
                int ret = videoDeviceManager.ReleaseAVideoDeviceManager();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage startVideoDeviceTest(ServerMessage message)
            {
              //int ret = videoDeviceManager.StartVideoDeviceTest()
                return null;
            }
            public ServerMessage stopVideoDeviceTest(ServerMessage message)
            {
                return null;
            }
            public ServerMessage getVideoDeviceCollectionCount(ServerMessage message)
            {
                int count = videoDeviceManager.GetVideoDeviceCount();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("count", count);
                infoData.Add("return", count);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getVideoDeviceCollectionDevice(ServerMessage message)
            {
                int index = (int)message.info["index"];
                string deviceName = "";
                string deviceId = "";
                int error = videoDeviceManager.GetVideoDevice(index, ref deviceName, ref deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("deviceName", deviceName);
                infoData.Add("deviceId", deviceId);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setVideoDeviceCollectionDevice(ServerMessage message)
            {
                string deviceId = (string)message.info["deviceId"];
                int error = videoDeviceManager.SetVideoDevice(deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getCurrentVideoDevice(ServerMessage message)
            {
                string deviceId = "";
                int error = videoDeviceManager.GetCurrentVideoDevice(ref deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("deviceId", deviceId);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage creatAAudioRecordingDeviceManager(ServerMessage message)
            {
                bool success = audioRecordingDeviceManager.CreateAAudioRecordingDeviceManager();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("success", success);
                infoData.Add("return", success);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage releaseAAudioRecordingDeviceManager(ServerMessage message)
            {
                int error = audioRecordingDeviceManager.ReleaseAAudioRecordingDeviceManager();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);

            }
            public ServerMessage getAudioRecordingDeviceCount(ServerMessage message)
            {
                int count = audioRecordingDeviceManager.GetAudioRecordingDeviceCount();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("count", count);
                infoData.Add("return", count);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage getAudioRecordingDevice(ServerMessage message)
            {
                int index = (int)message.info["index"];
                string deviceName = "";
                string deviceId = "";
                int error = audioRecordingDeviceManager.GetAudioRecordingDevice(index, ref deviceName, ref deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("deviceName", deviceName);
                infoData.Add("deviceId", deviceId);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setAudioRecordingDevice(ServerMessage message)
            {
                string deviceId = (string)message.info["deviceId"];
                int error = audioRecordingDeviceManager.SetAudioRecordingDevice(deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 
            public ServerMessage setAudioRecordingDeviceVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int error = audioRecordingDeviceManager.SetAudioRecordingDeviceVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage getAudioRecordingDeviceVolume(ServerMessage message)
            {
                int error = -1;
                int volume = audioRecordingDeviceManager.GetAudioRecordingDeviceVolume();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                if (volume > 0) 
                {
                    error = 0;
                    infoData.Add("return", 0);
                    infoData.Add("volume", volume);
                    infoData.Add("error", 0);
                }
                else
                {
                    error = volume;
                    infoData.Add("return", error);
                    infoData.Add("volume", 0);
                    infoData.Add("error", error);
                }
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setAudioRecordingDeviceMute(ServerMessage message)
            {
                bool mute = (bool)message.info["muted"];
                int error = audioRecordingDeviceManager.SetAudioRecordingDeviceMute(mute);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage isAudioRecordingDeviceMute(ServerMessage message)
            {
                bool isMute = audioRecordingDeviceManager.IsAudioRecordingDeviceMute();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("isMute", isMute);
                infoData.Add("return", isMute);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage getCurrentRecordingDeviceInfo(ServerMessage message)
            {
                string deviceId = "";
                string deviceName = "";
                int error = audioRecordingDeviceManager.GetCurrentRecordingDeviceInfo(ref deviceName, ref deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("deviceId", deviceId);
                infoData.Add("deviceName", deviceName);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getCurrentRecordingDevice(ServerMessage message)
            {
                string deviceId = "";
                int error = audioRecordingDeviceManager.GetCurrentRecordingDevice(ref deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                infoData.Add("deviceId", deviceId);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }      
            public ServerMessage startAudioRecordingDeviceTest(ServerMessage message)
            {
                int interval = (int)message.info["interval"];
                int error = audioRecordingDeviceManager.StartAudioRecordingDeviceTest(interval);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }         

            public ServerMessage stopAudioRecordingDeviceTest(ServerMessage message)
            {
                int error = audioRecordingDeviceManager.StopAudioRecordingDeviceTest();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage getAudioPlaybackDeviceCount(ServerMessage message)
            {
                int count = audioPlaybackDeviceManager.GetAudioPlaybackDeviceCount();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                if (count > 0)
                {
                    infoData.Add("return", count);
                    infoData.Add("count", count);
                    infoData.Add("error", 0);
                }
                else
                {
                    infoData.Add("return", count);
                    infoData.Add("count", 0);
                    infoData.Add("error", count);
                }
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   
            public ServerMessage creatAAudioPlaybackDeviceManager(ServerMessage message)
            {
                bool error = audioPlaybackDeviceManager.CreateAAudioPlaybackDeviceManager();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }        
            public ServerMessage releaseAAudioPlaybackDeviceManager(ServerMessage message)
            {
                int error = audioPlaybackDeviceManager.ReleaseAAudioPlaybackDeviceManager();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }         

            public ServerMessage getAudioPlaybackDevice(ServerMessage message)
            {
                int index = (int)message.info["index"];
                string deviceName = "";
                string deviceId = "";
                int error = audioPlaybackDeviceManager.GetAudioPlaybackDevice(index, ref deviceName, ref deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   
            public ServerMessage setAudioPlaybackDevice(ServerMessage message)
            {
                string deviceId = (string)message.info["deviceId"];
                int error = audioPlaybackDeviceManager.SetAudioPlaybackDevice(deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }          
            public ServerMessage setAudioPlaybackDeviceVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int error = audioPlaybackDeviceManager.SetAudioPlaybackDeviceVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage getAudioPlaybackDeviceVolume(ServerMessage message)
            {
                int volume = audioPlaybackDeviceManager.GetAudioPlaybackDeviceVolume();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                if (volume > 0)
                {
                    infoData.Add("return", volume);
                    infoData.Add("volume", volume);
                    infoData.Add("error", 0);
                }
                else
                {
                    infoData.Add("return", volume);
                    infoData.Add("error", volume);
                    infoData.Add("volume", 0);
                }
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage setAudioPlaybackDeviceMute(ServerMessage message)
            {
                bool muted = (bool)message.info["muted"];
                int error = audioPlaybackDeviceManager.SetAudioPlaybackDeviceMute(muted);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage isAudioPlaybackDeviceMute(ServerMessage message)
            {
                bool mute = (bool)audioPlaybackDeviceManager.IsAudioPlaybackDeviceMute();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("mute", mute);
                infoData.Add("return", mute);
                infoData.Add("error", 0);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage startAudioPlaybackDeviceTest(ServerMessage message)
            {
                string filePath = (string)message.info["filaPath"];
                int error = audioPlaybackDeviceManager.StartAudioPlaybackDeviceTest(filePath);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage stopAudioPlaybackDeviceTest(ServerMessage message)
            {
                int error = audioPlaybackDeviceManager.StopAudioPlaybackDeviceTest();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   

            public ServerMessage getCurrentPlaybackDeviceInfo(ServerMessage message)
            {
                string deviceName = "";
                string deviceId = "";
                int error = audioPlaybackDeviceManager.GetCurrentPlaybackDeviceInfo(ref deviceName, ref deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("deviceName", deviceName);
                infoData.Add("deviceId", deviceId);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }       
            public ServerMessage getCurrentPlaybackDevice(ServerMessage message)
            {
                string deviceId = "";
                int error  = audioPlaybackDeviceManager.GetCurrentPlaybackDevice(ref deviceId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }    
            public ServerMessage pushVideoFrame(ServerMessage message)
            {
                int ret = mRtcEngine.PushVideoFrame(new ExternalVideoFrame());
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return null;
            }      
            public ServerMessage setExternalVideoSource(ServerMessage message)
            {
                bool enable = (bool)message.info["enable"];
                bool useTexture = (bool)message.info["useTexture"];
                int error = mRtcEngine.SetExternalVideoSource(enable, useTexture);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }       
            public ServerMessage setExternalAudioSource(ServerMessage message)
            {
                bool enabled = (bool)message.info["enabled"];
                int sampleRate = (int)message.info["sampleRate"];
                int channels = (int)message.info["channels"];
                int error = mRtcEngine.SetExternalAudioSource(enabled, sampleRate, channels);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage pushAudioFrame(ServerMessage message)
            {
                return null;
            }
            public ServerMessage registerVideoRawDataObserver(ServerMessage message)
            {   
                int ret = videoRawDataManager.RegisterVideoRawDataObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }         
            public ServerMessage unRegisterVideoRawDataObserver(ServerMessage message)
            {
                int ret = videoRawDataManager.UnRegisterVideoRawDataObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }         
            public ServerMessage registerAudioRawDataObserver(ServerMessage message)
            {
                int ret = audioRawDataManager.RegisterAudioRawDataObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }           
            public ServerMessage unRegisterAudioRawDataObserver(ServerMessage message)
            {
                int ret = audioRawDataManager.UnRegisterAudioRawDataObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }        

            public ServerMessage setRenderMode(ServerMessage message)
            {
                return null;
            }  

            public ServerMessage getAudioMixingPlayoutVolume(ServerMessage message)
            {
                int ret = mRtcEngine.GetAudioMixingPlayoutVolume();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", 0);
                infoData.Add("volume", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   
            public ServerMessage getAudioMixingPublishVolume(ServerMessage message)
            {
                int ret = mRtcEngine.GetAudioMixingPublishVolume();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", 0);
                infoData.Add("volume", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }       
            public ServerMessage setLocalVoiceChanger(ServerMessage message)
            {
                int voice_changer_preset = (int)message.info["voice_changer_preset"];
                int ret = mRtcEngine.SetLocalVoiceChanger((VOICE_CHANGER_PRESET)voice_changer_preset);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }     
            public ServerMessage setLocalVoiceReverbPreset(ServerMessage message)
            {
                int audio_reverb_reset = (int)message.info["audio_reverb_reset"];
                int ret = mRtcEngine.SetLocalVoiceReverbPreset((AUDIO_REVERB_PRESET)audio_reverb_reset);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }     
            public ServerMessage enableSoundPositionIndication(ServerMessage message)
            {
                bool enable = (bool)message.info["enable"];
                int ret = mRtcEngine.EnableSoundPositionIndication(enable);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }    
            public ServerMessage setLocalVoiceEqualization(ServerMessage message)
            {
                int bandFrequency = (int)message.info["bandFrequency"];
                int bandGain = (int)message.info["bandGain"];
                int ret = mRtcEngine.SetLocalVoiceEqualization((AUDIO_EQUALIZATION_BAND_FREQUENCY) bandFrequency, bandGain);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);

            }
            public ServerMessage setLocalVoiceReverb(ServerMessage message)
            {
                int reverbKey = (int)message.info["reverbKey"];
                int value = (int)message.info["value"];
                int ret = mRtcEngine.SetLocalVoiceReverb((AUDIO_REVERB_TYPE)reverbKey, value);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }  
            public ServerMessage setCameraCapturerConfiguration(ServerMessage message)
            {
                // int preference = (int)message.info["preference"];
                // int cameraDirection = (int)message.info["cameraDirection"];
                // CameraCapturerConfiguration capturerConfiguration = new CameraCapturerConfiguration();
                // capturerConfiguration.preference = (CAPTURER_OUTPUT_PREFERENCE)preference;
                // capturerConfiguration.cameraDirection = (CAMERA_DIRECTION)cameraDirection;
                CameraCapturerConfiguration cameraCapturerConfiguration = JSON.JsonToObject<CameraCapturerConfiguration>(message.info["cameraCapturerConfiguration"].ToJson());
                int ret = mRtcEngine.SetCameraCapturerConfiguration(cameraCapturerConfiguration);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }      
            public ServerMessage setRemoteUserPriority(ServerMessage message)
            {
                long uid = JSON.ParseNumberToLong(message.info["uid"]);
                int priorityType = (int)message.info["priorityType"];
                int ret = mRtcEngine.SetRemoteUserPriority((uint)uid, (PRIORITY_TYPE)priorityType);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }      
            public ServerMessage setLogFileSize(ServerMessage message)
            {
                long size = JSON.ParseNumberToLong(message.info["size"]);
                int ret = mRtcEngine.SetLogFileSize((uint)size);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }       
            public ServerMessage setExternalAudioSink(ServerMessage message)
            {
                bool enable = (bool)message.info["enable"];
                int sampleRate = (int)message.info["sampleRate"];
                int channels = (int)message.info["channels"];
                int ret = mRtcEngine.SetExternalAudioSink(enable, sampleRate, channels);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }      
            public ServerMessage pullAudioFrame(ServerMessage message)
            {
                return null;
            }      
            public ServerMessage startLastmileProbeTest(ServerMessage message)
            {
                LastmileProbeConfig lastmileProbeConfig = JSON.JsonToObject<LastmileProbeConfig>(message.info["lastmileProbeConfig"].ToJson());
                // bool probeUplink = (bool)message.info["probeUplink"];
                // bool probeDownlink = (bool)message.info["probeDownlink"];
                // uint expectedUplinkBitrate = (uint)message.info["expectedUplinkBitrate"];
                // uint expectedDownlinkBitrate = (uint)message.info["expectedDownlinkBitrate"];
                // LastmileProbeConfig lastmileProbeConfig = new LastmileProbeConfig();
                // lastmileProbeConfig.probeUplink = probeUplink;
                // lastmileProbeConfig.probeDownlink = probeDownlink;
                // lastmileProbeConfig.expectedUplinkBitrate = expectedUplinkBitrate;
                // lastmileProbeConfig.expectedDownlinkBitrate = expectedDownlinkBitrate;
                int ret = mRtcEngine.StartLastmileProbeTest(lastmileProbeConfig);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }       
            public ServerMessage stopLastmileProbeTest(ServerMessage message)
            {
                int ret = mRtcEngine.StopLastmileProbeTest();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage addVideoWatermark(ServerMessage message)
            {
                RtcImage rtcImage = JSON.JsonToObject<RtcImage>(message.info["rtcImage"].ToJson());
                WatermarkOptions options;
                Rectangle p;
                p.width = rtcImage.width;
                p.height = rtcImage.height;
                p.x = 0;
                p.y = 0;
                options.positionInLandscapeMode = p;
                options.positionInPortraitMode = p;
                options.visibleInPreview = true;
                int ret = mRtcEngine.AddVideoWatermark(rtcImage);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }    

            public ServerMessage clearVideoWatermarks(ServerMessage message)
            {
                int ret = mRtcEngine.ClearVideoWatermarks();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }        
            public ServerMessage registerLocalUserAccount(ServerMessage message)
            {
                string appId = (string)message.info["appId"];
                string userAccount = (string)message.info["userAccount"];
                int ret = mRtcEngine.RegisterLocalUserAccount(appId, userAccount);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }        
            public ServerMessage joinChannelWithUserAccount(ServerMessage message)
            {
                string token = (string)message.info["token"];
                string channelId = (string)message.info["channelId"];
                string userAccount = (string)message.info["userAccount"];
                int ret = mRtcEngine.JoinChannelWithUserAccount(token, channelId, userAccount);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage getUserInfoByUserAccount(ServerMessage message)
            {

                //int ret = mRtcEngine.GetUserInfoByUserAccount()
                return null;
            } 

            public ServerMessage getUserInfoByUid(ServerMessage message)
            {
                return null;
            } 

            public ServerMessage setBeautyEffectOptions(ServerMessage message)
            {
                bool enable = (bool)message.info["enable"];
                BeautyOptions beautyOptions = JSON.JsonToObject<BeautyOptions>(message.info["beautyOptions"].ToJson());
                //int lighteningContrastLevel = (int)message.info["lighteningContrastLevel"];
                //double lighteningLevel = (double)message.info["lighteningLevel"];
                //double smoothnessLevel = (double)message.info["smoothnessLevel"];
                //double rednessLevel = (double)message.info["rednessLevel"];

                //BeautyOptions beauty = new BeautyOptions();
                //beauty.lighteningContrastLevel = (BeautyOptions.LIGHTENING_CONTRAST_LEVEL)lighteningContrastLevel;
                //beauty.lighteningLevel = (float)lighteningLevel;
                //beauty.smoothnessLevel = (float)smoothnessLevel;
                //beauty.rednessLevel = (float)rednessLevel;
                int ret = mRtcEngine.SetBeautyEffectOptions(enable, beautyOptions);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setInEarMonitoringVolume(ServerMessage message)
            {
                int volume = (int)message.info["volume"];
                int ret = mRtcEngine.SetInEarMonitoringVolume(volume);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage startScreenCaptureByDisplayId(ServerMessage message)
            {
                return null;
            }

            public ServerMessage startScreenCaptureByScreenRect(ServerMessage message)
            {
                Rectangle screenRectangle = JSON.JsonToObject<Rectangle>(message.info["screenRectangle"].ToJson());
                Rectangle regionRectangle = JSON.JsonToObject<Rectangle>(message.info["regionRectangle"].ToJson());
                ScreenCaptureParameters screenCaptureParameters = JSON.JsonToObject<ScreenCaptureParameters>(message.info["screenCaptureParameters"].ToJson());
                int ret = mRtcEngine.StartScreenCaptureByScreenRect(screenRectangle, regionRectangle, screenCaptureParameters);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage setScreenCaptureContentHint(ServerMessage message)
            {
                VideoContentHint videoContentHint = (VideoContentHint)(int)message.info["videoContentHint"];
                int ret = mRtcEngine.SetScreenCaptureContentHint(videoContentHint);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }       
            public ServerMessage updateScreenCaptureParameters(ServerMessage message)
            {
                ScreenCaptureParameters screenCaptureParameters = JSON.JsonToObject<ScreenCaptureParameters>(message.info["screenCaptureParameters"].ToJson());
                int ret = mRtcEngine.UpdateScreenCaptureParameters(screenCaptureParameters);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }          
            public ServerMessage updateScreenCaptureRegion(ServerMessage message)
            {
                Rectangle rectangle = JSON.JsonToObject<Rectangle>(message.info["rectangle"].ToJson());
                int ret = mRtcEngine.UpdateScreenCaptureRegion(rectangle);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            } 

            public ServerMessage stopScreenCapture(ServerMessage message)
            {
                int ret = mRtcEngine.StopScreenCapture();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage addInjectStreamUrl(ServerMessage message)
            {
                string url = (string)message.info["url"];
                InjectStreamConfig streamConfig = JSON.JsonToObject<InjectStreamConfig>(message.info["streamConfig"].ToJson());
                int ret = mRtcEngine.AddInjectStreamUrl(url, streamConfig);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }       
            public ServerMessage removeInjectStreamUrl(ServerMessage message)
            {
                string url = (string)message.info["url"];
                int ret = mRtcEngine.RemoveInjectStreamUrl(url);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }

            public ServerMessage enableLoopbackRecording(ServerMessage message)
            {
                bool enabled = (bool)message.info["enabled"];
                string deviceName = (string)message.info["deviceName"];
                int ret = mRtcEngine.EnableLoopbackRecording(enabled, deviceName);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }       
            public ServerMessage setAudioSessionOperationRestriction(ServerMessage message)
            {
                AUDIO_SESSION_OPERATION_RESTRICTION restriction = (AUDIO_SESSION_OPERATION_RESTRICTION)(int)message.info["restriction"];
                int ret = mRtcEngine.SetAudioSessionOperationRestriction(restriction);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage switchChannel(ServerMessage message)
            {
                string token = (string)message.info["token"];
                string channelId = (string)message.info["channelName"];
                int error = mRtcEngine.SwitchChannel(token, channelId);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", error);
                infoData.Add("error", error);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage startChannelMediaRelay(ServerMessage message)
            {
                ChannelMediaRelayConfiguration channelMediaRelayConfiguration = JSON.JsonToObject<ChannelMediaRelayConfiguration>(message.info["channelMediaRelayConfiguration"].ToJson());
                int ret = mRtcEngine.StartChannelMediaRelay(channelMediaRelayConfiguration);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage updateChannelMediaRelay(ServerMessage message)
            {
                ChannelMediaRelayConfiguration channelMediaRelayConfiguration = JSON.JsonToObject<ChannelMediaRelayConfiguration>(message.info["channelMediaRelayConfiguration"].ToJson());
                int ret = mRtcEngine.UpdateChannelMediaRelay(channelMediaRelayConfiguration);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }   
            public ServerMessage stopChannelMediaRelay(ServerMessage message)
            {
                int ret = mRtcEngine.StopChannelMediaRelay();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage sendMetadata(ServerMessage message)
            {
                return null;
            }
            public ServerMessage registerPacketObserver(ServerMessage message)
            {
                int ret = packetObserver.RegisterPacketObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage unRegisterPacketObserver(ServerMessage message)
            {
                int ret = packetObserver.UnRegisterPacketObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }     
            public ServerMessage registerMediaMetadataObserver(ServerMessage message)
            {
                int ret = metadataObserver.RegisterMediaMetadataObserver(METADATA_TYPE.VIDEO_METADATA);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage unRegisterMediaMetadataObserver(ServerMessage message)
            {
                int ret = metadataObserver.UnRegisterMediaMetadataObserver();
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage setMirrorApplied(ServerMessage message)
            {
                bool wheatherApply = (bool)message.info["wheatherApply"];
                int ret = mRtcEngine.SetMirrorApplied(wheatherApply);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }
            public ServerMessage startScreenCaptureByWindowId(ServerMessage message)
            {
                return null;
            }

            public ServerMessage setAudioMixingPitch(ServerMessage message)
            {
                int pitch = (int)message.info["pitch"];
                int ret = mRtcEngine.SetAudioMixingPitch(pitch);
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("return", ret);
                infoData.Add("error", ret);
                return ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, message.device, message.cmd, message.sequence, infoData, null);
            }


            public void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("channel", channelName);
                infoData.Add("uid", uid);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onJoinChannelSuccess", 0, infoData, null));
            }
        
            void OnLeaveChannelHandler(RtcStats stats)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onLeaveChannel", 0, infoData, null));
                streamViewManager.RemoveAllRemoteStreamViews();
            }

            void OnReJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("channel", channelName);
                infoData.Add("uid", uid);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onReJoinChannelSuccess", 0, infoData, null));
            }

            void OnConnectionLostHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onConnectionLost", 0, infoData, null));
            }

            void OnConnectionInterruptedHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onConnectionInterrupted", 0, infoData, null));
            }

            void OnRequestTokenHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRequestToken", 0, infoData, null));
            }

            void OnUserJoinedHandler(uint uid, int elapsed)
            {
                streamViewManager.AddRemoteStreamView(uid);
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onUserJoined", 0, infoData, null));
            }

            void OnUserOfflineHandler(uint uid, USER_OFFLINE_REASON reason)
            {
                streamViewManager.RemoveRemoteStreamView(uid);
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("reasion", (int)reason);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onUserOffline", 0, infoData, null));
            }

            void OnVolumeIndicationHandler(AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("speakers", speakers);
                infoData.Add("speakerNumber", speakerNumber);
                infoData.Add("totalVolume", totalVolume);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onVolumeIndication", 0, infoData, null));

            }

            void OnUserMutedAudioHandler(uint uid, bool muted)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("muted", muted);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onUserMutedAudio", 0, infoData, null));
            }

            void OnSDKWarningHandler(int warn, string msg)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("warn", warn);
                infoData.Add("msg", msg);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onSDKWarning", 0, infoData, null));
            }

            void OnSDKErrorHandler(int error, string msg)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", error);
                infoData.Add("msg", msg);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.UPLOAD_MESSAGE, Application.DeviceID, "onSDKError", 0, infoData, null));

            }

            void OnRtcStatsHandler(RtcStats rtcStats)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>
                {
                    { "error", 0 },
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
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRtcStats", 0, infoData, null));
            }

            void OnAudioMixingFinishedHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onAudioMixingFinished", 0, infoData, null));                
            }


            void OnAudioRouteChangedHandler(AUDIO_ROUTE route)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("route", (int)route);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onAudioRouteChanged", 0, infoData, null));
            }

            void OnFirstRemoteVideoDecodedHandler(uint uid, int width, int height, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("width", width);
                infoData.Add("height", height);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onFirstRemoteVideoDecoded", 0, infoData, null));
            }

            void OnVideoSizeChangedHandler(uint uid, int width, int height, int rotation)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("width", width);
                infoData.Add("height", height);
                infoData.Add("rotation", rotation);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onVideoSizeChanged", 0, infoData, null));
            }

            void OnClientRoleChangedHandler(CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("oldRole", (int)oldRole);
                infoData.Add("width", (int)newRole);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onClientRoleChanged", 0, infoData, null));
            }

            void OnUserMuteVideoHandler(uint uid, bool muted)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("muted", muted);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onUserMutedVideo", 0, infoData, null));
            }
        
            void OnMicrophoneEnabledHandler(bool isEnabled)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("isEnabled", isEnabled);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onMicrophoneEnabled", 0, infoData, null));
            }

            void OnApiExecutedHandler(int err, string api, string result)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", err);
                infoData.Add("api", api);
                infoData.Add("result", result);
                Application.Logger.Info(TAG, "----------" + api + " : " + result);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onApiExecuted", 0, infoData, null));
            }

            void OnLastmileQualityHandler(int quality)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("quality", quality);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onLastmileQuality", 0, infoData, null));
            }

            void OnFirstLocalAudioFrameHandler(int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onFirstLocalAudioFrame", 0, infoData, null));
            }

            void OnFirstRemoteAudioFrameHandler(uint userId, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("userId", userId);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onFirstRemoteAudioFrame", 0, infoData, null));
            }

            void OnAudioQualityHandler(uint userId, int quality, ushort delay, ushort lost)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", userId);
                infoData.Add("quality", quality);
                infoData.Add("delay", delay);
                infoData.Add("lost", lost);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onAudioQuality", 0, infoData, null));
            }
        
            void OnStreamInjectedStatusHandler(string url, uint userId, int status)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("url", url);
                infoData.Add("userId", userId);
                infoData.Add("status", status);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onStreamInjectStatus", 0, infoData, null));
            }

            void OnStreamUnpublishedHandler(string url)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("url", url);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onStreamUnpublished", 0, infoData, null));
            }

            void OnStreamPublishedHandler(string url, int error)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("url", url);
                infoData.Add("error", error);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onStreamPublished", 0, infoData, null));
            }

            void OnStreamMessageErrorHandler(uint userId, int streamId, int code, int missed, int cached)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("userId", userId);
                infoData.Add("streamId", streamId);
                infoData.Add("code", code);
                infoData.Add("missed", missed);
                infoData.Add("cached", cached);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onStreamMessageError", 0, infoData, null));
            }

            void OnStreamMessageHandler(uint userId, int streamId, byte[] data, int length)
            {
                String dataText = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", userId);
                infoData.Add("streamId", streamId);
                infoData.Add("data", dataText);
                infoData.Add("length", length);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onStreamMessage", 0, infoData, null));
            }

            void OnConnectionBannedHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onConnectionBanned", 0, infoData, null));
            }

            void OnConnectionStateChangedHandler(CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("state", (int)state);
                infoData.Add("reason", (int)reason);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onConnectionStateChanged", 0, infoData, null));
            }

            void OnTokenPrivilegeWillExpireHandler(string token)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("token", token);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onTokenPrivilegewillExpire", 0, infoData, null));
            }

            void OnActiveSpeakerHandler(uint uid)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onActiveSpeaker", 0, infoData, null));
            }

            void OnVideoStoppedHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onVideoStopped", 0, infoData, null));
            }

            void OnFirstLocalVideoFrameHandler(int width, int height, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("width", width);
                infoData.Add("height", height);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onFirstLocalVideoFrame", 0, infoData, null));
            }

            void OnFirstRemoteVideoFrameHandler(uint uid, int width, int height, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("width", width);
                infoData.Add("height", height);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onFirstRemoteVideoFrame", 0, infoData, null));
            }

            void OnUserEnableVideoHandler(uint uid, bool enabled)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("enabled", enabled);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onUserEnableVideo", 0, infoData, null));
            }

            void OnUserEnableLocalVideoHandler(uint uid, bool enabled)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("enabled", enabled);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onUserEnableLocalVideo", 0, infoData, null));
            }

            void OnRemoteVideoStateChangedHandler(uint uid, REMOTE_VIDEO_STATE state, REMOTE_VIDEO_STATE_REASON reason, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("state", (int)state);
                infoData.Add("reason", (int)reason);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteVideoStateChanged", 0, infoData, null));
            }

            void OnLocalPublishFallbackToAudioOnlyHandler(bool isFallbackOrRecover)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("isFallbackOrRecover", isFallbackOrRecover);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "OnLocalPublishFallbackToAudioOnly", 0, infoData, null));
            }

            void OnRemoteSubscribeFallbackToAudioOnlyHandler(uint uid, bool isFallbackOrRecover)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("isFallbackOrRecover", isFallbackOrRecover);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteSubscribeFallbackToAudioOnly", 0, infoData, null));
            }

            void OnNetworkQualityHandler(uint uid, int txQuality, int rxQuality)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("txQuality", txQuality);
                infoData.Add("rxQuality", rxQuality);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onNetworkQuality", 0, infoData, null));
            }

            void OnLocalVideoStatsHandler(LocalVideoStats localVideoStats)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onLocalVideoStats", 0, infoData, null));
            }

            void OnRemoteVideoStatsHandler(RemoteVideoStats remoteVideoStats)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteVideoStats", 0, infoData, null));
            }

            void OnRemoteAudioStatsHandler(RemoteAudioStats remoteAudioStats)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteAudioStats", 0, infoData, null));
            }

            void OnAudioDeviceStateChangedHandler(string deviceId, int deviceType, int deviceState)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("deviceId", deviceId);
                infoData.Add("deviceType", deviceType);
                infoData.Add("deviceState", deviceState);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onAudioDeviceStateChanged", 0, infoData, null));
            }
    
            void OnCameraReadyHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onCameraReady", 0, infoData, null));
            }

            void OnCameraFocusAreaChangedHandler(int x, int y, int width, int height)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("x", x);
                infoData.Add("y", y);
                infoData.Add("width", width);
                infoData.Add("height", height);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onCameraFocusAreaChanged", 0, infoData, null));
            }

            void OnCameraExposureAreaChangedHandler(int x, int y, int width, int height)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("x", x);
                infoData.Add("y", y);
                infoData.Add("width", width);
                infoData.Add("height", height);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onCameraExposureAreaChanged", 0, infoData, null));
            }

            void OnRemoteAudioMixingBeginHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteAudioMixingBegin", 0, infoData, null));
            }

            void OnRemoteAudioMixingEndHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteAudioMixingEnd", 0, infoData, null));
            }

            void OnAudioEffectFinishedHandler(int soundId)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("soundId", soundId);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onAudioEffectFinished", 0, infoData, null));
            }

            void OnVideoDeviceStateChangedHandler(string deviceId, int deviceType, int deviceState)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("deviceId", deviceId);
                infoData.Add("deviceType", deviceType);
                infoData.Add("deviceState", deviceState);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onVideoDeviceStateChanged", 0, infoData, null));
            }

            void OnRemoteVideoTransportStatsHandler(uint uid, ushort delay, ushort lost, ushort rxKBitRate)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("delay", (int)delay);
                infoData.Add("lost", (int)lost);
                infoData.Add("rxkBitRate", (int)rxKBitRate);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteVideoTransportStats", 0, infoData, null));
            }
    
            void OnRemoteAudioTransportStatsHandler(uint uid, ushort delay, ushort lost, ushort rxKBitRate)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("delay", (int)delay);
                infoData.Add("lost", (int)lost);
                infoData.Add("rxKBitRate", (int)rxKBitRate);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteAudioTransportStats", 0, infoData, null));
            }

            void OnTranscodingUpdatedHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onTranscodingUpdated", 0, infoData, null));
            }

            void OnAudioDeviceVolumeChangedHandler(MEDIA_DEVICE_TYPE deviceType, int volume, bool muted)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("deviceType", deviceType);
                infoData.Add("volume", volume);
                infoData.Add("muted", muted);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onAudioDeviceVolumeChanged", 0, infoData, null));
            }

            void OnMediaEngineStartCallSuccessHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onMediaEngineStartCallSuccess", 0, infoData, null));
            }

            void OnMediaEngineLoadSuccessHandler()
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onMediaEngineLoadSuccess", 0, infoData, null));
            }

            void OnAudioMixingStateChangedHandler(AUDIO_MIXING_STATE_TYPE state, AUDIO_MIXING_ERROR_TYPE errorCode)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("state", (int)state);
                infoData.Add("errorCode", (int)errorCode);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onAudioMixingStateChanged", 0, infoData, null));
            }

            void OnFirstRemoteAudioDecodedHandler(uint uid, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onFirstRemoteAudioDecoded", 0, infoData, null));
            }

            void OnLocalVideoStateChangedHandler(LOCAL_VIDEO_STREAM_STATE localVideoState, LOCAL_VIDEO_STREAM_ERROR error)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("localVideoState", localVideoState);
                infoData.Add("error", error);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onLocalVideoStateChanged", 0, infoData, null));
            }

            void OnRtmpStreamingStateChangedHandler(string url, RTMP_STREAM_PUBLISH_STATE state, RTMP_STREAM_PUBLISH_ERROR errCode)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("url", url);
                infoData.Add("state", (int)state);
                infoData.Add("errCode", (int)errCode);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRtmpStreamingStateChanged", 0, infoData, null));
            }

            void OnNetworkTypeChangedHandler(NETWORK_TYPE type)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("type", (int)type);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onNetworkTypeChanged", 0, infoData, null));
            }
            
            void OnLastmileProbeResultHandler(LastmileProbeResult result)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onLastmileProbeResult", 0, infoData, null));
            }

            void OnLocalUserRegisteredHandler(uint uid, string userAccount)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("userAccount", userAccount);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onLocalUserRegistered", 0, infoData, null));
            }

            void OnUserInfoUpdatedHandler(uint uid, UserInfo userInfo)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("user_uid", userInfo.uid);
                infoData.Add("user_account", userInfo.userAccount);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onUserInfoUpdated", 0, infoData, null));
            }
    
            void OnLocalAudioStateChangedHandler(LOCAL_AUDIO_STREAM_STATE state, LOCAL_AUDIO_STREAM_ERROR error)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("state", (int)state);
                infoData.Add("error", (int)error);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onLocalAudioStateChanged", 0, infoData, null));
            }

            void OnRemoteAudioStateChangedHandler(uint uid, REMOTE_AUDIO_STATE state, REMOTE_AUDIO_STATE_REASON reason, int elapsed)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("uid", uid);
                infoData.Add("reason", reason);
                infoData.Add("elapsed", elapsed);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onRemoteAudioStateChanged", 0, infoData, null));
            }
            
            void OnLocalAudioStatsHandler(LocalAudioStats localAudioStats)
            {
                Dictionary<string,object> infoData = new Dictionary<string, object>();
                infoData.Add("error", 0);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onLocalVideoStateChanged", 0, infoData, null));
            }

            void OnChannelMediaRelayStateChangedHandler(CHANNEL_MEDIA_RELAY_STATE state, CHANNEL_MEDIA_RELAY_ERROR code)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("state", (int)state);
                infoData.Add("code", (int)code);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelMediaRelayStateChanged", 0, infoData, null));
            }

            void OnChannelMediaRelayEventHandler(CHANNEL_MEDIA_RELAY_EVENT events)
            {
                Dictionary<string, object> infoData = new Dictionary<string, object>();
                infoData.Add("events", (int)events);
                UploadMessageToServer(ServerMessageFactory.CreateServerMessage(TYPE.CALLBACK_MESSAGE, Application.DeviceID, "onChannelMediaRelayEvent", 0, infoData, null));
            }

            void OnRecordAudioFrameHandler(AudioFrame audioFrame)
            {
                //Application.Logger.Info(TAG, "OnRecordAudioFrameHandler");
            }

            void OnPlaybackAudioFrameHandler(AudioFrame audioFrame)
            {
                //Application.Logger.Info(TAG, "OnPlaybackAudioFrameHandler");
            }

            void OnMixedAudioFrameHandler(AudioFrame audioFrame)
            {
                //Application.Logger.Info(TAG, "OnMixedAudioFrameHandler");
            }

            void OnPlaybackAudioFrameBeforeMixingHandler(uint uid, AudioFrame audioFrame)
            {
                //Application.Logger.Info(TAG, "OnPlaybackAudioFrameBeforeMixingHandler");
            }

            void OnCaptureVideoFrameHandler(VideoFrame videoFrame)
            {
                //Application.Logger.Info(TAG, "OnCaptureVideoFrameHandler");
            }
       
            void OnRenderVideoFrameHandler(uint uid, VideoFrame videoFrame)
            {
                //Application.Logger.Info(TAG, "OnRenderVideoFrameHandler");
            }
        }
    }
}

