namespace ToleLibraries.ApiBus.Services.Interceptors
{
    public interface IServiceMethodCallInterceptor
    {
        void Intercept(Moment moment, CallContext context);
    }
}
