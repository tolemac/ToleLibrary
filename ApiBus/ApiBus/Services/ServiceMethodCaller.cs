using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToleLibraries.ApiBus.Exceptions;
using ToleLibraries.ApiBus.Services.Interceptors;

namespace ToleLibraries.ApiBus.Services
{
    internal class ServiceMethodCaller
    {
        private readonly IMethodCaller<CallMethodResult> _methodCaller;

        public ServiceMethodCaller(IMethodCaller<CallMethodResult> methodCaller)
        {
            _methodCaller = methodCaller;
        }

        public async Task<dynamic> Call(CallContext callContext)
        {

            if (callContext.Service.MethodRequireAuth(callContext.MethodName)
                && !callContext.AppContext.IsAuthenticated)
            {
                throw new AccessDeniedException($"Access denied calling '{callContext.MethodName}' method of '{callContext.Service.Name}' service.");
            }

            var interceptors = callContext.Service.GetInterceptorObjects().ToArray();

            if (CallInterceptorObjects(interceptors, callContext, Moment.Before))
                return callContext.Result;
            if (DistpatchBeforeInterceptors(callContext))
                return callContext.Result;

            CallMethodResult result;
            try
            {
                result = await _methodCaller.Call(callContext.Service.Type, callContext.ServiceObject,
                    callContext.MethodName, callContext.Arguments);
            }
            catch (MethodLocatorExcepcion ex)
            {
                callContext.Exception = ex;
                throw;
            }
            catch (ParameterCreationExcepcion ex)
            {
                callContext.Exception = ex;
                throw;
            }
            catch (Exception ex)
            {
                callContext.Exception = ex;
                CallInterceptorObjects(interceptors, callContext, Moment.OnException);
                DistpatchOnExceptionInterceptors(callContext.Service, callContext);
                throw new ExecutionException(callContext.Service.Name, callContext.Service.Type, ex);
            }

            callContext.Result = result.Result;
            callContext.IsVoidMethod = result.IsVoid;


            if (!CallInterceptorObjects(interceptors, callContext, Moment.After))
                DistpatchAfterInterceptors(callContext);

            return callContext.Result;
        }

        private static bool CallInterceptorObjects(IEnumerable<IServiceMethodCallInterceptor> interceptorObjects, CallContext callContext, Moment moment)
        {
            var preResult = callContext.Result;
            foreach (var interceptor in interceptorObjects)
            {
                interceptor.Intercept(moment, callContext);
                if (callContext.Result != preResult)
                    return true;
            }
            return false;
        }

        private static void DistpatchOnExceptionInterceptors(ServiceDescriptor serviceDescriptor, CallContext callContext)
        {
            var interceptors = serviceDescriptor.GetOnExceptionInterceptors(callContext.MethodName);
            foreach (var interceptor in interceptors)
            {
                interceptor.Action(callContext);
            }
        }

        private static bool DistpatchBeforeInterceptors(CallContext callContext)
        {
            var interceptors = callContext.Service.GetBeforeInterceptors(callContext.MethodName);
            foreach (var interceptor in interceptors)
            {
                interceptor.Action(callContext);
                if (callContext.Result != null)
                    return true;
            }
            return false;
        }

        private static void DistpatchAfterInterceptors(CallContext callContext)
        {
            var interceptors = callContext.Service.GetAfterInterceptors(callContext.MethodName);
            var preResult = callContext.Result;
            foreach (var interceptor in interceptors)
            {
                interceptor.Action(callContext);
                if (callContext.Result != preResult)
                    return;
            }
        }

    }
}