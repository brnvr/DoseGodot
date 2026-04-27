using Dose.Core.Library.Actors;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.Control
{
    public class InteractivityController(IUI? ui)
    {
        public IUI? UI { get; } = ui;
        public string NameCaptionLabelName { get; init; } = "NameCaption";

        public void OnTargetChange(IActor? target)
        {
            UI?.SetLabelValue(NameCaptionLabelName, target?.Context?.DisplayName ?? "");
        }
    }
}