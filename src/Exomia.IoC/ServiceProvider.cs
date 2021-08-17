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
using Exomia.IoC.Attributes;

namespace Exomia.IoC
{
    /// <summary> A service provider. This class cannot be inherited. </summary>
    public sealed class ServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider?                                      _parent;
        private readonly Dictionary<Type, IEntry>                               _entries   = new(16);
        private readonly Dictionary<Type, Func<IServiceProvider, Type, object>> _factories = new(16);

        /// <summary> Initializes a new instance of the <see cref="ServiceProvider"/> class. </summary>
        /// <param name="parent"> The parent. </param>
        private ServiceProvider(IServiceProvider? parent)
        {
            _parent = parent;
        }

        /// <inheritdoc/>
        public TService Get<TService>()
        {
            if (TryGet(typeof(TService), out object service))
            {
                return (TService)service;
            }
            throw new KeyNotFoundException(nameof(TService));
        }

        /// <inheritdoc/>
        public object Get(Type serviceType)
        {
            if (TryGet(serviceType, out object service))
            {
                return service;
            }
            throw new KeyNotFoundException(serviceType.ToString());
        }

        /// <inheritdoc/>
        public bool TryGet<TService>(out TService service)
        {
            bool result = TryGet(typeof(TService), out object s);
            service = (TService)s;
            return result;
        }

        /// <inheritdoc/>
        public bool TryGet(Type serviceType, out object service)
        {
            if (_entries.TryGetValue(serviceType, out IEntry? entry) || 
                serviceType.IsGenericType && _entries.TryGetValue(serviceType.GetGenericTypeDefinition(), out entry))
            {
                service = entry.ImplementationFactory(this);
                return true;
            }

            if (_factories.TryGetValue(serviceType, out Func<IServiceProvider, Type, object>? implementationFactory) ||
                serviceType.IsGenericType && _factories.TryGetValue(serviceType.GetGenericTypeDefinition(), out implementationFactory))
            {
                service = implementationFactory(this, serviceType);
                return true;
            }

            if (_parent != null)
            {
                return _parent.TryGet(serviceType, out service);
            }

            service = null!;
            return false;
        }

        /// <inheritdoc/>
        public IServiceProvider CreateScope(IServiceCollection? serviceCollection = null)
        {
            return (serviceCollection ?? new ServiceCollection())
                .Build(this);
        }

        internal static IServiceProvider Create(
            IServiceProvider?                                                parent,
            IDictionary<Type, (Type, ServiceKind)>                           typeEntries,
            IDictionary<Type, (Func<IServiceProvider, object>, ServiceKind)> factoryEntries,
            IDictionary<Type, Func<IServiceProvider, Type, object>>          factories)
        {
            ServiceProvider serviceProvider = new(parent);

            foreach ((Type service, (Type implementation, ServiceKind serviceKind)) in typeEntries)
            {
                switch (serviceKind)
                {
                    case ServiceKind.Transient:
                        serviceProvider._entries.Add(service, new Entry(CreateImplementationFactory(implementation)));
                        break;
                    case ServiceKind.Singleton:
                        serviceProvider._entries.Add(service, new SingletonEntry(CreateImplementationFactory(implementation)));
                        break;
                    default: throw new IndexOutOfRangeException(nameof(serviceKind));
                }
            }

            foreach ((Type service, (Func<IServiceProvider, object> implementationFactory, ServiceKind serviceKind)) in factoryEntries)
            {
                switch (serviceKind)
                {
                    case ServiceKind.Transient:
                        serviceProvider._entries.Add(service, new Entry(implementationFactory));
                        break;
                    case ServiceKind.Singleton:
                        serviceProvider._entries.Add(service, new SingletonEntry(implementationFactory));
                        break;
                    default: throw new IndexOutOfRangeException(nameof(serviceKind));
                }
            }

            foreach ((Type service, Func<IServiceProvider, Type, object> implementationFactory) in factories)
            {
                serviceProvider._factories.Add(service, implementationFactory);
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

            MethodInfo serviceProviderTryGetMethodInfo =
                typeof(IServiceProvider).GetMethod(
                    nameof(IServiceProvider.TryGet),
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    new[] {typeof(Type), typeof(object).MakeByRefType()},
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

                        return Expression.Convert(
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
                            return Expression.Convert(
                                Expression.Constant(useDefaultAttribute.DefaultValue),
                                p.ParameterType);
                        }

                        return p.HasDefaultValue
                            ? Expression.Constant(p.DefaultValue)
                            : Expression.Convert(Expression.Default(p.ParameterType), p.ParameterType);
                    }

                    IoCOptionalAttribute? optionalAttribute = p.GetCustomAttribute<IoCOptionalAttribute>();
                    if (optionalAttribute != null)
                    {
                        ParameterExpression outValueParameterExpression = Expression.Variable(typeof(object), "outValue");
                        return Expression.Block(new[]{ outValueParameterExpression },
                            Expression.Condition(
                            Expression.Call(serviceProviderParameter, serviceProviderTryGetMethodInfo, Expression.Constant(p.ParameterType), outValueParameterExpression),
                            Expression.Convert(outValueParameterExpression,                                                          p.ParameterType),
                            Expression.Convert(Expression.Constant(System.Activator.CreateInstance(optionalAttribute.OptionalType)), p.ParameterType)));
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
            /// <inheritdoc/>
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
            private object? _instance = null;

            /// <inheritdoc/>
            public Func<IServiceProvider, object> ImplementationFactory { get; }

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