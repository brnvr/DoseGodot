using System;
using Godot;

namespace Dose.Core.Library.Locations
{
    public interface ILocation
    {
        Guid Id { get; }
        TPortal SelectRandomPortal<TPortal>(Func<TPortal, bool>? predicate = null) where TPortal : IPortal;
        void Clear(int keepLevels);
    }
}
