using Godot;

namespace Dose.Core.Library.Common
{
    public interface ILookable
    {
        Vector3 TopLookTarget { get; }
        Vector3 CenterLookTarget { get; }
    }
}