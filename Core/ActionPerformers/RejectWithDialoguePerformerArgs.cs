using Dose.Core.Library.Common;
using Dose.Core.Scenes;

namespace Dose.Core.ActionPerformers
{
    public class RejectWithDialoguePerformerArgs : InteractionPerformerArgs
    {
        public Npc? Npc { get; init; }
        public string? Line { get; init; }
    }
}
