using System.Collections.Generic;
using Dose.Core.Library.Common;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActionPerformers
{
    public class MovePlayerPerformer(Player player) : ActionPerformer<object, MovePlayerPerformerArgs>
    {
        Node3D? _node;
        Vector3 _basePosition;

        public Player Player { get; } = player;


        public override MovePlayerPerformerArgs BuildArgs(Dictionary<string, object?> args)
        {
            var x = float.Parse((string?)args.GetValueOrDefault("x") ?? "0");
            var y = float.Parse((string?)args.GetValueOrDefault("y") ?? "0");
            var z = float.Parse((string?)args.GetValueOrDefault("z") ?? "0");

            return new MovePlayerPerformerArgs
            {
                Player = Player,
                Distance = new Vector3(x, y, z)
            };
        }

        public override bool Perform(object sender, MovePlayerPerformerArgs args)
        {
            if (sender is not Node3D node)
            {
                return true;
            }
        
            if (node != _node)
            {
                _node = node;
                _basePosition = node.Position;
            }

            if (_node.Position.DistanceTo(_basePosition) > 5)
            {
                return true;
            }
            else
            {
                node.Position += args.Distance;
                return false;
            }
        }
    }
}