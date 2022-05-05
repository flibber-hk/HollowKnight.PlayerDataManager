using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using MonoMod.ModInterop;
using UnityEngine.UI;
using HutongGames.PlayMaker.Actions;

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
        
        public override string GetVersion() => $"{GetType().Assembly.GetName().Version.ToString()} ({GS.BoolData.Where(x => x.Value.HasValue).Count()}/{GS.BoolData.Count()})";

        public override void Initialize()
        {
            Log("Initializing Mod...");
            const string PanelName = "PlayerDataManager.BoolMonitor";

            ModHooks.GetPlayerBoolHook += OverrideBool;
            ModHooks.GetPlayerIntHook += OverrideInt;

            if (GS.BoolData.Count > 0 || GS.IntData.Count > 0)
            {
                DebugMod.CreateSimpleInfoPanel(PanelName, 200);
                DebugMod.AddInfoToSimplePanel(PanelName, null, null);
            }

            if (GS.BoolData.Count > 0)
            {
                DebugMod.AddActionToKeyBindList(ApplyBoolValuesToSave, "Save Bool Overrides", "PD Bool Toggles");
            }
            foreach (string name in GS.BoolData.Keys)
            {
                DebugMod.AddActionToKeyBindList(Toggle(name), $"{name}", "PD Bool Toggles");
                DebugMod.AddInfoToSimplePanel(PanelName, name, () => DebugMod.GetStringForBool(PlayerData.instance.GetBoolInternal(name)));
            }

            if (GS.IntDataDict.Any(kvp => kvp.Value.IsToggleable))
            {
                DebugMod.AddActionToKeyBindList(ApplyIntValuesToSave, "Save Int Overrides", "PD Int Toggles");
            }
            foreach (IntOverrideContainer ic in GS.IntData)
            {
                string intName = ic.intName;
                if (ic.IsToggleable)
                {
                    DebugMod.AddActionToKeyBindList(Increment(intName), intName, "PD Int Toggles");
                }
                DebugMod.AddInfoToSimplePanel(PanelName, intName, () => PlayerData.instance.GetIntInternal(ic.intName).ToString());
            }
        }

        public void ApplyBoolValuesToSave()
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
                        PlayerData.instance.SetBoolInternal(pair.Key, newVal);
                    }
                }
            }
            DebugMod.LogToConsole($"Changed {ct} bool values in save data");
        }

        public void ApplyIntValuesToSave()
        {
            int ct = 0;
            foreach (IntOverrideContainer icc in GS.IntData)
            {
                if (icc.current is int newVal)
                {
                    if (newVal != PlayerData.instance.GetIntInternal(icc.intName))
                    {
                        ct++;
                        PlayerData.instance.SetIntInternal(icc.intName, newVal);
                    }
                }
            }
            DebugMod.LogToConsole($"Changed {ct} int values in save data");
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
                VersionStringUpdater.UpdateVersionString();
                RefreshMenu();
                DebugMod.LogToConsole($"Set {name} to {GS.BoolData[name]}");
            }

            return DoToggle;
        }

        public Action Increment(string name)
        {
            void DoIncrement()
            {
                IntOverrideContainer icc = GS.IntDataDict[name];
                if (icc.currentIndex is null)
                {
                    icc.currentIndex = 0;
                }
                else if (icc.currentIndex == icc.values.Length - 1)
                {
                    icc.currentIndex = null;
                }
                else
                {
                    icc.currentIndex = icc.currentIndex.Value + 1;
                }
            }

            return DoIncrement;
        }

        private bool OverrideBool(string name, bool orig)
        {
            if (GS.BoolData.TryGetValue(name, out bool? value))
            {
                return value ?? orig;
            }
            return orig;
        }

        private int OverrideInt(string name, int orig)
        {
            if (GS.IntDataDict.TryGetValue(name, out IntOverrideContainer container))
            {
                return container.current ?? orig;
            }
            return orig;
        }

        public bool ToggleButtonInsideMenu => false;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? _)
        {
            List<IMenuMod.MenuEntry> menuEntries = new();
            foreach (string boolName in GS.BoolData.Keys)
            {
                menuEntries.Add(new IMenuMod.MenuEntry()
                {
                    Name = boolName,
                    Values = new[] { "True", "False", "None" },
                    Saver = opt => { GS.BoolData[boolName] = opt == 0 ? true : opt == 1 ? false : null; VersionStringUpdater.UpdateVersionString(); },
                    Loader = () => GS.BoolData[boolName] == true ? 0 : GS.BoolData[boolName] == false ? 1 : 2
                });
            }

            foreach (string intName in GS.IntDataDict.Where(kvp => kvp.Value.IsToggleable).Select(kvp => kvp.Key))
            {
                IntOverrideContainer icc = GS.IntDataDict[intName];
                menuEntries.Add(new IMenuMod.MenuEntry()
                {
                    Name = intName,
                    Values = icc.values.Select(x => x.ToString()).Concat(new[] { "None" }).ToArray(),
                    Saver = opt => icc.currentIndex = opt == icc.values.Length ? null : opt,
                    Loader = () => icc.currentIndex ?? icc.values.Length
                });
            }

            return menuEntries;
        }
        public void RefreshMenu()
        {
            MenuScreen screen = ModHooks.BuiltModMenuScreens[this];
            foreach (MenuOptionHorizontal option in screen.gameObject.GetComponentsInChildren<MenuOptionHorizontal>())
            {
                option.menuSetting.RefreshValueFromGameSettings();
            }
        }
    }
}