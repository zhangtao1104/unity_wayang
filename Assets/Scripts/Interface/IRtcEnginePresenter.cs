using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace agora
{
    namespace unity
    {
        public interface IRtcEnginePresenter
        {
            void OnExecuteServerMessage(ServerMessage message);
        }
    }
}
