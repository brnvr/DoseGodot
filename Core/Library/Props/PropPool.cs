using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.DataStructures;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Library.Props
{
    public class PropPool(
        string configPath,
        PackedScene propScene,
        YamlConfigConverter configConverter,
        IObjectFromConfigBuilder<PropDefinition> builder
    ) : Pool<PropDefinition>(configPath, configConverter, builder)
    {
        public PackedScene PropScene { get; } = propScene;

        public Prop Create(PropDefinition definition)
        {
            var prop = PropScene.Instantiate<Prop>();
            prop.Initialize(definition);

            return prop;
        }

        public new PropPool Load()
        {
            return (PropPool)base.Load();
        }
    }
}
