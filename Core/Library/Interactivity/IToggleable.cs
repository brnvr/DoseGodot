using System.Collections.Generic;

namespace Dose.Core.Library.Interactivity
{
    public interface IToggleable
    {
        void Toggle();
    }

    public interface IToggleable<TState> : IToggleable where TState : struct
    {
        TState State { get; }
        IReadOnlyDictionary<TState, string> DisplayNameForState { get; }
    }
 }