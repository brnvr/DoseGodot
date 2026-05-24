using System;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Locations;
using Godot;

namespace Dose.Core.Library.Locations
{
    public class TextureDefinitionBuilder : IObjectFromConfigBuilder<TextureDefinition>
    {
        public static TextureDefinition Build(TextureConfig config)
        {
            return new TextureDefinition
            {
                TexturePath = config.TexturePath,
                Rarity = Enum.Parse<Rarity>(config.Rarity),
            };
        }

        public TextureDefinition Build(YamlConfig config)
        {
            return Build((TextureConfig)config);
        }
    }
}