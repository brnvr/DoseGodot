using Godot;

namespace Dose.Library.Actors
{
    public abstract partial class BaseItem : Node3D, IActor<BasePlayer>
    {
        public void HandleInteraction(BasePlayer player)
        {
            player.Inventory?.AddAndDestroy(this);
        }

        public abstract void Destroy();
    }
}