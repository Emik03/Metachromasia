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

    extension(Tag tag)
    {
        /// <summary>Indicates whether the bit is set.</summary>
        /// <param name="other">The bit to test.</param>
        /// <returns>Whether the parameter <paramref name="tag"/> contains the bits of <paramref name="other"/>.</returns>
        public bool this[Tag other] => (tag & other) == other;

        /// <summary>Converts the <see cref="Tag"/> into a <see cref="Plant.PlantTag"/>.</summary>
        /// <returns>
        /// The <see cref="Plant.PlantTag"/> with values set by the parameter <paramref name="tag"/>.
        /// This conversion is lossy.
        /// </returns>
        public Plant.PlantTag ToPlantTag =>
            new()
            {
                flyingPlant = tag[Tag.Fly],
                hardLandPlant = tag[Tag.HardLand],
                waterPlant = tag[Tag.Water],
                pumpkinPlant = tag[Tag.Pumpkin],
                lanternPlant = tag[Tag.Lantern],
                smallLanternPlant = tag[Tag.SmallLantern],
                puffPlant = tag[Tag.Puff],
                nutPlant = tag[Tag.Nut],
                tallNutPlant = tag[Tag.TallNut],
                potatoPlant = tag[Tag.Potato],
                caltropPlant = tag[Tag.Caltrop],
                tanglekelpPlant = tag[Tag.TangleKelp],
                magnetPlant = tag[Tag.Magnet],
                potPlant = tag[Tag.Pot],
                doubleBoxPlant = tag[Tag.Double],
                spickRockPlant = tag[Tag.SpikeRock],
                icePlant = tag[Tag.Ice],
                firePlant = tag[Tag.Fire],
            };

        /// <summary>Gets the enumeration of methods to patch.</summary>
        /// <returns>The enumerator responsible for getting the methods that need to be patched.</returns>
        public Enumerator GetEnumerator() => new(tag);
    }
}
