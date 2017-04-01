using System;

namespace ToleLibraries.DependencyResolver
{
    public interface IDependencyResolver
    {
        object Resolve(Type type);
        TType Resolve<TType>();
    }
}
