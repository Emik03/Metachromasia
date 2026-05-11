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
