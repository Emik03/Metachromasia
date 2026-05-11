// SPDX-License-Identifier: MPL-2.0
namespace Metachromasia;

/// <summary>Represents either an eager value or a lazy value.</summary>
/// <typeparam name="T">The type of value.</typeparam>
/// <param name="Lazy">The lazy value.</param>
/// <param name="Eager">The eager value.</param>
public readonly record struct LazyOrEager<T>(Func<T>? Lazy, T? Eager = default)
{
    /// <summary>Determines whether the value is lazy.</summary>
    [MemberNotNullWhen(false, nameof(Eager)), MemberNotNullWhen(true, nameof(Lazy))]
    public bool IsLazy => Lazy is not null;

    /// <summary>Gets the value.</summary>
    public T Value => IsLazy ? Lazy() : Eager;

    /// <summary>Creates the inner value.</summary>
    /// <param name="lazyOrEager">The union to get the value from.</param>
    /// <returns>The value.</returns>
    public static implicit operator T(LazyOrEager<T> lazyOrEager) => lazyOrEager.Value;

    /// <summary>Creates the eager value.</summary>
    /// <param name="eager">The value to store.</param>
    /// <returns>The eager value.</returns>
    public static implicit operator LazyOrEager<T>(T eager) => new(null, eager);
}
