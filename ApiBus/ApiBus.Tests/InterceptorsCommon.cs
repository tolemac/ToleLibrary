using ToleLibraries.ApiBus.Services;
using ToleLibraries.ApiBus.Services.Interceptors;

namespace ToleLibraries.ApiBus.Tests
{
    public class TestServiceInterceptor : IServiceMethodCallInterceptor
    {
        public void Intercept(Moment moment, CallContext context)
        {
            if (context.MethodName == "VoidMethodWithoutParams" && moment == Moment.Before)
            {
                context.Result = null;
            }
        }
    }

    public class TestServiceInterceptorByMethodName : InterceptorByMethodName
    {
        public void AfterVoidMethodWithoutParams(Moment moment, CallContext context)
        {
            context.Result = "";
        }
    }
}
