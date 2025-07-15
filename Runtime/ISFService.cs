using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SFramework.Core.Runtime
{
    public interface ISFService : ISFInjectable, IDisposable, ISFRegistered
    {
        UniTask Init(CancellationToken cancellationToken);
    }
}
