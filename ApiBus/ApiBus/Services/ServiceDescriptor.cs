using System;
using System.Collections.Generic;
using System.Linq;
using ToleLibraries.ApiBus.Services.Interceptors;

namespace ToleLibraries.ApiBus.Services
{
    public class ServiceDescriptor
    {
        private readonly Dictionary<string, bool> _methodAuth =
            new Dictionary<string, bool>();
        private readonly List<InterceptorDescriptor> _interceptorDescriptors =
            new List<InterceptorDescriptor>();
        private readonly List<IServiceMethodCallInterceptor> _interceptorObjects =
            new List<IServiceMethodCallInterceptor>();

        public string Name { get; }
        public Type Type { get; }

        public bool RequireAuth { get; set; } = true;

        public ServiceDescriptor(Type type): this(type.Name, type) { }

        public ServiceDescriptor(string name, Type type)
        {
            Name = NormaliceServiceName(name);
            Type = type;
        }

        private static string NormaliceServiceName(string name)
        {
            if (name.StartsWith("I") && name.Length > 2 && char.IsUpper(name[1]))
                return name.Substring(1);
            return name;
        }

        public void AddInterceptor(string methodName, Moment when, Action<CallContext> action)
        {
            _interceptorDescriptors.Add(new InterceptorDescriptor(methodName, when, action));
        }

        public void AddInterceptorObject(IServiceMethodCallInterceptor interceptor)
        {
            _interceptorObjects.Add(interceptor);
        }

        public IEnumerable<IServiceMethodCallInterceptor> GetInterceptorObjects()
        {
            return _interceptorObjects.ToArray();
        }

        public IEnumerable<InterceptorDescriptor> GetBeforeInterceptors(string methodName)
        {
            return _interceptorDescriptors.Where(i => i.MethodName == methodName && i.Moment == Moment.Before);
        }

        public IEnumerable<InterceptorDescriptor> GetAfterInterceptors(string methodName)
        {
            return _interceptorDescriptors.Where(i => i.MethodName == methodName && i.Moment == Moment.After);
        }

        public IEnumerable<InterceptorDescriptor> GetOnExceptionInterceptors(string methodName)
        {
            return _interceptorDescriptors.Where(i => i.MethodName == methodName && i.Moment == Moment.OnException);
        }

        public ServiceDescriptor SetMethodAuth(string methodName, bool requireAuth)
        {
            _methodAuth[methodName] = requireAuth;
            return this;
        }

        public bool MethodRequireAuth(string methodName)
        {
            if (!RequireAuth)
                return _methodAuth.ContainsKey(methodName) && _methodAuth[methodName];

            return !_methodAuth.ContainsKey(methodName) || _methodAuth[methodName];
        }
    }
}

