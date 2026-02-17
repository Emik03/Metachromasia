// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Metachromasia;

using Chars = System.ReadOnlySpan<char>;
using Patches = System.Collections.Generic.IReadOnlyCollection<System.Func<Patch>>;

public abstract partial class Localizable<TPlugin>(params Patches patches) : MelonMod
    where TPlugin : Localizable<TPlugin>
{
    static string? s_loc;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static HarmonyLib.Harmony? GetHarmony()
    {
        for (var i = 0; i < RegisteredMelons.Count && RegisteredMelons[i] is var melon; i++)
            if (typeof(TPlugin).Assembly == melon.MelonAssembly.Assembly)
                return melon.HarmonyInstance;

        return null;
    }

    [return: NotNullIfNotNull(nameof(predicate))]
    public static Il2CppSystem.Predicate<T>? ToIl2Cpp<T>(System.Predicate<T> predicate)
    {
        var ret = DelegateSupport.ConvertDelegate<Il2CppSystem.Predicate<T>>(predicate);
        Debug.Assert(predicate is null || ret is not null);
        return ret;
    }

    public static Chars Localize(string raw, string? fallback = null) =>
        MelonPreferences.GetEntryValue<string?>("PvZ_Fusion_Translator", "Language") is { } language &&
        Find(raw, language, out var first) ? first :
        Find(raw, null, out var second) ? second : fallback ?? $"{{{raw}}}";

    [EditorBrowsable(EditorBrowsableState.Advanced), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span2D<T> ToSpan2D<T>(Il2CppObjectBase obj)
    {
        // Hex dump of a 2-dimensional array:
        //             00  01  02  03  04  05  06  07  08  09  0a  0b  0c  0d  0e  0f
        //           +----------------------------------------------------------------+
        // 00000000: | 48, 55, 6f, a6, 76, 7f, 00, 00, 88, 14, a9, 9e, 76, 7f, 00, 00 |
        // 00000010: | 00, 00, 00, 00, 00, 00, 00, 00, c0, ed, d8, ae, 95, 7f, 00, 00 |
        // 00000020: | int *length..., 00, 00, 00, 00, int *height..., int *width.... |
        // 00000030: | 00, 00, 00, 00, 00, 00, 00, 00, int *values................... |
        //           +----------------------------------------------------------------+
        ref var root = ref Unsafe.AddByteOffset(ref Unsafe.NullRef<int>(), obj.Pointer);

        return Span2D<T>.DangerousCreate(
            ref Unsafe.As<int, T>(ref Unsafe.AddByteOffset(ref root, 0x38)),
            Unsafe.AddByteOffset(ref root, 0x28),
            Unsafe.AddByteOffset(ref root, 0x2c),
            0
        );
    }

    /// <inheritdoc />
    public override void OnEarlyInitializeMelon()
    {
        base.OnEarlyInitializeMelon();
        var isPort = GetType().Name is "Plugin";

        LoggerInstance.Msg(
            isPort
                ? "Credits: Original mod by LibraHP, MelonLoader port by Emik. Made with love. <3"
                : $"Using {nameof(Metachromasia)} v{typeof(Localizable<>).Assembly.GetName().Version}."
        );

        if ($"{(isPort ? GetType().Assembly.GetName().Name : GetType().Name)}.loc.txt".Debug() is var path &&
            string.IsNullOrWhiteSpace(s_loc = GetType().GetManifestResource<string>(path)))
            LoggerInstance.Error($"Failed to acquire localization! Is \"{path}\" included as an embedded resource?");
    }

    /// <inheritdoc />
    public override void OnInitializeMelon()
    {
        if ((GetHarmony() ?? HarmonyInstance) is { } harmony)
            Patch(harmony);
        else
            LoggerInstance?.Warning("Refusing to patch because no harmony instance could be found.");
    }

    protected virtual void Patch(HarmonyLib.Harmony harmony)
    {
        foreach (var p in patches)
        {
            var (target, impl, type) = p();
            var isPrefix = type is HarmonyPatchType.All or HarmonyPatchType.Prefix;
            var isPostfix = type is HarmonyPatchType.All or HarmonyPatchType.Postfix;
            harmony.Patch(target.Debug(), isPrefix ? new(impl.Debug()) : null, isPostfix ? new(impl.Debug()) : null);
        }
    }

    static bool Find(string? raw, string? preference, out Chars text)
    {
        const char Start = '{', End = '}', Separator = '.';

        foreach (var read in MemoryExtensions.AsSpan(s_loc).Tokenize(Start))
        {
            var header = SplitBy(read, End, out var rest);
            var key = SplitBy(header, Separator, out var lang);

            if (lang.Equals(preference, StringComparison.Ordinal) && key.Equals(raw, StringComparison.Ordinal))
                return (text = SplitBy(rest, Start, out _).Trim()) is var _;
        }

        text = default;
        return false;
    }

    static Chars SplitBy(scoped in Chars text, char separator, out Chars after)
    {
        if (text.IndexOf(separator) is var i && (i is -1 || i + 1 == text.Length))
        {
            after = default;
            return text;
        }

        after = text[(i + 1)..];
        return text[..i];
    }
}
