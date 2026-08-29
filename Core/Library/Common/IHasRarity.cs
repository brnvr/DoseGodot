namespace Dose.Core.Library.Common
{
    public enum Rarity
    {
        Force = -1,
        VeryCommon = 500,
        Common = 100,
        Uncommon = 25,
        Rare = 5,
        VeryRare = 1        
    }

    public interface IHasRarity
    {
        Rarity Rarity { get; }
    }
}
