using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActionPerformers
{
    public class MovePlayerPerformer(Player player) : ActionPerformer<MovePlayerPerformerArgs>
    {
        public Player Player { get; } = player;

        public override MovePlayerPerformerArgs BuildArgs(Dictionary<string, object?> args)
        {
            var x = (float)(args["x"] ?? 0);
            var y = (float)(args["y"] ?? 0);
            var z = (float)(args["z"] ?? 0);

            return new MovePlayerPerformerArgs
            {
                Player = Player,
                Distance = new Vector3(x, y, z)
            };
        }

        public override void Perform(MovePlayerPerformerArgs args)
        {
            args.Player.Position += args.Distance;
        }
    }
}