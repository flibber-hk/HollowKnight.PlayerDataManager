using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using MonoMod.ModInterop;

namespace PlayerDataManager
{
    public class PlayerDataManager : Mod, IGlobalSettings<GlobalSettings>, IMenuMod
    {
        internal static PlayerDataManager instance;

        public static GlobalSettings GS = new();
        public void OnLoadGlobal(GlobalSettings gs) => GS = gs;
        public GlobalSettings OnSaveGlobal() => GS;

        public PlayerDataManager() : base(null)
        {
            instance = this;
        }
        
		public override string GetVersion() => $"0.1 ({GS.BoolData.Count()})";

        public override void Initialize()
        {
            Log("Initializing Mod...");

            ModHooks.GetPlayerBoolHook += OverrideBool;

            DebugMod.AddActionToKeyBindList(ApplyValuesToSave, "Save Overrides", "PD Bool Toggles");
            foreach (string name in GS.BoolData.Keys)
            {
                DebugMod.AddActionToKeyBindList(Toggle(name), $"{name}", "PD Bool Toggles");
            }
        }

        public void ApplyValuesToSave()
        {
            int ct = 0;
            foreach (KeyValuePair<string, bool?> pair in GS.BoolData)
            {
                if (pair.Value.HasValue)
                {
                    bool newVal = pair.Value.Value;
                    if (newVal != PlayerData.instance.GetBoolInternal(pair.Key))
                    {
                        ct++;
                        PlayerData.instance.SetBoolInternal(pair.Key, pair.Value.Value);
                    }
                }
            }
            DebugMod.LogToConsole($"Changed {ct} bool values in save data");
        }

        public Action Toggle(string name)
        {
            void DoToggle()
            {
                if (GS.BoolData[name].HasValue)
                {
                    GS.BoolData[name] = !GS.BoolData[name].Value;
                }
                else
                {
                    // Get internal to prevent loop, just in case
                    GS.BoolData[name] = !PlayerData.instance.GetBoolInternal(name);
                }
                DebugMod.LogToConsole($"Set {name} to {GS.BoolData[name]}");
            }

            return DoToggle;
        }

        private bool OverrideBool(string name, bool orig)
        {
            if (GS.BoolData.TryGetValue(name, out bool? value))
            {
                return value ?? orig;
            }
            return orig;
        }

        public bool ToggleButtonInsideMenu => false;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? _)
        {
            List<IMenuMod.MenuEntry> menuEntries = new();
            foreach (string name in GS.BoolData.Keys)
            {
                menuEntries.Add(new IMenuMod.MenuEntry()
                {
                    Name = name,
                    Values = new[] { "True", "False", "None" },
                    Saver = opt => GS.BoolData[name] = opt == 0 ? true : opt == 1 ? false : null,
                    Loader = () => GS.BoolData[name] == true ? 0 : GS.BoolData[name] == false ? 1 : 2
                });
            }

            return menuEntries;
        }
    }
}