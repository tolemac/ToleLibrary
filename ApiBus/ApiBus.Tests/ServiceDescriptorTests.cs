using System.Linq;
using System.Reflection;
using ToleLibraries.ApiBus.Services;
using ToleLibraries.ApiBus.Services.Interceptors;
using Xunit;

namespace ToleLibraries.ApiBus.Tests
{
    public class ServiceDescriptorTests
    {
        [Fact]
        public void CreateDescriptorByTypeAndName()
        {
            var desc1 = new ServiceDescriptor("TestService", typeof(TestService));
            var desc2 = new ServiceDescriptor(typeof(TestService));
            var desc3 = new ServiceDescriptor(typeof(ITestService));

            Assert.True(desc1.Name == desc2.Name && desc1.Name == desc3.Name);
        }

        [Fact]
        public void ServiceRequireAuthByDefault()
        {
            var desc = new ServiceDescriptor(typeof(ITestService));

            Assert.True(desc.RequireAuth);
        }

        [Fact]
        public void ServiceMethodsRequireAuthWhenServiceRequireAuthByDefault()
        {
            var desc = new ServiceDescriptor(typeof(ITestService));
            var methods = typeof(ITestService).GetTypeInfo().GetMethods().Select(m => m.Name).ToList();

            Assert.True(methods.Any());

            foreach (var method in methods)
            {
                Assert.True(desc.MethodRequireAuth(method));
            }

            Assert.True(desc.RequireAuth);
        }

        [Fact]
        public void ServiceMethodsNoRequireAuthIfServiceNoRequireAuthByDefault()
        {
            var desc = new ServiceDescriptor(typeof(ITestService)) {RequireAuth = false};
            var methods = typeof(ITestService).GetTypeInfo().GetMethods().Select(m => m.Name).ToList();

            Assert.True(methods.Any());

            foreach (var method in methods)
            {
                Assert.False(desc.MethodRequireAuth(method));
            }

            Assert.False(desc.RequireAuth);
        }

        [Fact]
        public void CanSetMethodRequireAuthToFalseWhenServiceRequireAuth()
        {
            var desc = new ServiceDescriptor(typeof(ITestService));
            var methods = typeof(ITestService).GetTypeInfo().GetMethods().Select(m => m.Name).ToList();

            Assert.True(methods.Any());

            desc.SetMethodAuth(methods[0], false);

            Assert.False(desc.MethodRequireAuth(methods[0]));

            Assert.True(desc.RequireAuth);

        }

        [Fact]
        public void CanSetMethodRequireAuthToTrueWhenServiceNoRequireAuth()
        {
            var desc = new ServiceDescriptor(typeof(ITestService)){RequireAuth = false};
            var methods = typeof(ITestService).GetTypeInfo().GetMethods().Select(m => m.Name).ToList();

            Assert.True(methods.Any());

            desc.SetMethodAuth(methods[0], true);

            Assert.True(desc.MethodRequireAuth(methods[0]));

            Assert.False(desc.RequireAuth);
        }

        [Fact]
        public void CanAddMethodMomentInterceptor()
        {
            var desc = new ServiceDescriptor(typeof(ITestService)) { RequireAuth = false };

            desc.AddInterceptor("MethodName", Moment.Before, (callContext) => { });
            desc.AddInterceptor("MethodName", Moment.Before, (callContext) => { });
            desc.AddInterceptor("MethodName", Moment.After, (callContext) => { });
            desc.AddInterceptor("MethodName", Moment.After, (callContext) => { });
            desc.AddInterceptor("MethodName", Moment.OnException, (callContext) => { });
            desc.AddInterceptor("MethodName", Moment.OnException, (callContext) => { });

            Assert.True(desc.GetBeforeInterceptors("MethodName").Count() == 2);
            Assert.True(desc.GetAfterInterceptors("MethodName").Count() == 2);
            Assert.True(desc.GetOnExceptionInterceptors("MethodName").Count() == 2);
        }

        [Fact]
        public void InterceptorAreByNameAndMoment()
        {
            var desc = new ServiceDescriptor(typeof(ITestService)) { RequireAuth = false };

            desc.AddInterceptor("MethodName1", Moment.Before, (callContext) => { });
            desc.AddInterceptor("MethodName2", Moment.Before, (callContext) => { });
            desc.AddInterceptor("MethodName3", Moment.After, (callContext) => { });
            desc.AddInterceptor("MethodName4", Moment.After, (callContext) => { });
            desc.AddInterceptor("MethodName5", Moment.OnException, (callContext) => { });
            desc.AddInterceptor("MethodName6", Moment.OnException, (callContext) => { });

            Assert.True(desc.GetBeforeInterceptors("MethodName1").Count() == 1);
            Assert.True(!desc.GetAfterInterceptors("MethodName1").Any());
            Assert.True(!desc.GetOnExceptionInterceptors("MethodName1").Any());
            Assert.True(desc.GetBeforeInterceptors("MethodName2").Count() == 1);
            Assert.True(!desc.GetAfterInterceptors("MethodName2").Any());
            Assert.True(!desc.GetOnExceptionInterceptors("MethodName2").Any());
            Assert.True(!desc.GetBeforeInterceptors("MethodName3").Any());
            Assert.True(desc.GetAfterInterceptors("MethodName3").Count() == 1);
            Assert.True(!desc.GetOnExceptionInterceptors("MethodName3").Any());
            Assert.True(!desc.GetBeforeInterceptors("MethodName4").Any());
            Assert.True(desc.GetAfterInterceptors("MethodName4").Count() == 1);
            Assert.True(!desc.GetOnExceptionInterceptors("MethodName4").Any());
            Assert.True(!desc.GetBeforeInterceptors("MethodName5").Any());
            Assert.True(!desc.GetAfterInterceptors("MethodName5").Any());
            Assert.True(desc.GetOnExceptionInterceptors("MethodName5").Count() == 1);
            Assert.True(!desc.GetBeforeInterceptors("MethodName6").Any());
            Assert.True(!desc.GetAfterInterceptors("MethodName6").Any());
            Assert.True(desc.GetOnExceptionInterceptors("MethodName6").Count() == 1);
        }

        [Fact]
        public void CanAddMethodInterceptorObject()
        {
            var desc = new ServiceDescriptor(typeof(ITestService));

            desc.AddInterceptorObject(new TestServiceInterceptor());
            desc.AddInterceptorObject(new TestServiceInterceptorByMethodName());

            Assert.True(desc.GetInterceptorObjects().Count() == 2);
        }
    }
}
