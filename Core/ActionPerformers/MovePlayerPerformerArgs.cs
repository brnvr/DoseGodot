using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActionPerformers
{
    public class MovePlayerPerformerArgs
    {
        public required Player Player { get; init; }
        public Vector3 Distance { get; init; }
    }
}