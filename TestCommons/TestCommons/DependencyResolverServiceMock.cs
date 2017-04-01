using System;
using ToleLibraries.ApplicationContext;
using ToleLibraries.DependencyResolver;

namespace ToleLibraries.TestCommons
{
    public class DependencyResolverServiceMock: IDependencyResolver
    {
        #region Implementation of IDependencyResolver

        public object Resolve(Type type)
        {
            if (type == typeof(AuthenticatedApplicationContext) || type == typeof(IApplicationContext))
                return new AuthenticatedApplicationContext();
            if (type == typeof(GuestApplicationContext))
                return new GuestApplicationContext();

            return null;
        }

        public TType Resolve<TType>()
        {
            return (TType) Resolve(typeof(TType));
        }

        #endregion
    }
}
