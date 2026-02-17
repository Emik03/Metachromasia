// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

public static class Signatures // ReSharper disable InconsistentNaming
{
    public delegate void ActPatch<in T>(T __instance);

    public delegate bool PredPatch<in T>(T __instance);

    public delegate void ActPatch<in T, T0>(T __instance, ref T0 __0);

    public delegate bool PredPatch<in T, T0>(T __instance, ref T0 __0);

    public delegate void ActPatch<in T, T0, T1>(T __instance, ref T0 __0, ref T1 __1);

    public delegate bool PredPatch<in T, T0, T1>(T __instance, ref T0 __0, ref T1 __1);

    public delegate void ActPatch<in T, T0, T1, T2>(T __instance, ref T0 __0, ref T1 __1, ref T2 __2);

    public delegate bool PredPatch<in T, T0, T1, T2>(T __instance, ref T0 __0, ref T1 __1, ref T2 __2);

    public delegate void ActPatch<in T, T0, T1, T2, T3>(T __instance, ref T0 __0, ref T1 __1, ref T2 __2, ref T3 __3);

    public delegate bool PredPatch<in T, T0, T1, T2, T3>(T __instance, ref T0 __0, ref T1 __1, ref T2 __2, ref T3 __3);

    public delegate void ActWithRetPatch<in T, TR>(T __instance, ref TR __result);

    public delegate bool PredWithRetPatch<in T, TR>(T __instance, ref TR __result);

    public delegate void ActWithRetPatch<in T, TR, T0>(T __instance, ref TR __result, ref T0 __0);

    public delegate void ActWithRetPatch<in T, TR, T0, T1>(T __instance, ref TR __result, ref T0 __0, ref T1 __1);

    public delegate bool PredWithRetPatch<in T, TR, T0, T1>(T __instance, ref TR __result, ref T0 __0, ref T1 __1);
}
