using System.Collections.Generic;
using Dose.Core.Interactions;
using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActionPerformers
{
    public class RejectWithDialoguePerformer(Player player)
        : InteractionPerformer<ItemContext, RejectWithDialoguePerformerArgs>
    {
        public Player Player { get; } = player;

        public override RejectWithDialoguePerformerArgs BuildArgs(
            IActorContext? context,
            Dictionary<string, object?> args
        )
        {
            args.TryGetValue("actor", out var actor);

            return new RejectWithDialoguePerformerArgs
            {
                ActorContext = context,
                Npc = actor as Npc,
                Line = args.GetValueOrDefault("line") as string
            };
        }

        public override bool Perform(ItemContext item, RejectWithDialoguePerformerArgs args)
        {
            if (args.Npc == null)
            {
                GD.PushWarning($"{nameof(RejectWithDialoguePerformer)} requires an NPC target.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(args.Line))
            {
                GD.PushWarning($"{nameof(RejectWithDialoguePerformer)} requires a non-empty line.");
                return false;
            }

            args.Npc.OnInteract(new TalkInteraction<Player>(
                Player,
                new ShortDialogue(args.Line)
            ));

            return false;
        }
    }
}
