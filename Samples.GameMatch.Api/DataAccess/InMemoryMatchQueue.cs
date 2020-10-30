using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Samples.GameMatch.Api
{
    public class InMemoryMatchQueue : IObservedMatchQueue
    {
        private readonly List<IMatchQueueObserver> _observers = new List<IMatchQueueObserver>();
        private readonly ConcurrentQueue<MakeMatch> _queue = new ConcurrentQueue<MakeMatch>();

        public void Enqueue(MakeMatch request)
        {
            _queue.Enqueue(request);

            NotifyObservers();
        }

        public MakeMatch Dequeue()
            => _queue.TryDequeue(out var result)
                   ? result
                   : null;

        public IDisposable Subscribe(IMatchQueueObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new ObservableUnsubscriber(this, observer);
        }

        private void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.OnMatchEnqueued();
            }
        }

        private void Unsubscribe(IMatchQueueObserver observer)
        {
            _observers.Remove(observer);
        }

        private class ObservableUnsubscriber : IDisposable
        {
            private readonly InMemoryMatchQueue _observed;
            private readonly IMatchQueueObserver _observer;

            public ObservableUnsubscriber(InMemoryMatchQueue observed, IMatchQueueObserver observer)
            {
                _observed = observed;
                _observer = observer;
            }

            public void Dispose()
            {
                _observed?.Unsubscribe(_observer);
            }
        }
    }
}
