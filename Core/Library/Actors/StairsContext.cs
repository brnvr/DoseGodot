namespace Dose.Core.Library.Actors
{
    public class StairsContext : IActorContext
    {
        public string Identifier => "stairs";
        public string DisplayName => $"{Direction.ToString().ToLower()}stairs";
        public int MaxQuantity => -1;
        public StairsDirection Direction { get; init; }
    }
}