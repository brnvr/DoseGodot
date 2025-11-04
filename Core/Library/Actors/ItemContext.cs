using Dose.Core.Library.Common;
using Godot;

namespace Dose.Core.Library.Actors
{
    public abstract class ItemContext : IActorContext
    {
        public abstract string DisplayName { get; }
    }
}