namespace Dose.Core.Library.Actors
{
    public class DoorContext : IActorContext
    {
        public string DisplayName => FormattedNumber == "" ? "door" : $"door {FormattedNumber}";

        public int? Number { get; set; }
        public string FormattedNumber => Number is null ? "" : ((int)Number).ToString().PadLeft(3, '0');
    }
}