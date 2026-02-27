using System.Collections.Generic;
using Dose.Core.Actors.Items;
using Dose.Core.Library.Actors;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActorPools
{
    public class ItemPool : ActorPool<Item, ItemContext>
    {
        readonly static PackedScene _itemScene = GD.Load<PackedScene>("res://Configuration/Scenes/Item.tscn");
        readonly static List<ItemContext> _defaultItems =
        [
            new CurrantSyrup()
        ];
        
        public ItemPool() : base(_itemScene, _defaultItems) { }
    }
}   