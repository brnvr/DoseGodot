using Dose.Core.Library.Actors;
using Dose.Core.Library.Common;
using Dose.Core.Library.Control;
using Dose.Core.Library.DataStructures;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Interactions
{
    public class PickInteraction<TInteractor>(TInteractor interactor) : IInteraction<TInteractor>
    {
        public string DisplayName => "pick";
        public TInteractor Interactor { get; } = interactor;

        public void Perform(IActor subject)
        {
            if (Interactor is IPicker picker)
            {
                if (subject is IItem item)
                {
                    picker.Inventory?.AddAndDestroy(item);
                }
            }
        }
    }
}