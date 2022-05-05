using Newtonsoft.Json;
using System;
using System.Linq;

namespace PlayerDataManager
{
    public class IntOverrideContainer
    {
        public string intName;
        public int[] values;

        [JsonIgnore]
        public bool IsToggleable => values is not null && values.Length > 0;

        /// <summary>
        /// The index of the current value
        /// </summary>
        [JsonIgnore]
        internal int? currentIndex = null;
        /// <summary>
        /// The current value (which appears in the global settings)
        /// </summary>
        public int? current
        {
            get
            {
                if (currentIndex == null)
                {
                    return null;
                }
                else
                {
                    return values[currentIndex.Value];
                }
            }
            set
            {
                if (value == null)
                {
                    currentIndex = null;
                }
                else if (values.Contains(value.Value))
                {
                    currentIndex = Array.IndexOf(values, value.Value);
                }
            }
        }


        public IntOverrideContainer(string intName, int[] values = null)
        {
            this.intName = intName;
            this.values = values;
        }


    }

    public class IntOverrideContainerConverter : JsonConverter<IntOverrideContainer>
    {
        public override void WriteJson(JsonWriter writer, IntOverrideContainer value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override IntOverrideContainer ReadJson(JsonReader reader, Type objectType, IntOverrideContainer existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new((string)reader.Value);
            }
            else
            {
                return (IntOverrideContainer)serializer.Deserialize(reader, objectType);
            }
        }
    }
}
