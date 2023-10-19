using System;

namespace SFramework.Core.Runtime
{
    public interface ISFContainer
    {
        T Resolve<T>() where T : class;
        object Resolve(Type type);
        T[] ResolveMany<T>();
        object[] Bindings { get; }
    }
}