using Godot;

namespace Dose.Library.Actors
{
    public class DoorController : IActorController
    {
        public string DisplayName => "door";
    }

    public partial class TestDoorController : DoorController { }
}