using Dose.Core.Library.Actors;
using Dose.Core.Library.Interactivity;
using Dose.Core.Library.Locations;

namespace Dose.Core.Interactions
{
    public class UseStairsInteraction<TInteractor>(TInteractor interactor) : IInteraction<TInteractor>
    {
        public TInteractor Interactor { get; } = interactor;
        public string DisplayName => $"use stairs";

        public void Perform(IActor subject)
        {
            if (Interactor is IStairsUser stairsUser)
            {
                if (subject is IStairs stairs)
                {
                    stairsUser.UseStairs(stairs);
                }
            }
        }
    }
}