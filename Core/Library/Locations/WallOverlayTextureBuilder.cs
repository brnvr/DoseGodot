using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Locations;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class WallOverlayTextureBuilder : IObjectFromConfigBuilder<WallOverlayTextureDefinition>
    {
        public WallOverlayTextureDefinition Build(YamlConfig config)
        {
            var wallOverlayConfig = (WallOverlayTextureConfig)config;

            return new WallOverlayTextureDefinition
            {
                Texture = GD.Load<Texture2D>(wallOverlayConfig.TexturePath),
                Scale = wallOverlayConfig.Scale,
                TextureRepeat = wallOverlayConfig.TextureRepeat
            };
        }
    }
}
