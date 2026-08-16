using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Props;
using Godot;
using System;

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
                    : GD.Load<PackedScene>(propConfig.ModelPath),
                Rarity = Enum.Parse<Rarity>(propConfig.Rarity, true),
                Scale = propConfig.Scale,
                MinDistanceToWall = propConfig.MinDistanceToWall,
                MaxDistanceToWall = propConfig.MaxDistanceToWall,
                MinDistanceFromCorner = propConfig.MinDistanceFromCorner,
                MaxDistanceFromCorner = propConfig.MaxDistanceFromCorner
            };
        }
    }
}
