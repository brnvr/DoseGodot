using Godot;

namespace Dose.Core.Library.Common
{
    public interface IFollower<TTarget>
    {
        void Follow(TTarget target, float speed);
        void Stop();
        bool HasReached(TTarget target);
    }
}