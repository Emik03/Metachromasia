// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Metachromasia;

using Fusions = System.Collections.Generic.IReadOnlyCollection<(PlantType, PlantType)>;
using Interface = System.Collections.Generic.IReadOnlyDictionary<string, System.Action<GameObject>>;

public sealed record PlantData(int Id, bool AddSeedSlot = false, params Fusions Fusions) : Interface
{
    readonly System.Collections.Generic.Dictionary<string, System.Action<GameObject>> _dictionary =
        new(StringComparer.Ordinal);

    public PlantData(int id, params Fusions fusions)
        : this(id, false, fusions) { }

    public System.Action<GameObject> this[string name]
    {
        get => _dictionary[name];
        init => _dictionary[name] = value;
    }

    /// <inheritdoc />
    int System.Collections.Generic.IReadOnlyCollection<
        System.Collections.Generic.KeyValuePair<string, System.Action<GameObject>>>.Count =>
        _dictionary.Count;

    /// <inheritdoc />
    System.Collections.Generic.IEnumerable<string> Interface.Keys => _dictionary.Keys;

    /// <inheritdoc />
    System.Collections.Generic.IEnumerable<System.Action<GameObject>> Interface.Values => _dictionary.Values;

    public bool NoAdventure { get; init; }

    public required int Damage { get; init; }

    public int AttributeCount { get; [UsedImplicitly] init; }

    public int Health { get; [UsedImplicitly] init; } = 300;

    public int Price { get; [UsedImplicitly] init; } = 1000;

    public float AttackInterval { get; [UsedImplicitly] init; } = 1;

    public float Cooldown { get; [UsedImplicitly] init; } = 60;

    public float ProductionInterval { get; [UsedImplicitly] init; } = 1;

    public string? Prefix { get; init; }

    public PlantType Type => (PlantType)Id;

    public static explicit operator PlantDataManager.PlantData(PlantData data) =>
        new()
        {
            field_Public_PlantType_0 = data.Type,
            maxHealth = data.Health,
            cost = data.Price,
            attackInterval = data.AttackInterval,
            field_Public_Single_0 = data.ProductionInterval,
            cd = data.Cooldown,
            attackDamage = data.Damage,
        };

    /// <inheritdoc />
    public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

    /// <inheritdoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out System.Action<GameObject> value) =>
        _dictionary.TryGetValue(key, out value);

    /// <inheritdoc />
    public System.Collections.Generic.IEnumerator<
        System.Collections.Generic.KeyValuePair<string, System.Action<GameObject>>> GetEnumerator() =>
        _dictionary.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
