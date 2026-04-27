namespace Dose.Core.Library.Actors
{
    public class DoorContext : IActorContext
    {
        public string Identifier => "door";
        public string DisplayName => FormattedNumber == "" ? "door" : $"door {FormattedNumber}";
        public int MaxQuantity => -1;
        public int? Number { get; init; }
        public string FormattedNumber => Number is null ? "" : ((int)Number).ToString().PadLeft(3, '0');
    }
}