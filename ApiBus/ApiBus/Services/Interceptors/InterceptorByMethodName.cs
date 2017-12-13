using System.Reflection;

namespace ToleLibraries.ApiBus.Services.Interceptors
{
    public class InterceptorByMethodName : IServiceMethodCallInterceptor
    {
        public void Intercept(Moment moment, CallContext context)
        {
#if NET45 || NETCOREAPP2_0 || NETCOREAPP1_1
            var method = GetType().GetMethod(moment + context.MethodName);
#endif
#if NETSTANDARD1_6
            var method = GetType().GetTypeInfo().GetMethod(moment + context.MethodName);
#endif

            method?.Invoke(this, new object[] {moment, context});
        }
    }
}
