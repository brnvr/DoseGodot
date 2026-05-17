using System;
using System.Collections.Generic;
using System.Linq;
using Dose.Core.Library.Common;
using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Actors;
using Dose.Core.Library.Interactivity;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class ItemContextBuilder(Dictionary<string, IActionPerformer> actionPerformerMap) : IContextBuilder<IActorContext>
    {
        public Dictionary<string, IActionPerformer> ActionPerformerMap { get; } = actionPerformerMap;

        public ItemContext BuildContext(ItemConfig itemConfig)
        {
            return new ItemContext
            {
                Identifier = itemConfig.Identifier,
                DisplayName = itemConfig.DisplayName,
                Texture = ResourceLoader.Load<Texture2D>(itemConfig.TexturePath),
                MaxQuantity = itemConfig.MaxQuantity ?? -1,
                Execute = itemConfig.Execute.Select(config => new Executable
                {
                    Performer = ActionPerformerMap[config.Action],
                    Args = config.Args
                }),
                InteractWith = itemConfig.InteractWith.Select(config => new InteractionOperator
                {
                    ActorIdentifier = config.Actor,
                    Performer = ActionPerformerMap[config.Action],
                    Args = config.Args
                }),
                KeepAfterUse = itemConfig.KeepAfterUse
            };
        }

        IActorContext IContextBuilder<IActorContext>.BuildContext(YamlConfig config)
        {
            return BuildContext((ItemConfig)config);
        }
    }
}