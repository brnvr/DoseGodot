using Dose.Core.Library.Control;

namespace Dose.Core.Library.UI
{
    public interface IUI : IMenuContainer
    {
        void SetLabelValue(string labelName, string text);
        void SetLabelVisible(string labelName, bool visible);
    }
}