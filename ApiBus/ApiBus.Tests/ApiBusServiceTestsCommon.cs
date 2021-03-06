﻿using System;
using System.Collections.Concurrent;
using ToleLibraries.ApiBus.Services;
using ToleLibraries.ApplicationContext;
using ToleLibraries.DependencyResolver;
using ToleLibraries.TestCommons;

namespace ToleLibraries.ApiBus.Tests
{
    public class ApiBusDependencyResolverServiceMock : IDependencyResolver
    {
        public readonly ConcurrentDictionary<Type, object> Objects = new ConcurrentDictionary<Type, object>();

        #region Implementation of IDependencyResolver

        public object Resolve(Type type)
        {
            if (Objects.ContainsKey(type))
            {
                return Objects[type];
            }

            if (type == typeof(IApplicationContext))
                return new AuthenticatedApplicationContext();
            if (type == typeof(GuestApplicationContext))
                return new GuestApplicationContext();
            if (type == typeof(MethodCaller))
                return new MethodCaller();
            if (type == typeof(IMethodCaller<CallMethodResult>))
                return new MethodCaller();
            if (type == typeof(ServiceMethodCaller))
                return new ServiceMethodCaller(Resolve(typeof(IMethodCaller<CallMethodResult>)) as IMethodCaller<CallMethodResult>);

            if (type == typeof(ITestService) || type == typeof(TestService))
                return new TestService();

            return null;
        }

        public TType Resolve<TType>()
        {
            return (TType)Resolve(typeof(TType));
        }

        #endregion
    }
}
