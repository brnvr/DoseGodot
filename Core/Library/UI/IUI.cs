using Dose.Core.Library.Control;

namespace Dose.Core.Library.UI
{
    public interface IUI : IMenuContainer
    {
        void SetLabelValue(string labelName, string text);
        void SetCursorVisible(bool visible);
        void SetActorName(string? name);
        void SetActorNameVisible(bool visible);
        void ShowDialogueLine(string text);
        void HideDialogue();
    }
}
