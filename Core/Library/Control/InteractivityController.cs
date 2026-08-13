using Dose.Core.Library.Actors;
using Dose.Core.Library.UI;

namespace Dose.Core.Library.Control
{
    public class InteractivityController(IUI? ui)
    {
        IActor? _currentTarget;

        public IUI? UI { get; } = ui;
        public bool IsTargetCaptionEnabled { get; private set; } = true;

        public void OnTargetChange(IActor? target)
        {
            _currentTarget = target;

            if (IsTargetCaptionEnabled)
            {
                UpdateTargetCaption();
            }
        }

        public void SetTargetCaptionEnabled(bool enabled)
        {
            IsTargetCaptionEnabled = enabled;
            UpdateTargetCaption();
        }

        void UpdateTargetCaption()
        {
            UI?.SetActorName(
                IsTargetCaptionEnabled ? _currentTarget?.Context?.DisplayName : null
            );
        }
    }
}
