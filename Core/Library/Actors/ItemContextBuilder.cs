using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Actors;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class ItemContextBuilder(Dictionary<string, IActionPerformer> actionPerformerMap) : IObjectFromConfigBuilder<ItemContext>
    {
        public Dictionary<string, IActionPerformer> ActionPerformerMap { get; } = actionPerformerMap;

        public ItemContext Build(ItemConfig itemConfig)
        {
            return new ItemContext
            {
                Identifier = itemConfig.Identifier,
                DisplayName = itemConfig.DisplayName,
                Rarity = Enum.Parse<Rarity>(itemConfig.Rarity, true),
                PackId = itemConfig.PackId,
                Texture = ResourceLoader.Load<Texture2D>(itemConfig.TexturePath),
                EquipedTexture = ResourceLoader.Load<Texture2D>(itemConfig.EquipedTexturePath),
                MaxQuantity = itemConfig.MaxQuantity ?? -1,
                Execute = itemConfig.Execute.Select(config => new Executable
                {
                    Performer = ActionPerformerMap[config.Action],
                    Args = config.Args
                }),
                KeepAfterUse = itemConfig.KeepAfterUse,
                Scale = itemConfig.Scale,
                EquipedScale = itemConfig.EquipedScale
            };
        }

        ItemContext IObjectFromConfigBuilder<ItemContext>.Build(YamlConfig config)
        {
            return Build((ItemConfig)config);
        }
    }
}
