// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Metachromasia;

extern alias core;
using Inference = (System.Predicate<PlantType> Predicate, System.Converter<Plant.PlantTag, Plant.PlantTag>? Converter);

public static class EntityExtensions
{
    internal static System.Collections.Generic.IEnumerable<Inference> Inferences { get; } =
    [
        (TypeMgr.IsNut, x => x with { nutPlant = true }),
        (TypeMgr.IsPot, x => x with { potPlant = true }),
        (TypeMgr.BigNut, x => x with { nutPlant = true }),
        (TypeMgr.IsPuff, x => x with { puffPlant = true }),
        (TypeMgr.IsIcePlant, x => x with { icePlant = true }),
        (TypeMgr.IsFirePlant, x => x with { firePlant = true }),
        (TypeMgr.IsCaltrop, x => x with { caltropPlant = true }),
        (TypeMgr.IsPumpkin, x => x with { pumpkinPlant = true }),
        (TypeMgr.IsTallNut, x => x with { tallNutPlant = true }),
        (TypeMgr.IsPlantern, x => x with { lanternPlant = true }),
        (TypeMgr.IsWaterPlant, x => x with { waterPlant = true }),
        (TypeMgr.FlyingPlants, x => x with { flyingPlant = true }),
        (TypeMgr.IsPotatoMine, x => x with { potatoPlant = true }),
        (TypeMgr.IsMagnetPlants, x => x with { magnetPlant = true }),
        (TypeMgr.IsSpickRock, x => x with { spickRockPlant = true }),
        (TypeMgr.IsTangkelp, x => x with { tanglekelpPlant = true }),
        (TypeMgr.DoubleBoxPlants, x => x with { doubleBoxPlant = true }),
        (TypeMgr.IsSmallRangeLantern, x => x with { smallLanternPlant = true }),
    ];

    internal static MethodInfo Cast { get; } = typeof(Il2CppObjectBase).GetMethod(
        nameof(Il2CppObjectBase.Cast),
        BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public
        // ReSharper disable once NullableWarningSuppressionIsUsed
    )!;

    static readonly bool s_debug = IsEnvironmentVariableTrue("METACHROMASIA_DEBUG");

    static EntityExtensions()
    {
        static void WriteLine(object? _, Logger.LogEventArgs evt) => System.Console.WriteLine(evt.Message);

        if (!IsEnvironmentVariableTrue("METACHROMASIA_DEBUG_HARMONY") || Logger.ChannelFilter is Logger.LogChannel.All)
            return;

        HarmonyFileLog.Enabled = true;
        Logger.MessageReceived += WriteLine;
        Logger.ChannelFilter = Logger.LogChannel.All;
    }

    static readonly int s_hash = Animator.StringToHash("AttackSpeed");

    public static void Sync(this BigGatling gatling)
    {
        if (gatling.anim)
            gatling.anim.SetFloat(s_hash, gatling.multiplier * gatling.attackSpeed);
    }

    public static void Die(this Zombie zombie, Plant.DieReason reason) => zombie.Die((int)reason);

    public static void Launch(this Bullet bullet, BulletType bulletType, Vector2 offset, int times = 1)
    {
        for (var i = 0; i < times; i++)
        {
            var p = bullet.transform.position;
            var b = CreateBullet.Instance.SetBullet(p.x, p.y, bullet.theBulletRow, bulletType, BulletMoveWay.Cannon);
            var r = b.theBulletRow;
            b.cannonPos = bullet.cannonPos + offset;
            b.rb.velocity = new(1.5f, 0);
            b.theBulletRow = Math.Clamp(offset.y switch { > 0 => ++r, < 0 => --r, _ => r }, 0, Board.Instance.rowNum);
        }
    }

    public static void Launch(this Plant plant, BulletType bulletType)
    {
        var p = plant.shoot.transform.position;
        var y = Mouse.Instance.GetRowFromY(plant.cannonTarget.x, plant.cannonTarget.y);
        var b = CreateBullet.Instance.SetBullet(p.x, p.y + 1.5f, y, bulletType, BulletMoveWay.Cannon);
        b.cannonPos = plant.cannonTarget;
        b.rb.velocity = new(1.5f, 0);
    }

    public static bool PushBack(this Zombie zombie)
    {
        if (TypeMgr.IsDriverZombie(zombie.theZombieType) || TypeMgr.IsBossZombie(zombie.theZombieType))
            zombie.transform.Translate(Vector3.right);

        return false;
    }

    public static int Row(this BulletMoveWay b) =>
        b switch { BulletMoveWay.Three_up => -1, BulletMoveWay.Three_down => 1, _ => 0 };

    public static float BulletSpeed(this BigGatling gatling) =>
        gatling.attackSpeed is var v &&
        Math.Abs(2 - v) < Math.Max(Math.Max(Math.Abs(v), 2) * 1e-06f, Mathf.Epsilon * 8) ? 12 :
        Math.Max(Math.Max(Math.Abs(v), 1.5f) * 1e-06f, Mathf.Epsilon * 8) <=
        Math.Abs(1.5f - v) ? 6 : 9;

    public static Bullet Shoot(
        this Plant plant,
        BulletType bulletType = BulletType.Bullet_pea,
        int? damage = null,
        BulletMoveWay move = BulletMoveWay.MoveRight,
        float? speed = null,
        System.Action<Bullet>? forEach = null,
        System.ReadOnlySpan<float> y = default,
        float x = 0,
        Vector2? origin = null
    )
    {
        var p = origin ?? InferShootOrigin(plant);
        Bullet? b = null;

        foreach (var s in y.IsEmpty ? [0] : y)
        {
            b = CreateBullet.Instance.SetBullet(p.x + x, p.y + s, plant.thePlantRow + move.Row(), bulletType, move);

            if (damage is { } d)
                b.Damage = d;

            if (speed is { } e)
                b.normalSpeed = e;

            forEach?.Invoke(b);
        }

        System.Diagnostics.Debug.Assert(b is not null);
        return b;
    }

    public static Bullet Shoot(
        this Plant plant,
        BulletType bulletType,
        System.Func<int>? onDamage,
        BulletMoveWay move = BulletMoveWay.MoveRight,
        float? speed = null,
        System.Action<Bullet>? forEach = null,
        System.ReadOnlySpan<float> y = default,
        float x = 0,
        Vector2? origin = null
    )
    {
        var p = origin ?? InferShootOrigin(plant);
        Bullet? b = null;

        foreach (var f in y.IsEmpty ? [0] : y)
        {
            b = CreateBullet.Instance.SetBullet(p.x + x, p.y + f, plant.thePlantRow + move.Row(), bulletType, move);

            if (onDamage?.Invoke() is { } d)
                b.Damage = d;

            if (speed is { } s)
                b.normalSpeed = s;

            forEach?.Invoke(b);
        }

        System.Diagnostics.Debug.Assert(b is not null);
        return b;
    }

    public static Bullet Shoot(
        this Plant plant,
        System.Func<BulletType> onBulletType,
        System.Func<int>? onDamage,
        BulletMoveWay move = BulletMoveWay.MoveRight,
        float? speed = null,
        System.Action<Bullet>? forEach = null,
        System.ReadOnlySpan<float> y = default,
        float x = 0,
        Vector2? origin = null
    )
    {
        var p = origin ?? InferShootOrigin(plant);
        Bullet? b = null;

        foreach (var f in y.IsEmpty ? [0] : y)
        {
            b = CreateBullet.Instance.SetBullet(p.x + x, p.y + f, plant.thePlantRow + move.Row(), onBulletType(), move);

            if (onDamage?.Invoke() is { } d)
                b.Damage = d;

            if (speed is { } s)
                b.normalSpeed = s;

            forEach?.Invoke(b);
        }

        System.Diagnostics.Debug.Assert(b is not null);
        return b;
    }

    public static GameObject Particle(this Bullet bullet, ParticleType particle) =>
        CreateParticle.SetParticle(particle, bullet.transform.position, bullet.theBulletRow);

    public static GameObject SetCoin(
        this CreateItem creator,
        int theColumn,
        int theRow,
        ItemType theItemType,
        MoveType theMoveType,
        Vector3 pos = default,
        bool freeSet = false
    ) =>
        creator.SetCoin(theColumn, theRow, (int)theItemType, (int)theMoveType, pos, freeSet);

    public static System.Collections.Generic.IEnumerable<Plant> GetPlantHits(this Component c, float r = 1.5f) =>
        c // ReSharper disable once Unity.PreferNonAllocApi
            ? Physics2D.OverlapCircleAll(c.transform.position, r)
               .Select(x => x.GetComponent<Plant>())
               .Where(x => x)
               .Distinct()
            : [];

    public static System.Collections.Generic.IEnumerable<Zombie> GetHits(this Bullet b, float r = 1.5f) =>
        b // ReSharper disable once Unity.PreferNonAllocApi
            ? Physics2D.OverlapCircleAll(b.transform.position, r, b.zombieLayer)
               .Select(x => x.GetComponent<Zombie>())
               .Where(x => x && x is { isMindControlled: false })
            : [];

    public static System.Collections.Generic.IEnumerable<Zombie> GetHits(this Plant b, float r = 1.5f) =>
        b // ReSharper disable once Unity.PreferNonAllocApi
            ? Physics2D.OverlapCircleAll(b.transform.position, r, b.zombieLayer)
               .Select(x => x.GetComponent<Zombie>())
               .Where(x => x && x is { isMindControlled: false })
            : [];

    public static System.Collections.Generic.IEnumerable<Zombie> GetExplodableHits(this Bullet b, float r = 1.5f) =>
        b.GetHits().Where(x => !x.beforeDying && x.theZombieRow - b.theBulletRow is -1 or 0 or 1);

    public static System.Collections.Generic.IEnumerable<Zombie> GetExplodableHits(this Plant b, float r = 1.5f) =>
        b.GetHits().Where(x => !x.beforeDying && x.theZombieRow - b.thePlantRow is -1 or 0 or 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref PlantType Ref(this in Span2D<PlantType> span, PlantType row, PlantType column) =>
        ref span[(int)row, (int)column];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PlantType? Get(this in Span2D<PlantType> span, PlantType row, PlantType column) =>
        span[(int)row, (int)column] is not PlantType.Peashooter and var ret ? ret : null;

    public static T Random<T>(this System.Collections.Generic.IReadOnlyList<T> l) =>
        l[core::UnityEngine.Random.Range(0, l.Count)];

    public static T Random<T>(this Il2CppSystem.Collections.Generic.List<T> l) =>
        l[core::UnityEngine.Random.Range(0, l.Count)];

    internal static bool Eq(this System.Predicate<PlantType> predicate, Inference inference) =>
        predicate.Method.MetadataToken == inference.Predicate.Method.MetadataToken &&
        predicate.Method.Module == inference.Predicate.Method.Module;

    [return: NotNullIfNotNull(nameof(t))]
    internal static T? Debug<T>(this T? t, [CallerLineNumber] int id = 0)
    {
        if (!s_debug)
            return t; // ReSharper disable once ExplicitCallerInfoArgument

        _ = (t as MethodInfo)?.DeclaringType.Debug(id) is var _;

        new MelonLogger.Instance($"{id}@{t?.GetType().Name ?? typeof(T).Name}").Msg(
            $"{(t is MethodInfo { IsStatic: true } ? "Static " : "")
            }{t?.ToString() ?? "null :("
            }{(t is byte[] { Length: var b } ? $", Length: {b}" : "")}"
        );

        return t;
    }

    internal static T? GetManifestResource<T>(this Type type, string suffix)
        where T : class
    {
        string? name;

        // ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var n in type.Assembly.GetManifestResourceNames())
            if (n.EndsWith(suffix))
            {
                name = n;
                goto Found;
            }

        return null;

    Found:
        using var stream = type.Assembly.GetManifestResourceStream(name);

        switch (stream)
        {
            case null: return null;
            case var _ when typeof(T) == typeof(string):
            {
                using StreamReader sr = new(stream);
                return (T)(object)sr.ReadToEnd();
            }
            case var _ when typeof(T) == typeof(byte[]):
            {
                using MemoryStream memory = new();
                stream.CopyTo(memory);
                return (T)(object)memory.ToArray();
            }
            default: throw new NotSupportedException();
        }
    }

    static bool IsEnvironmentVariableTrue(string var) =>
        Environment.GetEnvironmentVariable(var) is { } v &&
        (int.TryParse(v, out var i) && i is not 0 || bool.TryParse(v, out var b) && b);

    static Vector3 InferShootOrigin(Plant plant) =>
        plant.shoot ? plant.shoot.position :
        plant.transform.Find("Shoot") is var a && a ? a.position :
        plant.transform.Find("Throw") is var b && b ? b.position :
        plant.transform.Find("head/Shoot") is var c && c ? c.position :
        throw new InvalidOperationException("Cannot infer bullet location.");

    extension(CreateParticle)
    {
        public static GameObject
            SetParticle(ParticleType theParticleType, Vector3 position, int row, bool setLayer = true) =>
            CreateParticle.SetParticle((int)theParticleType, position, row, setLayer);
    }

    extension(Lawnf)
    {
        public static bool TravelAdvanced(int buff) => Lawnf.TravelAdvanced((AdvBuff)buff);

        public static bool TravelUltimate(int buff) => Lawnf.TravelUltimate((UltiBuff)buff);

        public static int TravelUltimateLevel(int buff) => Lawnf.TravelUltimateLevel((UltiBuff)buff);
    }
}
