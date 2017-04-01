using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ToleLibraries.ApiBus.Exceptions;
using ToleLibraries.ApiBus.Services;
using Xunit;

namespace ToleLibraries.ApiBus.Tests
{
    public class ApiBusTests
    {
        private readonly ApiBusService _apiBusService;

        public ApiBusTests()
        {
            var dr = new ApiBusDependencyResolverServiceMock();
            _apiBusService = new ApiBusService(dr);

            _apiBusService.AddService(new ServiceDescriptor(typeof(ITestService)), sd => { });
        }

        [Fact]
        public async Task CallVoidMethodWithoutParams()
        {
            var parameters = JObject.FromObject(new { });
            var result = await _apiBusService.CallServiceMethod("TestService", "VoidMethodWithoutParams", parameters);
            Assert.True(result.IsVoidMethod);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CallVoidMethodWithParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject(),
                param3 = (string)null
            });
            var result = await _apiBusService.CallServiceMethod("TestService", "VoidMethodWithParams", parameters);
            Assert.True(result.IsVoidMethod);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CallVoidMethodWithDefaultParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject()
            });
            var result = await _apiBusService.CallServiceMethod("TestService", "VoidMethodWithParams", parameters);
            Assert.True(result.IsVoidMethod);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CallMethodCrashIfMethodNotExistsOrNoParameterMatchs()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = ""
            });
            await Assert.ThrowsAsync<MethodLocatorExcepcion>(async () => //ParameterCreationExcepcion
            {
                await _apiBusService.CallServiceMethod("TestService", "VoidMethodWithParams", parameters);
            });
            await Assert.ThrowsAsync<MethodLocatorExcepcion>(async () =>
            {
                await _apiBusService.CallServiceMethod("TestService", "NOTEXISTS-METHOD", parameters);
            });
        }

        [Fact]
        public async Task CallMethodCrashIfCantBuildMethodParameters()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = "Type Mistmatch"
            });
            await Assert.ThrowsAsync<ParameterCreationExcepcion>(async () =>
            {
                await _apiBusService.CallServiceMethod("TestService", "VoidMethodWithParams", parameters);
            });
        }

        [Fact]
        public async Task CallMethodWithoutParams()
        {
            var parameters = JObject.FromObject(new { });
            var result = await _apiBusService.CallServiceMethod("TestService", "MethodWithoutParams", parameters);
            Assert.False(result.IsVoidMethod);
            Assert.Equal(TestService.NoParamsResult, result.Result);
        }

        [Fact]
        public async Task CallMethodWithParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var result = await _apiBusService.CallServiceMethod("TestService", "MethodWithParams", parameters);
            Assert.False(result.IsVoidMethod);
            Assert.Equal(JsonConvert.SerializeObject(TestService.NoParamsResult),
                JsonConvert.SerializeObject(result.Result));
        }

        [Fact]
        public async Task CallAsyncVoidMethodWithParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = new TestObject(),
                param3 = (string)null
            });
            var result = await _apiBusService.CallServiceMethod("TestService", "AsyncVoidMethodWithParams", parameters);
            Assert.True(result.IsVoidMethod);
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CallAsyncMethodWithParams()
        {
            var parameters = JObject.FromObject(new
            {
                param1 = "",
                param2 = TestService.NoParamsResult
            });
            var result = await _apiBusService.CallServiceMethod("TestService", "AsyncMethodWithParams", parameters);
            Assert.False(result.IsVoidMethod);
            Assert.Equal(JsonConvert.SerializeObject(TestService.NoParamsResult),
                JsonConvert.SerializeObject(result.Result));
        }

        [Fact]
        public async Task CallMethodNotPublishOnInterfaceCrash()
        {
            var parameters = JObject.FromObject(new { });

            await Assert.ThrowsAsync<MethodLocatorExcepcion>(async () =>
            {
                await _apiBusService.CallServiceMethod("TestService", "MethodNotPublishOnInterface", parameters);
            });

        }

    }
}
