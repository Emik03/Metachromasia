// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Metachromasia;

using Fusions = IReadOnlyCollection<(PlantType, PlantType)>;
using Interface = IReadOnlyDictionary<string, Action<GameObject>>;

public sealed record PlantData(int Id, bool AddSeedSlot = false, Tag Tag = default, params Fusions Fusions) : Interface
{
    readonly Dictionary<string, Action<GameObject>> _dictionary = [with(StringComparer.Ordinal)];

    public PlantData(int id, params Fusions fusions)
        : this(id, false, default, fusions) { }

    public PlantData(int id, Tag tag, params Fusions fusions)
        : this(id, false, tag, fusions) { }

    public Action<GameObject> this[string name]
    {
        get => _dictionary[name];
        init => _dictionary[name] = value;
    }

    /// <inheritdoc />
    int IReadOnlyCollection<
        KeyValuePair<string, Action<GameObject>>>.Count =>
        _dictionary.Count;

    /// <inheritdoc />
    IEnumerable<string> Interface.Keys => _dictionary.Keys;

    /// <inheritdoc />
    IEnumerable<Action<GameObject>> Interface.Values => _dictionary.Values;

    public bool NoAdventure { get; init; }

    /// <summary>The damage of the plant.</summary>
    /// <remarks><para>In older versions: <c>attackDamage</c></para></remarks>
    public required int Damage { get; init; }

    public int AttributeCount { get; [UsedImplicitly] init; }

    /// <summary>The max health of the plant.</summary>
    /// <remarks><para>In older versions: <c>field_Public_Int32_0</c></para></remarks>
    public int Health { get; [UsedImplicitly] init; } = 300;

    /// <summary>The price in Sun for the card that places this plant.</summary>
    /// <remarks><para>In older versions: <c>field_Public_Int32_1</c></para></remarks>
    public int Price { get; [UsedImplicitly] init; } = 1000;

    /// <summary>The rate of attack.</summary>
    /// <remarks><para>In older versions: <c>field_Public_Single_0</c></para></remarks>
    public float AttackInterval { get; [UsedImplicitly] init; } = 1;

    /// <summary>The cooldown for the card that places this plant.</summary>
    /// <remarks><para>In older versions: <c>field_Public_Single_2</c></para></remarks>
    public float Cooldown { get; [UsedImplicitly] init; } = 60;

    /// <summary>The rate of production.</summary>
    /// <remarks><para>In older versions: <c>field_Public_Single_1</c></para></remarks>
    public float ProductionInterval { get; [UsedImplicitly] init; } = 1;

    public string? Prefix { get; init; }

    /// <summary>The id of this plant.</summary>
    /// <remarks><para>In older versions: <c>field_Public_PlantType_0</c></para></remarks>
    public PlantType Type => (PlantType)Id;

    public static explicit operator PlantDataManager.PlantData(PlantData data) =>
        new()
        {
            // field_Public_PlantType_0 = data.Type,
            // field_Public_Int32_0 = data.Health,
            // field_Public_Int32_1 = data.Price,
            // field_Public_Single_0 = data.AttackInterval,
            // field_Public_Single_1 = data.ProductionInterval,
            // field_Public_Single_2 = data.Cooldown,
            // attackDamage = data.Damage,
            cost = data.Price,
            cd = data.Cooldown,
            maxHealth = data.Health,
            thePlantType = data.Type,
            attackDamage = data.Damage,
            attackInterval = data.AttackInterval,
            produceInterval = data.ProductionInterval,
        };

    /// <summary>Gets the embedded assets from the plugin.</summary>
    /// <typeparam name="TPlugin">The plugin whose assembly contains assets.</typeparam>
    /// <returns>The list of assets from <typeparamref name="TPlugin"/>.</returns>
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static IReadOnlyList<Object> GetAssets<TPlugin>()
        where TPlugin : Localizable<TPlugin> =>
        s_assets.GetOrCreate(
            typeof(TPlugin).Assembly,
            _ => Il2CppAssetBundleManager.LoadFromMemory(GetEmbeddedBundle<TPlugin>()).LoadAllAssets()
        );

    /// <inheritdoc />
    public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out Action<GameObject> value) =>
        _dictionary.TryGetValue(key, out value);

    /// <inheritdoc />
    public IEnumerator<
        KeyValuePair<string, Action<GameObject>>> GetEnumerator() =>
        _dictionary.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    static readonly Dictionary<Assembly, IReadOnlyList<Object>> s_assets = [];

    static byte[] GetEmbeddedBundle<TPlugin>() =>
        $"{typeof(TPlugin).Assembly.GetName().Name}.bundle".Debug() is var name &&
        typeof(TPlugin).GetManifestResource<byte[]>(name) is { } bytes
            ? bytes
            : throw new InvalidOperationException(
                $"This assembly does not contain the following required file as an embedded resource: {name}"
            );
}
