namespace Dose.Core.Library.Actors
{
    public class DoorContext : IActorContext
    {
        public string DisplayName => "door";
    }

    public partial class TestDoorContext : DoorContext { }
}