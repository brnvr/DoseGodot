using Dose.Library.Common;
using Godot;

namespace Dose.Library.Actors
{
    public abstract class NpcController : IActorController
    {
        public abstract string DisplayName { get; }
    }
}