using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.IO;
using MyWebSocketServer.Sys;

namespace MyWebSocketServer.JSON
{
    public class JSONSerializer : ISerializer
    {
        private static JsonSerializer serializer;

        static JSONSerializer()
        {
            serializer = new JsonSerializer();
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializer.Converters.Add(new IsoDateTimeConverter());
        }

        public string Serialize(object o)
        {
            using (var stw = new StringWriter())
            {
                using (var jw = new JsonTextWriter(stw))
                {
                    serializer.Serialize(jw, o);
                    jw.Close();
                    return stw.ToString();
                }
            }
        }

        public T Deserialize<T>(string s)
        {
            using (var str = new StringReader(s))
            {
                using (var jr = new JsonTextReader(str))
                {
                    return serializer.Deserialize<T>(jr);
                }
            }
        }
    }
}
