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

    public static System.Func<Patch> Postfix(
        System.Linq.Expressions.Expression<System.Func<TPlant, Action>> target,
        Signatures.ActPatch<TPlant> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Postfix<T>(
        System.Linq.Expressions.Expression<System.Func<TBullet, System.Action<T>>> target,
        Signatures.ActPatch<TBullet, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static System.Func<Patch> Postfix<T>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Action<T>>> target,
        Signatures.ActPatch<TPlant, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Postfix<TResult>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Func<TResult>>> target,
        Signatures.ActWithRetPatch<TPlant, TResult> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Postfix<TZombie>(
        System.Linq.Expressions.Expression<System.Func<TZombie, System.Action<Plant>>> target,
        Signatures.ActPatch<TZombie, TPlant> impl,
        [CallerLineNumber] int line = 0
    )
        where TZombie : Zombie =>
        Fix(target, impl, line, IsTThenTPlant<TZombie>);

    public static System.Func<Patch> Prefix(
        System.Linq.Expressions.Expression<System.Func<TBullet, Action>> target,
        Signatures.ActPatch<TBullet> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static System.Func<Patch> Prefix(
        System.Linq.Expressions.Expression<System.Func<TPlant, Action>> target,
        Signatures.PredPatch<TPlant> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix(
        System.Linq.Expressions.Expression<System.Func<TPlant, Action>> target,
        Signatures.ActPatch<TPlant> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix(
        System.Linq.Expressions.Expression<System.Func<TBullet, System.Action<Zombie>>> target,
        Signatures.ActPatch<TBullet, Zombie> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static System.Func<Patch> Prefix<TZombie>(
        System.Linq.Expressions.Expression<System.Func<TZombie, System.Action<Collider2D>>> target,
        Signatures.ActPatch<TZombie, Collider2D> impl,
        [CallerLineNumber] int line = 0
    )
        where TZombie : Zombie =>
        Fix(target, impl, line, ArgCollidesTPlant<TZombie>);

    public static System.Func<Patch> Prefix<T>(
        System.Linq.Expressions.Expression<System.Func<TBullet, System.Action<T>>> target,
        Signatures.ActPatch<TBullet, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static System.Func<Patch> Prefix<T>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Action<T>>> target,
        Signatures.ActPatch<TPlant, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix<T>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Action<T>>> target,
        Signatures.PredPatch<TPlant, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix<T>(
        System.Linq.Expressions.Expression<System.Func<T, System.Action<Plant>>> target,
        Signatures.ActPatch<T, TPlant> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsAnyThenTPlant<T>);

    public static System.Func<Patch> Prefix<T1, T2>(
        System.Linq.Expressions.Expression<System.Func<TBullet, System.Action<T1, T2>>> target,
        Signatures.ActPatch<TBullet, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTBullet);

    public static System.Func<Patch> Prefix<T1, T2>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Action<T1, T2>>> target,
        Signatures.ActPatch<TPlant, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix<T1, T2>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Action<T1, T2>>> target,
        Signatures.PredPatch<TPlant, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix<T1, T2, T3>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Action<T1, T2, T3>>> target,
        Signatures.ActPatch<TPlant, T1, T2, T3> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix<T1, T2, T3, T4>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Action<T1, T2, T3, T4>>> target,
        Signatures.ActPatch<TPlant, T1, T2, T3, T4> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix<TResult>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Func<TResult>>> target,
        Signatures.ActWithRetPatch<TPlant, TResult> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);

    public static System.Func<Patch> Prefix<TZombie, TResult>(
        System.Linq.Expressions.Expression<System.Func<TZombie, System.Func<Plant, TResult>>> target,
        Signatures.ActWithRetPatch<TZombie, TResult, TPlant> impl,
        [CallerLineNumber] int line = 0
    )
        where TZombie : Zombie =>
        Fix(target, impl, line, IsTThenAnyThenTPlant<TZombie, TResult>);

    public static System.Func<Patch> Prefix<TResult, T1, T2>(
        System.Linq.Expressions.Expression<System.Func<TPlant, System.Func<T1, T2, TResult>>> target,
        Signatures.ActWithRetPatch<TPlant, TResult, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line, IsTPlant);
}
