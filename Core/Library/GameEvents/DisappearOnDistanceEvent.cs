using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Library.GameEvents
{
    public class DisappearOnDistanceEvent<TDisappearable>(Node3D reference, float distance) : IGameEvent<TDisappearable> where TDisappearable : Node3D, IDisappearable
    {
        public float Distance { get; } = distance;
        public Node3D Reference { get; } = reference;

        public void OnTrigger(TDisappearable target) => target.Disappear();
        public bool ShouldTrigger(TDisappearable target) => Reference.Position.DistanceTo(target.Position) <= Distance;
    }
}