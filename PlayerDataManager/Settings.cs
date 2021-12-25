using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerDataManager
{
    public class GlobalSettings
    {
        public Dictionary<string, bool?> BoolData = new()
        {
            [nameof(PlayerData.hasQuill)] = null,
            [nameof(PlayerData.unlockedCompletionRate)] = null
        };
        public List<string> IntData = new()
        {
            nameof(PlayerData.killsBuzzer)
        };
    }
}
