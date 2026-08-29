using System;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Locations;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class FloorOverlayTextureBuilder : IObjectFromConfigBuilder<FloorOverlayTextureDefinition>
    {
        public FloorOverlayTextureDefinition Build(YamlConfig config)
        {
            var floorOverlayConfig = (FloorOverlayTextureConfig)config;

            return new FloorOverlayTextureDefinition
            {
                Texture = GD.Load<Texture2D>(floorOverlayConfig.TexturePath),
                Rarity = Enum.Parse<Rarity>(floorOverlayConfig.Rarity, true),
                PackId = floorOverlayConfig.PackId,
                LocationRestriction = floorOverlayConfig.LocationRestriction,
                Scale = floorOverlayConfig.Scale,
                TextureRepeat = floorOverlayConfig.TextureRepeat
            };
        }
    }
}
