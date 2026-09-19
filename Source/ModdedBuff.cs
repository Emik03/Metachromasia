// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

/// <summary>Represents a buff that already has inline information of the index and description.</summary>
[RegisterTypeInIl2Cpp]
sealed class ModdedBuff : BaseBuff<AdvBuff>
{
    /// <summary>Contains the index of <see cref="AdvBuff"/> in <see cref="BuffType"/>.</summary>
    readonly int _index;

    /// <inheritdoc />
    public ModdedBuff()
        : base(ClassInjector.DerivedConstructorPointer<ModdedBuff>()) =>
        ClassInjector.DerivedConstructorBody(this);

    /// <inheritdoc />
    public ModdedBuff(int index, string description)
        : this() =>
        (_index, Description) = (index, description);

    /// <inheritdoc />
    [UsedImplicitly]
    public ModdedBuff(IntPtr ptr)
        : base(ptr) { }

    /// <inheritdoc />
    public override AdvBuff BuffType => (AdvBuff)_index;

    /// <inheritdoc />
    public override string? Description { get; }
}
