// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

[RegisterTypeInIl2Cpp]
sealed class ModdedBuff : BaseBuff<AdvBuff>
{
    readonly int _index;

    readonly string? _description;

    public ModdedBuff()
        : base(ClassInjector.DerivedConstructorPointer<ModdedBuff>()) =>
        ClassInjector.DerivedConstructorBody(this);

    public ModdedBuff(int index, string description)
        : this() =>
        (_index, _description) = (index, description);

    [UsedImplicitly]
    public ModdedBuff(IntPtr ptr)
        : base(ptr) { }

    /// <inheritdoc />
    public override AdvBuff BuffType => (AdvBuff)_index;

    /// <inheritdoc />
    public override string? GetDescription() => _description;
}
