using System;
using System.Reflection;
using JetBrains.Annotations;

namespace AbbLab.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        ///   <para>Initializes a new instance of the specified <paramref name="type"/> using its parameterless constructor.</para>
        /// </summary>
        /// <param name="type">The type to initialize an instance of.</param>
        /// <returns>The initialized instance of the specified <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="type"/> does not have a parameterless constructor.</exception>
        [Pure] public static object MakeInstance(this Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type, true)!;
            ConstructorInfo? ctor = type.FindConstructor(Type.EmptyTypes);
            if (ctor is null) throw new ArgumentException($"{type} doesn't have a parameterless constructor.", nameof(type));
            return ctor.Invoke(null);
        }
        /// <summary>
        ///   <para>Constructs a generic type from the specified <paramref name="type"/> and <paramref name="typeArguments"/> and initializes a new instance of that type using its parameterless constructor.</para>
        /// </summary>
        /// <param name="type">The generic type definition of the type to initialize an instance of.</param>
        /// <param name="typeArguments">The type arguments of the type to construct.</param>
        /// <returns>The initialized instance of a type constructed from the specified <paramref name="type"/> and <paramref name="typeArguments"/>.</returns>
        /// <exception cref="ArgumentException">The constructed type does not have a parameterless constructor.</exception>
        [Pure] public static object MakeGenericInstance(this Type type, params Type[] typeArguments)
            => type.MakeGenericType(typeArguments).MakeInstance();

        /// <summary>
        ///   <para>Returns the <see langword="default"/> value for the specified <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="type">The type to get the default value for.</param>
        /// <returns>An empty value type, if <paramref name="type"/> is a value type; otherwise, <see langword="null"/>, if it's a reference type.</returns>
        [Pure] public static object? GetDefaultValue(this Type type) => type.IsValueType ? Activator.CreateInstance(type, true) : null;

        private const BindingFlags allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags allExceptStaticFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags allExceptInstanceFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        ///   <para>Returns an array of all fields in the specified <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the fields in.</param>
        /// <returns>An array of all fields in the specified <paramref name="type"/>.</returns>
        [Pure] public static FieldInfo[] GetAllFields(this Type type) => type.GetFields(allFlags);
        /// <summary>
        ///   <para>Returns an array of all properties in the specified <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the properties in.</param>
        /// <returns>An array of all properties in the specified <paramref name="type"/>.</returns>
        [Pure] public static PropertyInfo[] GetAllProperties(this Type type) => type.GetProperties(allFlags);
        /// <summary>
        ///   <para>Returns an array of all methods in the specified <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the methods in.</param>
        /// <returns>An array of all methods in the specified <paramref name="type"/>.</returns>
        [Pure] public static MethodInfo[] GetAllMethods(this Type type) => type.GetMethods(allFlags);
        /// <summary>
        ///   <para>Returns an array of all constructors in the specified <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the constructors in.</param>
        /// <returns>An array of all constructors in the specified <paramref name="type"/>.</returns>
        [Pure] public static ConstructorInfo[] GetAllConstructors(this Type type) => type.GetConstructors(allExceptStaticFlags);

        /// <summary>
        ///   <para>Returns a field in the specified <paramref name="type"/> with the specified <paramref name="fieldName"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the fields in.</param>
        /// <param name="fieldName">The name of the field to search for.</param>
        /// <returns>An object representing the field, if found; otherwise, <see langword="null"/>.</returns>
        [Pure] public static FieldInfo? FindField(this Type type, string fieldName) => type.GetField(fieldName, allFlags);
        /// <summary>
        ///   <para>Returns a property in the specified <paramref name="type"/> with the specified <paramref name="propertyName"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the properties in.</param>
        /// <param name="propertyName">The name of the property to search for.</param>
        /// <returns>An object representing the property, if found; otherwise, <see langword="null"/>.</returns>
        [Pure] public static PropertyInfo? FindProperty(this Type type, string propertyName) => type.GetProperty(propertyName, allFlags);
        /// <summary>
        ///   <para>Returns a method in the specified <paramref name="type"/> with the specified <paramref name="methodName"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the methods in.</param>
        /// <param name="methodName">The name of the method to search for.</param>
        /// <returns>An object representing the method, if found; otherwise, <see langword="null"/>.</returns>
        [Pure] public static MethodInfo? FindMethod(this Type type, string methodName) => type.GetMethod(methodName, allFlags);
        /// <summary>
        ///   <para>Returns a method in the specified <paramref name="type"/> with the specified <paramref name="methodName"/> and <paramref name="parameterTypes"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the methods in.</param>
        /// <param name="methodName">The name of the method to search for.</param>
        /// <param name="parameterTypes">An array of method parameter types to search for.</param>
        /// <returns>An object representing the method, if found; otherwise, <see langword="null"/>.</returns>
        [Pure] public static MethodInfo? FindMethod(this Type type, string methodName, Type[]? parameterTypes)
            => type.GetMethod(methodName, allFlags, null, parameterTypes ?? Type.EmptyTypes, null);
        /// <summary>
        ///   <para>Returns a constructor in the specified <paramref name="type"/> with the specified <paramref name="parameterTypes"/>.</para>
        /// </summary>
        /// <param name="type">The type to search the constructors in.</param>
        /// <param name="parameterTypes">An array of constructor parameter types to search for.</param>
        /// <returns>An object representing the constructor, if found; otherwise, <see langword="null"/>.</returns>
        [Pure] public static ConstructorInfo? FindConstructor(this Type type, Type[]? parameterTypes)
            => type.GetConstructor(allExceptStaticFlags, null, parameterTypes ?? Type.EmptyTypes, null);
        /// <summary>
        ///   <para>Returns the static constructor of the specified <paramref name="type"/>.</para>
        /// </summary>
        /// <param name="type">The type to get the static constructor of.</param>
        /// <returns>An object representing the constructor, if found; otherwise, <see langword="null"/>.</returns>
        [Pure] public static ConstructorInfo? GetStaticConstructor(this Type type)
            => type.GetConstructor(allExceptInstanceFlags, null, Type.EmptyTypes, null);

    }
}
