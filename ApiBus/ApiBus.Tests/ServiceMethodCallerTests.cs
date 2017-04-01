using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ToleLibraries.ApiBus.Exceptions;
using ToleLibraries.ApiBus.Services;
using ToleLibraries.TestCommons;
using Xunit;

namespace ToleLibraries.ApiBus.Tests
{
    public class ServiceMethodCallerTests
    {
        [Fact]
        public async Task CallVoidMethodWithoutParams()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "VoidMethodWithoutParams";
            var parameters = JObject.FromObject(new { });

            var cc = new CallContext(new AuthenticatedApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await smc.Call(cc);

            Assert.True(mc.CallList.Last().Type == typeof(ITestService));
            Assert.True(mc.CallList.Last().Srv == srv);
            Assert.True(mc.CallList.Last().Parameters == parameters);
            Assert.True(mc.CallList.Last().MethodName == methodName);
        }

        [Fact]
        public async Task CallVoidMethodWithParams()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "VoidMethodWithParams";
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject(),
                param3 = (string)null
            });
            var cc = new CallContext(new AuthenticatedApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await smc.Call(cc);

            Assert.True(mc.CallList.Last().Type == typeof(ITestService));
            Assert.True(mc.CallList.Last().Srv == srv);
            Assert.True(mc.CallList.Last().Parameters == parameters);
            Assert.True(mc.CallList.Last().MethodName == methodName);
        }

        [Fact]
        public async Task CallVoidMethodWithDefaultParams()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "VoidMethodWithParams";
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject()
            });
            var cc = new CallContext(new AuthenticatedApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await smc.Call(cc);

            Assert.True(mc.CallList.Last().Type == typeof(ITestService));
            Assert.True(mc.CallList.Last().Srv == srv);
            Assert.True(mc.CallList.Last().Parameters == parameters);
            Assert.True(mc.CallList.Last().MethodName == methodName);
        }

        [Fact]
        public async Task CallMethodCrashIfMethodNotExistsOrNoParameterMatchs()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "VoidMethodWithParams";
            var parameters = JObject.FromObject(new
            {
                param1 = ""
            });
            var cc = new CallContext(new GuestApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await Assert.ThrowsAsync<AccessDeniedException>(async () =>
            {
                await smc.Call(cc);
            });
        }

        [Fact]
        public async Task CallMethodWithoutParams()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "MethodWithoutParams";
            var parameters = JObject.FromObject(new { });
            var cc = new CallContext(new AuthenticatedApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await smc.Call(cc);

            Assert.True(mc.CallList.Last().Type == typeof(ITestService));
            Assert.True(mc.CallList.Last().Srv == srv);
            Assert.True(mc.CallList.Last().Parameters == parameters);
            Assert.True(mc.CallList.Last().MethodName == methodName);
        }

        [Fact]
        public async Task CallMethodWithParams()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "MethodWithParams";
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var cc = new CallContext(new AuthenticatedApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await smc.Call(cc);

            Assert.True(mc.CallList.Last().Type == typeof(ITestService));
            Assert.True(mc.CallList.Last().Srv == srv);
            Assert.True(mc.CallList.Last().Parameters == parameters);
            Assert.True(mc.CallList.Last().MethodName == methodName);
        }

        [Fact]
        public async Task CallAsyncVoidMethodWithParams()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "AsyncVoidMethodWithParams";
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject(),
                param3 = (string)null
            });
            var cc = new CallContext(new AuthenticatedApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await smc.Call(cc);

            Assert.True(mc.CallList.Last().Type == typeof(ITestService));
            Assert.True(mc.CallList.Last().Srv == srv);
            Assert.True(mc.CallList.Last().Parameters == parameters);
            Assert.True(mc.CallList.Last().MethodName == methodName);
        }

        [Fact]
        public async Task CallAsyncMethodWithParams()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "AsyncMethodWithParams";
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var cc = new CallContext(new AuthenticatedApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await smc.Call(cc);

            Assert.True(mc.CallList.Last().Type == typeof(ITestService));
            Assert.True(mc.CallList.Last().Srv == srv);
            Assert.True(mc.CallList.Last().Parameters == parameters);
            Assert.True(mc.CallList.Last().MethodName == methodName);
        }

        [Fact]
        public async Task WhenCrashOnExecutionThrowsExecutionException()
        {
            var mc = new MethodCallerMock();
            var smc = new ServiceMethodCaller(mc);
            var srv = new TestService();
            var methodName = "MethodNotPublishOnInterface";
            var parameters = JObject.FromObject(new
            {
                param1 = 33
            });
            var cc = new CallContext(new AuthenticatedApplicationContext(), new ServiceDescriptor(typeof(ITestService)), srv, methodName, parameters);

            await Assert.ThrowsAsync<ExecutionException>(async () =>
            {
                await smc.Call(cc);
            });

            try
            {
                await smc.Call(cc);
            }
            catch (ExecutionException ex)
            {
                Assert.True(ex.InnerException.Message == "Test exception, param1 is 33.");
            }

        }
    }
}
