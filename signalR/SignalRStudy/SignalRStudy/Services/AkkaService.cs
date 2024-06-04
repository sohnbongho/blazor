using Akka.Actor;

namespace SignalRStudy.Services
{
    public class AkkaService : IHostedService
    {
        private readonly ActorSystem _actorSystem;

        public AkkaService(ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Akka.NET Actor System을 시작할 때 추가적인 설정을 할 수 있습니다.
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Akka.NET Actor System을 종료합니다.
            return CoordinatedShutdown.Get(_actorSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
        }
    }
}

