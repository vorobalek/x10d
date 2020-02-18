using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Расширения типа <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        public static Type[] GetBaseTypes(this Type type)
        {
            var current = type;
            var types = new List<Type>(new [] { type });

            while (current.BaseType != null)
            {
                current = current.BaseType;
                types.Add(current);
            }

            return types.ToArray();
        }

        /// <summary>
        /// Determines whether this instance is ineterface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is ineterface; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIneterface<T>(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(T));
        }

        /// <summary>
        /// Gets the properties with values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, TypedObject> GetPropertiesWithValues<T>(this T obj)
            where T: class
        {
            return new Dictionary<string, TypedObject>
                (typeof(T)
                .GetProperties()
                .Select(prop => new KeyValuePair<string, TypedObject>(prop.Name, new TypedObject(prop.GetType(), prop.GetValue(obj)))));
        }

        /// <summary>
        /// Gets the editable properties with values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, TypedObject> GetEditablePropertiesWithValues<T>(this T obj)
            where T: class
        {
            return new Dictionary<string, TypedObject>
                (typeof(T)
                .GetProperties()
                .Where(prop => prop.CanRead && prop.CanWrite)
                .Select(prop => new KeyValuePair<string, TypedObject>(prop.Name, new TypedObject(prop.GetType(), prop.GetValue(obj)))));
        }

        /// <summary>
        /// Получить имя производного типа.
        /// </summary>
        /// <param name="type">Краткое имя типа.</param>
        /// <returns></returns>
        public static string GetName(this Type type)
        {
            if (type?.IsGenericType ?? false)
            {
                var genericTypesNames = new List<string>();
                foreach (var item in type.GenericTypeArguments)
                {
                    genericTypesNames.Add(item.GetName());
                }

                return $"{type.Name.Split('`')[0]}<{string.Join(", ", genericTypesNames)}>";
            }

            return type?.Name;
        }

        /// <summary>
        /// Получить полное имя производного типа.
        /// </summary>
        /// <param name="type">Полное имя типа.</param>
        /// <returns></returns>
        public static string GetFullName(this Type type)
        {
            if (type?.IsGenericType ?? false)
            {
                var genericTypesNames = new List<string>();
                foreach (var item in type.GenericTypeArguments)
                {
                    genericTypesNames.Add(item.GetFullName());
                }

                return $"{type.Name.Split('`')[0]}<{string.Join(", ", genericTypesNames)}>";
            }

            return type?.FullName;
        }

        public static bool isPublic(this Type t)
        {
            return
                t.IsVisible
                && t.IsPublic
                && !t.IsNotPublic
                && !t.IsNested
                && !t.IsNestedPublic
                && !t.IsNestedFamily
                && !t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }

        public static bool isInternal(this Type t)
        {
            return
                !t.IsVisible
                && !t.IsPublic
                && t.IsNotPublic
                && !t.IsNested
                && !t.IsNestedPublic
                && !t.IsNestedFamily
                && !t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }

        // only nested types can be declared "protected"
        public static bool isProtected(this Type t)
        {
            return
                !t.IsVisible
                && !t.IsPublic
                && !t.IsNotPublic
                && t.IsNested
                && !t.IsNestedPublic
                && t.IsNestedFamily
                && !t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }

        // only nested types can be declared "private"
        public static bool isPrivate(this Type t)
        {
            return
                !t.IsVisible
                && !t.IsPublic
                && !t.IsNotPublic
                && t.IsNested
                && !t.IsNestedPublic
                && !t.IsNestedFamily
                && t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }
        public static bool isStatic(this Type t)
        {
            return
                t.IsSealed
                && t.IsAbstract;
        }

        public static bool isSealed(this Type t)
        {
            return
                t.IsSealed
                && !t.IsAbstract;
        }

        public static bool isAbstract(this Type t)
        {
            return
                !t.IsSealed
                && t.IsAbstract
                && !t.IsInterface;
        }

        public static bool isClass(this Type t)
        {
            return
                !t.IsValueType
                && !t.IsEnum
                && t.IsClass
                && !t.IsInterface
                && !t.isAsyncMethod();
        }

        public static bool isInterface(this Type t)
        {
            return
                !t.IsValueType
                && !t.IsEnum
                && !t.IsClass
                && t.IsInterface
                && !t.isAsyncMethod();
        }

        public static bool isEnum(this Type t)
        {
            return
                t.IsValueType
                && t.IsEnum
                && !t.IsClass
                && !t.IsInterface
                && !t.isAsyncMethod();
        }

        public static bool isStruct(this Type t)
        {
            return
                t.IsValueType
                && !t.IsEnum
                && !t.IsClass
                && !t.IsInterface
                && !t.isAsyncMethod();
        }

        public static bool isAsyncMethod(this Type t)
        {
            return
                t.GetInterfaces().Contains(typeof(IAsyncStateMachine));
        }
    }
}
