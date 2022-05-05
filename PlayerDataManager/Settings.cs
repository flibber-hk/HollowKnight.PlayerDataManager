using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PlayerDataManager
{
    public class GlobalSettings
    {
        public Dictionary<string, bool?> BoolData = new()
        {
            [nameof(PlayerData.hasQuill)] = null,
            [nameof(PlayerData.unlockedCompletionRate)] = null
        };

        [JsonProperty(ItemConverterType = typeof(IntOverrideContainerConverter))]
        public List<IntOverrideContainer> IntData
        {
            get
            {
                return IntDataDict.Values.ToList();
            }
            set
            {
                IntDataDict = value.ToDictionary(x => x.intName);
            }
        }

        [JsonIgnore]
        internal Dictionary<string, IntOverrideContainer> IntDataDict = new()
        {
            [nameof(PlayerData.killsBuzzer)] = new IntOverrideContainer(nameof(PlayerData.killsBuzzer)),
            [nameof(PlayerData.charmCost_1)] = new IntOverrideContainer(nameof(PlayerData.charmCost_1), new[] { 0, 1, 2 })
        };
    }
}
