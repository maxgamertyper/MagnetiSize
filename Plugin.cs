using BepInEx;
using HarmonyLib;
using System.Reflection;
using System;
using UnityEngine.Rendering;
using BoplFixedMath;

namespace MagnetiSize
{
    [BepInPlugin("com.maxgamertyper1.magnetisize", "Magneti Size", "1.0.0")]
    public class MagnetiSize : BaseUnityPlugin
    {
        private void Log(string message)
        {
            Logger.LogInfo(message);
        }

        private void Awake()
        {
            // Plugin startup logic
            Log($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            DoPatching();
        }

        private void DoPatching()
        {
            var harmony = new Harmony("com.maxgamertyper1.magnetisize");

            Patch(harmony, typeof(MagnetGun), "Shoot", "MagnetStrengthPrefix", true, false);
            Patch(harmony, typeof(MagnetGun), "TryPullItems", "PullStrengthPrefix", true, false);
            Patch(harmony, typeof(MagnetGun), "TryPickupNearbyItems", "TryPickupNearbyItemsPrefix", true, false);
        }

        private void OnDestroy()
        {
            Log($"Bye Bye From {PluginInfo.PLUGIN_GUID}");
        }

        private void Patch(Harmony harmony, Type OriginalClass , string OriginalMethod, string PatchMethod, bool prefix, bool transpiler)
        {
            MethodInfo MethodToPatch = AccessTools.Method(OriginalClass, OriginalMethod); // the method to patch
            MethodInfo Patch = AccessTools.Method(typeof(Patches), PatchMethod);
            
            if (prefix)
            {
                harmony.Patch(MethodToPatch, new HarmonyMethod(Patch));
            }
            else
            {
                if (transpiler)
                {
                    harmony.Patch(MethodToPatch, null, null, new HarmonyMethod(Patch));
                } else
                {
                    harmony.Patch(MethodToPatch, null, new HarmonyMethod(Patch));
                }
            }
            Log($"Patched {OriginalMethod} in {OriginalClass.ToString()}");
        }
    }

    public class Patches
    {
        public static void MagnetStrengthPrefix(ref MagnetGun __instance)
        {
            Fix multi = __instance.ability.playerCol.hurtBox.Scale;
            __instance.FireForce = multi * (Fix)10000;
            __instance.FireBlackHoleForce = multi * (Fix)30;
            __instance.FireMissileForce = multi * (Fix)75;
            __instance.FireBoulderForce = multi * (Fix)30000;
            __instance.FireForceMines = multi * (Fix)30;
            __instance.FireRagdollForce = multi * (Fix)60;
            __instance.FireRockForce = multi * (Fix)25000;
            __instance.fireRagdollDuration = multi * (Fix).3000000119;
        }
        public static void PullStrengthPrefix(ref MagnetGun __instance)
        {
            Fix multi = __instance.ability.playerCol.hurtBox.Scale;
            __instance.blackHolePullStr = multi * (Fix).6000000238;
            __instance.closePullStrMul = multi * (Fix)2;
            __instance.MachoSlimePullStr = multi * (Fix).0700000003;
            __instance.playerPullStr = multi * (Fix).25;
            __instance.projectilePullStr = multi * (Fix)500;
            __instance.pullStr = multi * (Fix)200;
            __instance.wallPullStr = multi * (Fix)5;
        }

        public static void TryPickupNearbyItemsPrefix(ref MagnetGun __instance)
        {
            Fix multi = __instance.ability.playerCol.hurtBox.Scale;
            __instance.pickupRadius = multi * (Fix)1.7000000477;
        }
    }
}
