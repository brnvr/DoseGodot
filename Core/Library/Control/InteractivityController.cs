using Dose.Core.Library.Actors;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.Control
{
    public class InteractivityController(IUI? ui)
    {
        IUI? UI { get; } = ui;

        public void OnTargetChange(IActor? target)
        {
            UI?.SetLabelValue("NameCaption", target?.Context?.DisplayName ?? "");
        }
    }
}