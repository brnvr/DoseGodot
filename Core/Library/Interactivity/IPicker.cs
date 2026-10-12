using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Library.Interactivity
{
    public interface IPicker
    {
        Inventory? Inventory { get; }
        Node3D PickupTarget { get; }
    }
}
