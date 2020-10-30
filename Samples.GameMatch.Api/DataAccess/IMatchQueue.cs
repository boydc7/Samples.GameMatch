using System;

namespace Samples.GameMatch.Api
{
    public interface IMatchQueue
    {
        void Enqueue(MakeMatch request);
        MakeMatch Dequeue();
    }

    public interface IObservedMatchQueue : IMatchQueue
    {
        IDisposable Subscribe(IMatchQueueObserver observer);
    }

    public interface IMatchQueueObserver
    {
        void OnMatchEnqueued();
    }
}
