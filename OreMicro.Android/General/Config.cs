using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;

namespace OreMicro.Android.General
{
    public static class Config
    {
        public static string BaseDir { get; set; }
        public static string BaseSrcPath { get; set; }
        public static string ErrologPath { get; set; }
        public static string AppVersion { get; set; }
        public static string IP { get; set; }
    }

    public class ConfigClass
    {
        public string BaseDir = Config.BaseDir;
        public string BaseSrcPath = Config.BaseSrcPath;
        public string ErrologPath = Config.ErrologPath;
        public string AppVersion = Config.AppVersion;
        public string IP = Config.IP;
    }

    [JsonConverter(typeof(JsonPathConverter))]
    public class ConfigJSON
    {
        [JsonProperty("BaseDir")]
        public string BaseDir { get; set; }
        [JsonProperty("BaseSrcPath")]
        public string BaseSrcPath { get; set; }
        [JsonProperty("ErrologPath")]
        public string ErrologPath { get; set; }
        [JsonProperty("AppVersion")]
        public string AppVersion { get; set; }



        public void LoadConfig()
        {
            try
            {
                string IP = GetLocalIPAddress();
                ConfigJSON configJSON = null;
                string configpath = AppDomain.CurrentDomain.BaseDirectory + "/Config.json";
                using (StreamReader r = new StreamReader(configpath))
                {
                    string json = r.ReadToEnd();
                    configJSON = JsonConvert.DeserializeObject<ConfigJSON>(json);
                }

                Config.BaseDir = configJSON.BaseDir;
                Config.BaseSrcPath = configJSON.BaseSrcPath;
                Config.ErrologPath = configJSON.ErrologPath;
                Config.AppVersion = configJSON.AppVersion;
                Config.IP = IP;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "null";
            }
            catch (Exception ex)
            {
                return "null";
            }
        }
    }

    public class JsonPathConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            object targetObj = Activator.CreateInstance(objectType);

            foreach (PropertyInfo prop in objectType.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                JsonPropertyAttribute att = prop.GetCustomAttributes(true)
                                                .OfType<JsonPropertyAttribute>()
                                                .FirstOrDefault();

                string jsonPath = (att != null ? att.PropertyName : prop.Name);
                JToken token = jo.SelectToken(jsonPath);

                if (token != null && token.Type != JTokenType.Null)
                {
                    object value = token.ToObject(prop.PropertyType, serializer);
                    prop.SetValue(targetObj, value, null);
                }
            }

            return targetObj;
        }

        public override bool CanConvert(Type objectType)
        {
            // CanConvert is not called when [JsonConverter] attribute is used
            return false;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite
        {
            get { return false; }
        }


    }
}
