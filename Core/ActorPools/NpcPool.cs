using System.Collections.Generic;
using Dose.Core.ActorArgs;
using Dose.Core.Actors.Npcs;
using Dose.Core.Library.Actors;
using Dose.Core.Scenes;
using Godot;

namespace Dose.Core.ActorPools
{
    public class NpcPool : ActorPool<Npc, NpcContext, NpcArgs>
    {
        readonly static PackedScene _npcScene = GD.Load<PackedScene>("res://Configuration/Scenes/Npc.tscn");
        readonly static List<NpcContext> _defaultItems =
        [
            new Agnes()
        ];
        
        public NpcPool() : base(_npcScene, _defaultItems) { }
    }
}   