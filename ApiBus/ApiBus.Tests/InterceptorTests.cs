using System.Collections.Concurrent;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToleLibraries.ApiBus.Exceptions;
using ToleLibraries.ApiBus.Services;
using ToleLibraries.ApiBus.Services.Interceptors;
using Xunit;

namespace ToleLibraries.ApiBus.Tests
{
    public class InterceptorTests
    {
        private ApiBusService CreateApiBus()
        {
            var dr = new ApiBusDependencyResolverServiceMock();
            var abs = new ApiBusService(dr);
            abs.AddService(new ServiceDescriptor(typeof(ITestService)), sd => { });
            return abs;
        }

        private string _serviceName = "TestService";

        [Fact]
        public async void MomentBeforeAndAfterWithActionInterceptors()
        {
            var abs = CreateApiBus();
            var parameters = JObject.FromObject(new { });
            var callList = new ConcurrentBag<CallData>();
            var methodName = "VoidMethodWithoutParams";
            int counter1 = 0;

            abs.AddInterceptor(_serviceName, methodName, Moment.Before, (context) =>
            {
                callList.Add(new CallData(Moment.Before, counter1++));
            });
            abs.AddInterceptor(_serviceName, methodName, Moment.After, (context) =>
            {
                callList.Add(new CallData(Moment.After, counter1++));
            });

            await abs.CallServiceMethod(_serviceName, methodName, parameters);
            var arr = callList.OrderBy(i => i.Order).ToArray();

            Assert.True(arr.Any());
            Assert.True(arr[0].Moment == Moment.Before);
            Assert.True(arr[1].Moment == Moment.After);
            Assert.True(arr[0].When < arr[1].When);
        }

        [Fact]
        public async void MomentBeforeAndAfterWithObjectInterceptors()
        {
            var abs = CreateApiBus();
            var parameters = JObject.FromObject(new { });
            var methodName = "VoidMethodWithoutParams";
            var inter = new TestServiceInterceptor();
            var interbn = new TestServiceInterceptorByMethodName();


            abs.AddInterceptorObject(_serviceName, inter);
            abs.AddInterceptorObject(_serviceName, interbn);

            await abs.CallServiceMethod(_serviceName, methodName, parameters);
            var arr1 = inter.CallList.OrderBy(i => i.Order).ToArray();
            var arr2 = interbn.CallList.OrderBy(i => i.Order).ToArray();

            Assert.True(arr1.Length == 2);
            Assert.True(arr2.Length == 1);
            Assert.True(arr1[0].Moment == Moment.Before);
            Assert.True(arr1[1].Moment == Moment.After);
            Assert.True(arr2[0].Moment == Moment.After);
            Assert.True(arr1[0].When.Ticks < arr1[1].When.Ticks);
            Assert.True(arr2[0].When.Ticks != arr1[0].When.Ticks);
            Assert.True(arr2[0].When.Ticks != arr1[1].When.Ticks);
        }

        [Fact]
        public async void IfBeforeInterceptorSetResult_ExecutionStops()
        {
            var abs = CreateApiBus();
            var parameters = JObject.FromObject(new { });
            var callList = new ConcurrentBag<CallData>();
            var methodName = "VoidMethodWithoutParams";
            var resultValue = 5;

            abs.AddInterceptor(_serviceName, methodName, Moment.Before, (context) =>
            {
                callList.Add(new CallData(Moment.Before, 0));
                context.Result = resultValue;
            });
            abs.AddInterceptor(_serviceName, methodName, Moment.After, (context) =>
            {
                callList.Add(new CallData(Moment.After, 0));
                context.Result = resultValue;
            });

            var result = await abs.CallServiceMethod(_serviceName, methodName, parameters);

            Assert.True(result.Result == resultValue);
            Assert.True(callList.Count == 1);
        }

        [Fact]
        public async void IfAfterInterceptorSetResult_ReturnValueIsFromInterceptor()
        {
            var abs = CreateApiBus();
            var methodName = "AsyncMethodWithParams";
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var result = await abs.CallServiceMethod(_serviceName, methodName, parameters);

            Assert.False(result.IsVoidMethod);
            Assert.Equal(JsonConvert.SerializeObject(TestService.NoParamsResult),
                JsonConvert.SerializeObject(result.Result));

            var resultValue = 5;

            abs.AddInterceptor(_serviceName, methodName, Moment.After, (context) =>
            {
                context.Result = resultValue;
            });

            result = await abs.CallServiceMethod(_serviceName, methodName, parameters);

            Assert.True(result.Result == resultValue);
        }

        [Fact]
        public async void OnExceptionInterceptorIsCalled()
        {
            var dr = new ApiBusDependencyResolverServiceMock();
            dr.Objects.TryAdd(typeof(IMethodCaller<CallMethodResult>), new MethodCallerMock());
            var abs = new ApiBusService(dr);
            abs.AddService(new ServiceDescriptor(typeof(TestService)), sd => { });
            var methodName = "MethodNotPublishOnInterface";
            var parameters = JObject.FromObject(new
            {
                param1 = 33
            });
            var called = false;

            abs.AddInterceptor(_serviceName, methodName, Moment.OnException, (context) =>
            {
                called = true;
                context.Result = "Error";
            });

            CallContext result;
            try
            {
                result = await abs.CallServiceMethod(_serviceName, methodName, parameters);
            }
            catch (ExecutionException ex)
            {
                result = ex.Context;
            }

            Assert.True(result != null);
            Assert.True(called);
            Assert.True(result != null && result.Exception != null);
            if (result != null) Assert.True((bool) (result.Result == "Error"));
        }
    }
}
