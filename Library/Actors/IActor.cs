using System;
using Dose.Library.Common;
using Godot;
using Microsoft.VisualBasic;

namespace Dose.Library.Actors
{
    public interface IActor
    {
        IActorContext? Context { get; }
    }

    public interface IActor<TInteractor> : IActor
    {
        void HandleInteraction(IInteraction<TInteractor> interaction);
        IInteraction<TInteractor> GetDefaultInteraction(TInteractor interactor);
    }
}