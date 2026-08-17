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
                PackId = propConfig.PackId,
                LocationRestriction = propConfig.LocationRestriction,
                Scale = propConfig.Scale,
                YOffset = propConfig.YOffset,
                MinDistanceFromWall = propConfig.MinDistanceFromWall,
                MaxDistanceFromWall = propConfig.MaxDistanceFromWall,
                MinDistanceFromCorner = propConfig.MinDistanceFromCorner,
                MaxDistanceFromCorner = propConfig.MaxDistanceFromCorner
            };
        }
    }
}
