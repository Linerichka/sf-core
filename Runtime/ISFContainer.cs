using System;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public interface ISFContainer
    {
        Transform Root { get; }
        T Resolve<T>() where T : class;
        object Resolve(Type type);
        T[] ResolveMany<T>();
        object[] Bindings { get; }
    }
}