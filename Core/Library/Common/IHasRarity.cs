namespace Dose.Core.Library.Common
{
    public enum Rarity
    {
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