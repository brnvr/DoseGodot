using System;
using Godot;

namespace Dose.Core.Library.Locations
{
    public interface ILocation
    {
        Guid Id { get; }
        void OnEntered();
        void OnLeft();
        TPortal SelectRandomPortal<TPortal>(Func<TPortal, bool>? predicate = null) where TPortal : IPortal;
        void Clear(int keepLevels);
    }
}
