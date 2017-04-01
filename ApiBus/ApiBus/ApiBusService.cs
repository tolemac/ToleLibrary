using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ToleLibraries.ApiBus.Services;
using ToleLibraries.ApiBus.Services.Interceptors;
using ToleLibraries.ApplicationContext;
using ToleLibraries.DependencyResolver;

namespace ToleLibraries.ApiBus
{
    public class ApiBusService
    {
        private IDependencyResolver DependencyResolver { get; }
        private readonly ServiceContainer _serviceContainer;
        
        public ApiBusService(IDependencyResolver dependencyResolver)
        {
            DependencyResolver = dependencyResolver;
            _serviceContainer = new ServiceContainer();
        }

        public ApiBusService AddService(ServiceDescriptor serviceDescriptor, Action<ServiceDescriptor> configureService = null)
        {
            configureService?.Invoke(serviceDescriptor);
            _serviceContainer.AddService(serviceDescriptor);
            return this;
        }

        public async Task<CallContext> CallServiceMethod(string serviceName, string methodName, object arguments)
        {
            return await CallServiceMethod(serviceName, methodName, JObject.FromObject(arguments));
        }

        public async Task<CallContext> CallServiceMethod(string serviceName, string methodName, JObject arguments)
        {
            var serviceDescriptor = _serviceContainer.GetServiceDescriptorByName(serviceName);

            var appContext = DependencyResolver.Resolve<IApplicationContext>();
            var serviceMethodCaller = DependencyResolver.Resolve<ServiceMethodCaller>();
            var serviceObject = DependencyResolver.Resolve(serviceDescriptor.Type);

            var callContext = new CallContext(appContext, serviceDescriptor, serviceObject, methodName, arguments);

            await serviceMethodCaller.Call(callContext);

            return callContext;
        }

        public ApiBusService AddInterceptor(string serviceName, string methodName, 
            Moment when, Action<CallContext> action)
        {
            _serviceContainer.GetServiceDescriptorByName(serviceName).AddInterceptor(methodName, when, action);
            return this;
        }

        public ApiBusService AddInterceptorObject(string serviceName, IServiceMethodCallInterceptor interceptor)
        {
            _serviceContainer.GetServiceDescriptorByName(serviceName).AddInterceptorObject(interceptor);
            return this;
        }
    }
}
