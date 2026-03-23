// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

[Flags]
public enum Tag
{
    None,
    Ulti,
    Snow,
    AntiCrush = 1 << 2,
    BigNut = 1 << 3,
    Fly = 1 << 8,
    HardLand = 1 << 9,
    Water = 1 << 10,
    Pumpkin = 1 << 11,
    Lantern = 1 << 12,
    SmallLantern = 1 << 13,
    Puff = 1 << 14,
    Nut = 1 << 15,
    TallNut = 1 << 16,
    Potato = 1 << 17,
    Caltrop = 1 << 18,
    TangleKelp = 1 << 19,
    Magnet = 1 << 20,
    Pot = 1 << 21,
    Double = 1 << 22,
    SpikeRock = 1 << 23,
    Ice = 1 << 24,
    Fire = 1 << 25,
}

public static class TagExtensions
{
    public static bool Has(this Tag tag, Tag other) => (tag & other) == other;

    public static Enumerator GetEnumerator(this Tag tag) => new(tag);

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

    public struct Enumerator(Tag tag) : IEnumerator<Delegate?>
    {
        Bits<Tag>.Enumerator _enumerator = tag;

        /// <inheritdoc />
        readonly object? IEnumerator.Current => Current;

        public readonly Delegate? Current =>
            _enumerator.Current switch
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
        public void Reset() => _enumerator.Reset();

        /// <inheritdoc />
        public bool MoveNext() => _enumerator.MoveNext();
    }
}
