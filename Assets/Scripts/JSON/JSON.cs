using UnityEngine;
using LitJson;

namespace agora
{
    namespace unity
    {
        public class JSON
        {
            public static T JsonToObject<T>(string JsonMessage)
            {
                return JsonMapper.ToObject<T>(JsonMessage);
            }

           // public static 

            public static string ObjectToJson<T>(T stringMessage)
            {
                return JsonMapper.ToJson(stringMessage);
            }
        }
    }
}
