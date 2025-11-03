using System;
using Dose.Library.Common;
using Godot;

namespace Dose.Library.Actors
{
    public interface IActor
    {
        IActorController? Controller { get; }
    }

    public interface IActor<TInteractor> : IActor
    {
        void HandleInteraction(TInteractor other);
    }
}