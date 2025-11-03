using Dose.Library.Common;
using Godot;

namespace Dose.Library.Actors
{
    public abstract class ItemController : IActorController
    {
        public abstract string DisplayName { get; }
    }
}