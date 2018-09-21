using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MmWizard.Models;
using MmWizard.Protocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MmWizard.Middle
{
    /// <summary>
    /// 自定义json转换
    /// </summary>
    public class UdfJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);

            if (t.Type != JTokenType.Object)
            {
                t.WriteTo(writer);
            }
            else
            {
                JObject jo = (JObject)t;
                jo.WriteTo(writer);
            }

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            if (reader.TokenType != JsonToken.StartObject) return null;
            var jObject = JObject.Load(reader);
            if (!(objectType.BaseType != typeof(BModel)) || jObject["v"] == null)
            {
                return jObject.ToObject(objectType);
            }

            var type = typeof(Args<>).MakeGenericType(objectType);

            dynamic obj = jObject.ToObject(type);
            if (obj?.v?._Args != null)
            {
                obj.v._Args = obj;
            }

            return Convert.ChangeType(obj?.v, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
            return objectType == typeof(object);
        }

        public override bool CanWrite => true;

        public override bool CanRead => true;
    }
}
