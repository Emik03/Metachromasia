// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

/// <summary>Contains extensions for the type <see cref="Tag"/>.</summary>
public static class TagExtensions
{
    /// <summary>Represents the enumerator for <see cref="Tag"/>.</summary>
    /// <param name="tag">The <see cref="Tag"/> to enumerate.</param>
    [StructLayout(LayoutKind.Auto)]
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

    /// <param name="tag">The tag to convert.</param>
    extension(Tag tag)
    {
        /// <summary>Indicates whether the bit is set.</summary>
        /// <param name="other">The bit to test.</param>
        /// <returns>Whether the parameter <paramref name="tag"/> contains the bits of <paramref name="other"/>.</returns>
        public bool Has(Tag other) => (tag & other) == other;

        /// <summary>Gets the enumeration of methods to patch.</summary>
        /// <returns>The enumerator responsible for getting the methods that need to be patched.</returns>
        public Enumerator GetEnumerator() => new(tag);

        /// <summary>Converts the <see cref="Tag"/> into a <see cref="Plant.PlantTag"/>.</summary>
        /// <returns>
        /// The <see cref="Plant.PlantTag"/> with values set by the parameter <paramref name="tag"/>.
        /// This conversion is lossy.
        /// </returns>
        public Plant.PlantTag ToPlantTag() =>
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
    }
}
