using Dose.Core.Library.Configuration;
using Dose.Core.Library.Configuration.Actors;
using Dose.Core.Library.DataStructures.Dialogues;
using Dose.Core.Library.ESpeak;
using Godot;

namespace Dose.Core.Library.Actors
{
    public class NpcContextBuilder() : IActorContextBuilder
    {
        public static NpcContext BuildContext(NpcConfig npcConfig)
        {
            return new NpcContext
            {
                DisplayName = npcConfig.DisplayName,
                Texture = ResourceLoader.Load<Texture2D>(npcConfig.TexturePath),
                Voice = new Voice(npcConfig.Voice, 16000),
                Dialogue = new ShortDialogue("somedays i wish time would go slower")
            };
        }

        IActorContext IActorContextBuilder.BuildContext(YamlConfig config)
        {
            return BuildContext((NpcConfig)config);
        }
    }
}