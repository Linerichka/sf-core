using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SFramework.Core.Runtime
{
    public interface ISFService : ISFInjectable, IDisposable
    {
        UniTask Init(CancellationToken cancellationToken);
    }
}
