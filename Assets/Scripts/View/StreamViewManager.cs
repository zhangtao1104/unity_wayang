using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using agora_gaming_rtc;
using agora_utilities;

namespace agora
{
    namespace unity
    {
        public class StreamViewManager
        {
            private readonly string localVideoSurfaceName = "localVideoSurface";
            private List<uint> remoteUIdList = new List<uint>();
            private const float Offset = 100;
            public void AddLocalStreamView()
            {
                GameObject go = GameObject.Find(localVideoSurfaceName);
                if (go == null)
                {
                    VideoSurface videoSurface = makeImageSurface(localVideoSurfaceName);
                    if (videoSurface != null)
                    {
                        //videoSurface.SetForUser((uint)optionalUid);
                        videoSurface.SetEnable(true);
                        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                        videoSurface.SetGameFps(30);
                    }
                }
            }

            public void RemoveLocalStreamView()
            {
                GameObject go = GameObject.Find(localVideoSurfaceName);
                if (go != null)
                {
                    Object.Destroy(go);
                }
            }

            public void AddRemoteStreamView(uint uid)
            {
                GameObject go = GameObject.Find(uid.ToString());
                if (go == null)
                {
                    VideoSurface videoSurface = makeImageSurface(uid.ToString());
                    if (videoSurface != null)
                    {
                        videoSurface.SetForUser (uid);
                        videoSurface.SetEnable (true);
                        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
                        videoSurface.SetGameFps(30);
                        remoteUIdList.Add(uid);
                    }
                }
            }

            public void RemoveRemoteStreamView(uint uid)
            {
                UnityEngine.GameObject go = UnityEngine.GameObject.Find(uid.ToString());
                if (go != null)
                {
                    UnityEngine.Object.Destroy(go);
                    remoteUIdList.Remove(uid);
                }
            }

            public void RemoveAllRemoteStreamViews()
            {
                foreach (var uid in remoteUIdList)
                {
                    UnityEngine.GameObject go = UnityEngine.GameObject.Find(uid.ToString());
                    if (go != null)
                    {
                        UnityEngine.Object.Destroy(go);
                    }
                }
                remoteUIdList.Clear();
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

