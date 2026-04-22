using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActionPerformers
{
    public class ReceivePerformer(Player player) : InteractionPerformer<ItemContext>
    {
        public Player Player { get; } = player;

        public override void Perform(ItemContext item, InteractionPerformerArgs args)
        {
            if (args.ActorContext is null) return;

            if (!item.KeepAfterUse)
            {
                Player.Inventory.Drop(item);
            }
            
            GD.Print($"Item delivered to \"{args.ActorContext.DisplayName}\": {item.Identifier}");
        }
    }
}