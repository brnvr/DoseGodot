using System;
using Godot;

namespace Dose.Core.Library.Actors
{
    public abstract partial class BaseItem : Node3D, IActor<BasePlayer>
    {
        public IActorContext? Context { get; private set; }

        public abstract void Destroy();

        public IInteraction<BasePlayer> GetDefaultInteraction(BasePlayer interactor)
        {
            throw new NotImplementedException();
        }

        public void HandleInteraction(IInteraction<BasePlayer> interaction)
        {
            //player.Inventory?.AddAndDestroy(this);
        }
    }
}