using Dose.Core.Library.GameEvents;
using Dose.Core.Scenes;

namespace Dose.Core.ActorArgs
{
    public class NpcArgs
    {
        public Player? Player { get; init; }
        public IGameEvent<Npc>? GameEvent { get; init; }
    }
}
