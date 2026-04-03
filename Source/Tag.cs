// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

/// <summary>Represents the set of attributes that a <see cref="Plant"/> can possess.</summary>
[Flags]
public enum Tag
{
    /// <summary>No tag.</summary>
    None,

    /// <summary>This value is reserved for internal purposes and is not meant to be used outside of it.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    Reserved,

    /// <summary>Whether the plant is an ultimate, limiting its use in Odyssey levels.</summary>
    Ulti,

    /// <summary>Whether the plant is related to the snow levels.</summary>
    Snow = 1 << 2,

    /// <summary>Whether the plant is incapable of being crushed.</summary>
    AntiCrush = 1 << 3,

    /// <summary>Whether the plant is a giant nut that rolls across the field.</summary>
    BigNut = 1 << 4,

    /// <summary>Whether the plant is flying.</summary>
    Fly = 1 << 8,

    /// <summary>Whether the plant is in hard land.</summary>
    HardLand = 1 << 9,

    /// <summary>Whether the plant can only be placed on water.</summary>
    Water = 1 << 10,

    /// <summary>Whether the plant is a pumpkin, which can be placed on top of other plants.</summary>
    Pumpkin = 1 << 11,

    /// <summary>Whether the plant is illuminated.</summary>
    Lantern = 1 << 12,

    /// <summary>Whether the plant is a small lantern.</summary>
    SmallLantern = 1 << 13,

    /// <summary>Whether the plant is a puff-shroom.</summary>
    Puff = 1 << 14,

    /// <summary>Whether the plant is a nut.</summary>
    Nut = 1 << 15,

    /// <summary>Whether the plant is a tall nut.</summary>
    TallNut = 1 << 16,

    /// <summary>Whether the plant is a potato mine.</summary>
    Potato = 1 << 17,

    /// <summary>Whether the plant is a caltrop.</summary>
    Caltrop = 1 << 18,

    /// <summary>Whether the plant is a tangle kelp.</summary>
    TangleKelp = 1 << 19,

    /// <summary>Whether the plant is magnetized.</summary>
    Magnet = 1 << 20,

    /// <summary>Whether the plant is a pot, in which other plants can be planted on top of.</summary>
    Pot = 1 << 21,

    /// <summary>Whether the plant takes up two spaces, such as a cob cannon.</summary>
    Double = 1 << 22,

    /// <summary>Whether the plant is a spike rock.</summary>
    SpikeRock = 1 << 23,

    /// <summary>Whether the plant derives from an ice-shroom.</summary>
    Ice = 1 << 24,

    /// <summary>Whether the plant derives from a jalapeno.</summary>
    Fire = 1 << 25,
}

/// <summary>Contains extensions for the type <see cref="Tag"/>.</summary>
public static class TagExtensions
{
    /// <summary>Indicates whether the bit is set.</summary>
    /// <param name="tag">The tag to test.</param>
    /// <param name="other">The bit to test.</param>
    /// <returns>Whether the parameter <paramref name="tag"/> contains the bits of <paramref name="other"/>.</returns>
    public static bool Has(this Tag tag, Tag other) => (tag & other) == other;

    /// <summary>Gets the enumeration of methods to patch.</summary>
    /// <param name="tag">The tag to enumerate over.</param>
    /// <returns>The enumerator responsible for getting the methods that need to be patched.</returns>
    public static Enumerator GetEnumerator(this Tag tag) => new(tag);

    /// <summary>Converts the <see cref="Tag"/> into a <see cref="Plant.PlantTag"/>.</summary>
    /// <param name="tag">The tag to convert.</param>
    /// <returns>
    /// The <see cref="Plant.PlantTag"/> with values set by the parameter <paramref name="tag"/>.
    /// This conversion is lossy.
    /// </returns>
    public static Plant.PlantTag ToPlantTag(this Tag tag) =>
        new()
        {
            flyingPlant = tag.Has(Tag.Fly),
            hardLandPlant = tag.Has(Tag.HardLand),
            waterPlant = tag.Has(Tag.Water),
            pumpkinPlant = tag.Has(Tag.Pumpkin),
            lanternPlant = tag.Has(Tag.Lantern),
            smallLanternPlant = tag.Has(Tag.SmallLantern),
            puffPlant = tag.Has(Tag.Puff),
            nutPlant = tag.Has(Tag.Nut),
            tallNutPlant = tag.Has(Tag.TallNut),
            potatoPlant = tag.Has(Tag.Potato),
            caltropPlant = tag.Has(Tag.Caltrop),
            tanglekelpPlant = tag.Has(Tag.TangleKelp),
            magnetPlant = tag.Has(Tag.Magnet),
            potPlant = tag.Has(Tag.Pot),
            doubleBoxPlant = tag.Has(Tag.Double),
            spickRockPlant = tag.Has(Tag.SpikeRock),
            icePlant = tag.Has(Tag.Ice),
            firePlant = tag.Has(Tag.Fire),
        };

    /// <summary>Represents the enumerator for <see cref="Tag"/>.</summary>
    /// <param name="tag">The <see cref="Tag"/> to enumerate.</param>
    public struct Enumerator(Tag tag) : IEnumerator<Delegate?>
    {
        /// <summary>The current bit.</summary>
        Tag _current = Tag.Reserved;

        /// <inheritdoc />
        readonly object? IEnumerator.Current => Current;

        /// <inheritdoc />
        public readonly Delegate? Current =>
            _current switch
            {
                Tag.Ulti => Lawnf.IsUltiPlant,
                Tag.Snow => TypeMgr.IsSnowPlant,
                Tag.AntiCrush => TypeMgr.UncrashablePlant,
                Tag.BigNut => TypeMgr.BigNut,
                Tag.Fly => TypeMgr.FlyingPlants,
                Tag.Water => TypeMgr.IsWaterPlant,
                Tag.Pumpkin => TypeMgr.IsPumpkin,
                Tag.Lantern => TypeMgr.IsPlantern,
                Tag.SmallLantern => TypeMgr.IsSmallRangeLantern,
                Tag.Puff => TypeMgr.IsPuff,
                Tag.Nut => TypeMgr.IsNut,
                Tag.TallNut => TypeMgr.IsTallNut,
                Tag.Potato => TypeMgr.IsPotatoMine,
                Tag.Caltrop => TypeMgr.IsCaltrop,
                Tag.TangleKelp => TypeMgr.IsTangkelp,
                Tag.Magnet => TypeMgr.IsMagnetPlants,
                Tag.Pot => TypeMgr.IsPot,
                Tag.Double => TypeMgr.DoubleBoxPlants,
                Tag.SpikeRock => TypeMgr.IsSpickRock,
                Tag.Ice => TypeMgr.IsIcePlant,
                Tag.Fire => TypeMgr.IsFirePlant,
                _ => null,
            };

        /// <inheritdoc />
        readonly void IDisposable.Dispose() { }

        /// <inheritdoc />
        public void Reset() => _current = Tag.Reserved;

        /// <inheritdoc />
        public bool MoveNext()
        {
            while ((_current = (Tag)((int)_current << 1)) is not Tag.None)
                if ((tag & _current) is not Tag.None)
                    return true;

            return false;
        }
    }
}
