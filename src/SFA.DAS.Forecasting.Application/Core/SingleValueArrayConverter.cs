using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SFA.DAS.Forecasting.Application.Core;

public class SingleValueArrayConverter<T> : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var jo = new JObject();
		var type = value.GetType();
		jo.Add("type", type.Name);

		foreach (PropertyInfo prop in type.GetProperties())
		{
			if (prop.CanRead)
			{
				object propVal = prop.GetValue(value, null);
				if (propVal != null)
				{
					jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
				}
			}
		}
		jo.WriteTo(writer);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		object retVal = new Object();
		if (reader.TokenType == JsonToken.StartObject)
		{
			retVal = (T)serializer.Deserialize(reader, typeof(T));
		}
		else if (reader.TokenType == JsonToken.StartArray)
		{
			var values = (List<T>)serializer.Deserialize(reader, new List<T>().GetType());
			retVal = values.FirstOrDefault();
		}
		return retVal;
	}

	public override bool CanConvert(Type objectType)
	{
		return true;
	}
}