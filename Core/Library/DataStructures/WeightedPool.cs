using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;

namespace Dose.Core.Library.DataStructures
{
    public class WeightedPool<T>(
        string configPath,
        YamlConfigConverter configConverter,
        IObjectFromConfigBuilder<T> builder
    ) : Pool<T>(configPath, configConverter, builder) where T : IHasRarity
    {
        public WeightedList<T> GetWeightedList()
        {
            return new RarityWeighter<T>([.. Content]).Weight();
        }

        public new WeightedPool<T> Load()
        {
            return (WeightedPool<T>)base.Load();
        }
    }
}