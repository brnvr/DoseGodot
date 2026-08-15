using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Props;
using Godot;

namespace Dose.Core.Library.Props
{
    public class PropDefinitionBuilder : IObjectFromConfigBuilder<PropDefinition>
    {
        public PropDefinition Build(YamlConfig config)
        {
            var propConfig = (PropConfig)config;

            return new PropDefinition
            {
                Texture = propConfig.IsBillboard
                    ? GD.Load<Texture2D>(propConfig.TexturePath)
                    : null,
                ModelScene = propConfig.IsBillboard
                    ? null
                    : GD.Load<PackedScene>(propConfig.ModelPath)
            };
        }
    }
}
