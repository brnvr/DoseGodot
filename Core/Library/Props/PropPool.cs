using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.DataStructures;
using Godot;

namespace Dose.Core.Library.Props
{
    public class PropPool(
        string configPath,
        PackedScene propScene,
        YamlConfigConverter configConverter,
        IObjectFromConfigBuilder<PropDefinition> builder
    ) : WeightedPool<PropDefinition>(configPath, configConverter, builder)
    {
        public PackedScene PropScene { get; } = propScene;

        public new PropPool Load()
        {
            return (PropPool)base.Load();
        }
    }
}
