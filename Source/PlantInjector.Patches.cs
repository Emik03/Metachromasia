// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Metachromasia;

public abstract partial class PlantInjector<TPlugin, TPlant, TBullet> // ReSharper disable InconsistentNaming
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void MatchThisPlant(PlantType __0, ref bool __result)
    {
        if (__0 == Plant.Type)
            __result = true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool ArgCollidesTPlant<TZombie>(Object __instance, Object __0)
        where TZombie : Zombie =>
        __instance &&
        __instance.TryCast<TZombie>() is { theZombieRow: var zombieRow } &&
        __0 &&
        __0.GetComponent<TPlant>() is { thePlantRow: var plantRow } plant &&
        Matches(plant) &&
        plantRow == zombieRow;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsAnyThenTPlant<T>(T __instance, Object __0) => IsTPlant(__0);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsTThenTPlant<TZombie>(Object __instance, Object __0)
        where TZombie : Zombie =>
        __instance && __instance.TryCast<TZombie>() is not null && IsTPlant(__0);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsTThenAnyThenTPlant<TZombie, T>(Object __instance, T __result, Object __0)
        where TZombie : Zombie =>
        __instance && __instance.TryCast<TZombie>() is not null && IsTPlant(__0);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsTBullet(Object __instance) =>
        __instance && __instance.TryCast<TBullet>() is { } bullet && Matches(bullet);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static bool IsTPlant(Object __instance) =>
        __instance && __instance.TryCast<TPlant>() is { } plant && Matches(plant);

    public static Func<Patch> Postfix(
        Expression<Func<TPlant, Action>> target,
        Signatures.ActPatch<TPlant> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Postfix<T>(
        Expression<Func<TBullet, Action<T>>> target,
        Signatures.ActPatch<TBullet, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static Func<Patch> Postfix<T>(
        Expression<Func<TPlant, Action<T>>> target,
        Signatures.ActPatch<TPlant, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Postfix<TResult>(
        Expression<Func<TPlant, Func<TResult>>> target,
        Signatures.ActWithRetPatch<TPlant, TResult> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Postfix<TZombie>(
        Expression<Func<TZombie, Action<Plant>>> target,
        Signatures.ActPatch<TZombie, TPlant> impl,
        [CallerLineNumber] int line = 0
    )
        where TZombie : Zombie =>
        Fix(target, impl, line, IsTThenTPlant<TZombie>);

    public static Func<Patch> Prefix(
        Expression<Func<TBullet, Action>> target,
        Signatures.ActPatch<TBullet> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static Func<Patch> Prefix(
        Expression<Func<TPlant, Action>> target,
        Signatures.PredPatch<TPlant> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix(
        Expression<Func<TPlant, Action>> target,
        Signatures.ActPatch<TPlant> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix(
        Expression<Func<TBullet, Action<Zombie>>> target,
        Signatures.ActPatch<TBullet, Zombie> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static Func<Patch> Prefix<TZombie>(
        Expression<Func<TZombie, Action<Collider2D>>> target,
        Signatures.ActPatch<TZombie, Collider2D> impl,
        [CallerLineNumber] int line = 0
    )
        where TZombie : Zombie =>
        Fix(target, impl, line, ArgCollidesTPlant<TZombie>);

    public static Func<Patch> Prefix<T>(
        Expression<Func<TBullet, Action<T>>> target,
        Signatures.ActPatch<TBullet, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static Func<Patch> Prefix<T>(
        Expression<Func<TPlant, Action<T>>> target,
        Signatures.ActPatch<TPlant, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix<T>(
        Expression<Func<TPlant, Action<T>>> target,
        Signatures.PredPatch<TPlant, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix<T>(
        Expression<Func<T, Action<Plant>>> target,
        Signatures.ActPatch<T, TPlant> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsAnyThenTPlant<T>);

    public static Func<Patch> Prefix<T1, T2>(
        Expression<Func<TBullet, Action<T1, T2>>> target,
        Signatures.ActPatch<TBullet, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static Func<Patch> Prefix<T1, T2>(
        Expression<Func<TPlant, Action<T1, T2>>> target,
        Signatures.ActPatch<TPlant, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix<T1, T2>(
        Expression<Func<TPlant, Action<T1, T2>>> target,
        Signatures.PredPatch<TPlant, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix<T1, T2, T3>(
        Expression<Func<TPlant, Action<T1, T2, T3>>> target,
        Signatures.ActPatch<TPlant, T1, T2, T3> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix<T1, T2, T3, T4>(
        Expression<Func<TPlant, Action<T1, T2, T3, T4>>> target,
        Signatures.ActPatch<TPlant, T1, T2, T3, T4> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix<TResult>(
        Expression<Func<TPlant, Func<TResult>>> target,
        Signatures.ActWithRetPatch<TPlant, TResult> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static Func<Patch> Prefix<TZombie, TResult>(
        Expression<Func<TZombie, Func<Plant, TResult>>> target,
        Signatures.ActWithRetPatch<TZombie, TResult, TPlant> impl,
        [CallerLineNumber] int line = 0
    )
        where TZombie : Zombie =>
        Fix(target, impl, line, IsTThenAnyThenTPlant<TZombie, TResult>);

    public static Func<Patch> Prefix<TResult, T1, T2>(
        Expression<Func<TPlant, Func<T1, T2, TResult>>> target,
        Signatures.ActWithRetPatch<TPlant, TResult, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);
}
