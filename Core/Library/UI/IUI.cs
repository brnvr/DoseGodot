namespace Dose.Core.Library.UI
{
    public interface IUI
    {
        void SetLabelValue(string labelName, string text);
        void SetLabelVisible(string labelName, bool visible);
    }
}