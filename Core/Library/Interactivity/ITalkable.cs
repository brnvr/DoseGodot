using Dose.Core.Library.DataStructures.Dialogues;

namespace Dose.Core.Library.Interactivity
{
    public interface ITalkable
    {
        IDialogue? GetDialogue();
    }
}