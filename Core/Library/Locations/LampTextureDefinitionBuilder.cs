using System;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Locations;

namespace Dose.Core.Library.Locations
{
    public class LampTextureDefinitionBuilder : IObjectFromConfigBuilder<LampTextureDefinition>
    {
        public LampTextureDefinition Build(YamlConfig config)
        {
            var lampConfig = (LampTextureConfig)config;

            return new LampTextureDefinition
            {
                TexturePath = lampConfig.TexturePath,
                Rarity = Enum.Parse<Rarity>(lampConfig.Rarity),
                PackId = lampConfig.PackId,
                LocationRestriction = lampConfig.LocationRestriction,
                Scale = lampConfig.Scale
            };
        }
    }
}
