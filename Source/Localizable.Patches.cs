// SPDX-License-Identifier: MPL-2.0
// ReSharper disable once CheckNamespace
namespace Metachromasia;

using static OpCodes;

public abstract partial class Localizable<TPlugin>
{
    /// <summary>Creates a postfix.</summary>
    /// <typeparam name="TInstance">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <param name="target">The method to postfix.</param>
    /// <param name="impl">The postfix method.</param>
    /// <param name="line">Used as part of the generated method name to make it easier to locate to source.</param>
    /// <returns>The postfix.</returns>
    public static Func<Patch> Postfix<TInstance>(
        Expression<Func<TInstance, Action>> target,
        Signatures.ActPatch<TInstance> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Postfix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Postfix<TInstance, T>(
        Expression<Func<TInstance, Action<T>>> target,
        Signatures.ActPatch<TInstance, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Postfix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Postfix<TInstance, T1, T2>(
        Expression<Func<TInstance, Action<T1, T2>>> target,
        Signatures.ActPatch<TInstance, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Postfix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Postfix<TInstance, T1, T2, T3>(
        Expression<Func<TInstance, Action<T1, T2, T3>>> target,
        Signatures.ActPatch<TInstance, T1, T2, T3> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Postfix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Postfix<TInstance, T1, T2, T3, T4>(
        Expression<Func<TInstance, Action<T1, T2, T3, T4>>> target,
        Signatures.ActPatch<TInstance, T1, T2, T3, T4> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Postfix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Postfix<TInstance, TResult, T>(
        Expression<Func<TInstance, Func<T, TResult>>> target,
        Signatures.ActWithRetPatch<TInstance, TResult, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Postfix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Postfix<TInstance, TResult, T1, T2>(
        Expression<Func<TInstance, Func<T1, T2, TResult>>> target,
        Signatures.ActWithRetPatch<TInstance, TResult, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Postfix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Func<Patch> PostfixDangerous<TInstance, TAccessor, TImpl>(
        Expression<Func<TInstance, TAccessor>> target,
        TImpl impl,
        [CallerLineNumber] int line = 0
    )
        where TAccessor : Delegate
        where TImpl : Delegate =>
        Fix(target, impl, line, null, nameof(Postfix));

    /// <summary>Creates a prefix.</summary>
    /// <typeparam name="TInstance">The type of instance, or <see cref="ValueTuple"/> for static methods.</typeparam>
    /// <param name="target">The method to prefix.</param>
    /// <param name="impl">The prefix method.</param>
    /// <param name="line">Used as part of the generated method name to make it easier to locate to source.</param>
    /// <returns>The prefix.</returns>
    public static Func<Patch> Prefix<TInstance>(
        Expression<Func<TInstance, Action>> target,
        Signatures.ActPatch<TInstance> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Prefix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Prefix<TInstance>(
        Expression<Func<TInstance, Action>> target,
        Signatures.PredPatch<TInstance> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Prefix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Prefix<TInstance, T>(
        Expression<Func<TInstance, Action<T>>> target,
        Signatures.ActPatch<TInstance, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Prefix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Prefix<TInstance, T>(
        Expression<Func<TInstance, Action<T>>> target,
        Signatures.PredPatch<TInstance, T> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Prefix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Prefix<TInstance, T1, T2>(
        Expression<Func<TInstance, Action<T1, T2>>> target,
        Signatures.PredPatch<TInstance, T1, T2> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Prefix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Prefix<TInstance, T1, T2, T3>(
        Expression<Func<TInstance, Action<T1, T2, T3>>> target,
        Signatures.PredPatch<TInstance, T1, T2, T3> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Prefix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Prefix<TInstance, TResult>(
        Expression<Func<TInstance, Func<TResult>>> target,
        Signatures.ActWithRetPatch<TInstance, TResult> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Prefix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    public static Func<Patch> Prefix<TInstance, TResult>(
        Expression<Func<TInstance, Func<TResult>>> target,
        Signatures.PredWithRetPatch<TInstance, TResult> impl,
        [CallerLineNumber] int line = 0
    ) =>
        Fix(target, impl, line);

    /// <inheritdoc cref="Prefix{TInstance}(Expression{Func{TInstance, Action}}, Signatures.ActPatch{TInstance}, int)"/>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static Func<Patch> PrefixDangerous<TInstance, TAccessor, TImpl>(
        Expression<Func<TInstance, TAccessor>> target,
        TImpl impl,
        [CallerLineNumber] int line = 0
    )
        where TAccessor : Delegate
        where TImpl : Delegate =>
        Fix(target, impl, line, null, nameof(Prefix));

    /// <summary>Raw method for creating a patch.</summary>
    /// <typeparam name="T">The signature for the implementation.</typeparam>
    /// <param name="targetEx">The target expression.</param>
    /// <param name="impl">The implementation.</param>
    /// <param name="line">The line number.</param>
    /// <param name="prefix">The added prefix.</param>
    /// <param name="name">The type of infix.</param>
    /// <returns>The infix.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected static Func<Patch> Fix<T>(
        LambdaExpression targetEx,
        T impl,
        int line,
        Delegate? prefix = null,
        [CallerMemberName] string name = ""
    )
        where T : Delegate =>
        () =>
        {
            var target = Target(targetEx);
            var patchType = Enum.Parse<HarmonyPatchType>(name);
            var realTarget = GetOverridenTarget(SignatureOf<T>(), target) ?? target;
            return Make(realTarget, impl, prefix, patchType, line);
        };

    /// <summary>Emits the function call.</summary>
    /// <param name="il">The generator to append instructions to.</param>
    /// <param name="del">The delegate to call.</param>
    /// <param name="newParams">The parameters.</param>
    // ReSharper disable once SuggestBaseTypeForParameter
    static void EmitCall(ILGenerator il, Delegate del, Type[] newParams)
    {
        var cast = typeof(Il2CppObjectBase).GetMethod(
            nameof(Il2CppObjectBase.Cast),
            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public
        );

        var dParams = del.Method.GetParameters();
        var delType = del.Method.DeclaringType;
        var isStatic = dParams.Length - 1 == newParams.Length;
        Debug.Assert(cast is not null);
        Debug.Assert(delType is not null);
        RuntimeHelpers.RunClassConstructor(delType.TypeHandle);

        var target = delType == del.Target?.GetType()
            ? delType.GetFields(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => x.FieldType == delType)
            : null;

        if (target is not null)
            il.Emit(Ldsfld.Debug(), target.Debug());

        for (var i = 0; i < dParams.Length && dParams[i].ParameterType is var p; i++)
        {
            if (i is 0 && isStatic)
            {
                il.Emit(Ldnull);
                continue;
            }

            var offset = isStatic ? 1 : 0;
            var newParam = newParams[i - offset];

            if (p.IsByRef && !newParam.IsByRef)
                Emit(il, i, Ldarga_S);
            else
            {
                Emit(il, i, Ldarg_0, Ldarg_1, Ldarg_2, Ldarg_3, Ldarg_S);

                if (!p.IsByRef && newParam.IsByRef)
                    il.Emit(LoadIndirectly(p).Debug());
            }

            if (typeof(Il2CppObjectBase).IsAssignableFrom(p) && !ToDeref(p).IsAssignableFrom(ToDeref(newParam)))
                il.Emit(Callvirt.Debug(), cast.MakeGenericMethod(p).Debug());
        }

        il.Emit(target is null ? Call.Debug() : Callvirt.Debug(), del.Method.Debug());
    }

    /// <summary>Emits the opcode.</summary>
    /// <param name="il">The generator to append instructions to.</param>
    /// <param name="i">The index into <paramref name="opCodes"/>.</param>
    /// <param name="opCodes">The opcodes.</param>
    static void Emit(ILGenerator il, int i, params ReadOnlySpan<OpCode> opCodes)
    {
        if (opCodes.Length - 1 is var last && i < last)
            il.Emit(opCodes[i].Debug());
        else
            il.Emit(opCodes[last].Debug(), i.Debug());
    }

    /// <summary>Emits the instructions for calling the method.</summary>
    /// <typeparam name="T">The type of implementation.</typeparam>
    /// <param name="il">The generator to append instructions to.</param>
    /// <param name="impl">The implementation.</param>
    /// <param name="prefix">The added prefix.</param>
    /// <param name="patchType">The type of patch.</param>
    /// <param name="newParams">The new parameters.</param>
    static void Emit<T>(ILGenerator il, T impl, Delegate? prefix, HarmonyPatchType patchType, Type[] newParams)
        where T : Delegate
    {
        if (prefix is null)
        {
            EmitCall(il, impl, newParams);
            il.Emit(Ret.Debug());
            return;
        }

        var jump = il.DefineLabel();
        EmitCall(il, prefix, newParams);
        il.Emit(Brfalse_S.Debug(), jump.Debug());
        EmitCall(il, impl, newParams);

        if (patchType is HarmonyPatchType.Prefix)
        {
            if (SignatureOf<T>().ReturnType != typeof(bool))
                il.Emit(Ldc_I4_0.Debug());

            il.Emit(Ret.Debug());
        }

        il.MarkLabel(jump.Debug());

        if (patchType is HarmonyPatchType.Prefix)
            il.Emit(Ldc_I4_1.Debug());

        il.Emit(Ret.Debug());
    }

    /// <summary>Gets the method that the lambda represents.</summary>
    /// <exception cref="InvalidOperationException">
    /// The parameter <paramref name="expression"/>'s method cannot be resolved.
    /// </exception>
    /// <param name="expression">The expression to get the method of.</param>
    /// <returns>The method of the lambda.</returns>
    static MethodInfo Target(LambdaExpression expression) =>
        expression.Body switch
        {
            MethodCallExpression { Method: { } method } => method,
            UnaryExpression
            {
                Operand: MethodCallExpression { Object: ConstantExpression { Value: MethodInfo method } },
            } => method,
            _ => throw new InvalidOperationException(expression.Body.GetType().FullName),
        };

    /// <summary>Gets the overriden target.</summary>
    /// <param name="signature">The signature.</param>
    /// <param name="target">The target.</param>
    /// <returns>The overriden target.</returns>
    static MethodInfo? GetOverridenTarget(MethodBase signature, MethodBase target) =>
        signature.GetParameters()[0]
           .ParameterType.GetMethod(
                target.Name,
                target.GetGenericArguments().Length,
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly,
                null,
                [..target.GetParameters().Select(x => x.ParameterType)],
                null
            );

    /// <summary>Gets the signature of the type.</summary>
    /// <typeparam name="T">The type to get the signature of.</typeparam>
    /// <returns>The signature of the type parameter <typeparamref name="T"/>.</returns>
    static MethodInfo SignatureOf<T>()
    {
        const BindingFlags Instance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        var method = typeof(T).GetMethod(nameof(Action.Invoke), Instance);
        Debug.Assert(method is not null);
        return method;
    }

    /// <summary>Creates the patch.</summary>
    /// <typeparam name="T">The type of implementation.</typeparam>
    /// <param name="methodBase">The method to patch.</param>
    /// <param name="impl">The implementation.</param>
    /// <param name="prefix">The added prefix.</param>
    /// <param name="patchType">The type of patch.</param>
    /// <param name="line">The line number of the caller's caller.</param>
    /// <param name="id">The line number of the caller.</param>
    /// <returns>The patch.</returns>
    static Patch Make<T>(
        MethodInfo methodBase,
        T impl,
        Delegate? prefix,
        HarmonyPatchType patchType,
        int line,
        [CallerLineNumber] int id = 0
    )
        where T : Delegate
    {
        const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

        const MethodAttributes MethodAttribute = MethodAttributes.Public | MethodAttributes.Static;

        const TypeAttributes TypeAttribute = TypeAttributes.Public |
            TypeAttributes.Abstract |
            TypeAttributes.Sealed |
            TypeAttributes.AutoClass |
            TypeAttributes.AnsiClass |
            TypeAttributes.BeforeFieldInit;

        Debug.Assert(methodBase.DeclaringType is not null);
        var signature = SignatureOf<T>();
        var signatureParams = signature.GetParameters()[(methodBase.IsStatic ? 1 : 0)..];
        StringBuilder methodNameBuilder = new(methodBase.Name);

        for (var type = methodBase.DeclaringType; type is not null; type = type.DeclaringType)
            methodNameBuilder.Insert(0, '_').Insert(0, type.Name);

        var methodName = methodNameBuilder.Append('_').Append(id).Append('_').Append(line).ToString();
        var typeName = $"{nameof(Metachromasia)}.{new StackFrame(2).GetMethod()?.DeclaringType?.FullName}";
        var assemblyAndModuleName = $"{typeName}-{methodName}";
        var newAssembly = AssemblyBuilder.DefineDynamicAssembly(new(assemblyAndModuleName), AssemblyBuilderAccess.Run);
        var newType = newAssembly.DefineDynamicModule(assemblyAndModuleName).DefineType(new(typeName), TypeAttribute);
        var newRet = prefix is null || patchType is not HarmonyPatchType.Prefix ? signature.ReturnType : typeof(bool);

        Type[] newParams =
        [
            ..methodBase.IsStatic ? Enumerable.Empty<Type>() : [methodBase.DeclaringType],
            ..methodBase.ReturnType == typeof(void) ? Enumerable.Empty<Type>() : [ToRef(methodBase.ReturnParameter)],
            ..methodBase.GetParameters().Select(ToRef),
        ];

        var newMethod = newType.DefineMethod(methodName, MethodAttribute, newRet, newParams);

        for (var i = 0; i < signatureParams.Length && signatureParams[i] is var param; i++)
        {
            newMethod.DefineParameter(i + 1, param.Attributes, param.Name);

            if (param.Attributes is not ParameterAttributes.None)
                (i + 1, param.Attributes, param.Name).Debug();
        }

        Emit(newMethod.GetILGenerator(), impl, prefix, patchType, newParams);
        var reflectedType = newType.CreateType();
        Debug.Assert(reflectedType is not null);
        return (methodBase, reflectedType.GetMethods(BindingFlag).Single(x => !x.IsConstructor), patchType);
    }

    /// <summary>Gets the referenced type.</summary>
    /// <param name="p">The parameter type to make by-reference.</param>
    /// <returns>The by-referenced type.</returns>
    static Type ToRef(ParameterInfo p) => p.ParameterType.IsByRef ? p.ParameterType : p.ParameterType.MakeByRefType();

    /// <summary>Gets the non-referenced type.</summary>
    /// <param name="type">The type to potentially get the underlying type of.</param>
    /// <returns>The non-referenced type.</returns>
    static Type ToDeref(Type type) => type.IsByRef ? type.GetElementType()! : type;

    /// <summary>Gets the load indirect instruction for the respective type.</summary>
    /// <param name="type">The type to get the load indirect instruction for.</param>
    /// <returns>The instruction.</returns>
    static OpCode LoadIndirectly(Type type) =>
        (type.IsEnum ? type.GetEnumUnderlyingType() : type) switch
        {
            var x when x == typeof(nint) || x == typeof(nuint) => Ldind_I,
            var x when x == typeof(sbyte) => Ldind_I1,
            var x when x == typeof(short) => Ldind_I2,
            var x when x == typeof(int) => Ldind_I4,
            var x when x == typeof(long) || x == typeof(ulong) => Ldind_I8,
            var x when x == typeof(float) => Ldind_R4,
            var x when x == typeof(double) => Ldind_R8,
            var x when x == typeof(byte) => Ldind_U1,
            var x when x == typeof(ushort) => Ldind_U2,
            var x when x == typeof(uint) => Ldind_U4,
            { IsPointer: true } => Ldind_I,
            { IsClass: true } or { IsInterface: true } => Ldind_Ref,
            _ => Ldobj,
        };
}
