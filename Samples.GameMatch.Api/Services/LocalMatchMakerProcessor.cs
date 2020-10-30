using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Samples.GameMatch.Api
{
    public class LocalMatchMakerProcessor : IHostedService, IMatchQueueObserver
    {
        private readonly object _lockObject = new object();
        private readonly Dictionary<string, Task> _workers;
        private readonly IObservedMatchQueue _observedMatchQueue;
        private readonly IMatchMakerService _matchMaker;
        private readonly ILogger<LocalMatchMakerProcessor> _log;

        private int _disposalCount;
        private bool _inShutdown;

        public LocalMatchMakerProcessor(IObservedMatchQueue observedMatchQueue,
                                        IMatchMakerService matchMaker,
                                        ILogger<LocalMatchMakerProcessor> log)
        {
            _observedMatchQueue = observedMatchQueue;
            _matchMaker = matchMaker;
            _log = log;
            observedMatchQueue.Subscribe(this);
            _workers = new Dictionary<string, Task>();
        }

        public Task StartAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _inShutdown = true;

            while (_workers.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(250, cancellationToken);
            }
        }

        public void OnMatchEnqueued()
        {
            if (_inShutdown)
            {
                return;
            }

            StartWorker();
        }

        private void StartWorker()
        {
            var consumerId = Guid.NewGuid().ToString("N");

            try
            {
                // We got work to do
                lock(_lockObject)
                {
                    _workers.Add(consumerId, Task.Run(Receive).ContinueWith(t => OnWorkerComplete(consumerId)));
                }
            }
            catch(Exception ex)
            {
                _log.LogError(ex, "Error trying to StartWorker in LocalMatchMakerProcessor");

                OnWorkerComplete(consumerId);

                if (_workers.ContainsKey(consumerId))
                {
                    lock(_lockObject)
                    {
                        try
                        {
                            _workers.Remove(consumerId);
                        }
                        catch(Exception x)
                        {
                            _log.LogWarning(x, "Worker removal failed");
                        }
                    }
                }
            }
        }

        private void Receive()
        {
            while (true)
            {
                if (_inShutdown)
                {
                    return;
                }

                try
                {
                    var request = _observedMatchQueue.Dequeue();

                    if (request == null)
                    { // Nuttin to do
                        return;
                    }

                    _log.LogDebug("Processing MakeMatch request {MakeMatchRequest}", request.ToJson());

                    _matchMaker.Match(request);
                }
                catch(Exception x)
                {
                    _log.LogError(x, "Error processing MakeMatch request in LocalMatchMakerProcessor");

                    // Normally would retry, push to DLQ, etc....but not here...
                }
            }
        }

        private void OnWorkerComplete(string consumerId)
        {
            if (string.IsNullOrEmpty(consumerId))
            {
                return;
            }

            if (!_workers.ContainsKey(consumerId))
            {
                return;
            }

            try
            {
                lock(_lockObject)
                {
                    _workers.Remove(consumerId);
                }
            }
            catch(Exception x)
            {
                _log.LogWarning(x, "Worker removal failed");
            }
        }

        public virtual void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposalCount, 1, 0) > 0)
            {
                return;
            }

            if (_workers.Count > 0)
            {
                Task.WaitAll(_workers.Values.ToArray(), 25000);
            }
        }
    }
}
