﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace X10D.Infrastructure
{
    /// <summary>
    /// Интерфейс сервиса создания объектов с использованием внедрения зависимостей.
    /// </summary>
    public interface IActivator
    {
        IList<Assembly> Assemblies { get; }
        IList<Type> GetTypes(Func<Type, bool> predicate = null);
        /// <summary>
        /// Получить наследника типа <paramref name="targetType"/>
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        Type GetInheritor(Type targetType, bool useCaching = true);

        /// <summary>
        /// Получить наследника типа <paramref name="targetType"/>, удовлетворяющую предикату.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        Type GetInheritor(Type targetType, Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить всех наследников типа <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<Type> GetInheritors(Type targetType, bool useCaching = true);

        /// <summary>
        /// Получить всех наследников типа <paramref name="targetType"/>, удовлетворяющие предикату.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<Type> GetInheritors(Type targetType, Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить наследника типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        Type GetInheritor<T>(bool useCaching = true);

        /// <summary>
        /// Получить наследника типа <typeparamref name="T"/>, удовлетворяющую предикату.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        Type GetInheritor<T>(Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить всех наследников типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<Type> GetInheritors<T>(bool useCaching = true);

        /// <summary>
        /// Получить всех наследников типа <typeparamref name="T"/>, удовлетворяющие предикату.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<Type> GetInheritors<T>(Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить реализацию типа <paramref name="targetType"/>
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        Type GetImplementation(Type targetType, bool useCaching = true);

        /// <summary>
        /// Получить реализацию типа <paramref name="targetType"/>, удовлетворяющую предикату.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        Type GetImplementation(Type targetType, Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить все реализации типа <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<Type> GetImplementations(Type targetType, bool useCaching = true);

        /// <summary>
        /// Получить все реализации типа <paramref name="targetType"/>, удовлетворяющие предикату.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<Type> GetImplementations(Type targetType, Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить экземпляр типа <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        object GetInstance(Type targetType, bool useCaching = true);

        /// <summary>
        /// Получить экземпляр типа <paramref name="targetType"/>, удовлетворяющую предикату.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        object GetInstance(Type targetType, Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить все экземпляры типа <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<object> GetInstances(Type targetType, bool useCaching = true);

        /// <summary>
        /// Получить все экземпляры типа <paramref name="targetType"/>, удовлетворяющие предикату.
        /// </summary>
        /// <param name="targetType">Целевой тип.</param>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<object> GetInstances(Type targetType, Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить реализацию типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        Type GetImplementation<T>(bool useCaching = true);

        /// <summary>
        /// Получить реализацию типа <typeparamref name="T"/>, удовлетворяющую предикату.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        Type GetImplementation<T>(Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить все реализации типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<Type> GetImplementations<T>(bool useCaching = true);

        /// <summary>
        /// Получить все реализации типа <typeparamref name="T"/>, удовлетворяющие предикату.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<Type> GetImplementations<T>(Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить экземпляр типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        T GetInstance<T>(bool useCaching = true);

        /// <summary>
        /// Получить экземпляр типа <typeparamref name="T"/>, удовлетворяющую предикату.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        T GetInstance<T>(Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить все экземпляры типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<T> GetInstances<T>(bool useCaching = true);

        /// <summary>
        /// Получить все экземпляры типа <typeparamref name="T"/>, удовлетворяющие предикату.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="predicate">Предикат-функция.</param>
        /// <param name="useCaching">Если <c>true</c> будет использован локальный кэш.</param>
        /// <returns></returns>
        IEnumerable<T> GetInstances<T>(Func<Type, bool> predicate, bool useCaching = true);

        /// <summary>
        /// Получить сервис или создать экземпляр типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <returns></returns>
        T GetServiceOrCreateInstance<T>();

        /// <summary>
        /// Получить сервис или создать экземпляр определенного типа.
        /// </summary>
        /// <param name="type">Целевой тип</param>
        /// <returns></returns>
        object GetServiceOrCreateInstance(Type type);

        /// <summary>
        /// Получить сервис типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <returns></returns>
        T GetService<T>();

        /// <summary>
        /// Получить сервис определенного типа.
        /// </summary>
        /// <param name="type">Целевой тип</param>
        /// <returns></returns>
        object GetService(Type type);

        /// <summary>
        /// Создать экземпляр типа <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <returns></returns>
        T CreateInstance<T>();

        /// <summary>
        /// Создать экземпляр определенного типа.
        /// </summary>
        /// <param name="type">Целевой тип</param>
        /// <returns></returns>
        object CreateInstance(Type type);
    }
}