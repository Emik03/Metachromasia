// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Metachromasia;

extern alias core;

/// <summary>Provides extensions for entities.</summary>
public static class EntityExtensions
{
    /// <summary>Whether to log debug information.</summary>
    static readonly bool s_debug = IsEnvironmentVariableTrue("METACHROMASIA_DEBUG");

    /// <summary>Hooks onto <see cref="HarmonyLib.Tools.Logger"/> if the appropriate environment flag is set.</summary>
    static EntityExtensions()
    {
        static void WriteLine(object? _, Logger.LogEventArgs evt) => Console.WriteLine(evt.Message);

        if (Logger.ChannelFilter is Logger.LogChannel.All || !IsEnvironmentVariableTrue("METACHROMASIA_DEBUG_HARMONY"))
            return;

        HarmonyFileLog.Enabled = true;
        Logger.MessageReceived += WriteLine;
        Logger.ChannelFilter = Logger.LogChannel.All;
    }

    /// <summary>The hash for <c>AttackSpeed</c>.</summary>
    static readonly int s_attackSpeed = Animator.StringToHash("AttackSpeed");

    extension(BigGatling gatling)
    {
        /// <summary>Gets the bullet speed.</summary>
        public float BulletSpeed =>
            gatling.attackSpeed is var v &&
            Math.Abs((int)(2 - v)) < Math.Max(Math.Max(Math.Abs(v), 2) * 1e-06f, Mathf.Epsilon * 8) ? 12 :
            Math.Max(Math.Max(Math.Abs(v), 1.5f) * 1e-06f, Mathf.Epsilon * 8) <=
            Math.Abs(1.5f - v) ? 6 : 9;

        /// <summary>Synchronizes the animation.</summary>
        public void Sync()
        {
            if (gatling.anim)
                gatling.anim.SetFloat(s_attackSpeed, gatling.multiplier * gatling.attackSpeed);
        }
    }

    extension(Bullet bullet)
    {
        /// <summary>Creates a bullet that launches.</summary>
        /// <param name="bulletType"></param>
        /// <param name="offset"></param>
        /// <param name="times"></param>
        public void Launch(BulletType bulletType, Vector2 offset, int times = 1)
        {
            var row = bullet.theBulletRow;
            var p = bullet.transform.position;
            var rowNum = Board.Instance.rowNum;

            for (var i = 0; i < times; i++)
            {
                var b = CreateBullet.Instance.SetBullet(p.x, p.y, row, bulletType, BulletMoveWay.Cannon);
                var r = b.theBulletRow;
                b.rb.velocity = new(1.5f, 0);
                b.cannonPos = bullet.cannonPos + offset;
                b.theBulletRow = Math.Clamp(offset.y switch { > 0 => r + 1, < 0 => r - 1, _ => r }, 0, rowNum);
            }
        }

        /// <summary>Creates the particle from the bullet.</summary>
        /// <param name="particle">The <see cref="ParticleType"/> to create.</param>
        /// <returns>The particle <see cref="GameObject"/>.</returns>
        public GameObject Particle(ParticleType particle) =>
            CreateParticle.SetParticle(particle, bullet.transform.position, bullet.theBulletRow);

        /// <summary>Gets the zombies that can explode within the bullet's range.</summary>
        /// <param name="r">The radius.</param>
        /// <returns>The enumeration containing zombies that can explode that are hit by this bullet.</returns>
        public IEnumerable<Zombie> GetExplodableHits(float r = 1.5f) =>
            bullet.GetHits().Where(x => !x.beforeDying && x.theZombieRow - bullet.theBulletRow is -1 or 0 or 1);

        /// <summary>Gets the zombies within the bullet's range.</summary>
        /// <param name="r">The radius.</param>
        /// <returns>The enumeration containing zombies hit by this bullet.</returns>
        public IEnumerable<Zombie> GetHits(float r = 1.5f) =>
            bullet // ReSharper disable once Unity.PreferNonAllocApi
                ? Physics2D.OverlapCircleAll(bullet.transform.position, r, bullet.zombieLayer)
                   .Select(x => x.GetComponent<Zombie>())
                   .Where(x => x && x is { isMindControlled: false })
                : [];
    }

    extension(BulletMoveWay b)
    {
        /// <summary>Gets the row offset.</summary>
        public int Row => b switch { BulletMoveWay.Three_up => -1, BulletMoveWay.Three_down => 1, _ => 0 };
    }

    extension(Component c)
    {
        /// <summary>Gets the plant hits.</summary>
        /// <param name="r">The radius.</param>
        /// <returns>The enumeration of plants.</returns>
        public IEnumerable<Plant> GetPlantHits(float r = 1.5f) =>
            c // ReSharper disable once Unity.PreferNonAllocApi
                ? Physics2D.OverlapCircleAll(c.transform.position, r)
                   .Select(x => x.GetComponent<Plant>())
                   .Where(x => x)
                   .Distinct()
                : [];
    }

    extension(CreateItem creator)
    {
        /// <summary>Alternative signature for <see cref="CreateItem.SetCoin"/>.</summary>
        public GameObject SetCoin(
            int theColumn,
            int theRow,
            ItemType theItemType,
            MoveType theMoveType,
            Vector3 pos = default,
            bool freeSet = false
        ) =>
            creator.SetCoin(theColumn, theRow, (int)theItemType, (int)theMoveType, pos, freeSet);
    }

    extension(CreateParticle)
    {
        /// <summary>Alternative signature for <see cref="CreateParticle.SetParticle"/>.</summary>
        public static GameObject SetParticle(
            ParticleType theParticleType,
            Vector3 position,
            int row,
            bool setLayer = true
        ) =>
            CreateParticle.SetParticle((int)theParticleType, position, row, setLayer);
    }

    extension<T>(IReadOnlyList<T> l)
    {
        /// <summary>Gets a random element of the list.</summary>
        /// <returns>The random element.</returns>
        public T Random => l[Random.Range(0, l.Count)];
    }

    extension(Lawnf)
    {
        /// <summary>Alternative signature for <see cref="Lawnf.TravelAdvanced"/>.</summary>
        public static bool TravelAdvanced(int buff) => Lawnf.TravelAdvanced((AdvBuff)buff);

        /// <summary>Alternative signature for <see cref="Lawnf.TravelUltimate"/>.</summary>
        public static bool TravelUltimate(int buff) => Lawnf.TravelUltimate((UltiBuff)buff);

        /// <summary>Alternative signature for <see cref="Lawnf.TravelUltimateLevel"/>.</summary>
        public static int TravelUltimateLevel(int buff) => Lawnf.TravelUltimateLevel((UltiBuff)buff);
    }

    extension<T>(Il2CppSystem.Collections.Generic.List<T> l)
    {
        /// <summary>Gets a random element of the list.</summary>
        /// <returns>The random element.</returns>
        public T Random => l[Random.Range(0, l.Count)];
    }

    extension(Il2CppSystem.Object obj)
    {
        /// <summary>Attempts to unbox the object into the specified value type.</summary>
        /// <typeparam name="T">The type of value to attempt to unbox.</typeparam>
        /// <returns>The unboxed value, or <see langword="null"/> if the object is not of that type.</returns>
        public T? TryUnbox<T>()
            where T : unmanaged =>
            Il2CppClassPointerStore<T>.NativeClassPtr != default &&
            IL2CPP.il2cpp_class_is_assignable_from(
                Il2CppClassPointerStore<T>.NativeClassPtr,
                IL2CPP.il2cpp_object_get_class(obj.Pointer)
            )
                ? Unsafe.AddByteOffset(ref Unsafe.NullRef<T>(), IL2CPP.il2cpp_object_unbox(obj.Pointer))
                : default;
    }

    extension(Plant plant)
    {
        /// <summary>Creates a bullet that launches.</summary>
        /// <param name="bulletType">The type of bullet to create.</param>
        public void Launch(BulletType bulletType)
        {
            var p = plant.shoot.transform.position;
            var y = Mouse.Instance.GetRowFromY(plant.cannonTarget.x, plant.cannonTarget.y);
            var b = CreateBullet.Instance.SetBullet(p.x, p.y + 1.5f, y, bulletType, BulletMoveWay.Cannon);
            b.cannonPos = plant.cannonTarget;
            b.rb.velocity = new(1.5f, 0);
        }

        /// <summary>Damages the plant.</summary>
        /// <param name="damage">The amount of damage to apply.</param>
        public void TakeDamage(int damage) => plant.TakeDamage(damage, null);

        /// <summary>Shoots a bullet.</summary>
        /// <param name="bulletType">The bullet type.</param>
        /// <param name="damage">The damage of the bullet.</param>
        /// <param name="move">The type of movement of the bullet.</param>
        /// <param name="speed">The speed of the bullet.</param>
        /// <param name="forEach">Invoked at the end of initializing each individual bullet.</param>
        /// <param name="y">The y offsets for each bullet.</param>
        /// <param name="x">The x offset for all bullets.</param>
        /// <param name="origin">The origin point for spawning bullets at.</param>
        /// <returns>The last created bullet.</returns>
        public Bullet Shoot(
            LazyOrEager<BulletType> bulletType = default,
            LazyOrEager<int?> damage = default,
            BulletMoveWay move = BulletMoveWay.MoveRight,
            float? speed = null,
            Action<Bullet>? forEach = null,
            ReadOnlySpan<float> y = default,
            float x = 0,
            Vector2? origin = null
        )
        {
            var p = origin ?? InferShootOrigin(plant);
            Bullet? b = null;

            foreach (var f in y.IsEmpty ? [0] : y)
            {
                b = CreateBullet.Instance.SetBullet(p.x + x, p.y + f, plant.thePlantRow + move.Row, bulletType, move);

                if (damage.Value is { } d)
                    b.Damage = d;

                if (speed is { } s)
                    b.normalSpeed = s;

                forEach?.Invoke(b);
            }

            System.Diagnostics.Debug.Assert(b is not null);
            return b;
        }

        /// <summary>Gets the zombies that can explode within the plant's range.</summary>
        /// <param name="r">The radius.</param>
        /// <returns>The enumeration containing zombies that can explode that are hit by this plant.</returns>
        public IEnumerable<Zombie> GetHits(float r = 1.5f) =>
            plant // ReSharper disable once Unity.PreferNonAllocApi
                ? Physics2D.OverlapCircleAll(plant.transform.position, r, plant.zombieLayer)
                   .Select(x => x.GetComponent<Zombie>())
                   .Where(x => x && x is { isMindControlled: false })
                : [];

        /// <summary>Gets the zombies within the plant's range.</summary>
        /// <param name="r">The radius.</param>
        /// <returns>The enumeration containing zombies hit by this plant.</returns>
        public IEnumerable<Zombie> GetExplodableHits(float r = 1.5f) =>
            plant.GetHits().Where(x => !x.beforeDying && x.theZombieRow - plant.thePlantRow is -1 or 0 or 1);
    }

    extension(Type type)
    {
        /// <summary>Gets the manifest resource.</summary>
        /// <exception cref="NotSupportedException">
        /// The type parameter <typeparamref name="T"/> is not a <see cref="string"/>
        /// or a single-dimensional zero-indexed <see cref="byte"/> array.
        /// </exception>
        /// <typeparam name="T">
        /// The return type. Must be <see cref="string"/> or a single-dimensional zero-indexed <see cref="byte"/> array.
        /// </typeparam>
        /// <param name="suffix">The suffix of the requested resource.</param>
        /// <returns></returns>
        public T? GetManifestResource<T>(string suffix)
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
    }

    extension<T>(T? t)
    {
        /// <summary>Logs the value if diagnostics are enabled.</summary>
        /// <param name="line">The line number of the caller, making it easier to trace back in source.</param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(t))]
        internal T? Debug([CallerLineNumber] int line = 0)
        {
            if (!s_debug)
                return t; // ReSharper disable once ExplicitCallerInfoArgument

            _ = (t as MethodInfo)?.DeclaringType.Debug(line) is var _;

            new MelonLogger.Instance($"{line}@{t?.GetType().Name ?? typeof(T).Name}").Msg(
                $"{(t is MethodInfo { IsStatic: true } ? "Static " : "")
                }{t?.ToString() ?? "null :("
                }{(t is not string and ICollection { Count: var b } ? $", Length: {b}" : "")}"
            );

            return t;
        }
    }

    extension(Zombie zombie)
    {
        /// <summary>Damages the zombie.</summary>
        /// <param name="damageType">The type of damage.</param>
        /// <param name="damage">The amount of damage to apply.</param>
        public void TakeDamage(DamageType damageType, int damage) => zombie.TakeDamage(damage, null, damageType);

        /// <summary>Pushes back the zombie.</summary>
        /// <returns>The value <see langword="false"/>.</returns>
        public bool PushBack()
        {
            if (TypeMgr.IsDriverZombie(zombie.theZombieType) || TypeMgr.IsBossZombie(zombie.theZombieType))
                zombie.transform.Translate(Vector3.right);

            return false;
        }
    }

    /// <summary>Determines whether the environment variable is set to a truthy value.</summary>
    /// <param name="var">The environment variable name to check.</param>
    /// <returns>Whether the parameter <paramref name="var"/> is set to a truthy value.</returns>
    static bool IsEnvironmentVariableTrue(string var) =>
        Environment.GetEnvironmentVariable(var) is { } v &&
        (int.TryParse(v, out var i) && i is not 0 || bool.TryParse(v, out var b) && b);

    /// <summary>Infers the origin position of the shoot.</summary>
    /// <exception cref="InvalidOperationException">The origin cannot be inferred.</exception>
    /// <param name="plant">The plant to infer its shoot origin.</param>
    /// <returns>The origin for bullets.</returns>
    static Vector3 InferShootOrigin(Plant plant) =>
        plant.shoot ? plant.shoot.position :
        plant.transform.Find("Shoot") is var a && a ? a.position :
        plant.transform.Find("Throw") is var b && b ? b.position :
        plant.transform.Find("head/Shoot") is var c && c ? c.position :
        throw new InvalidOperationException("Cannot infer bullet location.");
}
