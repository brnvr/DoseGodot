using Godot;
using YamlDotNet.Core.Events;

namespace Dose.Core.Library.Common
{
     class Following<TTarget>
    {
        readonly Timer _timer;

        public IFollower<TTarget> Follower { get; }
        public TTarget Target { get; }
        public float Speed { get; }
        public float TargetUpdateIntervalSeconds { get; }

        public Following(IFollower<TTarget> follower, TTarget target, float speed, float targetUpdateIntervalSeconds)
        {
            Follower = follower;
            Target = target;
            Speed = speed;
            TargetUpdateIntervalSeconds = targetUpdateIntervalSeconds;

            _timer = new Timer
            {
                WaitTime = TargetUpdateIntervalSeconds
            };

            _timer.Timeout += Process;
        }

        public void Start(Node parent)
        {
            var existingParent = _timer.GetParent();

            existingParent?.RemoveChild(_timer);

            parent.AddChild(_timer);
            Process();
            _timer.Start();
        }

        public void Stop()
        {
            Follower.Stop();
            _timer.Stop();
        } 

        void Process()
        {
            if (Follower.HasReached(Target))
            {
                Follower.Stop();
                return;
            }

            Follower.Follow(Target, Speed);
        }
    }
}