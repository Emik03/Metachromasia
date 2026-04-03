// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

/// <summary>Contains a bunch of delegates containing signatures used for patches.</summary>
/// <remarks><para>
/// This type doesn't cover up to the same amount of parameters as <see cref="Action"/>
/// or <see cref="Func{TResult}"/>, but will cover the majority of signature use cases.
/// </para></remarks>
public static class Signatures // ReSharper disable InconsistentNaming
{
    /// <summary>Patch for <see cref="Action"/>.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    public delegate void ActPatch<in T>(T __instance);

    /// <summary>Patch for <see cref="Action"/> in prefixes that can skip execution.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    public delegate bool PredPatch<in T>(T __instance);

    /// <summary>Patch for <see cref="Action{T}"/>.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    public delegate void ActPatch<in T, T0>(T __instance, ref T0 __0);

    /// <summary>Patch for <see cref="Action{T}"/> in prefixes that can skip execution.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    public delegate bool PredPatch<in T, T0>(T __instance, ref T0 __0);

    /// <summary>Patch for <see cref="Action{T1, T2}"/>.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    /// <typeparam name="T1">The type of the second parameter.</typeparam>
    public delegate void ActPatch<in T, T0, T1>(T __instance, ref T0 __0, ref T1 __1);

    /// <summary>Patch for <see cref="Action{T1, T2}"/> in prefixes that can skip execution.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    /// <typeparam name="T1">The type of the second parameter.</typeparam>
    public delegate bool PredPatch<in T, T0, T1>(T __instance, ref T0 __0, ref T1 __1);

    /// <summary>Patch for <see cref="Action{T1, T2, T3}"/>.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    /// <typeparam name="T1">The type of the second parameter.</typeparam>
    /// <typeparam name="T2">The type of the third parameter.</typeparam>
    public delegate void ActPatch<in T, T0, T1, T2>(T __instance, ref T0 __0, ref T1 __1, ref T2 __2);

    /// <summary>Patch for <see cref="Action{T1, T2, T3}"/> in prefixes that can skip execution.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    /// <typeparam name="T1">The type of the second parameter.</typeparam>
    /// <typeparam name="T2">The type of the third parameter.</typeparam>
    public delegate bool PredPatch<in T, T0, T1, T2>(T __instance, ref T0 __0, ref T1 __1, ref T2 __2);

    /// <summary>Patch for <see cref="Action{T1, T2, T3, T4}"/>.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    /// <typeparam name="T1">The type of the second parameter.</typeparam>
    /// <typeparam name="T2">The type of the third parameter.</typeparam>
    /// <typeparam name="T3">The type of the fourth parameter.</typeparam>
    public delegate void ActPatch<in T, T0, T1, T2, T3>(T __instance, ref T0 __0, ref T1 __1, ref T2 __2, ref T3 __3);

    /// <summary>Patch for <see cref="Action{T1, T2, T3, T4}"/> in prefixes that can skip execution.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    /// <typeparam name="T1">The type of the second parameter.</typeparam>
    /// <typeparam name="T2">The type of the third parameter.</typeparam>
    /// <typeparam name="T3">The type of the fourth parameter.</typeparam>
    public delegate bool PredPatch<in T, T0, T1, T2, T3>(T __instance, ref T0 __0, ref T1 __1, ref T2 __2, ref T3 __3);

    /// <summary>Patch for <see cref="Func{TResult}"/>.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="TR">The type of non-void return value.</typeparam>
    public delegate void ActWithRetPatch<in T, TR>(T __instance, ref TR __result);

    /// <summary>Patch for <see cref="Func{TResult}"/> in prefixes that can skip execution.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="TR">The type of non-void return value.</typeparam>
    public delegate bool PredWithRetPatch<in T, TR>(T __instance, ref TR __result);

    /// <summary>Patch for <see cref="Func{T, TResult}"/>.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="TR">The type of non-void return value.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    public delegate void ActWithRetPatch<in T, TR, T0>(T __instance, ref TR __result, ref T0 __0);

    /// <summary>Patch for <see cref="Func{T, TResult}"/> in prefixes that can skip execution.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="TR">The type of non-void return value.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    public delegate void PredWithRetPatch<in T, TR, T0>(T __instance, ref TR __result, ref T0 __0);

    /// <summary>Patch for <see cref="Func{T1, T2, TResult}"/>.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="TR">The type of non-void return value.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    /// <typeparam name="T1">The type of the second parameter.</typeparam>
    public delegate void ActWithRetPatch<in T, TR, T0, T1>(T __instance, ref TR __result, ref T0 __0, ref T1 __1);

    /// <summary>Patch for <see cref="Func{T1, T2, TResult}"/> in prefixes that can skip execution.</summary>
    /// <typeparam name="T">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <typeparam name="TR">The type of non-void return value.</typeparam>
    /// <typeparam name="T0">The type of the first parameter.</typeparam>
    /// <typeparam name="T1">The type of the second parameter.</typeparam>
    public delegate bool PredWithRetPatch<in T, TR, T0, T1>(T __instance, ref TR __result, ref T0 __0, ref T1 __1);
}
