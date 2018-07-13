using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Core
{
	public class SingleValueArrayConverter<T> : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
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
}