using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace agora
{
    namespace unity
    {
        public class RtcEnginePresenter : IRtcEnginePresenter
        {
            ApplicationContract applicationContract {
                get;
                set;
            } 

            MessageComand messageComand 
            {
                get;
                set;
            }

            RtcEngineControl rtcEngineControl
            {
                get;
                set;
            }

            public RtcEnginePresenter(ApplicationContract contract)
            {
                applicationContract = contract;
                rtcEngineControl = new RtcEngineControl();
                rtcEngineControl.SetRtcEnginePresenter(this);
            }

            public void release()
            {
                rtcEngineControl.Destroy();
            }

            public void OnExecuteServerMessage(ServerMessage message)
            {
                rtcEngineControl.OnExecuteServerMessage(message);
            }

            public void UploadMessageToServer(ServerMessage message)
            {
                applicationContract.UploadMessageToServer(message);
            }

            public static string GetSdkVersion()
            {
                return RtcEngineControl.GetSdkVersion();
            }
        }
    }
}
