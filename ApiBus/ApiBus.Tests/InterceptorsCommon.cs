using System;
using System.Collections.Concurrent;
using ToleLibraries.ApiBus.Services;
using ToleLibraries.ApiBus.Services.Interceptors;

namespace ToleLibraries.ApiBus.Tests
{
    public class CallData
    {
        public Moment Moment { get; }
        public DateTime When { get; set; } = DateTime.Now;
        public int Order { get; set; }

        public CallData(Moment moment, int order)
        {
            Moment = moment;
            Order = order;
        }
    }

    public class TestServiceInterceptor : IServiceMethodCallInterceptor
    {
        public ConcurrentBag<CallData> CallList = new ConcurrentBag<CallData>();
        private int _count;

        public void Intercept(Moment moment, CallContext context)
        {
            CallList.Add(new CallData(moment, _count++) {When = DateTime.Now});
            if (context.MethodName == "VoidMethodWithoutParams" && moment == Moment.Before)
            {
                context.Result = null;
            }
        }
    }

    public class TestServiceInterceptorByMethodName : InterceptorByMethodName
    {
        public ConcurrentBag<CallData> CallList = new ConcurrentBag<CallData>();
        private int _count;

        public void AfterVoidMethodWithoutParams(Moment moment, CallContext context)
        {
            CallList.Add(new CallData(moment, _count++) { When = DateTime.Now });
            context.Result = "";
        }
    }
}
