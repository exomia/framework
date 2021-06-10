#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Exomia.Framework.Core.IOC.Attributes;

namespace Exomia.Framework.Core.IOC
{
    /// <summary> A service provider. </summary>
    public class ServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider?        _parent;
        private readonly Dictionary<Type, IEntry> _entries = new Dictionary<Type, IEntry>(16);

        /// <summary> Prevents a default instance of the <see cref="ServiceProvider"/> class from being created. </summary>
        private ServiceProvider(IServiceProvider? parent)
        {
            _parent = parent;
        }

        /// <inheritdoc />
        public TService Get<TService>()
        {
            if (TryGet(typeof(TService), out object service))
            {
                return (TService)service;
            }
            throw new KeyNotFoundException(nameof(TService));
        }

        /// <inheritdoc />
        public object Get(Type serviceType)
        {
            if (TryGet(serviceType, out object service))
            {
                return service;
            }
            throw new KeyNotFoundException(nameof(serviceType));
        }

        /// <inheritdoc />
        public bool TryGet<TService>(out TService service)
        {
            bool result = TryGet(typeof(TService), out object s);
            service = (TService)s;
            return result;
        }

        /// <inheritdoc />
        public bool TryGet(Type serviceType, out object service)
        {
            if (_entries.TryGetValue(serviceType, out IEntry? entry))
            {
                service = entry.ImplementationFactory(this);
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGet(serviceType, out service);
            }

            service = null!;
            return false;
        }

        /// <inheritdoc />
        public IServiceProvider CreateScope(IServiceCollection? serviceCollection = null)
        {
            return (serviceCollection ?? new ServiceCollection())
                .Build(this);
        }

        /// <summary> Creates a new <see cref="IServiceProvider"/>. </summary>
        /// <param name="parent">         The parent. </param>
        /// <param name="typeEntries">    The type entries. </param>
        /// <param name="factoryEntries"> The factory entries. </param>
        /// <returns> An <see cref="IServiceProvider"/>. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        internal static IServiceProvider Create(
            IServiceProvider?                                                parent,
            IDictionary<Type, (Type, ServiceKind)>                           typeEntries,
            IDictionary<Type, (Func<IServiceProvider, object>, ServiceKind)> factoryEntries)
        {
            ServiceProvider serviceProvider = new ServiceProvider(parent);

            foreach ((Type key, (Type implementation, ServiceKind serviceKind)) in typeEntries)
            {
                switch (serviceKind)
                {
                    case ServiceKind.Transient:
                        serviceProvider._entries.Add(key, new Entry(CreateImplementationFactory(implementation)));
                        break;
                    case ServiceKind.Singleton:
                        serviceProvider._entries.Add(key, new SingletonEntry(CreateImplementationFactory(implementation)));
                        break;
                    default: throw new IndexOutOfRangeException(nameof(serviceKind));
                }
            }

            foreach ((Type key, (Func<IServiceProvider, object> implementationFactory, ServiceKind serviceKind)) in factoryEntries)
            {
                switch (serviceKind)
                {
                    case ServiceKind.Transient:
                        serviceProvider._entries.Add(key, new Entry(implementationFactory));
                        break;
                    case ServiceKind.Singleton:
                        serviceProvider._entries.Add(key, new SingletonEntry(implementationFactory));
                        break;
                    default: throw new IndexOutOfRangeException(nameof(serviceKind));
                }
            }

            // INFO:
            // The service provider itself is registered here as a singleton, in order to retrieve itself as a injected service if required.
            // 
            // Note: 
            // Entry is used here as it was a singleton, because it will always return the same instance and doesn't need to be wrapped as singleton!
            serviceProvider._entries[typeof(IServiceProvider)] = new Entry(p => serviceProvider);
            return serviceProvider;
        }

        private static Func<IServiceProvider, object> CreateImplementationFactory(Type implementation)
        {
            ConstructorInfo[] constructorInfos = implementation.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructorInfos.Length <= 0)
            {
                throw new NotSupportedException($"The implementation type '{implementation}' must have at least a public default constructor!");
            }

            ConstructorInfo ctor = constructorInfos.FirstOrDefault(c => c.GetCustomAttribute<IoCConstructorAttribute>() != null)
                                   ?? constructorInfos.OrderByDescending(c => c.GetParameters().Length).First();

            ParameterExpression serviceProviderParameter = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            MethodInfo serviceProviderGetMethodInfo =
                typeof(IServiceProvider).GetMethod(
                    nameof(IServiceProvider.Get),
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    new[] {typeof(Type)},
                    null)
                ?? throw new NullReferenceException();

            Expression[] parameters = Array.ConvertAll(
                ctor.GetParameters(),
                p =>
                {
                    IoCParameterFactoryAttribute? parameterFactoryAttribute = p.GetCustomAttribute<IoCParameterFactoryAttribute>();
                    if (parameterFactoryAttribute != null)
                    {
                        MethodInfo parameterFactoryMethodInfo =
                            parameterFactoryAttribute.FactoryType.GetMethod(
                                parameterFactoryAttribute.MethodName,
                                BindingFlags.Instance | BindingFlags.Public)
                            ?? throw new NullReferenceException();

                        return (Expression)Expression.Convert(
                            Expression.Call(
                                Expression.Call(
                                    serviceProviderParameter,
                                    serviceProviderGetMethodInfo,
                                    Expression.Constant(parameterFactoryAttribute.FactoryType)),
                                parameterFactoryMethodInfo),
                            p.ParameterType);
                    }

                    IoCUseDefaultAttribute? useDefaultAttribute = p.GetCustomAttribute<IoCUseDefaultAttribute>();
                    if (useDefaultAttribute != null)
                    {
                        if (useDefaultAttribute.DefaultValue != null)
                        {
                            return (Expression)Expression.Convert(
                                Expression.Constant(useDefaultAttribute.DefaultValue),
                                p.ParameterType);
                        }

                        return (Expression)Expression.Convert(
                            p.HasDefaultValue 
                                ? Expression.Constant(p.DefaultValue) 
                                : Expression.Default(p.ParameterType),
                            p.ParameterType);

                    }

                    return (Expression)Expression.Convert(
                            Expression.Call(serviceProviderParameter, serviceProviderGetMethodInfo, Expression.Constant(p.ParameterType)),
                            p.ParameterType);
                });

            return (Func<IServiceProvider, object>)Expression.Lambda(
                    typeof(Func<IServiceProvider, object>),
                    Expression.New(ctor, parameters),
                    serviceProviderParameter)
                .Compile();
        }

        private interface IEntry
        {
            Func<IServiceProvider, object> ImplementationFactory { get; }
        }

        private sealed class Entry : IEntry
        {
            public Func<IServiceProvider, object> ImplementationFactory { get; }

            /// <summary> Initializes a new instance of the <see cref="Entry" /> class. </summary>
            /// <param name="implementationFactory"> The implementation factory. </param>
            /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
            public Entry(Func<IServiceProvider, object> implementationFactory)
            {
                ImplementationFactory = implementationFactory ?? throw new ArgumentNullException(nameof(implementationFactory));
            }
        }

        private sealed class SingletonEntry : IEntry
        {
            private object?                        _instance = null;
            public  Func<IServiceProvider, object> ImplementationFactory { get; }

            /// <summary> Initializes a new instance of the <see cref="SingletonEntry" /> class. </summary>
            /// <param name="implementationFactory"> The implementation factory. </param>
            /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
            public SingletonEntry(Func<IServiceProvider, object> implementationFactory)
            {
                if (implementationFactory == null) { throw new ArgumentNullException(nameof(implementationFactory)); }
                ImplementationFactory = p => _instance ??= implementationFactory(p);
            }
        }
    }
}