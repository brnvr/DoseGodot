using System;
using Dose.Core.Library.Common;
using Godot;
using Microsoft.VisualBasic;

namespace Dose.Core.Library.Actors
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