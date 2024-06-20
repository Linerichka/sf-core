using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;



namespace SFramework.Core.Runtime
{
    public interface ISFContainer
    {
        Transform Root { get; }
        T Resolve<T>() where T : class;
        object Resolve(Type type);
        T[] ResolveMany<T>();
        object[] Bindings { get; }

        UniTask InitServices(CancellationToken cancellationToken);
    }
}