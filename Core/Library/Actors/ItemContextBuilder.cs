using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Actors;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class ItemContextBuilder() : IActorContextBuilder
    {
        public static ItemContext BuildContext(ItemConfig itemConfig)
        {
            return new ItemContext
            {
                DisplayName = itemConfig.DisplayName,
                Texture = ResourceLoader.Load<Texture2D>(itemConfig.TexturePath),
                MaxQuantity = itemConfig.MaxQuantity ?? -1
            };
        }

        IActorContext IActorContextBuilder.BuildContext(YamlConfig config)
        {
            return BuildContext((ItemConfig)config);
        }
    }
}