using System;
using System.Reflection;
using MonoMod.Utils;

namespace PlayerDataManager
{
    internal static class VersionStringUpdater
    {
        private static FastReflectionDelegate _updateVersionString = Type.GetType("Modding.ModLoader, Assembly-CSharp")
            .GetMethod("UpdateModText", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
            .GetFastDelegate();

        public static void UpdateVersionString()
            => _updateVersionString.Invoke(null, null);
    }
}
