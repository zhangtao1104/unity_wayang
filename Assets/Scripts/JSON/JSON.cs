using UnityEngine;
using LitJson;

namespace agora
{
    namespace unity
    {
        public class JSON
        {
            public static void InitJsonMapper()
            {
                JsonMapper.RegisterImporter<int, long>((int value) =>
                {
                    return (long)value;
                });
            }
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
