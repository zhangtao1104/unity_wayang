using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using agora_gaming_rtc;
using agora_utilities;

namespace agora
{
    namespace unity
    {
        public class ChannelStreamViewManager
        {
            private readonly string channelLocalVideoSurfaceName = "channelLocalVideoSurface";
            private Dictionary<string, List<uint>> channelUidListDict = new Dictionary<string, List<uint>>();

            private const float Offset = 100;
            public void ChannelAddLocalStreamView()
            {
                GameObject go = GameObject.Find(channelLocalVideoSurfaceName);
                if (go == null)
                {
                    VideoSurface videoSurface = makeImageSurface(channelLocalVideoSurfaceName);
                    if (videoSurface != null)
                    {
                        videoSurface.SetEnable(true);
                        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                        videoSurface.SetGameFps(30);
                    }
                }
            }

            public void ChannelRemoveLocalStreamView()
            {
                if (channelUidListDict.Count > 0)
                {
                    // User has joined multiple channels, we only remove local view when user leave all channel
                    return;
                }
                GameObject go = GameObject.Find(channelLocalVideoSurfaceName);
                if (go != null)
                {
                    Object.Destroy(go);
                }
            }

            public void ChannelAddRemoteStreamView(string channelId, uint uid)
            {
                GameObject go = GameObject.Find(uid.ToString());
                if (go == null)
                {
                    VideoSurface videoSurface = makeImageSurface(channelId + "_" + uid.ToString());
                    if (videoSurface != null)
                    {
                        videoSurface.SetForMultiChannelUser(channelId, uid);
                        videoSurface.SetEnable(true);
                        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                        videoSurface.SetGameFps(30);
                        if (!channelUidListDict.ContainsKey(channelId))
                        {
                            channelUidListDict.Add(channelId, new List<uint>());
                        }
                        channelUidListDict[channelId].Add(uid);
                    }
                }
            }

            public void ChannelRemoveRemoteStreamView(string channelId, uint uid)
            {
                UnityEngine.GameObject go = UnityEngine.GameObject.Find(channelId + "_" + uid.ToString());
                if (go != null)
                {
                    UnityEngine.Object.Destroy(go);
                    if (channelUidListDict.ContainsKey(channelId))
                    {
                        channelUidListDict[channelId].Remove(uid);
                    }
                }
            }

            public void ChannelRemoveAllRemoteStreamViews(string channelId)
            {
                if (channelUidListDict.ContainsKey(channelId))
                {
                    foreach (var uid in channelUidListDict[channelId])
                    {
                        UnityEngine.GameObject go = UnityEngine.GameObject.Find(channelId + "_" + uid.ToString());
                        if (go != null)
                        {
                            UnityEngine.Object.Destroy(go);
                        }
                    }
                    channelUidListDict.Remove(channelId);
                }

            }

            private VideoSurface makeImageSurface(string goName)
            {

                GameObject go = new GameObject();
                if (go == null)
                {
                    return null;
                }
                go.name = goName;

                // to be renderered onto
                go.AddComponent<RawImage>();

                // make the object draggable
                go.AddComponent<UIElementDragger>();
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas != null)
                {
                    go.transform.parent = canvas.transform;
                }
                // set up transform
                go.transform.Rotate(0f, 0.0f, 180.0f);
                float xPos = Random.Range(Offset - Screen.width / 2f, Screen.width / 2f - Offset);
                float yPos = Random.Range(Offset, Screen.height / 2f - Offset);
                go.transform.localPosition = new Vector3(xPos, yPos, 0f);
                go.transform.localScale = new Vector3(3f, 4f, 1f);

                // configure videoSurface
                VideoSurface videoSurface = go.AddComponent<VideoSurface>();
                return videoSurface;
            }
        }
    }
}
        
